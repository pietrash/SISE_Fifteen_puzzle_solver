using FifteenPuzzle;

class Prog
{
    static void Main(string[] args)
    {
        // [0] - type
        // [1] - option
        // [2] - filePath
        // [3] - solPath
        // [4] - statsPath

        Solver solver = new(new Field(args[2]), 20, args[0], args[1]);

        File.WriteAllText(args[3], solver.Results());
        File.WriteAllText(args[4], solver.AdditionalInfo());
    }
}