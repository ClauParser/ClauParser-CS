using System;
using System.Diagnostics;

namespace ClauParser_sharp
{
    class Program
    {
        static void Main(string[] args)
        {
            UserType global;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            LoadData.LoadDataFromFile("input.eu4", out global, 0, 0);

            sw.Stop();
            
            //  Console.Out.WriteLine(buffer);

            Console.WriteLine(sw.ElapsedMilliseconds.ToString(), "ms");
            LoadData.SaveWizDB(global, "output.eu4");
        }
    }
}
