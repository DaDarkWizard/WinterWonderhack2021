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
            Console.WriteLine("Do not lick the cat");
            var detector = new MaskDetector();
            detector.Run();
            
            //Console.Read();
        }
    }
}
