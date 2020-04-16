using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace LongNumberGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = @"C:\\Users\\Masoud\\external-merge-sort\\input.txt";
            if (!File.Exists(file))
            {
                long lowerRange = long.MinValue;
                long upperRange = long.MaxValue;
                Random rand = new Random();
                RandomExtensionMethods.Generate(lowerRange, upperRange, rand, file);
                SortMethods.W("200 Million random Long Number Generated");
                SortMethods.MemoryUsage();
            }

            SortMethods.Split(file);
            SortMethods.MemoryUsage();

            SortMethods.SortTheChunks();
            SortMethods.MemoryUsage();

            SortMethods.MergeTheChunks();
            SortMethods.MemoryUsage();
        }
    }
}
