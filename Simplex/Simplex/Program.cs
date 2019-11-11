﻿using System;
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
                
            },
                Z = new double []{ 2, 3 }

            };
            //var s1 = new TwoPhaseSimplex(o1);
            //s1.Solve();



            var o_a = new Options()
            {
                GreaterThanEq = new EqType[] { EqType.GreatherThan, EqType.LessThan },
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
                GreaterThanEq = new EqType[] { EqType.Equal, EqType.Equal},
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
                GreaterThanEq = new EqType[] { EqType.Equal, EqType.Equal, EqType.Equal},
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
                GreaterThanEq = new EqType[] { EqType.GreatherThan, EqType.LessThan },
                Table = new double[,] {
                { 1, 1, 2},
                { 2, 2, 4},
            },
                Z = new double[] { 1, 2 }
            };
            var s_4 = new TwoPhaseSimplex(o_4);
            //s_4.Solve();

            Console.ReadLine();
        }

    }
}

