using System;
using NEFClassLib.FuzzyNumbers;

namespace NEFClassLib.Partitions
{
    public class GaussPartition : IPartition<GaussFuzzyNumber>
    {
        // private Bounds mBounds;
        private GaussFuzzyNumber[] mFuzzyParts;

        public GaussPartition(Bounds bounds, int fuzzyPartsCount)
        {
            // mBounds = bounds;
            mFuzzyParts = new GaussFuzzyNumber[fuzzyPartsCount];

            double b1 = (bounds.MaxValue - bounds.MinValue) / (fuzzyPartsCount + 1);
            double b = b1 / Math.Sqrt(2 * Math.Log(2));
            for (int i = 0; i < fuzzyPartsCount; ++i)
                mFuzzyParts[i] = new GaussFuzzyNumber(bounds.MinValue + (i + 1) * b1, b, i == 0, i == fuzzyPartsCount - 1);
        }

        public GaussPartition(GaussPartition other)
        {
            int fuzzyPartsCount = other.mFuzzyParts.Length;
            mFuzzyParts = new GaussFuzzyNumber[fuzzyPartsCount];
            for (int i = 0; i < fuzzyPartsCount; i++)
                mFuzzyParts[i] = new GaussFuzzyNumber(other.mFuzzyParts[i]);
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

        public void Adapt(int index, double deltaA, double deltaB)
        {
            mFuzzyParts[index].Adapt(deltaA, deltaB);
        }

        public int PartitionSize
        {
            get { return mFuzzyParts.Length; }
        }

        public GaussFuzzyNumber this[int index]
        {
            get { return mFuzzyParts[index]; }
        }
    }
}
