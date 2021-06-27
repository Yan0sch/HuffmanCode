using System;
using System.IO;
using System.Text;
using Utils;
using System.Diagnostics;

namespace HuffmanTree
{
    class Program
    {

        // This is a Benchmark of the huffman code
        static void Main(string[] args)
        {
            Console.WriteLine("This is a Benchmark of my Huffman tree.\nPlease note, that the speed differs on different systems!");

            Console.Write("Please input a path: ");
            string path = Console.ReadLine();
            
            TimeSpan time;
            DateTime start = DateTime.Now;
            Tree tree = new Tree(path);
            time = DateTime.Now - start;
            Console.WriteLine("Huffman tree created. took {0}h {1}min {2}s {3}ms", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
            
            start = DateTime.Now;
            tree.Encode();
            time = DateTime.Now - start;
            Console.WriteLine("Message encoded. took {0}h {1}min {2}s {3}ms", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
            
            tree.text = new byte[1];
            Console.WriteLine("Clear the text: {0}", Tools.ByteToString(tree.text));
            start = DateTime.Now;
            tree.Decode();
            time = DateTime.Now - start;
            Console.WriteLine("Message decoded. took {0}h {1}min {2}s {3}ms", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
            Console.WriteLine("uncompressed text: {0} Byte\ncompressed text {1} Byte",tree.text.Length, tree.encodedText.Length);

            Console.Write("Do you want to see the decoded text [y/n]");
            string input = Console.ReadLine();
            if(input.ToLower() == "y") Console.WriteLine(Tools.ByteToString(tree.text));

            Console.Write("Do you want to see the tree [y/n]");
            input = Console.ReadLine();
            if(input.ToLower() == "y") Console.WriteLine(tree);
        }
    }
}