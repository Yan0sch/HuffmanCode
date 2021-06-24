using System;
using System.Text;
using System.IO;
using Utils;
using System.Collections.Generic;

namespace HuffmanTree
{
    class Node
    {
        public double prob;
        public int layer;
        public Node[] children = new Node[2];     // ! every node has exactly two children, not more and not less
        public byte name;
        public bool used;
        public byte code { get; private set; }
        public byte codeLen = 0;

        public Node(byte name, double prob, int depth = 0, Node child1 = null, Node child2 = null)
        {
            this.name = name;               // set to null if there are more than two chars
            this.prob = prob;               // probability of one ore more chars in the message
            this.layer = depth;             // layer of the Node

            // check if the children parameters are correct
            if ((child1 == null && child2 != null) || (child1 != null && child2 == null))
                throw new ArgumentException("Every node takes exactly two children or no children!");
            this.children[0] = child1;
            this.children[1] = child2;
        }

        public Node(string name, double prob, int depth = 0, Node[] children = null)
        {
            // check if exactly two have been entered
            if (children.Length != 2) throw new ArgumentException("Every node takes exactly two children or no children!");
            // Node(name, prob, depth, children[0], children[1]);
        }

        public void setCode(byte value)
        {
            code += value;
            codeLen += 1;
            if (children[0] == null || children[0] == null) return;

            // update the code of the children recursive
            foreach (Node child in children) child.setCode((byte)(value << 1));
        }

        public string ToString(int maxLayer)
        {
            if (children[0] != null && children[1] != null)
            {
                StringBuilder offset = new StringBuilder();
                for (int i = 0; i < maxLayer - layer; i++)
                {
                    offset.Append("  ");
                }
                return $"Node {prob}; {Convert.ToString(code, 2)}\n" + offset.ToString() + $"├ {children[0].ToString(maxLayer)}\n" + offset.ToString() + $"└ {children[1].ToString(maxLayer)}";
            }
            return $"{(char)name}: {prob}; {Convert.ToString(code, 2).PadLeft(codeLen, '0')}";
        }

        // overide compare operators to compare the layer if the probabilites are equal and the probabilites if they are different
        public static bool operator >(Node n1, Node n2)
        {
            if (Compare.IsApproximatelyEqual(n1.prob, n2.prob, 0.01))
            {
                return n1.layer > n2.layer;
            }
            return n1.prob > n2.prob;
        }
        public static bool operator <(Node n1, Node n2)
        {
            if (Compare.IsApproximatelyEqual(n1.prob, n2.prob, 0.01))
            {
                return n1.layer < n2.layer;
            }
            return n1.prob < n2.prob;
        }
    }

    public class Tree
    {
        private double[] charProbability = new double[256];     // list with the probability of each char (index ~ UTF-8)
        public byte[] text;                 // note that when you change the text, you need to create a new Huffman tree

        public byte[] encodedText { get; private set; }
        private List<Node> tree = new List<Node>();          // every char is represented by 8 bit, so the max number of possible chars is 256 (0..255)
        private Node root;
        private (byte, byte)[] codes = new (byte, byte)[256];         // store the codes in a array, first value is the code, second value is the length, index ~ UTF-8

        public Tree(string text)
        {
            this.text = Tools.StringToByte(text);
            CountChars();
            CreateTree();
            CreateCodes();
        }

        public Tree(byte[] text)
        {
            this.text = text;
            CountChars();
            CreateTree();
            CreateCodes();
        }

        public Tree(StreamReader reader, bool encoded)
        {
            if (encoded)
            {
                ReadEncodedFile(reader.ReadToEnd());
            }
            else
            {
                // Tree(reader.ReadToEnd());
            }
        }


        // TODO implement Reading and Writing methods
        private void ReadEncodedFile(string path)
        {
            // TODO
        }

        // save Nodes: "{name}{codelen}{code}{child1}{child2}"
        // name: UTF-8 --> 8 bit
        // codelen: 1 to 8 --> 4 bit (0 to 15)
        // code: code value --> 1 to 8 bit
        // children: null (0) or another node --> 8 bit or 13 to 20 bit
        private string NodesToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (Node n in tree)
            {

            }
            return "";
        }
        public void SaveFile(string path)
        {
            StreamWriter writer = new StreamWriter(path);


        }

        private void CountChars()
        {
            // count the chars in the message, divide it by the message length and add it to the probability
            foreach (byte b in text)
            {
                charProbability[b] += (double)1 / text.Length;
            }
        }
        private void CreateTree()
        {
            // create an array with Nodes from the array with the char probabilities
            int nodeCount = 0;
            for (int i = 0; i < charProbability.Length; i++)
            {
                if (charProbability[i] > 0)
                {
                    tree.Add(new Node((byte)i, charProbability[i]));     // name of the node is the character
                    nodeCount++;
                }
            }

            
            int numberOfUnusedNodes = nodeCount;            // store the number of the nodes, that don't belong to another node

            int node1 = 0;                              // connect the two nodes with the lowest probability to a higher node
            int node2 = 1;                              // init with the first two nodes

            while (numberOfUnusedNodes > 1)
            {
                for (int i = 0; i < nodeCount; i++)
                {
                    if (node1 == i || node2 == i) continue;
                    if (tree[i].used) continue;
                    // the lowsest probability should be at node2 --> only prove if node1 prob is lower than the current prob
                    // --> node1 and node2 are the indices of the two lowest elements 
                    if (tree[node1] < tree[node2] || tree[node2].used) Tools.Swap(ref node1, ref node2);
                    if (tree[node1] > tree[i] || tree[node1].used) node1 = i;
                }

                // add the two nodes with the lowest prob together in a new node
                // the new node is written to the end of the array so it don't delete the child nodes
                tree.Add(new Node(0, tree[node1].prob + tree[node2].prob,                // prob of the new node is the sum of the childs
                                    Math.Max(tree[node1].layer, tree[node2].layer) + 1,     // layer is max Layer +1
                                    tree[node1], tree[node2]));                             // set the two childs

                // set the probability of the program to 1 so that the upper code ignore it
                tree[node1].used = true;
                tree[node2].used = true;

                // add a zero to the code of node1 and a one to the code of node2
                tree[node1].setCode(0);
                tree[node2].setCode(1);

                nodeCount++;
                numberOfUnusedNodes--;            // you "use" two nodes and add a new on so the number of unused nodes decreases by 1
            }
            root = tree[nodeCount - 1];      // store the start node extra
        }


        // Just add this to have a full array with the codes for all UFT-8 chars
        private void CreateCodes()
        {
            foreach (Node n in tree)
            {
                if (n.layer > 0 || n == null) break;
                codes[n.name] = (n.code, n.codeLen);
            }
        }

        private (byte, byte) encodeChar(byte b)
        {
            if (codes[b].Item2 == 0) throw new ArgumentException($"Character {b} is not in the tree!");
            return codes[b];
        }

        public void Encode()
        {
            int index = 3;           // 3 bits at the beginning to store the padding at the end
            byte len, code;
            int numOfBits = 3;
            BitArray result = new BitArray();

            foreach (byte b in text)
            {
                (code, len) = encodeChar(b);            // encode the char and get the length (because 0s at the beginning might disappier)
                for (int i = len; i > 0; i--) result[index++] = (byte)((code >> (i - 1)) % 2);
                numOfBits += len;
            }

            // store the padding at the end in the first three bits
            byte leftBits = (byte)(8 - (numOfBits % 8));
            result[0] = (byte)((leftBits >> 2) % 2);
            result[1] = (byte)((leftBits >> 1) % 2);
            result[2] = (byte)(leftBits % 2);

            // convert the byte array to a string
            encodedText = result.bitArray.ToArray();
        }

        public void Decode()
        {
            List<byte> result = new List<byte>();
            Node currentNode = root;
            int index = 3;

            BitArray textBitArray = new BitArray();
            textBitArray.bitArray = new List<byte>(encodedText);
            textBitArray.bytes = encodedText.Length;

            byte overhead = (byte)(textBitArray.bitArray[0] >> 5);

            while (index < textBitArray.bitCapacity - overhead)
            {
                currentNode = currentNode.children[textBitArray[index++]];

                if (currentNode.children[0] == null)
                {
                    result.Add(currentNode.name);
                    currentNode = root;
                }
            }
            text = result.ToArray();
        }

        public override string ToString()
        {
            return root.ToString(root.layer);
        }
    }
}