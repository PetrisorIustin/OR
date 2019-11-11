using System;
using System.Linq;

namespace Simplex
{
    class Program
    {

        static void Main(string[] args)
        {
            var o1 = new Options() {
                GreaterThanEq = new EqType[] { EqType.Equal, EqType.GreatherThan, EqType.LessThan},
                Table = new double[,] {
                { 3, 2, 14},
                { 2, -4, 2 },
                { 4, 3, 19 },
            } };

            var s1 = new TwoPhaseSimplex(o1);
            s1.Solve();
            //s1.PrintSolution();


            Console.ReadLine();
        }

    }
}
