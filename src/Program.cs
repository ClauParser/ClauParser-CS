using System;
using System.Diagnostics;

namespace ClauParser_sharp
{
    class Program
    {
        static void Main(string[] args)
        {
            InFileReserver test = new InFileReserver("input.eu4");
            string buffer;
            long buffer_len;
            long[] token_arr;
            long token_arr_len;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            test.Run(8, out buffer, out buffer_len, out token_arr, out token_arr_len);

            sw.Stop();
            
            //  Console.Out.WriteLine(buffer);

            Console.WriteLine(sw.ElapsedMilliseconds.ToString(), "ms");
            Console.Out.WriteLine(token_arr_len); // 12234640
        }
    }
}
