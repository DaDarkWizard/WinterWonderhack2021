using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterWonderHack
{
    class MaskDetector
    {
        public void Run()
        {
            Mat x = new Mat("../../../Pictures/default0.png", ImreadModes.Grayscale);

            ImageEncodingParam param = new ImageEncodingParam(ImwriteFlags.PngStrategy, (int)ImwritePNGFlags.StrategyDefault);
            x.SaveImage("../../../Output/output1.png", new ImageEncodingParam[] {param});

        }
    }
}
