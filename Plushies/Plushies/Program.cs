using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plushies
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 100; i++)
            {
                Go();
            }

            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }

        static void Go()
        {
            var r = new Random();
            var diffPlush = 15;
            int sales = 10000;
            int itemsPerTx = 10;
            int totalStock = sales * itemsPerTx;
            int stockOfEach = totalStock / diffPlush; // fuck rounding because fraction of plushie is not plushie
            int stockOfEachReally = stockOfEach * diffPlush; // we may have lost some to rounding
            var stock = new List<int>();

            for (int i = 0; i < diffPlush; i++)
            {
                for (int j = 0; j < stockOfEach; j++)
                {
                    stock.Add(i); // doesn't matter if we stock them deterministically as we extract them randomly
                }
            }

            var dupes = new List<int>();
            for (int i = 0; i < itemsPerTx; i++) dupes.Add(0); // initialise our duplicates map
            // sell em till they're all sold
            while (stock.Count > 0)
            {
                // someone is going to buy some
                int numberToBuy = stock.Count < itemsPerTx ? stock.Count : itemsPerTx; // either take items per tx or stock count, whichever lower
                var shoppingCart = new List<int>();
                // we buy many
                for (int j = 0; j < numberToBuy; j++)
                {
                    // we buy one with randomisation.
                    int randomItem = r.Next(0, stock.Count);
                    shoppingCart.Add(stock[randomItem]);
                    stock.RemoveAt(randomItem);
                }

                // unboxing time!
                var hash = new Dictionary<int, int>();
                // whats our worst duplicates case?
                foreach (var item in shoppingCart)
                {
                    if (hash.ContainsKey(item)) hash[item]++;
                    else hash.Add(item, 0);
                }
                var winner = hash.Max(x => x.Value);
                dupes[winner]++;
            }

            // results
            for (int i = 0; i < dupes.Count; i++)
            {
                Console.WriteLine("{0} dupes happened: {1} times", i, dupes[i]);
            }

            var dir = @"c:\users\username\documents\plushies\data";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var filePath = Path.Combine(dir, "Plushie_Sim_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".csv");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("{0},{1},{2}", "# dupes", String.Empty, "# times");
                    for (int i = 0; i < dupes.Count; i++)
                    {
                        writer.WriteLine("{0},{1},{2},{3}", i, "dupes happened: ", dupes[i], "times");
                    }
                }
            }
        }
    }
}
