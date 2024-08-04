using PC0;

var domains = GetDomains();
Dictionary<int, Func<int, bool>> unaryConstraints = new();
Dictionary<VariableList<int>, Func<List<int>, bool>> constraints = new();

/*var board = new int[,]
{
    {0, 0, 0,  0, 0, 0,  0, 0, 0 },
    {0, 0, 0,  0, 0, 0,  0, 0, 0 },
    {0, 0, 0,  0, 0, 0,  0, 0, 0 },

    {0, 0, 0,  0, 0, 0,  0, 0, 0 },
    {0, 0, 0,  0, 0, 0,  0, 0, 0 },
    {0, 0, 0,  0, 0, 0,  0, 0, 0 },

    {0, 0, 0,  0, 0, 0,  0, 0, 0 },
    {0, 0, 0,  0, 0, 0,  0, 0, 0 },
    {0, 0, 0,  0, 0, 0,  0, 0, 0 },
};*/


var board = new int[,]
{
    {0, 0, 0,  2, 0, 0,  0, 9, 0 },
    {9, 0, 3,  0, 6, 0,  2, 0, 7 },
    {0, 5, 4,  0, 0, 0,  8, 0, 0 },

    {4, 7, 0,  0, 0, 0,  0, 1, 0 },
    {0, 0, 2,  4, 0, 7,  0, 0, 0 },
    {5, 0, 0,  9, 0, 2,  0, 7, 0 },

    {0, 4, 0,  0, 0, 9,  7, 0, 0 },
    {0, 0, 1,  0, 0, 0,  5, 0, 0 },
    {0, 2, 6,  0, 5, 0,  0, 0, 0 },
};

for (int x = 0; x < 9; x++)
{
    for (int y = 0; y < 9; y++)
    {
        if (board[x, y] == 0)
            continue;

        var index = XYtoI(x, y);
        domains[index] = new List<int> { board[x, y] };
    }
}

for (int i = 0; i < 9; i++)
{
    var row = GetRow(i);
    var column = GetColumn(i);
    var box = GetBox(i);
    for (int j = 0; j < 9; j++)
    {
        var rowKey = new VariableList<int>(row);
        var columnKey = new VariableList<int>(column);
        var boxKey = new VariableList<int>(box);

        /*constraints.Add(rowKey, MutuallyExclusiveConstraint());
        constraints.Add(columnKey, MutuallyExclusiveConstraint());
        constraints.Add(boxKey, MutuallyExclusiveConstraint());*/
        constraints.Add(rowKey, ContainsAllConstraint());
        constraints.Add(columnKey, ContainsAllConstraint());
        constraints.Add(boxKey, ContainsAllConstraint());


        row = row.RotateThrough();
        column = column.RotateThrough();
        box = box.RotateThrough();
    }
}

var solver = new Solver<int>(domains, unaryConstraints, constraints);
solver.Solve();
solver.Solve();
;

for (int i = 0; i < domains.Length; i++)
{
    (int x, int y) v = ItoXY(i);

    if (domains[i].Count == 1)
    {
        board[v.x, v.y] = domains[i][0];
    }
}
for (int i = 0; i < 9; i++)
{
    for (int j = 0; j < 9; j++)
    {
        Console.Write(board[i, j] == 0 ? "♥ " : board[i, j] + " ");
        if (j % 3 == 2)
            Console.Write(" ");
    }
    if (i % 3 == 2)
        Console.WriteLine();
    Console.WriteLine();
}

List<int>[] GetDomains()
{
    var ret = new List<int>[81];
    for (int i = 0; i < 81; i++)
    {
        ret[i] = new List<int>();
        for (int j = 1; j <= 9; j++)
            ret[i].Add(j);
        ret[i].Shuffle();
    }
    return ret;
}

VariableList<int> GetBox(int i)
{
    var ret = new VariableList<int>();
    (int x, int y) topleft = i switch
    {
        0 => (0, 0),
        1 => (3, 0),
        2 => (6, 0),
        3 => (0, 3),
        4 => (3, 3),
        5 => (6, 3),
        6 => (0, 6),
        7 => (3, 6),
        8 => (6, 6)
    };

    for (int dx = 0; dx < 3; dx++)
        for (int dy = 0; dy < 3; dy++)
            ret.Add(XYtoI(topleft.x + dx, topleft.y + dy));
    return ret;
}

VariableList<int> GetColumn(int x)
{
    var ret = new VariableList<int>();
    for (int y = 0; y < 9; y++)
        ret.Add(XYtoI(x, y));
    return ret;
}

VariableList<int> GetRow(int y)
{
    var ret = new VariableList<int>();
    for (int x = 0; x < 9; x++)
        ret.Add(XYtoI(x, y));
    return ret;
}



Func<List<int>, bool> ContainsAllConstraint()
{
    return (list) =>
    {
        for (int i = 1; i <= 9; i++)
            if (!list.Contains(i))
                return false;
        return true;
    };
}

Func<List<int>, bool> MutuallyExclusiveConstraint()
{
    return (list) =>
    {
        for (int i = 0; i < list.Count; i++)
            for (int j = 0; j < list.Count; j++)
                if (i != j && list[i] == list[j])
                    return false;
        return true;
    };
}

int XYtoI(int x, int y)
{
    return x + y * 9;
}

(int x, int y) ItoXY(int i)
{
    return (i % 9, i / 9);
}