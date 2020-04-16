using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LongNumberGenerator
{
    public static class RandomExtensionMethods
    {
        public static long NextLong(this Random random, long min, long max)
        {
            if (max <= min)
                throw new ArgumentOutOfRangeException("max", "max must be > min!");

            ulong uRange = (ulong)(max - min);

            ulong ulongRand;
            do
            {
                byte[] buf = new byte[8];
                random.NextBytes(buf);
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
            } while (ulongRand > ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange);

            return (long)(ulongRand % uRange) + min;
        }

        public static void Generate(long lowerRange, long upperRange, Random rnd, string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                for (int i = 1; i < 200000000; i++)
                {
                    long number = NextLong(rnd, lowerRange, upperRange);
                    writer.Write(number + "\n");
                }
                writer.Flush();
                writer.Close();
            }
        }
    }
}
