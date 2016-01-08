using System;
using System.Collections.Generic;
using System.Linq;
using Logging;
using NEFClassLib.FuzzyNumbers;
using NEFClassLib.Partitions;
using NEFClassLib.Solvers;

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
                TrainRulesConjuaate(trainDataset, trainConfig);
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
                err = getFullError(trainDataset, ref misClassedFinal);

                // if ((iteration + 1) % 10 == 0)
                    Log.LogMessage(LOG_TAG, "It: {0}. Error: {1}, MisClassed: {2}", iteration + 1, err.ToString("0.######"), misClassedFinal);

                // trainConfig.OptimizationSpeed *= Math.Exp(-iteration / 100);

            } while (++iteration <= trainConfig.MaxIterations && Math.Abs(err - prevErr) > trainConfig.Accuracy);
        }

        private static GaussPartition[] CopyPartitions(GaussPartition[] partitions)
        {
            GaussPartition[] newpartitions = new GaussPartition[partitions.Length];
            for (int i = 0; i < partitions.Length; i++)
                newpartitions [i] = new GaussPartition (partitions [i]);

            return newpartitions;
        }

        private void TrainRulesConjugate(NCDataSet trainDataset, TrainConfiguration trainConfig){
            int iteration = 0;

            GaussPartition[] previousIterationPartitions;
            List<int> gradientDimensions = Enumerable.Range(0, mPartitions.Length).Select(x => mPartitions[x].PartitionSize).ToList();
            int moduloDimension = gradientDimensions.Sum();//mPartitions.Length;
            GradientContainer direction_a = null, direction_b = null;// = new GradientContainer (trainDataset, mPartitions.Length, gradientDimensions, dE_dA, GetClassIndex, Propagate);

            double norm_a = 0.0, norm_b = 0.0;
            double err = 0, prevErr;
            do {
                if(iteration % moduloDimension == 0){
                    direction_a = new GradientContainer(trainDataset, mPartitions.Length, gradientDimensions, dE_dA, GetClassIndex, Propagate);
                    norm_a = direction_a.Norm();
                    direction_a.NormalizeWith(-norm_a);
                    direction_b = new GradientContainer(trainDataset, mPartitions.Length, gradientDimensions, dE_dB, GetClassIndex, Propagate);
                    norm_b = direction_b.Norm();
                    direction_b.NormalizeWith(-norm_b);
                } else {
                    GradientContainer gradient_a = new GradientContainer(trainDataset, mPartitions.Length, gradientDimensions, dE_dA, GetClassIndex, Propagate);
                    GradientContainer gradient_b = new GradientContainer(trainDataset, mPartitions.Length, gradientDimensions, dE_dB, GetClassIndex, Propagate);

                    double current_norm_a = gradient_a.Norm();
                    double current_norm_b = gradient_b.Norm();
                    double beta_a = Math.Pow(current_norm_a, 2) / Math.Pow(norm_a, 2);
                    double beta_b = Math.Pow(current_norm_b, 2) / Math.Pow(norm_b, 2);


                    gradient_a.EqualSum(-1.0, beta_a, direction_a);
                    gradient_b.EqualSum(-1.0, beta_b, direction_b);
                    gradient_a.Normalize();
                    gradient_b.Normalize();

                    direction_a = gradient_a;
                    direction_b = gradient_b;
                    norm_a = current_norm_a;
                    norm_b = current_norm_b;
                }


                previousIterationPartitions = mPartitions;

                prevErr = err;
                err = 0;

                Func<double, double> errorFunc = alpha => {
                    mPartitions = CopyPartitions(previousIterationPartitions);
                    WeightsEqPlusGrad(alpha, direction_a, direction_b);

                    int _misclassed = 0;
                    return getFullError(trainDataset, ref _misclassed);
                };

                double resultalpha = GoldenEquationSolver.Solve(errorFunc, -1.0, 1.0);

                mPartitions = previousIterationPartitions;
                WeightsEqPlusGrad(resultalpha, direction_a, direction_b);
                int misclassed = 0;
                err = getFullError(trainDataset, ref misclassed);

                Log.LogMessage(LOG_TAG, "It: {0}. Error: {1}, MisClassed: {2}, alpha: {3}", iteration, err.ToString("0.######"), misclassed, resultalpha.ToString("0.#####"));
            } while(++iteration <= trainConfig.MaxIterations && Math.Abs (err - prevErr) > trainConfig.Accuracy);
        }

        private void WeightsEqPlusGrad(double alpha, GradientContainer gradA, GradientContainer gradB)
        {
            for (int i = 0; i < mPartitions.Length; i++) {
                for (int j = 0; j < mPartitions[i].PartitionSize; j++) {
                    double deltaA = alpha * gradA.arr[i][j];
                    double deltaB = alpha * gradB.arr[i][j];

                    mPartitions[i].Adapt(j, deltaA, deltaB);
                }
            }
        }

        private double getFullError(NCDataSet dataset, ref int misClassed)
        {
            double err = 0.0;
            for (int i=0; i<dataset.Length; ++i) {
                NCEntity entity = dataset [i];

                string result = Classify (entity);
                bool isCorrect = (result == entity.Class);

                if (!isCorrect)
                    ++misClassed;

                double[] targetOutput = new double[mOutputLayer.Length];
                targetOutput[GetClassIndex(entity.Class)] = 1.0;

                for (int z = 0; z < mOutputLayer.Length; ++z)
                    err += Math.Pow(mOutputLayer[z] - targetOutput[z], 2);
            }
            return err /= dataset.Length;
        }

        private void AdaptByPattern(NCEntity pattern, TrainConfiguration config)
        {
            Propagate(pattern);

            for (int i = 0; i < mPartitions.Length; ++i)
            {
                for (int j = 0; j < mPartitions[i].PartitionSize; ++j)
                {
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
                var v = mRules [maxInd].Antecedents [i] == j ? mHiddenLayer [maxInd] : 0;
                result += (targetOutput[k] - mOutputLayer[k]) * mConclusionWeights[maxInd] * v; 
            }

            result *= (-2) * (mInputLayer[i] - mPartitions[i][j].A) / Math.Pow (mPartitions[i][j].B, 2);

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
                var v = mRules [maxInd].Antecedents [i] == j ? 1 : 0; // TODO: check hiddenlayer not 1;
                result += (targetOutput[k] - mOutputLayer[k]) * mConclusionWeights[maxInd] * v;
            }

            result *= (-2) * Math.Pow(mInputLayer[i] - mPartitions[i][j].A, 2) / Math.Pow(mPartitions[i][j].B, 3);

            return result;
        }

        private double dE_dW(int j, int k, int resultClass)
        {
            return (-2) * ((resultClass == k ? 1 : 0) - mOutputLayer[k]) * mHiddenLayer[j];
        }
    }
}
