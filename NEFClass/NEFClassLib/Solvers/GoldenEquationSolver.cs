using System;

namespace NEFClassLib.Solvers
{
    public static class GoldenEquationSolver
    {
        private readonly static double GR = (Math.Sqrt(5)-1)/2;
        public static double Solve(Func<double, double> function, double a, double b, double tolerance = 1e-5) {
            double c, d, fc, fd;
            c = b - GR * (b - a);
            d = a + GR * (b - a);
            while (Math.Abs (c - d) > tolerance) {
                fc = function (c);
                fd = function (d);
                if (fc < fd) {
                    b = d;
                    d = c;  // fd=fc;fc=f(c)
                    c = b - GR * (b - a);
                } else {
                    a = c;
                    c = d;
                    d = a + GR * (b - a);
                }
            }
            return (b + a) / 2;
        }
    }
}

