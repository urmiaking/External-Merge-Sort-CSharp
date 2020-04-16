using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace LongNumberGenerator
{
    public static class SortMethods
    {
        public static void MergeTheChunks()
        {
            W("Merging");

            string[] paths = Directory.GetFiles("C:\\Users\\Masoud\\external-merge-sort\\tmp\\", "sorted*.dat");
            int chunks = paths.Length; // Number of chunks
            int recordsize = 65; // estimated record size
            int records = 200000000; // total number of records
            int maxusage = 100000000; // max memory usage(~100MB)
            int buffersize = maxusage / chunks; // size in bytes of each buffer
            double recordoverhead = 7.5; // The overhead of using Queue<>
            int bufferlen = (int)(buffersize / recordsize / recordoverhead); // number of records in each buffer

            StreamReader[] readers = new StreamReader[chunks];
            for (int i = 0; i < chunks; i++)
                readers[i] = new StreamReader(paths[i]);

            Queue<string>[] queues = new Queue<string>[chunks];
            for (int i = 0; i < chunks; i++)
                queues[i] = new Queue<string>(bufferlen);

            W("Priming the queues");
            for (int i = 0; i < chunks; i++)
                LoadQueue(queues[i], readers[i], bufferlen);
            W("Priming the queues complete");

            
            StreamWriter sw = new StreamWriter("C:\\Users\\Masoud\\external-merge-sort\\output.txt");
            bool done = false;
            int lowest_index, j, progress = 0;
            string lowest_value;
            while (!done)
            {
                // Report the progress
                if (++progress % 5000 == 0)
                    Console.Write("{0:f2}%   \r",
                      100.0 * progress / records);

                // Find the chunk with the lowest value
                lowest_index = -1;
                lowest_value = "";
                for (j = 0; j < chunks; j++)
                {
                    if (queues[j] != null)
                    {
                        if (lowest_index < 0 || String.CompareOrdinal(queues[j].Peek(), lowest_value) < 0)
                        {
                            lowest_index = j;
                            lowest_value = queues[j].Peek();
                        }
                    }
                }

                // All queues are empty then it's done
                if (lowest_index == -1) { done = true; break; }

                sw.WriteLine(lowest_value);

                queues[lowest_index].Dequeue();
                // Have we emptied the queue? Top it up
                if (queues[lowest_index].Count == 0)
                {
                    LoadQueue(queues[lowest_index], readers[lowest_index], bufferlen);
                    // Was there nothing left to read?
                    if (queues[lowest_index].Count == 0)
                    {
                        queues[lowest_index] = null;
                    }
                }
            }
            sw.Close();

            // Close and delete the files
            for (int i = 0; i < chunks; i++)
            {
                readers[i].Close();
                // File.Delete(paths[i]);
            }

            W("Merging complete");
        }

        static void LoadQueue(Queue<string> queue, StreamReader file, int records)
        {
            for (int i = 0; i < records; i++)
            {
                if (file.Peek() < 0) break;
                queue.Enqueue(file.ReadLine());
            }
        }

        public static void SortTheChunks()
        {
            W("Sorting chunks");
            foreach (string path in Directory.GetFiles("C:\\Users\\Masoud\\external-merge-sort\\tmp\\", "split*.dat"))
            {
                Console.Write("{0}     \r", path);

                var sr = new StreamReader(path);
                string line;
                var counter = 0;
                
                List<long> numbers = new List<long>();
                while ((line = sr.ReadLine()) != null)
                {
                    numbers.Add(long.Parse(line));
                    counter++;
                }

                numbers.Sort();
                
                string newpath = path.Replace("split", "sorted");
                
                using (StreamWriter writer = new StreamWriter(newpath, false, Encoding.UTF8))
                {
                    foreach (var number in numbers)
                    {
                        writer.WriteLine(number);
                    }
                    writer.Flush();
                    writer.Close();
                }
                
                // File.Delete(path);
            }
            W("Sorting chunks completed");
        }

        public static void Split(string file)
        {
            W("Splitting");
            int split_num = 1;
            StreamWriter sw = new StreamWriter(string
                .Format("C:\\Users\\Masoud\\external-merge-sort\\tmp\\split{0:d5}.dat", split_num));
            long read_line = 0;
            using (StreamReader sr = new StreamReader(file))
            {
                while (sr.Peek() >= 0)
                {
                    // Progress reporting
                    if (++read_line % 5000 == 0)
                        Console.Write("{0:f2}%   \r",
                          100.0 * sr.BaseStream.Position / sr.BaseStream.Length);

                    sw.WriteLine(sr.ReadLine());

                    if (sw.BaseStream.Length > 16000000 && sr.Peek() >= 0)
                    {
                        sw.Close();
                        split_num++;
                        sw = new StreamWriter(string
                            .Format("C:\\Users\\Masoud\\external-merge-sort\\tmp\\split{0:d5}.dat", split_num));
                    }
                }
            }
            sw.Close();
            W("Splitting complete");
        }

        public static void W(string s)
        {
            Console.WriteLine("{0}: {1}", (DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()), s);
        }

        public static void MemoryUsage()
        {
            W(String.Format("{0} MB peak working set | {1} MB private bytes",
              Process.GetCurrentProcess().PeakWorkingSet64 / 1024 / 1024,
              Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024
              ));
        }
    }
}
