using System;
using System.Collections.Generic;

namespace NEFClassLib.Solvers
{
    public class GradientContainer
    {
        public double[][] arr; //TODO;

        public GradientContainer(NCDataSet trainDataSet,
            int iCount, List<int> jCounts,
            Func<int, int, int, double> localGradientFunction,
            Func<string, int> getClassIndexFunction,
            Action<NCEntity> propagateFunction
        )
        {
            arr = new double[iCount][];
            for(int i=0; i<iCount; i++)
                arr[i] = new double[ jCounts[i] ];

            for (int patternIndex = 0; patternIndex < trainDataSet.Length; patternIndex++) {
                NCEntity pattern = trainDataSet[patternIndex];
                propagateFunction(pattern);
                for (int i = 0; i < iCount; i++) {
                    int jCount = jCounts[i];
                    // arr[i] = new double[jCount];
                    for (int j = 0; j < jCount; j++) {
                        arr[i][j] += localGradientFunction(i, j, getClassIndexFunction(pattern.Class));
                    }
                }
            }
        }

        public GradientContainer(GradientContainer other)
        {
            arr = new double[other.arr.Length][];
            for (int i = 0; i < other.arr.Length; i++) {
                arr[i] = (double[]) other.arr[i].Clone();
            }
        }

        public double Norm()
        {
            double norm = 0.0;
            for (int i = 0; i < arr.Length; i++) {
                for (int j = 0; j < arr[i].Length; j++) {
                    norm += Math.Pow(arr[i][j], 2);
                }
            }
            return Math.Sqrt(norm);
        }

        public void Normalize() // be careful. returns norm with wat was normalized
        {
            double norm = Norm();
            NormalizeWith(norm);
        }

        public void NormalizeWith(double norm)
        {
            for (int i = 0; i < arr.Length; i++)
                for (int j = 0; j < arr[i].Length; j++)
                    arr[i][j] /= norm;
        }

        public void EqualSum(double alpha, double beta, GradientContainer other)
        {
            var arr2 = arr;
            for (int i = 0; i < arr.Length; i++) {
                for (int j = 0; j < arr[i].Length; j++) {
                    arr2[i][j] += alpha*arr[i][j] + beta*other.arr[i][j];
                }
            }
        }

    }
}

