using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlushiesAverage
{
    class Program
    {
        static void Main(string[] args)
        {
            // give me files made by the other program
            var dir = @"c:\users\username\documents\plushies\data";
            var outDir = @"c:\users\username\documents\plushies\avg";

            var allDupes = Process(dir);
      
            var avgs = new Dictionary<int, double>();

            // we know there are 10 rows per file
            for (int i = 0; i < 10; i++)
            {
                var toAverage = new List<int>();
                foreach(var hash in allDupes)
                {
                    // but do we?
                    if (hash.Count != 10) throw new InvalidOperationException("Incorrect file");
                    toAverage.Add(hash[i]);
                }
                avgs.Add(i, toAverage.Average()); 
            }
            foreach(var kp in avgs)
            {
                Console.WriteLine("{0} - average: {1}", kp.Key, kp.Value);
            }
            if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
            Write(Path.Combine(outDir, "Plushie_Avg_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".csv"), avgs);
            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }

        private static void Write(string filePath, Dictionary<int, double> avgs)
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("# of dupes", "average");
                    foreach (var kp in avgs)
                    {
                        writer.WriteLine("{0},{1}", kp.Key, kp.Value);
                    }
                }
            }
        }

        private static List<Dictionary<int, int>> Process(string directory)
        {
            var allDupes = new List<Dictionary<int, int>>();
            var info = new DirectoryInfo(directory);
            if (!info.Exists) throw new ArgumentException("directory: " + directory + " does not exist");
            var files = info.GetFiles();
            foreach (var file in files)
            {
                allDupes.Add(ProcessFile(file.FullName));
            }
            foreach (var dir in info.GetDirectories())
            {
                allDupes.AddRange(Process(dir.FullName));
            }

            return allDupes;
        }

        private static Dictionary<int, int> ProcessFile(string filePath)
        {
            var obj = new Dictionary<int, int>();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                using (TextReader reader = new StreamReader(stream))
                {
                    // no headers pls
                    reader.ReadLine();
                    string all = reader.ReadToEnd();
                    string[] data = all.Split(Environment.NewLine.ToCharArray());
                    foreach (string line in data)
                    {
                        if (String.IsNullOrEmpty(line)) continue; // shit hack, my split is wrong, this "fixes" it
                        string[] lineData = line.Split(','); // naive but okay cause I know there are no , symbols in here aside from cell spacers
                        if (lineData.Length != 4)
                        {
                            throw new InvalidOperationException("Invalid line");
                        }
                        obj.Add(Convert.ToInt32(lineData[0]), Convert.ToInt32(lineData[2]));
                    }
                }
                return obj;
            }
        }
    }
}
