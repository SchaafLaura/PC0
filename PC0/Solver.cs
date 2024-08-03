namespace PC0
{
    internal class Solver<T>
    {
        List<T>[] domains;
        Dictionary<int, Func<T, bool>> unaryConstraints;
        Dictionary<VariableList<int>, Func<List<T>, bool>> constraints;

        HashSet<VariableList<int>> workList;

        public Solver(
             List<T>[] domains,
             Dictionary<int, Func<T, bool>> unaryConstraints,
             Dictionary<VariableList<int>, Func<List<T>, bool>> constraints)
        {
            this.domains = domains;
            this.unaryConstraints = unaryConstraints;
            this.constraints = constraints;
            workList = new HashSet<VariableList<int>>();
        }
        private void MakeInitialDomainsConsistant()
        {
            foreach (var c in unaryConstraints)
            {
                var variable = c.Key;
                var constraint = c.Value;
                var domain = domains[variable];
                for (int i = domain.Count - 1; i >= 0; i--)
                    if (!constraint(domain[i]))
                        domain.RemoveAt(i);
            }
        }
        public bool Solve()
        {
            MakeInitialDomainsConsistant();
            AddPathsToWorkList();
            do
            {
                var workingPath = workList.ElementAt(workList.RandomIndex());
                workList.Remove(workingPath);

                if (PathReduce(workingPath, new List<T>(), 0))
                {
                    if (domains[workingPath[0]].IsEmpty())
                        return false; // failed to solve

                    foreach (var path in GetPaths(workingPath[0]))
                        if (!path.Equals(workingPath))
                            workList.Add(path);
                }
                Console.WriteLine(workList.Count);
            }
            while (!workList.IsEmpty());
            ;
            return true;
        }

        private bool PathReduce(VariableList<int> path, List<T> fixedValues, int depth)
        {
            if (path.Count == fixedValues.Count)
                return Consistent(path, fixedValues);

            var domain = domains[path[depth]];
            var changed = false;
            for (int i = domain.Count - 1; i >= 0; i--)
            {
                var v = domain[i];
                fixedValues.Add(v);
                bool valid = PathReduce(path, fixedValues, depth + 1);
                fixedValues.RemoveAt(fixedValues.Count - 1);

                if (valid && depth != 0)
                    return true;

                if (!valid && depth == 0)
                {
                    changed = true;
                    domain.Remove(v);
                }
            }
            return changed;
        }
        private void AddPathsToWorkList()
        {
            for (int i = 0; i < domains.Length; i++)
            {
                var paths = GetPaths(i);
                foreach (var path in paths)
                    workList.Add(path);
            }
        }
        private List<VariableList<int>> GetPaths(int i)
        {
            List<VariableList<int>> ret = new();
            foreach (var c in constraints)
                if (c.Key.Contains(i))
                    ret.Add(c.Key);
            return ret;
        }
        private bool Consistent(VariableList<int> variables, List<T> values)
        {
            var constraint = constraints[variables];
            return constraint.Invoke(values);
        }
    }
}
