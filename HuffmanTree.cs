using System;
using System.Text;
using System.IO;
using Utils;

namespace HuffmanCode
{
    class Node
    {
        internal double prob;
        internal int layer;
        internal Node child1;
        internal Node child2;
        internal string name;
        internal bool used;
        internal byte code { get; private set; }
        internal byte codeLength = 0;
        public Node(string name, double prob, int depth = 0, Node child1 = null, Node child2 = null)
        {
            this.name = name;               // set to null if there are more than two chars
            this.prob = prob;               // probability of one ore more chars in the message
            this.layer = depth;             // layer of the Node
            this.child1 = child1;    // the two Nodes that come next in the tree (null if its the last Node)
            this.child2 = child2;
        }

        public void setCode(byte value)
        {
            code += value;
            if (child1 == null || child2 == null) return;
            child1.setCode((byte)(value << 1));
            child1.codeLength++;
            child2.setCode((byte)(value << 1));
            child2.codeLength++;
        }

        public string ToString(int maxLayer)
        {
            if (child1 != null && child2 != null)
            {
                StringBuilder offset = new StringBuilder();
                for (int i = 0; i < maxLayer - layer; i++)
                {
                    offset.Append("  ");
                }
                return $"{name}: {prob}; code {Convert.ToString(code, 2)}\n" + offset.ToString() + $"├ {child1.ToString(maxLayer)}\n" + offset.ToString() + $"└ {child2.ToString(maxLayer)}";
            }
            return name + $": {prob}; {Convert.ToString(code, 2)}; len: {codeLength}";
        }

        // overide compare operators to compare the layer if the probabilites are equal and the probabilites if they are different
        public static bool operator >(Node n1, Node n2)
        {
            if (Compare.IsApproximatelyEqual(n1.prob, n2.prob, 0.01))
            {
                return n1.layer > n2.layer || n1.used;
            }
            return n1.prob > n2.prob || n1.used;
        }
        public static bool operator <(Node n1, Node n2)
        {
            if (Compare.IsApproximatelyEqual(n1.prob, n2.prob, 0.01))
            {
                return n1.layer < n2.layer || n2.used;
            }
            return n1.prob < n2.prob || n2.used;
        }
    }

    class Tree
    {
        private Node[] tree = new Node[256];            // every char is represented by 8 bit, so the max number of possible chars is 256 (0..255)
        public Node startNode;
        public Tree(double[] charProbability)
        {
            CreateTree(charProbability);
        }

        private void CreateTree(double[] charProbability)
        {
            // create an array with Nodes from the array with the char probabilities
            int currentIndex = 0;
            for (int i = 0; i < charProbability.Length; i++)
            {
                if (charProbability[i] > 0) tree[currentIndex++] = new Node(((char)i).ToString(), charProbability[i]);      // name of the node is the character
            }
            int numberOfNodes = currentIndex;            // store the number of the nodes, because the hole array is much bigger

            int node1 = 0;
            int node2 = 1;                           // connect the two nodes with the lowest probability to a higher node

            while (numberOfNodes > 1)
            {
                for (int i = 0; i < currentIndex; i++)
                {
                    if (node1 == i || node2 == i) continue;
                    if (tree[i].used) continue;
                    // the lowsest probability should be at node2 --> only prove if node1 prob is lower than the current prob
                    // --> node1 and node2 are the indices of the two lowest elements 
                    if (tree[node1] < tree[node2])
                    {
                        int tmp = node1;
                        node1 = node2;
                        node2 = tmp;
                    }
                    if (tree[node1] > tree[i]) node1 = i;
                }

                // add the two nodes with the lowest prob together in a new node (name1+name2, prob = node1.prob + node2.prob, layer = highest layer of both +1)
                // the new node is written to the end of the array so it don't delete the child nodes
                tree[currentIndex++] = new Node(tree[node1].name + tree[node2].name,
                                                    tree[node1].prob + tree[node2].prob,
                                                    Math.Max(tree[node1].layer, tree[node2].layer) + 1,
                                                    tree[node1], tree[node2]);

                // set the probability of the program to 1 so that the upper code ignore it
                tree[node1].used = true;
                tree[node2].used = true;

                // add a zero to the code of node1 and a one to the code of node2
                tree[node1].setCode(0);
                tree[node1].codeLength++;
                tree[node2].setCode(1);
                tree[node2].codeLength++;
                numberOfNodes--;            // you "remove" two nodes and add a new on so the number of nodes decreases by 1
            }
            startNode = tree[currentIndex - 1];
        }

        public (byte, byte) convertChar(char c)
        {
            foreach (Node n in tree)
            {
                if (n.name.Length > 1 || n == null) break;
                if (n.name[0] == c) return (n.code, n.codeLength);
            }
            throw new ArgumentException($"Character {c} is not in the tree!");
        }

        public override string ToString()
        {
            return startNode.ToString(startNode.layer);
        }
    }
}