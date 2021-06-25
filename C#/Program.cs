<<<<<<< HEAD:Program.cs
﻿using System;
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
            StreamReader reader = new StreamReader(path, Encoding.Default, true);
            string file = reader.ReadToEnd();
            
            TimeSpan time;
            DateTime start = DateTime.Now;
            Tree tree = new Tree(file);
            time = DateTime.Now - start;
            Console.WriteLine("Huffman tree created. took {0}h {1}min {2}s {3}ms", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
            
            start = DateTime.Now;
            tree.Encode();
            time = DateTime.Now - start;
            Console.WriteLine("Message encoded. took {0}h {1}min {2}s {3}ms", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);

            start = DateTime.Now;
            tree.Decode();
            time = DateTime.Now - start;
            Console.WriteLine("Message decoded. took {0}h {1}min {2}s {3}ms", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);

            Console.WriteLine(Tools.ByteToString(tree.text));
            FileInfo fi = new FileInfo(path);
            Console.WriteLine($"Size of file: {fi.Length} bytes\nSize of compressed text: {tree.encodedText.Length} bytes");
        }
    }
}