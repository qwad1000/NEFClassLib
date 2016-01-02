using System;
using Logging;
using NEFClassLib.Partitions;
using NEFClassLib.FuzzyNumbers;

namespace NEFClassLib
{
    public class NEFClassNetwork : BaseNEFClassNetwork<Partition, TriangleFuzzyNumber>
    {
        private const string LOG_TAG = "NEFClassNetwork";
        private int[] mMinAncedent;

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
            double err = 0, prevErr = 0;
            do
            {
                prevErr = err;
                err = 0;
                int misClassed = 0;
                for (int i = 0; i < trainDataset.Length; ++i)
                {
                    bool isCorrect = true;
                    double patternError = 0;
                    AdaptByPattern(trainDataset[i], trainConfig, out patternError, out isCorrect);

                    err += patternError;
                    if (!isCorrect)
                        ++misClassed;
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
            } while (++iteration <= trainConfig.MaxIterations && Math.Abs(err - prevErr) > trainConfig.Accuracy);
        }
        
        private void AdaptByPattern(NCEntity pattern, TrainConfiguration trainConfig, out double error, out bool isCorrect)
        {
            Propagate(pattern);

            int maxIndex = 0;
            for (int i = 0; i < mOutputLayer.Length; ++i)
                if (mOutputLayer[i] > mOutputLayer[maxIndex])
                    maxIndex = i;

            isCorrect = (GetClassName(maxIndex) == pattern.Class) && (mOutputLayer[maxIndex] > 0);

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

        protected override void Propagate(NCEntity entity)
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
    }
}
