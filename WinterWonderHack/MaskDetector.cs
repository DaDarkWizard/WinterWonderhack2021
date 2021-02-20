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
            Mat rawImage = new Mat("../../../Pictures/default7.png", ImreadModes.Grayscale);
            rawImage.Line(0, 0, 10, 10, new Scalar { Val0 = 10, Val1 = 10, Val2 = 10, Val3 = 10 });

            Cv2.Canny(rawImage, rawImage, 150, 245);
            
            Cv2.NamedWindow("Bobby", WindowFlags.Normal);
            Cv2.ImShow("Bobby", rawImage);
            Cv2.StartWindowThread();
            while (true)
            {
                int key = Cv2.WaitKey();
                if (key == 27)
                    break;
            }
            
            ImageEncodingParam param = new ImageEncodingParam(ImwriteFlags.PngStrategy, (int)ImwritePNGFlags.StrategyDefault);
            rawImage.SaveImage("../../../Output/output1.png", new ImageEncodingParam[] {param});

            
            
            //Cv2.ImShow()
        }
    }
}
