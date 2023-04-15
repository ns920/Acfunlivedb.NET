using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acfunlivedb.NET.Utils
{
    public static class ConsoleLog
    {
        public static void WriteLine(string str)
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine(time + " " + str);
        }
    }
}