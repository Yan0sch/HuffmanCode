using System;
using System.Text;

namespace Utils{

    class Compare{
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

    class Tools{
        public static void Swap<T>(ref T a, ref T b){
            T tmp = a;
            a = b;
            b = tmp;
        }

        public static string ByteToString(byte[] arr){
            StringBuilder result = new StringBuilder();

            foreach(byte b in arr){
                result.Append((char) b);
            }
            return result.ToString();
        }
    }
}