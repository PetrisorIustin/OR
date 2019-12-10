using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex
{
    public class DualSimplex
    {
        private double[,] t;
        private readonly int m;
        private readonly int n;
        public DualSimplex(double[,] table)
        {
            t = table;
            m = t.GetLength(0) - 1;
            n = t.GetLength(1) - 1;
        }
        public void Solve()
        {
            while (LessThanZero())
            {
                Console.WriteLine("Matrice sistem");
                PrintSolution();
                for (var k = 0; k < m; k++)
                {
                    if (t[k, n] < 0)
                    {
                        if (AllColumnsAreZero(k))
                        {
                            Console.WriteLine("The problem has no optimum solution - and its dual is unbounded.");
                            return;
                        }
                        int l = FindMin(k);

                        for (var i = 0; i <= m; i++)
                        {
                            if (i == k)
                            {
                                continue;
                            }
                            for (var j = 0; j <= n; j++)
                            {
                                if (j == l)
                                {
                                    continue;
                                }
                                t[i, j] = (t[i, j] * t[k, l] - t[i, l] * t[k, j]) / t[k, l];
                            }
                        }


                        for (var i = 0; i <= m; i++)
                        {
                            if (i == k)
                            {
                                continue;
                            }
                            t[i, l] = 0;
                        }

                        for (var j = 0; j <= n; j++)
                        {
                            if (j == l)
                            {
                                continue;
                            }
                            t[k, j] = t[k, j] / t[k, l];
                        }
                        t[k, l] = 1;
                        break;
                    }
                }
            }
        }

        public double[,] Solution()
        {
            return t;
        }

        private bool LessThanZero()
        {
            for (var i = 0; i < m; i++)
            {
                if (t[i, n] < 0)
                {
                    return true;
                }
            }
            return false;
        }

        private bool AllColumnsAreZero(int k)
        {
            for (var i = 0; i < n; i++)
            {
                if (t[k, i] < 0)
                {
                    return false;
                }
            }
            return true;
        }

        private int FindMin(int k)
        {
            int index = 0;
            double min = int.MaxValue;
            for (var l = 0; l < n; l++)
            {
                if(t[k, l] == 0)
                {
                    continue;
                }
                var value = Math.Abs(t[m, l] / t[k, l]);
                if (min > value && value > 0)
                {
                    index = l;
                    min = value;
                }
            }
            Console.WriteLine($"{k},{index},min= {min}");

            return index;

        }


        public void PrintSolution()
        {
            for (int i = 0; i < t.GetLength(0); i++)
            {
                for (int j = 0; j < t.GetLength(1); j++)
                {
                    Console.Write(t[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }
    }
}
