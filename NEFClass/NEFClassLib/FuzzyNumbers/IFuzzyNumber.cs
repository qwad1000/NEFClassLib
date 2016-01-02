using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEFClassLib.FuzzyNumbers
{
    public interface IFuzzyNumber
    {
        double GetMembership(double x);
    }
}
