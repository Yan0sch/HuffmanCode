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
        public Node child1;
        public Node child2;
        public string name;
        public bool used;
        public byte code { get; private set; }
        public byte codeLen = 0;

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
            child1.codeLen++;
            child2.setCode((byte)(value << 1));
            child2.codeLen++;
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
                return $"{name}: {prob}; {Convert.ToString(code, 2)}\n" + offset.ToString() + $"├ {child1.ToString(maxLayer)}\n" + offset.ToString() + $"└ {child2.ToString(maxLayer)}";
            }
            return name + $": {prob}; {Convert.ToString(code, 2).PadLeft(codeLen, '0')}";
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
        public string message {get; private set;}
        public string encodedMessage {get; private set;}
        private List<Node> tree = new List<Node>();          // every char is represented by 8 bit, so the max number of possible chars is 256 (0..255)
        private Node startNode;
        public Tree(string message)
        {
            this.message = message;
            CountChars();
            CreateTree();
        }
        private void CountChars()
        {
            // count the chars in the message, divide it by the message length and add it to the probability
            foreach (char c in message)
            {
                charProbability[(byte)c] += (double)1 / message.Length;
            }
        }
        private void CreateTree()
        {
            // create an array with Nodes from the array with the char probabilities
            int nodeCount = 0;
            for (int i = 0; i < charProbability.Length; i++)
            {
                if (charProbability[i] > 0){
                    tree.Add(new Node(((char)i).ToString(), charProbability[i]));     // name of the node is the character
                    nodeCount++;
                }
            }
            int numberOfUnusedNodes = nodeCount;            // store the number of the nodes, because the hole array is much bigger

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

                // add the two nodes with the lowest prob together in a new node (name1+name2, prob = node1.prob + node2.prob, layer = highest layer of both +1)
                // the new node is written to the end of the array so it don't delete the child nodes
                tree.Add(new Node(tree[node1].name + tree[node2].name,
                                                    tree[node1].prob + tree[node2].prob,
                                                    Math.Max(tree[node1].layer, tree[node2].layer) + 1,
                                                    tree[node1], tree[node2]));

                // set the probability of the program to 1 so that the upper code ignore it
                tree[node1].used = true;
                tree[node2].used = true;

                // add a zero to the code of node1 and a one to the code of node2
                tree[node1].setCode(0);
                tree[node1].codeLen++;
                tree[node2].setCode(1);
                tree[node2].codeLen++;
                nodeCount++;
                numberOfUnusedNodes--;            // you "remove" two nodes and add a new on so the number of nodes decreases by 1
            }
            startNode = tree[nodeCount - 1];
        }

        private (byte, byte) encodeChar(char c)
        {
            foreach (Node n in tree)
            {
                if (n.name.Length > 1 || n == null) break;
                if (n.name[0] == c) return (n.code, n.codeLen);
            }
            throw new ArgumentException($"Character {c} is not in the tree!");
        }

        public void Encode()
        {
            int leftBits = 8 - 3;           // 3 bits at the beginning to store the padding at the end
            int len;
            int code;
            byte currentByte = 0;
            List<byte> result = new List<byte>();

            foreach (char c in message)
            {
                (code, len) = encodeChar(c);            // encode the char and get the length (because 0s at the beginning might disappier)
                leftBits -= len;
                if (leftBits > 0) currentByte += (byte) (code << leftBits);
                else
                {
                    currentByte += (byte) (code >> -(leftBits));
                    result.Add(currentByte);
                    currentByte = 0;
                    leftBits += 8;
                    currentByte += (byte) (code << leftBits);
                }                
            }
            if(leftBits < 8){
                result.Add(currentByte);
            }
            if(leftBits >= 8) leftBits = 0;
            
            byte firstBit = (byte) (result[0]|(leftBits<<5));
            result[0] |= (byte) (leftBits<<5);
            
            encodedMessage = Tools.ByteToString(result.ToArray());
        }

        public void Decode(){
            StringBuilder result = new StringBuilder();
            Node currentNode = startNode;
            int currentBit = 7 - 3;             // first three bits contain the amount padding at the end
            int idx = 0;

            int overhead = ((byte) encodedMessage[0]) >> 5;

            while(idx < encodedMessage.Length){
                if(((byte) encodedMessage[idx] >> currentBit) % 2 == 0) currentNode = currentNode.child1;
                else currentNode = currentNode.child2;

                currentBit--;                

                // when reached the bottom layer get the char
                if(currentNode.child1 == null) {
                    result.Append(currentNode.name);
                    currentNode = startNode;
                }

                // when reached the end of the current byte reset the counter and go to the next byte
                if(currentBit < 0) {
                    currentBit = 7;
                    idx++;
                }
                if(idx >= encodedMessage.Length-1 && currentBit < overhead) break;          // check if the program reached the end of the message
            }
            message = result.ToString();
        }

        public override string ToString()
        {
            return startNode.ToString(startNode.layer);
        }
    }
}