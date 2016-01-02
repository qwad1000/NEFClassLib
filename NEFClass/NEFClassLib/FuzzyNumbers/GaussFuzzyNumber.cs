using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEFClassLib.FuzzyNumbers
{
    public class GaussFuzzyNumber : IFuzzyNumber
    {
        private const double MIN_WIDTH = 0.1;

        private double mA;
        private double mB;

        private Boolean mLeftShouldered;
        private Boolean mRightShouldered;

        public GaussFuzzyNumber(double a, double b, bool leftShouldered, bool rightShouldered)
        {
            mA = a;
            mB = b;

            mLeftShouldered = leftShouldered;
            mRightShouldered = rightShouldered;
        }

        public double GetMembership(double x)
        {
            if (mLeftShouldered && x <= mA)
                return 1;

            if (mRightShouldered && x >= mA)
                return 1;

            return Math.Exp(-(x - mA) * (x - mA) / (2 * mB * mB));
        }

        public void Adapt(double deltaA, double deltaB)
        {
            mA += deltaA;

            if (mB + deltaB <= 0)
                deltaB = 0;
            mB += deltaB;
        }

        public double A
        {
            get { return mA; }
            set { mA = value; }
        }

        public double B
        {
            get { return mB; }
            set { mB = value; }
        }
    }
}
