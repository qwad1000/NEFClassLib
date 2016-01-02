using System;
using System.Collections.Generic;
using Logging;
using NEFClassLib.Partitions;
using NEFClassLib.FuzzyNumbers;

namespace NEFClassLib
{
    public class NEFClassMNetwork : BaseNEFClassNetwork<GaussPartition, GaussFuzzyNumber>
    {
        private const string LOG_TAG = "NEFClassMNetwork";
        private double[] mConclusionWeights;
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

			for (int i = 0; i < mOutputLayer.Length; ++i)
            {
                int maxInd = mMaxIndices[i];
                double deltaW = -config.OptimizationSpeed * 0.1 * dE_dW(maxInd, i, GetClassIndex(pattern.Class));
                mConclusionWeights[maxInd] += deltaW;
            }
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

		protected override void CreateRulesBase(NCDataSet trainDataset, TrainConfiguration trainConfig)
		{
			base.CreateRulesBase(trainDataset, trainConfig);

			Console.WriteLine("CreateRulesBase from NEFClassMNetwork");
			mConclusionWeights = new double[mRules.Length];
			for (int i = 0; i < mConclusionWeights.Length; ++i)
				mConclusionWeights[i] = 1.0;
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
    }
}
