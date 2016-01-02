using System;
using NEFClassLib.FuzzyNumbers;

namespace NEFClassLib.Partitions
{
    public class Partition : IPartition<TriangleFuzzyNumber>
    {
        private Bounds mBounds;
        private TriangleFuzzyNumber[] mFuzzyParts;

        public Partition(Bounds bounds, int fuzzyPartsCount)
        {
            mBounds = bounds;
            mFuzzyParts = new TriangleFuzzyNumber[fuzzyPartsCount];

            double width = (bounds.MaxValue - bounds.MinValue) / (fuzzyPartsCount + 1);
            for (int i = 0; i < fuzzyPartsCount; ++i)
                mFuzzyParts[i] = new TriangleFuzzyNumber(bounds.MinValue + i * width, bounds.MinValue + (i + 1) * width, bounds.MinValue + (i + 2) * width, i == 0, i == fuzzyPartsCount - 1);
        }

        public double[] GetMembershipVector(double x)
        {
            int size = mFuzzyParts.Length;

            double[] result = new double[size];
            for (int i = 0; i < size; ++i)
                result[i] = mFuzzyParts[i].GetMembership(x);
            
            return result;
        }

        public double GetMaxMembership(double x)
        {
            double result = 0.0;

            int size = mFuzzyParts.Length;
            for (int i = 0; i < size; ++i)
                result = Math.Max(mFuzzyParts[i].GetMembership(x), result);

            return result;
        }

        public int GetMaxMembershipIndex(double x)
        {
            int maxIndex = 0;

            int size = mFuzzyParts.Length;
            for (int i = 1; i < size; ++i)
                if (mFuzzyParts[i].GetMembership(x) > mFuzzyParts[maxIndex].GetMembership(x))
                    maxIndex = i;

            return maxIndex;
        }

        public void Adapt(int index, double deltaA, double deltaB, double deltaC)
        {
            if ((mFuzzyParts[index].Left + deltaA) < mBounds.MinValue) deltaA = 0.0;
            if ((mFuzzyParts[index].Right + deltaC) > mBounds.MaxValue) deltaC = 0.0; 
            
            mFuzzyParts[index].Adapt(deltaA, deltaB, deltaC);
        }

        public int PartitionSize
        {
            get { return mFuzzyParts.Length; }
        }

        public TriangleFuzzyNumber this[int index]
        {
            get { return mFuzzyParts[index]; }
        }
    }
}
