using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simplex
{
    public class Options
    {
        public List<EqType> GreaterThanEq { get; set; }
        public double[,] Table { get; set; }
        public double[] Z { get; set; }

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

    public class Variable {
        public int Number { get; set; }
        public VarType VarType { get; set; }
        public double Value { get; set; }
        public Variable(int number, VarType varType, double value = 0.0)
        {
            Number = number;
            VarType = varType;
            Value = value;
        }
    }




    public struct Solution {
        public List<Variable> X { get; set; }
        public double Z { get; set; }
    }



    public class TwoPhaseSimplex
    {
        private double[,] initialA;
        private readonly List<EqType> eqTypes;
        public List<Variable> ColumnVarTypes { get; set; }
        public List<Variable> RowVarTypes { get; set; }
        public List<int> VarNumber { get; set; }
        private readonly int m;
        private readonly int n;
        private readonly double[] Z;
        public Options Options { get; set; }
        public TwoPhaseSimplex(Options options)
        {
            Options = options;
            initialA = options.Table;
            eqTypes = options.GreaterThanEq;
            m = initialA.GetLength(0) - 1;
            n = initialA.GetLength(1) - 1;
            ColumnVarTypes = new List<Variable>();
            for (int i = 0; i < n; i++)
            {
                ColumnVarTypes.Add(new Variable(i,VarType.Primary));
            }
            RowVarTypes = new List<Variable>();
            Z = options.Z;
        }

        

        public Solution? Solve()
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
                _PrintSolution(sol, ColumnVarTypes, RowVarTypes);
                return null;
            }
            else
            {
                Console.WriteLine("The problem has feasable solution");
                var afterPhaseOneA = CalculateAAfterPhaseOne(sol);
                _PrintSolution(afterPhaseOneA, ColumnVarTypes, RowVarTypes);
                var finalSolution = Phase2(afterPhaseOneA, afterPhaseOneA.GetLength(0) - 1, afterPhaseOneA.GetLength(1) - 1);
                _PrintSolution(finalSolution, ColumnVarTypes, RowVarTypes);
                var z = finalSolution[finalSolution.GetUpperBound(0), finalSolution.GetUpperBound(1)];
                return new Solution
                {
                    X = ConstructSolution(finalSolution),
                    Z = z
                };
            }
             // _PrintSolution(sol, ColumnVarTypes, RowVarTypes);
        }


        public List<Variable> ConstructSolution(double[,] sol)
        {
            var x = new List<Variable>();
            for (int i = 0; i < sol.GetUpperBound(1); i++)
            {
                Variable var = RowVarTypes.FirstOrDefault(r => r.Number == i);
                if (var != null)
                {
                    var idx = RowVarTypes.IndexOf(var);
                    x.Add(new Variable(i, var.VarType, sol[idx, sol.GetUpperBound(1)]));
                }
                else
                {
                    var idx = RowVarTypes.IndexOf(var);
                    x.Add(new Variable(i, ColumnVarTypes.FirstOrDefault(c => c.Number == i).VarType, 0));
                }
            }
            return x;
        }


        private double[,] CalculateAAfterPhaseOne(double[,] sol)
        {
            for (var i = 0; i <= sol.GetLength(1) - 1; i++)
            {
                if(ColumnVarTypes[i].VarType == VarType.Artifical)
                {
                    sol = Utils.TrimColumnArray(i, sol);
                    ColumnVarTypes.RemoveAt(i);
                    i--;
                }
            }
            _PrintSolution(sol, ColumnVarTypes, RowVarTypes);
            var newZ = new double[sol.GetLength(1)];
            var newZ2 = new double[sol.GetLength(1)];
            for (var i = 0; i < Z.Length; i++)
            {
                newZ[i] = Z[i];
            }
            for (var i = Z.Length; i < sol.GetLength(1); i++)
            {
                newZ[i] = 0;
            }

            
            for (var i = 0; i < newZ.Length; i++)
            {
                var sum = 0.0;
                for (var j = 0; j < sol.GetUpperBound(0); j++)
                {
                    var col = RowVarTypes[j].Number;
                    sum = sum + newZ[col] * sol[j, i];
                }
                newZ2[i] = -sum + newZ[i];
            }
            Utils.PopulateLastRow(sol, newZ2);
            return sol;
        }

        private bool StillArtificialVariablesInBase()
        {
            return RowVarTypes.Count(x => x.VarType == VarType.Artifical) != 0;
        }

        public double[,] Phase1(double [,] initialA, int m, int n)
        {
            while (LessThanZero(initialA, m, n) || (StillArtificialVariablesInBase() && initialA[m, n] == 0))
            {
                _PrintSolution(initialA, ColumnVarTypes, RowVarTypes);
                if (!LessThanZero(initialA, m, n) && initialA[m,n] == 0 && StillArtificialVariablesInBase())
                {
                    for (var l = 0; l < n; l++)
                    {
                        if (ColumnVarTypes[l].VarType == VarType.Artifical)
                        {
                            if (AllColumnsAreLessOrEqualToZero(initialA, l, m, n))
                            {
                                Console.WriteLine("The problem has no optimum solution (although it has feasible solutions), i.e. the objective functions is unbounded");
                                return initialA;
                            }
                            int k = 0;

                            for (var i = 0; i <= m; i++)
                            {
                                if (i == m && (ColumnVarTypes[i].VarType == VarType.Artifical || initialA[i, l] == 0))
                                {
                                    Utils.TrimArray(l, i, initialA);
                                    RowVarTypes.RemoveAt(i);
                                    ColumnVarTypes.RemoveAt(l);
                                    m--;
                                    n--;
                                    continue;
                                }
                                if (ColumnVarTypes[i].VarType == VarType.Artifical || initialA[i, l] == 0)
                                {
                                    continue;
                                }
                                k = i;
                                RowVarTypes[k] = ColumnVarTypes[l];
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
                    continue;
                }

                for (var l = 0; l < n; l++)
                {
                    if (initialA[m, l] < 0)
                    {
                        if (AllColumnsAreLessOrEqualToZero(initialA, l, m, n))
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
                                if(i == 2 && j == 5)
                                {
                                    var asda = 0;
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
            _PrintSolution(initialA, ColumnVarTypes, RowVarTypes);
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
                        if (AllColumnsAreLessOrEqualToZero(initialA, l, m, n))
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
            int primaryV = initialA.GetUpperBound(1);
            int artV = 0;
            for (int i = 0; i < initialA.GetLength(0); i++)
            {
                if (eqTypes[i] == EqType.GreatherThan)
                {
                    var addedColumn = new double[m + 1];
                    addedColumn.Populate(0);
                    addedColumn[i] = -1;
                    A1 = Utils.AddColumn(A1, addedColumn, A1.GetLength(1) - 1);
                    ColumnVarTypes.Add(new Variable(primaryV++, VarType.Slack));
                    addedColumn = new double[m + 1];
                    addedColumn.Populate(0);
                    addedColumn[i] = 1;
                    A1 = Utils.AddColumn(A1, addedColumn, A1.GetLength(1) - 1);
                    ColumnVarTypes.Add(new Variable(artV++, VarType.Artifical));
                    artV--;
                    RowVarTypes.Add(new Variable(artV++, VarType.Artifical));
                }
                if (eqTypes[i] == EqType.LessThan)
                {
                    var addedColumn = new double[m + 1];
                    addedColumn.Populate(0);
                    addedColumn[i] = 1;
                    A1 = Utils.AddColumn(A1, addedColumn, A1.GetLength(1) - 1);
                    ColumnVarTypes.Add(new Variable(primaryV++,VarType.SlackArtifical));
                    primaryV--;
                    RowVarTypes.Add(new Variable(primaryV++, VarType.SlackArtifical));
                }
                if (eqTypes[i] == EqType.Equal)
                {
                    var addedColumn = new double[m + 1];
                    addedColumn.Populate(0);
                    addedColumn[i] = 1;
                    A1 = Utils.AddColumn(A1, addedColumn, A1.GetLength(1) - 1);
                    ColumnVarTypes.Add(new Variable(artV++, VarType.Artifical));
                    artV--;
                    RowVarTypes.Add(new Variable(artV++, VarType.Artifical));
                }
            }
            ColumnVarTypes.Add(new Variable(100, VarType.RHS));
            RowVarTypes.Add(new Variable(100, VarType.RHS));
            return A1;
        }


        private double[] CalculateNewZ(double[,] t)
        {
            var zRow = new double[t.GetLength(1)];
            for (int i = 0; i < t.GetLength(1); i++)
            {
                var sum = 0.0;
                if(ColumnVarTypes[i].VarType == VarType.Artifical || ColumnVarTypes[i].VarType == VarType.SlackArtifical)
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

        private bool AllColumnsAreLessOrEqualToZero(double[,] initialA, int h, int m, int n)
        {
            for (var i = 0; i < m; i++)
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

        private void _PrintSolution(double [,] t, List<Variable> columnVarTypes, List<Variable> rowVarTypes)
        {
            var j = 1;
            var k = 1;
            Console.Write(" \t");
            for (int i = 0; i < columnVarTypes.Count; i++)
            {
                string x;
                if(columnVarTypes[i].VarType == VarType.Artifical)
                {
                    x = $"y{columnVarTypes[i].Number+1}";
                    ++k;
                }
                else if(columnVarTypes[i].VarType == VarType.RHS)
                {
                    x = $"RHS";
                }else
                {
                    x = $"x{columnVarTypes[i].Number + 1}";
                    j++;
                }
                Console.Write(x + "\t");
            }

            Console.WriteLine();
            for (int i = 0; i < t.GetLength(0); i++)
            {
                if (rowVarTypes[i].VarType == VarType.Artifical)
                {
                    Console.Write($"y{rowVarTypes[i].Number + 1}\t");
                }
                else if(rowVarTypes[i].VarType == VarType.RHS)
                {
                    Console.Write("z\t");
                }
                else
                {
                    Console.Write($"x{rowVarTypes[i].Number + 1}\t");
                }
                for (j = 0; j < t.GetLength(1); j++)
                {
                    Console.Write(t[i, j].ToString("0.##") + "\t");
                }
                Console.WriteLine();
            }
        }
        
    }
}
