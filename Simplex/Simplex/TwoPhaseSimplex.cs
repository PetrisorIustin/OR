using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simplex
{
    public class Options
    {
        public EqType[] GreaterThanEq { get; set; }
        public double[,] Table { get; set; }
        public double[] z { get; set; }

    }

    public enum EqType
    {
        GreatherThan,
        LessThan,
        Equal
    }

    public enum VarType
    {
        Primary,
        Slack,
        Artifical,
        SlackArtifical,
        RHS
    }







    public class TwoPhaseSimplex
    {
        private double[,] initialA;
        private readonly EqType[] eqTypes;
        public List<VarType> ColumnVarTypes { get; set; }
        public List<VarType> RowVarTypes { get; set; }
        private readonly int m;
        private readonly int n;
        private readonly double[] z;
        public TwoPhaseSimplex(Options options)
        {
            initialA = options.Table;
            eqTypes = options.GreaterThanEq;
            m = initialA.GetLength(0) - 1;
            n = initialA.GetLength(1) - 1;
            ColumnVarTypes = Enumerable.Repeat(VarType.Primary, n).ToList();
            RowVarTypes = new List<VarType>();
        }

        

        public void Solve()
        {
            var A1 = AddAuxiliarVariables();

            //_PrintSolution(A1);
            var lastRow = CalculateNewZ(A1);
            var A2 = Utils.AddRow(A1, lastRow);
            //_PrintSolution(A2, VarTypes);
            //_PrintSolution(A2);
            var sol = Phase1(A2, A2.GetLength(0) - 1, A2.GetLength(1) - 1);
            if(sol[sol.GetUpperBound(0), sol.GetUpperBound(1)] != 0)
            {
                Console.WriteLine("The problem has no feasable solution");
                return;
            }
            else
            {
                Console.WriteLine("The problem has feasable solution");
                if(ColumnVarTypes.Count(x => x == VarType.Artifical) == 0)
                {
                    var afterPhaseOneA = CalculateAAfterPhaseOne(sol);
                    var finalSolution = Phase1(afterPhaseOneA, afterPhaseOneA.GetLength(0) - 1, afterPhaseOneA.GetLength(1) - 1);
                }
            }
            _PrintSolution(sol, ColumnVarTypes, RowVarTypes);
        }

        private double[,] CalculateAAfterPhaseOne(double[,] sol)
        {
            throw new NotImplementedException();
        }

        private bool StillArtificialVariablesInBase()
        {
            return ColumnVarTypes.Count(x => x == VarType.Artifical) == 0;
        }

        public double[,] Phase1(double [,] initialA, int m, int n)
        {
            while (LessThanZero(initialA, m, n))
            {
                _PrintSolution(initialA, ColumnVarTypes, RowVarTypes);
                for (var l = 0; l < n; l++)
                {
                    if (initialA[m, l] < 0)
                    {
                        if (AllColumnsAreZero(initialA, l, m, n))
                        {
                            Console.WriteLine("The problem has no optimum solution (although it has feasible solutions), i.e. the objective functions is unbounded");
                            return initialA;
                        }
                        int k = FindMin(initialA, l, m, n);
                        RowVarTypes[k] = ColumnVarTypes[l];

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
                                initialA[i, j] = (initialA[i, j] * initialA[k, l] - initialA[i, l] * initialA[k, j]) / initialA[k, l];
                            }
                        }

                        for (var i = 0; i <= m; i++)
                        {
                            if (i == k)
                            {
                                continue;
                            }
                            initialA[i, l] = 0;
                        }

                        for (var j = 0; j <= n; j++)
                        {
                            if (j == l)
                            {
                                continue;
                            }
                            initialA[k, j] = initialA[k, j] / initialA[k, l];
                        }
                        initialA[k, l] = 1;
                        break;
                    }
                }
            }
            return initialA;
        }

        public double[,] Phase2(double[,] initialA, int m, int n)
        {
            while (LessThanZero(initialA, m, n))
            {
                _PrintSolution(initialA, ColumnVarTypes, RowVarTypes);
                for (var l = 0; l < n; l++)
                {
                    if (initialA[m, l] < 0)
                    {
                        if (AllColumnsAreZero(initialA, l, m, n))
                        {
                            Console.WriteLine("The problem has no optimum solution (although it has feasible solutions), i.e. the objective functions is unbounded");
                            return initialA;
                        }
                        int k = FindMin(initialA, l, m, n);
                        RowVarTypes[k] = ColumnVarTypes[l];

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
                                initialA[i, j] = (initialA[i, j] * initialA[k, l] - initialA[i, l] * initialA[k, j]) / initialA[k, l];
                            }
                        }

                        for (var i = 0; i <= m; i++)
                        {
                            if (i == k)
                            {
                                continue;
                            }
                            initialA[i, l] = 0;
                        }

                        for (var j = 0; j <= n; j++)
                        {
                            if (j == l)
                            {
                                continue;
                            }
                            initialA[k, j] = initialA[k, j] / initialA[k, l];
                        }
                        initialA[k, l] = 1;
                        break;
                    }
                }
            }
            return initialA;
        }



        public double[,] AddAuxiliarVariables()
        {
            double[,] A1 = Utils.Copy(initialA);
            for (int i = 0; i < initialA.GetLength(0); i++)
            {
                if (eqTypes[i] == EqType.GreatherThan)
                {
                    var addedColumn = new double[m + 1];
                    addedColumn.Populate(0);
                    addedColumn[i] = -1;
                    A1 = Utils.AddColumn(A1, addedColumn, A1.GetLength(1) - 1);
                    ColumnVarTypes.Add(VarType.Slack);
                    addedColumn = new double[m + 1];
                    addedColumn.Populate(0);
                    addedColumn[i] = 1;
                    A1 = Utils.AddColumn(A1, addedColumn, A1.GetLength(1) - 1);
                    ColumnVarTypes.Add(VarType.Artifical);
                    RowVarTypes.Add(VarType.Artifical);
                }
                if (eqTypes[i] == EqType.LessThan)
                {
                    var addedColumn = new double[m + 1];
                    addedColumn.Populate(0);
                    addedColumn[i] = 1;
                    A1 = Utils.AddColumn(A1, addedColumn, A1.GetLength(1) - 1);
                    ColumnVarTypes.Add(VarType.SlackArtifical);
                    RowVarTypes.Add(VarType.Slack);
                }
                if (eqTypes[i] == EqType.Equal)
                {
                    var addedColumn = new double[m + 1];
                    addedColumn.Populate(0);
                    addedColumn[i] = 1;
                    A1 = Utils.AddColumn(A1, addedColumn, A1.GetLength(1) - 1);
                    ColumnVarTypes.Add(VarType.Artifical);
                    RowVarTypes.Add(VarType.Artifical);
                }
            }
            ColumnVarTypes.Add(VarType.RHS);
            RowVarTypes.Add(VarType.RHS);
            return A1;
        }


        private double[] CalculateNewZ(double[,] t)
        {
            var zRow = new double[t.GetLength(1)];
            for (int i = 0; i < t.GetLength(1); i++)
            {
                var sum = 0.0;
                if(ColumnVarTypes[i] == VarType.Artifical || ColumnVarTypes[i] == VarType.SlackArtifical)
                {
                    zRow[i] = 0;
                    continue;
                }
                else
                {
                    for (int j = 0; j < t.GetLength(0); j++)
                    {
                        if (eqTypes[j] == EqType.GreatherThan || eqTypes[j] == EqType.Equal)
                        {
                            var t1 = t[j, i];
                            Console.WriteLine($"t[{j}, {i}] = {t1}");
                            sum += t1;
                        }
                    }
                }
                zRow[i] = -sum;
            }
            return zRow;
        } 

        public double[,] Solution()
        {
            return initialA;
        }

        private bool LessThanZero(double[,] initialA, int m, int n)
        {
            for (var i = 0; i < n; i++)
            {
                if (initialA[m, i] < 0)
                {
                    return true;
                }
            }
            return false;
        }

        private bool AllColumnsAreZero(double[,] initialA, int h, int m, int n)
        {
            for (var i = 0; i < m - 1; i++)
            {
                if (initialA[i, h] > 0)
                {
                    return false;
                }
            }
            return true;
        }

        private int FindMin(double [,] initialA, int l, int m, int n)
        {
            int index = 0;
            double min = int.MaxValue;
            for (var k = 0; k < m; k++)
            {
                var value = initialA[k, n] / initialA[k, l];
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
            _PrintSolution(initialA, ColumnVarTypes, RowVarTypes);
        }

        private void _PrintSolution(double [,] t, List<VarType> columnVarTypes, List<VarType> rowVarTypes)
        {
            var j = 1;
            var k = 1;
            Console.Write(" \t");
            for (int i = 0; i < columnVarTypes.Count; i++)
            {
                string x;
                if(columnVarTypes[i] == VarType.Artifical)
                {
                    x = $"y{k}";
                    ++k;
                }
                else if(columnVarTypes[i] == VarType.RHS)
                {
                    x = $"RHS";
                }else
                {
                    x = $"x{j}";
                    j++;
                }
                Console.Write(x + "\t");
            }

            Console.WriteLine();
            for (int i = 0; i < t.GetLength(0); i++)
            {
                if (rowVarTypes[i] == VarType.Artifical)
                {
                    Console.Write("y\t");
                }
                else if(rowVarTypes[i] == VarType.RHS)
                {
                    Console.Write("z\t");
                }
                else
                {
                    Console.Write("x\t");
                }
                for (j = 0; j < t.GetLength(1); j++)
                {
                    Console.Write(t[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }
        
    }
}
