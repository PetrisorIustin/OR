using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex
{
    public class Simplex
    {
        private double[,] t;
        private readonly int m;
        private readonly int n;
        public Simplex(double[,] table)
        {
            t = table;
            m = t.GetLength(0) - 1;
            n = t.GetLength(1) - 1;
        }
        public void Solve()
        {
            while (LessThanZero())
            {
                for (var l = 0; l < n; l++)
                {
                    if (t[m, l] < 0)
                    {
                        if (AllColumnsAreZero(l))
                        {
                            Console.WriteLine("The problem has no optimum solution (although it has feasible solutions), i.e. the objective functions is unbounded");
                            return;
                        }
                        int k = FindMin(l);

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
            for (var i = 0; i < n; i++)
            {
                if (t[m, i] < 0)
                {
                    return true;
                }
            }
            return false;
        }

        private bool AllColumnsAreZero(int h)
        {
            for (var i = 0; i < m - 1; i++)
            {
                if (t[h, i] > 0)
                {
                    return false;
                }
            }
            return true;
        }

        private int FindMin(int l)
        {
            int index = 0;
            double min = int.MaxValue;
            for (var k = 0; k < m; k++)
            {
                var value = t[k, n] / t[k, l];
                if (min > value && value > 0)
                {
                    index = k;
                    min = value;
                }
            }
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
