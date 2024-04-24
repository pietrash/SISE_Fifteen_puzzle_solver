using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FifteenPuzzle
{
    public class Node
    {
        private char move;
        private List<Node> children = new();
        private Node parent;

        public Node(char move, Node parent)
        {
            this.move = move;
            for (int i = 0; i < children.Count; i++)
            {
                children[i] = null;
            }

            this.parent = parent;
        }

        public void AddChild(Node node)
        {
            children.Add(node);
        }

        public void RemoveChild(Node node)
        {
            children.Remove(node);
        }

        public Node GetParent()
        {
            return parent;
        }

        public List<Node> GetChildren()
        {
            return children;
        }

        public char GetMove()
        {
            return move;
        }
    }
}
