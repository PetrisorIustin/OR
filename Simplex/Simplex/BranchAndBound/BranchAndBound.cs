using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simplex.BranchAndBound
{
    public class BranchAndBound
    {
        Stack<TwoPhaseSimplex> _stack = new Stack<TwoPhaseSimplex>();

        TwoPhaseSimplex _initiaProblem;
        List<Variable> x_bar;
        double z_bar;
        public BranchAndBound(Options options)
        {
            options.Z = options.Z.Select(z => z * (-1)).ToArray();
            _initiaProblem = new TwoPhaseSimplex(options);
        }

        public void Solve()
        {
            _stack.Push(_initiaProblem);
            x_bar = null;
            z_bar = double.MinValue;
            int maxIter = 15;
            int k = 0;
            while(_stack.Count != 0 && k < maxIter)
            {
                k++;
                var problem = _stack.Pop();
                var _sol = problem.Solve();
                if(_sol == null)
                {
                    Console.WriteLine("unbounded problem");
                    continue;
                }
                var sol = _sol.GetValueOrDefault();
                var z = sol.Z;
                PrintSolution(sol.X, sol.Z);
                if (z > z_bar)
                {
                    if(sol.X.Where(x => x.VarType == VarType.Primary).All(xi => xi.Value == (int)xi.Value))
                    {
                        x_bar = sol.X;
                        z_bar = z;
                    }
                    else
                    {
                        double maxVal = double.MinValue; 
                        int index = -1;
                        for (int i = 0; i < sol.X.Count; i++)
                        {
                            var xi = sol.X[i];
                            if (xi.Value - Math.Floor(xi.Value) > maxVal && xi.VarType == VarType.Primary)
                            {
                                maxVal = xi.Value - Math.Floor(xi.Value);
                                index = i;
                            }
                        }
                        var chosen_variable = sol.X[index];
                        var constraints1 = problem.Options.Constraints.Select(x => (EqType)x).ToList();
                        var constraints2 = problem.Options.Constraints.Select(x => (EqType)x).ToList();
                        constraints1.Add(EqType.LessThan);
                        constraints2.Add(EqType.GreatherThan);
                        var table1 = Utils.Copy(problem.Options.Table);
                        var table2 = Utils.Copy(problem.Options.Table);
                        var row1 = new double[problem.Options.Table.GetLength(1)];
                        row1.Populate(0);
                        row1[index] = 1;
                        row1[problem.Options.Table.GetUpperBound(1)] = Math.Floor(chosen_variable.Value);
                        table1 = Utils.AddRow(table1, row1);

                        var row2 = new double[problem.Options.Table.GetLength(1)];
                        row2.Populate(0);
                        row2[index] = 1;
                        row2[problem.Options.Table.GetUpperBound(1)] = Math.Floor(chosen_variable.Value) + 1;
                        table2 = Utils.AddRow(table2, row2);
                        Options newProblem1 = new Options
                        {
                            Constraints = constraints1,
                            Table = Utils.Copy(table1),
                            Z = problem.Options.Z
                        };
                        Options newProblem2 = new Options
                        {
                            Constraints = constraints2,
                            Table = Utils.Copy(table2),
                            Z = problem.Options.Z
                        };

                        _stack.Push(new TwoPhaseSimplex(newProblem1));
                        _stack.Push(new TwoPhaseSimplex(newProblem2));
                    }
                }

            }
            if(x_bar == null)
            {
                Console.WriteLine("Infeasabile solution");
            }
            else
            {
                PrintSolution(x_bar, z_bar);
            }
            

        }

        public void PrintSolution(List<Variable> l, double z)
        {
            l.Where(x => x.VarType == VarType.Primary).ToList().ForEach(x =>
            {
                Console.WriteLine($"x{x.Number + 1}={x.Value}");
            });
            Console.WriteLine($"z={z}");
        }
    }
}
