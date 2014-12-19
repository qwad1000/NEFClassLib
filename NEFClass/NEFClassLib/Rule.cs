using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEFClassLib
{
    public class Rule
    {
        private int[] mAntecedents;
        private int mResultClass;

        public Rule(int[] antecedents, int resultClass)
        {
            mAntecedents = new int[antecedents.Length];
            for (int i = 0; i < antecedents.Length; ++i)
                mAntecedents[i] = antecedents[i];

            mResultClass = resultClass;
        }

        public int[] Antecedents
        {
            get { return mAntecedents; }
        }

        public int ResultClass
        {
            get { return mResultClass; }
            set { mResultClass = value; }
        }
    }
}
