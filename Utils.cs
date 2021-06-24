using System;
using System.Text;
using System.Collections.Generic;

namespace Utils
{
    class Compare
    {
        // This code snippet is stolen from: https://docs.microsoft.com/de-de/dotnet/api/system.double?view=net-5.0#Equality
        // Because of how doubles are calculated and stored you can't really check if they are equal so I decide to check if they are approximately equal
        // The method calculates the difference between two double relative to the max value (min value if 0)
        public static bool IsApproximatelyEqual(double value1, double value2, double epsilon)
        {
            // If they are equal anyway, just return True.
            if (value1.Equals(value2))
                return true;

            // Handle NaN, Infinity.
            if (Double.IsInfinity(value1) | Double.IsNaN(value1))
                return value1.Equals(value2);
            else if (Double.IsInfinity(value2) | Double.IsNaN(value2))
                return value1.Equals(value2);

            // Handle zero to avoid division by zero
            double divisor = Math.Max(value1, value2);
            if (divisor.Equals(0))
                divisor = Math.Min(value1, value2);

            return Math.Abs((value1 - value2) / divisor) <= epsilon;
        }
    }

    class Tools
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        public static string ByteToString(byte[] arr)
        {
            StringBuilder result = new StringBuilder();

            foreach (byte b in arr)
            {
                result.Append((char)b);
            }
            return result.ToString();
        }
    }

    // Manages a array of bits, wich are stored in 8 bit blocks (bytes)
    // based on the Collection List<byte>
    class BitArray
    {
        public List<byte> bitArray {get; private set;}
        public int bytes = 0;
        public int bitCapacity
        {
            get { return bytes * 8; }
        }

        public byte this[int i]
        {
            get
            {
                int byteIndex = i / 8;
                int bitIndex = 8 - (i % 8);
                return (byte) ((bitArray[byteIndex] >> bitIndex) % 2);
            }
            set
            {
                int byteIndex = i / 8;
                int bitIndex = 7 - (i % 8);
                while (byteIndex >= bytes){
                    bitArray.Add(0);
                    bytes++;
                }
                if (value == 0) bitArray[byteIndex] &= (byte)(~(1 << bitIndex));
                else if (value == 1) bitArray[byteIndex] |= (byte)(1 << bitIndex);
                else throw new ArgumentException("Not allowed number. 0 or 1 expected.");
            }
        }

        public BitArray(){
            bitArray = new List<byte>();
        }

        public override string ToString()
        {
            return Tools.ByteToString(bitArray.ToArray());
        }
    }
}