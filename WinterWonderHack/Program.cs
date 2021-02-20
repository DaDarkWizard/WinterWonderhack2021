using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterWonderHack
{
    class Program
    {
        static void Main(string[] args)
        {
            var detector = new MaskDetector();
            detector.Run();
            Console.Write("Do not lick the cat");
            Console.Read();
        }
    }
}
