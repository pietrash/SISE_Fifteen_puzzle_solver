using System.Diagnostics;
using System.Text;

namespace FifteenPuzzle
{
    public class Solver
    {
        private Node root;
        private Node solvedNode = null;
        private Field original;
        private HashSet<int> existingFields;
        private Stopwatch stopwatch = new();

        private bool solved = false;
        private char[] searchOrder = { 'R', 'L', 'U', 'D' };
        private int depth;
        private int visitedNodes = 0;
        private int processedNodes = 0;
        private int maximumDepth = 0;

        public Solver(Field field, int depth, string type, string option)
        {
            root = new Node('n', null);

            original = field;
            existingFields = new HashSet<int>() { original.GetHash() };
            this.depth = depth;

            stopwatch.Start();

            if (type.ToLower() == "dfs")
            {
                searchOrder = option.ToCharArray();
                DFS(root);

            }
            if (type.ToLower() == "bfs")
            {
                searchOrder = option.ToCharArray();
                BFS();

            }
            if (type.ToLower() == "astr") AStar(option);
        }

        // returns Field state in a given node
        private Field GetNodeField(Node node)
        {
            List<Node> nodes = new List<Node>();

            while (node != null)
            {
                nodes.Add(node);
                node = node.GetParent();
            }

            Field tempField = original.Clone();
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                tempField.Move(nodes[i].GetMove());
            }
            return tempField;
        }

        public string AdditionalInfo()
        {
            TimeSpan ts = stopwatch.Elapsed;
            StringBuilder sb = new StringBuilder();

            sb.Append(Results().Split('\n')[0]);
            sb.Append('\n');

            sb.Append(visitedNodes);
            sb.Append('\n');

            sb.Append(processedNodes);
            sb.Append('\n');

            sb.Append(maximumDepth);
            sb.Append('\n');

            sb.Append(Math.Round(ts.TotalMicroseconds / 1000, 3));

            return sb.ToString();
        }

        public string Results()
        {
            if (!solved || solvedNode == null) return "-1";

            List<Node> nodes = new List<Node>();
            Node tempNode = solvedNode;
            while (tempNode != null)
            {
                nodes.Add(tempNode);
                tempNode = tempNode.GetParent();
            }

            nodes.Remove(root);

            StringBuilder sb = new StringBuilder();

            sb.Append(nodes.Count);
            sb.Append('\n');

            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                sb.Append(nodes[i].GetMove());
            }

            return sb.ToString();
        }

        // Generates all possible nodes for a given parent
        private void GenerateNodes(Node parent, bool checkDuplicates)
        {
            processedNodes++;

            for (int i = 0; i < 4; i++)
            {
                Field newField = GetNodeField(parent);

                if (!newField.Move(searchOrder[i])) continue;
                if (parent.GetMove() == 'U' && searchOrder[i] == 'D') continue;
                if (parent.GetMove() == 'D' && searchOrder[i] == 'U') continue;
                if (parent.GetMove() == 'R' && searchOrder[i] == 'L') continue;
                if (parent.GetMove() == 'L' && searchOrder[i] == 'R') continue;

                if (newField.IsSolved())
                {
                    stopwatch.Stop();
                    solved = true;
                    solvedNode = new Node(searchOrder[i], parent);
                }

                if (checkDuplicates)
                {
                    if (existingFields.Contains(newField.GetHash())) continue;
                    existingFields.Add(newField.GetHash());
                }

                Node newNode = new Node(searchOrder[i], parent);
                int newDepth = GetCurrentDepth(newNode);
                if (newDepth > maximumDepth) maximumDepth = newDepth;
                visitedNodes++;

                parent.AddChild(newNode);
            }
        }

        private int GetCurrentDepth(Node node)
        {
            int counter = 0;
            while (node.GetParent() != null)
            {
                node = node.GetParent();
                counter++;
            }
            return counter;
        }

        private void DFS(Node parent)
        {
            if (GetCurrentDepth(parent) == depth || solved) return;

            GenerateNodes(parent, false);
            List<Node> children = parent.GetChildren();

            for (int i = 0; i < children.Count; i++) DFS(children[i]);
            for (int i = 0; i < children.Count; i++) parent.RemoveChild(children[i]);
        }

        private void BFS()
        {
            List<Node> queue = new() { root };
            while (queue.Count != 0 && !solved)
            {
                GenerateNodes(queue[0], true);
                List<Node> children = queue[0].GetChildren();

                for (int i = 0; i < children.Count; i++)
                {
                    if (GetCurrentDepth(children[i]) > depth) continue;
                    queue.Add(children[i]);
                }
                queue.RemoveAt(0);
            }
        }

        private void AStar(string option)
        {
            List<Node> visitedNodes = new();
            Node currentNode = root;

            while (!solved)
            {
                // Generating new moves
                GenerateNodes(currentNode, false);
                visitedNodes.AddRange(currentNode.GetChildren());

                // Check which move has the lowest cost
                int lowestCostIndex = 0;
                for (int i = 0; i < visitedNodes.Count; i++)
                {
                    if (GetCost(visitedNodes[i], option) < GetCost(visitedNodes[lowestCostIndex], option))
                    {
                        lowestCostIndex = i;
                    }
                }

                currentNode = visitedNodes[lowestCostIndex];
                visitedNodes.RemoveAt(lowestCostIndex);
            }
        }

        private int GetCost(Node node, string option)
        {
            return GetCurrentDepth(node) + GetDistance(node, option);
        }

        private int GetDistance(Node node, string option)
        {
            if (option == "manh") return ManhattanDistance(node);
            if (option == "hamm") return HammingDistance(node);

            throw new ArgumentException(option);
        }

        private int HammingDistance(Node node)
        {
            Field field = GetNodeField(node);
            Field solvedPuzzle = field.GenerateSolvedField();

            int height = field.GetHeight();
            int width = field.GetWidth();
            int distance = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (field.GetCell(i, j) != solvedPuzzle.GetCell(i, j)) distance++;
                }
            }
            return distance;
        }

        private int ManhattanDistance(Node node)
        {
            Field field = GetNodeField(node);
            Field solvedPuzzle = field.GenerateSolvedField();

            int height = field.GetHeight();
            int width = field.GetWidth();
            int distance = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int currentValue = field.GetCell(i, j);
                    if (currentValue != 0)
                    {
                        for (int x = 0; x < height; x++)
                        {
                            for (int y = 0; y < width; y++)
                            {
                                if (solvedPuzzle.GetCell(x, y) == currentValue)
                                {
                                    distance += Math.Abs(x - i) + Math.Abs(y - j);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return distance;
        }
    }
}