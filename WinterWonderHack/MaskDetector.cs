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
          
           
            Cv2.NamedWindow("Bobby", WindowFlags.Normal);
            Cv2.ImShow("Bobby", x);
            Cv2.StartWindowThread();

            ImageEncodingParam param = new ImageEncodingParam(ImwriteFlags.PngStrategy, (int)ImwritePNGFlags.StrategyDefault);
            x.SaveImage("../../../Output/output1.png", new ImageEncodingParam[] {param});
            x.Canny(x, x, 0, 300, 3);



            //Cv2.ImShow()
        }
    }
}
