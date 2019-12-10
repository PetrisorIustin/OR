using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Simplex.BranchAndBound
{
    public class BranchAndBound
    {
        Stack<TwoPhaseSimplex> _stack = new Stack<TwoPhaseSimplex>();

        TwoPhaseSimplex _initiaProblem;
        double[] x_bar;
        double z_bar;
        public BranchAndBound(TwoPhaseSimplex tfs)
        {
            _initiaProblem = tfs;
        }

        public void Solve()
        {
            _stack.Push(_initiaProblem);
            x_bar = null;
            z_bar = double.MinValue;
            while(_stack.Count == 0)
            {
                var problem = _stack.Pop();
                problem.Solve();


            }


        }
    }
}
