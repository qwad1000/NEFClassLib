using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;

namespace NEFClassLib
{
    public class NEFClassMNetwork
    {
        private const string LOG_TAG = "NEFClassMNetwork";

        private double[] mInputLayer;
        private double[] mHiddenLayer;
        private double[] mOutputLayer;

        private GaussPartition[] mPartitions;
        private Rule[] mRules;
        private double[] mConclusionWeights;
        private string[] mClassNames;

        private int[] mMaxIndices;

        public NEFClassMNetwork(NCDataSet trainDataset, TrainConfiguration trainConfig)
        {
            Log.LogMessage(LOG_TAG, "Начало обучения.");

            mClassNames = trainDataset.GetClassesList();

            mInputLayer = new double[trainDataset.Dimension];
            mOutputLayer = new double[mClassNames.Length];
            mMaxIndices = new int[mClassNames.Length];

            mPartitions = new GaussPartition[trainDataset.Dimension];
            for (int i = 0; i < trainDataset.Dimension; ++i)
                mPartitions[i] = new GaussPartition(trainDataset.GetAttributeBounds(i), trainConfig.FuzzyPartsCount[i]);

            CreateRulesBase(trainDataset, trainConfig);
            mHiddenLayer = new double[mRules.Length];

            if (trainConfig.DoOptimization)
                TrainRules(trainDataset, trainConfig);
        }

        private void TrainRules(NCDataSet trainDataset, TrainConfiguration trainConfig)
        {
            int iteration = 0;
            double err = 0, prevErr = 0;
            do
            {
                prevErr = err; err = 0;
                List<int> indices = new List<int>();
                for (int i = 0; i < trainDataset.Length; ++i)
                    indices.Add(i);

                Random random = new Random();
                for (int i = 0; i < trainDataset.Length; ++i)
                {
                    int patternIndex = random.Next(indices.Count);
                    NCEntity pattern = trainDataset[patternIndex];
                    indices.Remove(patternIndex);

                    AdaptByPattern(pattern, trainConfig);
                }

                int misClassedFinal = 0;
                for (int i = 0; i < trainDataset.Length; ++i)
                {
                    string result = Classify(trainDataset[i]);
                    bool isCorrect = (result == trainDataset[i].Class);

                    if (!isCorrect)
                        ++misClassedFinal;

                    double[] targetOutput = new double[mOutputLayer.Length];
                    targetOutput[GetClassIndex(trainDataset[i].Class)] = 1.0;

                    for (int z = 0; z < mOutputLayer.Length; ++z)
                        err += (mOutputLayer[z] - targetOutput[z]) * (mOutputLayer[z] - targetOutput[z]);
                }

                err /= trainDataset.Length;
                if ((iteration + 1) % 10 == 0)
                    Log.LogMessage(LOG_TAG, "It: {0}. Error: {1}, MisClassed: {2}", iteration + 1, err.ToString("0.######"), misClassedFinal);

                // trainConfig.OptimizationSpeed *= Math.Exp(-iteration / 100);

            } while (++iteration <= trainConfig.MaxIterations && Math.Abs(err - prevErr) > trainConfig.Accuracy);
        }

        private double dE_dA(int i, int j, int resultClass)
        {
            double result = 0;

            double[] targetOutput = new double[mOutputLayer.Length];
            targetOutput[resultClass] = 1.0;

            for (int k = 0; k < mOutputLayer.Length; ++k)
            {
                int maxInd = mMaxIndices[k];
                result += (targetOutput[k] - mOutputLayer[k]) * mConclusionWeights[maxInd] * (mRules[maxInd].Antecedents[i] == j ? mHiddenLayer[maxInd] : 0);
            }

            result *= (-2) * (mInputLayer[i] - mPartitions[i][j].A) / (mPartitions[i][j].B * mPartitions[i][j].B);

            return result;
        }

        private double dE_dB(int i, int j, int resultClass)
        {
            double result = 0;

            double[] targetOutput = new double[mOutputLayer.Length];
            targetOutput[resultClass] = 1.0;

            for (int k = 0; k < mOutputLayer.Length; ++k)
            {
                int maxInd = mMaxIndices[k];
                result += (targetOutput[k] - mOutputLayer[k]) * mConclusionWeights[maxInd] * (mRules[maxInd].Antecedents[i] == j ? 1 : 0);
            }

            result *= (-2) * (mInputLayer[i] - mPartitions[i][j].A) * (mInputLayer[i] - mPartitions[i][j].A) / (mPartitions[i][j].B * mPartitions[i][j].B * mPartitions[i][j].B);

            return result;
        }

        private double dE_dW(int j, int k, int resultClass)
        {
            return (-2) * ((resultClass == k ? 1 : 0) - mOutputLayer[k]) * mHiddenLayer[j];
        }

        private void AdaptByPattern(NCEntity pattern, TrainConfiguration config)
        {
            Propagate(pattern);

            for (int i = 0; i < mPartitions.Length; ++i)
            {
                for (int j = 0; j < mPartitions[i].PartitionSize; ++j)
                {
                    GaussFuzzyNumber fuzzyNumber = mPartitions[i][j];

                    double deltaA = -config.OptimizationSpeed * dE_dA(i, j, GetClassIndex(pattern.Class));
                    double deltaB = -config.OptimizationSpeed * dE_dB(i, j, GetClassIndex(pattern.Class));

                    mPartitions[i].Adapt(j, deltaA, deltaB);
                }
            }

            Propagate(pattern);

            for (int k = 0; k < mOutputLayer.Length; ++k)
            {
                int maxInd = mMaxIndices[k];
                double deltaW = -config.OptimizationSpeed * 0.1 * dE_dW(maxInd, k, GetClassIndex(pattern.Class));
                mConclusionWeights[maxInd] += deltaW;
            }
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

            mConclusionWeights = new double[mRules.Length];
            for (int i = 0; i < mConclusionWeights.Length; ++i)
                mConclusionWeights[i] = 1.0;
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

        public double[] GetOutput(NCEntity entity)
        {
            Propagate(entity);
            return mOutputLayer;
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
                    activation *= membership[j][mRules[i].Antecedents[j]];

                mHiddenLayer[i] = activation;
            }

            for (int i = 0; i < mOutputLayer.Length; ++i)
            {
                mOutputLayer[i] = 0;
                mMaxIndices[i] = 0;
                for (int r = 0; r < mRules.Length; ++r)
                {
                    if (mRules[r].ResultClass != i)
                        continue;

                    if (mConclusionWeights[mMaxIndices[i]] * mHiddenLayer[mMaxIndices[i]] < mConclusionWeights[r] * mHiddenLayer[r])
                    {
                        mMaxIndices[i] = r;
                        mOutputLayer[i] = mConclusionWeights[r] * mHiddenLayer[r];
                    }
                }
            }
        }
    }
}
