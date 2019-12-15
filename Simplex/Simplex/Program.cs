using System;
using System.Collections.Generic;
using System.Linq;

namespace Simplex
{
    class Program
    {

        static void Main(string[] args)
        {
            // DualSimplex();
            BranchAndBound();

            Console.ReadLine();
        }



        static void DualSimplex()
        {
            //var s1 = new DualSimplex(new double[,] {
            //        { -2, 2, 5, 4, 1, 0, 0, -25 },
            //        { 7, 2, 6, -2, 0, 1, 0, 35 },
            //        { -4, -5, 3, 2, 0, 0, 1, -15 },
            //        { 2, 2, 4, 5, 0, 0, 0, 0 }
            //    }
            //    );
            //s1.Solve();


            var s2 = new DualSimplex(new double[,] {
                    { -1, -3, -1, 1, 0, 0, -3 },
                    { -2, 0, -4, 0, 1, 0, -3 },
                    { -1, -2, 0, 0, 0, 1, -5 },
                    { 430, 450, 420, 0, 0, 0, 0 }
                }
                );
            s2.Solve();
            s2.PrintSolution();
        }



        static void TwoPhase()
        {
            var o1 = new Options()
            {
                Constraints = new List<EqType> { EqType.LessThan, EqType.LessThan },
                Table = new double[,] {
                { 1, -2, 2, 6},
                { 1, 1, 2, 8},

            },
                Z = new double[] { 1, -1, 2 }

            };
            //var s1 = new TwoPhaseSimplex(o1);
            //s1.Solve();



            var o_a = new Options()
            {
                Constraints = new List<EqType> { EqType.GreatherThan, EqType.LessThan },
                Table = new double[,] {
                { 1, 1, 6},
                { 2, 3, 4 }
            },
                Z = new double[] { -1, 0 }
            };
            var s_a = new TwoPhaseSimplex(o_a);
            //s_a.Solve();



            var o_b = new Options()
            {
                Constraints = new List<EqType> { EqType.Equal, EqType.Equal },
                Table = new double[,] {
                { 2, 1, 1, 4},
                { 1, 1, 2, 2 }
            },
                Z = new double[] { 1, 1, 0 }
            };
            var s_b = new TwoPhaseSimplex(o_b);
            s_b.Solve();


            var o_c = new Options()
            {
                Constraints = new List<EqType> { EqType.Equal, EqType.Equal, EqType.Equal },
                Table = new double[,] {
                { 1, 2, -1, 1, 0},
                { 2, -2, 3, 3, 9},
                { 1, -1, 2, -1, 6},
            },
                Z = new double[] { -3, 1, 3, -1 }
            };
            var s_c = new TwoPhaseSimplex(o_c);
            //s_c.Solve();


            var o_4 = new Options()
            {
                Constraints = new List<EqType> { EqType.GreatherThan, EqType.LessThan },
                Table = new double[,] {
                { 1, 1, 2},
                { 2, 2, 4},
            },
                Z = new double[] { 1, 2 }
            };
            var s_4 = new TwoPhaseSimplex(o_4);
            //s_4.Solve();
        }


        static void BranchAndBound()
        {
            var o = new Options()
            {
                Constraints = new List<EqType> { EqType.LessThan, EqType.LessThan },
                Table = new double[,] {
                { 10, 3, 52},
                { 2, 3, 18},

            },
                Z = new double[] { 5, 6 }

            };
            var s = new BranchAndBound.BranchAndBound(o);
            s.Solve();

            var o1 = new Options()
            {
                Constraints = new List<EqType> { EqType.LessThan, EqType.LessThan },
                Table = new double[,] {
                { 1, -2, 2, 6},
                { 1, 1, 2, 8},

            },
                Z = new double[] { 1, -1, 2 }

            };


            var s1 = new BranchAndBound.BranchAndBound(o1);
            //s1.Solve();

            var o2 = new Options()
            {
                Constraints = new List<EqType> { EqType.LessThan, EqType.LessThan },
                Table = new double[,] {
                { -1, 1, 0},
                { 6, 2, 21},

            },
                Z = new double[] { 2, 1 }

            };


            var s2 = new BranchAndBound.BranchAndBound(o2);
            //s2.Solve();



            var o3 = new Options()
            {
                Constraints = new List<EqType> { EqType.LessThan, EqType.LessThan, EqType.LessThan },
                Table = new double[,] {
                { 1, 2, 1, -1, 8},
                { 1, 2, 0, 2, 7},
                { 4, 1, 3, 0, 8},

            },
                Z = new double[] { 1, 1, 1, 2 }

            };


            //var s3 = new BranchAndBound.BranchAndBound(o3);
            //s3.Solve();
        }
    }
}

