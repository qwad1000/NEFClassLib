using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;

namespace NEFClassLib
{
    public class Rule
    {
        private int[] mAntecedents;
        private int mResultClass;

        public Rule(int[] antecedents, int resultClass)
        {
            mAntecedents = new int[antecedents.Length];
            for (int i = 0; i < antecedents.Length; ++i)
                mAntecedents[i] = antecedents[i];

            mResultClass = resultClass;
        }

        public int[] Antecedents
        {
            get { return mAntecedents; }
        }

        public int ResultClass
        {
            get { return mResultClass; }
            set { mResultClass = value; }
        }
    }

    public struct TrainConfiguration
    {
        public static string TRAIN_RULES_SIMPLE = "Простое";
        public static string TRAIN_RULES_BEST = "Лучшее";
        public static string TRAIN_RULES_BEST_FOR_CLASS = "Лучшее для класса";

        public int RuleNodesMax { get; set; }
        public bool DoOptimization { get; set; }
        public double OptimizationSpeed { get; set; }
        public int[] FuzzyPartsCount { get; set; }
        public string RulesTrainAlgo { get; set; }
        public int MaxIterations { get; set; }
        public double Accuracy { get; set; }
    }

    public class NEFClassNetwork
    {
        private const string LOG_TAG = "NEFClassNetwork";

        private double[] mInputLayer;
        private double[] mHiddenLayer;
        private double[] mOutputLayer;

        private int[] mMinAncedent;

        private Partition[] mPartitions;
        private Rule[] mRules;
        private string[] mClassNames;

        public NEFClassNetwork(NCDataSet trainDataset, TrainConfiguration trainConfig)
        {
            Log.LogMessage(LOG_TAG, "Начало обучения.");

            mClassNames = trainDataset.GetClassesList();

            mInputLayer = new double[trainDataset.Dimension];
            mOutputLayer = new double[mClassNames.Length];

            mPartitions = new Partition[trainDataset.Dimension];
            for (int i = 0; i < trainDataset.Dimension; ++i)
                mPartitions[i] = new Partition(trainDataset.GetAttributeBounds(i), trainConfig.FuzzyPartsCount[i]);

            CreateRulesBase(trainDataset, trainConfig);
            mHiddenLayer = new double[mRules.Length];
            mMinAncedent = new int[mRules.Length];

            if (trainConfig.DoOptimization)
                TrainRules(trainDataset, trainConfig);
        }

        private void TrainRules(NCDataSet trainDataset, TrainConfiguration trainConfig)
        {
            int iteration = 0;
            double err1 = 0, err2 = 0;
            do
            {
                err2 = err1;
                err1 = 0;
                int misClassed = 0;
                for (int i = 0; i < trainDataset.Length; ++i)
                {
                    bool isCorrect = true;
                    double patternError = 0;
                    AdaptByPattern(trainDataset[i], trainConfig, out patternError, out isCorrect);

                    err1 += patternError;
                    if (!isCorrect)
                        ++misClassed;
                }

                if ((iteration + 1) % 10 == 0)
                    Log.LogMessage(LOG_TAG, "It: {0}. Error: {1}, MisClassed: {2}", iteration + 1, err1.ToString("0.######"), misClassed);
            } while (++iteration <= trainConfig.MaxIterations && Math.Abs(err2 - err1) > trainConfig.Accuracy);
        }

        private void CreateRulesBase(NCDataSet trainDataset, TrainConfiguration trainConfig)
        {
            if (trainConfig.RulesTrainAlgo == TrainConfiguration.TRAIN_RULES_SIMPLE)
                mRules = CreateRulesSimple(trainDataset, trainConfig);
            else if (trainConfig.RulesTrainAlgo == TrainConfiguration.TRAIN_RULES_BEST)
                mRules = CreateRulesBest(trainDataset, trainConfig);
            else if (trainConfig.RulesTrainAlgo == TrainConfiguration.TRAIN_RULES_BEST_FOR_CLASS)
                mRules = CreateRulesBestPerClass(trainDataset, trainConfig);
            else
                throw new ArgumentException("Wrong rules train algorithm: " + trainConfig.RulesTrainAlgo);
        }

        private Rule[] CreateRulesFromDataset(NCDataSet trainDataset, int maxRules)
        {
            List<Rule> rules = new List<Rule>();

            for (int i = 0; i < trainDataset.Length; ++i)
            {
                if (maxRules > 0 && rules.Count >= maxRules)
                    break;

                NCEntity entity = trainDataset[i];

                int[] maxFS = new int[entity.Dimension];
                for (int j = 0; j < entity.Dimension; ++j)
                    maxFS[j] = mPartitions[j].GetMaxMembershipIndex(entity[j]);

                bool found = false;
                for (int k = 0; k < rules.Count; ++k)
                {
                    bool equal = true;
                    for (int z = 0; z < rules[k].Antecedents.Length; ++z)
                        equal = equal && (rules[k].Antecedents[z] == maxFS[z]);

                    if (equal)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                    continue;

                rules.Add(new Rule(maxFS, GetClassIndex(entity.Class)));
                Log.LogMessage(LOG_TAG, "Образец {0} добавил правило {1}", i + 1, rules.Count);
            }

            return rules.ToArray();
        }

        private Rule[] CreateRulesSimple(NCDataSet trainDataset, TrainConfiguration trainConfig)
        {
            Log.LogMessage(LOG_TAG, "Генерация базы правил по алгоритму: simple");
            Rule[] rules = CreateRulesFromDataset(trainDataset, trainConfig.RuleNodesMax);
            Log.LogMessage(LOG_TAG, "База правил сгенерирована.");

            return rules;
        }

        private void CheckRulesConnections(Rule[] rules, NCDataSet checkDataset)
        {
            double[,] activations = new double[rules.Length, mClassNames.Length];

            for (int i = 0; i < checkDataset.Length; ++i)
            {
                NCEntity entity = checkDataset[i];

                for (int r = 0; r < rules.Length; ++r)
                {
                    double activation = 1;
                    for (int j = 0; j < entity.Dimension; ++j)
                        activation = Math.Min(activation, mPartitions[j].GetMembershipVector(entity[j])[rules[r].Antecedents[j]]);

                    int entityClass = GetClassIndex(entity.Class);
                    activations[r, entityClass] += activation;
                }
            }

            for (int r = 0; r < rules.Length; ++r)
            {
                int maxActivationIndex = 0;
                for (int k = 1; k < mClassNames.Length; ++k)
                    if (activations[r, maxActivationIndex] < activations[r, k])
                        maxActivationIndex = k;

                if (activations[r, rules[r].ResultClass] < activations[r, maxActivationIndex])
                {
                    Log.LogMessage(LOG_TAG, "Следствие правила {0} изменено с {1} на {2}", r, rules[r].ResultClass, maxActivationIndex);
                    rules[r].ResultClass = maxActivationIndex;
                }
            }
        }

        private double[] CalculateRulesPerformance(Rule[] rules, NCDataSet checkDataset)
        {
            double[] performance = new double[rules.Length];

            for (int i = 0; i < checkDataset.Length; ++i)
            {
                NCEntity entity = checkDataset[i];

                for (int r = 0; r < rules.Length; ++r)
                {
                    double activation = 1;
                    for (int j = 0; j < entity.Dimension; ++j)
                        activation = Math.Min(activation, mPartitions[j].GetMembershipVector(entity[j])[rules[r].Antecedents[j]]);
                    
                    performance[r] += activation * (rules[r].ResultClass == GetClassIndex(entity.Class) ? 1 : -1);
                }
            }

            return performance;
        }

        private Rule[] CreateRulesBest(NCDataSet trainDataset, TrainConfiguration trainConfig)
        {
            Log.LogMessage(LOG_TAG, "Генерация базы правил по алгоритму: best");
            Rule[] rules = CreateRulesFromDataset(trainDataset, 0);

            CheckRulesConnections(rules, trainDataset);

            double[] rulesPerformance = CalculateRulesPerformance(rules, trainDataset);
            double minPerformance = -trainDataset.Length - 10;

            List<Rule> resultRules = new List<Rule>();
            for (int i = 0; i < trainConfig.RuleNodesMax; ++i)
            {
                int ruleToSelect = 0;
                for (int j = 1; j < rules.Length; ++j)
                    if (rulesPerformance[ruleToSelect] < rulesPerformance[j])
                        ruleToSelect = j;

                if (rulesPerformance[ruleToSelect] != minPerformance)
                {
                    resultRules.Add(rules[ruleToSelect]);
                    rulesPerformance[ruleToSelect] = minPerformance;
                }
            }

            Log.LogMessage(LOG_TAG, "Отобрано {0} лучших правил.", resultRules.Count);
            Log.LogMessage(LOG_TAG, "База правил сгенерирована.");
            
            return resultRules.ToArray();
        }

        private Rule[] CreateRulesBestPerClass(NCDataSet trainDataset, TrainConfiguration trainConfig)
        {
            Log.LogMessage(LOG_TAG, "Генерация базы правил по алгоритму: best per class");
            Rule[] rules = CreateRulesFromDataset(trainDataset, 0);

            CheckRulesConnections(rules, trainDataset);

            double[] rulesPerformance = CalculateRulesPerformance(rules, trainDataset);
            double minPerformance = -trainDataset.Length - 10;

            List<Rule> resultRules = new List<Rule>();

            for (int i = 0; i < mClassNames.Length; ++i)
                for (int z = 0; z < trainConfig.RuleNodesMax / mClassNames.Length; ++z)
                {
                    int ruleToSelect = 0;
                    while (rules[ruleToSelect].ResultClass != i) ++ruleToSelect;

                    for (int j = 0; j < rules.Length; ++j)
                        if (rulesPerformance[ruleToSelect] < rulesPerformance[j] && rules[j].ResultClass == i)
                            ruleToSelect = j;

                    if (rulesPerformance[ruleToSelect] != minPerformance)
                    {
                        resultRules.Add(rules[ruleToSelect]);
                        rulesPerformance[ruleToSelect] = minPerformance;
                    }
                }

            int alreadySelected = resultRules.Count;
            for (int i = 0; i < trainConfig.RuleNodesMax - alreadySelected; ++i)
            {
                int ruleToSelect = 0;
                for (int j = 0; j < rules.Length; ++j)
                    if (rulesPerformance[ruleToSelect] < rulesPerformance[j])
                        ruleToSelect = j;

                if (rulesPerformance[ruleToSelect] != minPerformance)
                {
                    resultRules.Add(rules[ruleToSelect]);
                    rulesPerformance[ruleToSelect] = minPerformance;
                }
            }

            Log.LogMessage(LOG_TAG, "Отобрано {0} лучших правил.", resultRules.Count);
            Log.LogMessage(LOG_TAG, "База правил сгенерирована.");

            return resultRules.ToArray();
        }

        private void Propagate(NCEntity entity)
        {
            for (int i = 0; i < entity.Dimension; ++i)
                mInputLayer[i] = entity[i];

            double[][] membership = new double[entity.Dimension][];
            for (int i = 0; i < entity.Dimension; ++i)
                membership[i] = mPartitions[i].GetMembershipVector(entity[i]);

            for (int i = 0; i < mRules.Length; ++i)
            {
                double activation = 1;
                for (int j = 0; j < mInputLayer.Length; ++j)
                    if (membership[j][mRules[i].Antecedents[j]] < activation)
                    {
                        activation = membership[j][mRules[i].Antecedents[j]];
                        mMinAncedent[i] = j;
                    }

                mHiddenLayer[i] = activation;
            }

            for (int i = 0; i < mOutputLayer.Length; ++i)
            {
                mOutputLayer[i] = 0;
                for (int r = 0; r < mRules.Length; ++r)
                {
                    if (mRules[r].ResultClass != i)
                        continue;

                    mOutputLayer[i] = Math.Max(mOutputLayer[i], mHiddenLayer[r]);
                }
            }
        }
        
        private void AdaptByPattern(NCEntity pattern, TrainConfiguration trainConfig, out double error, out bool isCorrect)
        {
            Propagate(pattern);

            int maxIndex = 0;
            for (int i = 0; i < mOutputLayer.Length; ++i)
                if (mOutputLayer[i] > mOutputLayer[maxIndex])
                    maxIndex = i;

            isCorrect = maxIndex == GetClassIndex(pattern.Class) && mOutputLayer[maxIndex] > 0;

            double[] targetOutput = new double[mOutputLayer.Length];
            double[] deltaOut = new double[mOutputLayer.Length];

            targetOutput[GetClassIndex(pattern.Class)] = 1;
            for (int i = 0; i < mOutputLayer.Length; ++i)
                deltaOut[i] = targetOutput[i] - mOutputLayer[i];
            
            for (int r = 0; r < mRules.Length; ++r)
            {
                if (mHiddenLayer[r] > 0)
                {
                    double[] deltaHidden = new double[mHiddenLayer.Length];
                    deltaHidden[r] = deltaOut[mRules[r].ResultClass] * mHiddenLayer[r] * (1 - mHiddenLayer[r]);

                    int minAncedent = mMinAncedent[r];
                    TriangleFuzzyNumber fuzzyPart = mPartitions[minAncedent][mRules[r].Antecedents[minAncedent]];

                    double factor = deltaHidden[r] * (fuzzyPart.Right - fuzzyPart.Left);
                    double deltaB = trainConfig.OptimizationSpeed * factor * Math.Sign(mInputLayer[minAncedent] - fuzzyPart.MainValue);
                    double deltaA = -trainConfig.OptimizationSpeed * factor + deltaB;
                    double deltaC = trainConfig.OptimizationSpeed * factor + deltaB;

                    mPartitions[minAncedent].Adapt(mRules[r].Antecedents[minAncedent], deltaA, deltaB, deltaC);
                }
            }

            error = 0.0;
            for (int i = 0; i < mOutputLayer.Length; ++i)
                error += deltaOut[i] * deltaOut[i];
        }

        public int GetClassIndex(string cls)
        {
            for (int i = 0; i < mClassNames.Length; ++i)
                if (mClassNames[i] == cls)
                    return i;

            return -1;
        }

        public string GetClassName(int index)
        {
            return mClassNames[index];
        }

        public string Classify(NCEntity entity)
        {
            Propagate(entity);

            int resultClass = 0;
            for (int i = 0; i < mOutputLayer.Length; ++i)
                if (mOutputLayer[i] > mOutputLayer[resultClass])
                    resultClass = i;

            if (mOutputLayer[resultClass] > 0)
                return GetClassName(resultClass);

            return "";
        }
    }
}
