using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEFClassLib.FuzzyNumbers
{
    public class TriangleFuzzyNumber : IFuzzyNumber
    {
        private const double MIN_WIDTH = 0.1;

        private double mLeft;   
        private double mMainValue;
        private double mRight;

        private Boolean mLeftShouldered;
        private Boolean mRightShouldered;

        public TriangleFuzzyNumber(double left, double main, double right, bool leftShouldered, bool rightShouldered)
        {
            mLeft = left;
            mMainValue = main;
            mRight = right;

            mLeftShouldered = leftShouldered;
            mRightShouldered = rightShouldered;
        }

        public double GetMembership(double x)
        {
            if (mLeftShouldered && x <= mMainValue)
                return 1;

            if (mRightShouldered && x >= mMainValue)
                return 1;

            if (x <= mLeft || x >= mRight)
                return 0;

            if (x <= mMainValue)
                return (x - mLeft) / (mMainValue - mLeft);
            else
                return (mRight - x) / (mRight - mMainValue);
        }

        public void Adapt(double deltaA, double deltaB, double deltaC)
        {
            if (mLeftShouldered || (mLeft + deltaA) > (mMainValue - MIN_WIDTH)) deltaA = 0.0;
            if (mRightShouldered || (mRight + deltaC) < (mMainValue + MIN_WIDTH)) deltaC = 0.0;
            if ((mLeft + deltaA + MIN_WIDTH) > (mMainValue + deltaB) ||
                (mRight + deltaC - MIN_WIDTH) < (mMainValue + deltaB)) deltaB = 0.0; 

            mLeft += deltaA;
            mMainValue += deltaB;
            mRight += deltaC;
        }

        public double Left
        {
            get { return mLeft; }
            set { mLeft = value; }
        }

        public double Right
        {
            get { return mRight; }
            set { mRight = value; }
        }

        public double MainValue
        {
            get { return mMainValue; }
            set { mMainValue = value; }
        }
    }
}
