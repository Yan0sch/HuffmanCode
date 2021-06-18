using System;
using System.IO;
using System.Text;

namespace HuffmanCode
{
    class HuffmanCoder
    {
        private double[] charProbability = new double[256];     // list with the probability of each char (index ~ UTF-8)
        private Tree tree;
        private string message;
        public HuffmanCoder(string message)
        {
            this.message = message;
            CountChars();
            tree = new Tree(charProbability);
            /*
            for (int i = 0; i < charProbability.Length; i++)
            {
                Console.WriteLine("{0} {1}: {2:F2}", i, (char)i, charProbability[i]);
            }*/
        }
        private void CountChars()
        {
            // count the chars in the message, divide it by the message length and add it to the probability
            foreach (char c in message)
            {
                charProbability[(byte)c] += (double) 1 / message.Length;
            }
        }
    }
}