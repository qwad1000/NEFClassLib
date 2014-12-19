using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEFClassLib
{
    public struct Bounds
    {
        private double mMinValue;
        private double mMaxValue;

        public Bounds(double min, double max)
        {
            mMinValue = min;
            mMaxValue = max;
        }

        public double MinValue
        {
            get { return mMinValue; }
            set { mMinValue = value; }
        }
        public double MaxValue
        {
            get { return mMaxValue; }
            set { mMaxValue = value; }
        }
    }
}
