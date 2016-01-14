using Logging;
using System;
using System.Collections.Generic;
using NEFClassLib.Partitions;
using NEFClassLib.FuzzyNumbers;

namespace NEFClassLib
{
    public abstract class BaseNEFClassNetwork <PartitionType, NumberType> : INEFClass
        where PartitionType : IPartition<NumberType> 
        where NumberType : IFuzzyNumber

    {
        private const string LOG_TAG = "BaseNEFClassNetwork";

        protected double[] mInputLayer;
        protected double[] mHiddenLayer;
        protected double[] mOutputLayer;

        protected PartitionType[] mPartitions;

        protected Rule[] mRules;
        protected string[] mClassNames;

        protected virtual void CreateRulesBase(NCDataSet trainDataset, TrainConfiguration trainConfig)
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

        protected Rule[] CreateRulesFromDataset(NCDataSet trainDataset, int maxRules)
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

        protected abstract void Propagate(NCEntity entity);

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
    }
}
