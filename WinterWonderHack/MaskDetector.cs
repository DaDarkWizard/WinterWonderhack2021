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
        Mat rawImage;
        Mat processed = new Mat();
        Mat moreProcessed = new Mat();
        int i = 0;

        public void Run()
        {            
            for (i = 0; i< 8; i++)
            {
                rawImage = new Mat("../../../Pictures/default" + i + ".png", ImreadModes.Grayscale);
                rawImage.Line(0, 0, 10, 10, new Scalar { Val0 = 10, Val1 = 10, Val2 = 10, Val3 = 10 });

                int threshhold = 10;
                Cv2.ResizeWindow("Bobby" + i, new Size { Height = rawImage.Height / 2, Width = rawImage.Width / 2 });
                TrackbarHandlers(threshhold, new IntPtr { });

                Cv2.NamedWindow("Bobby" + i, WindowFlags.Normal);
                

                // Reasonable window size time.
                
                Cv2.MoveWindow("Bobby" + i, 0, 0);
            
                Cv2.CreateTrackbar("Lower Canny", "Bobby" + i, ref threshhold, 20, TrackbarHandlers);

                

                
                Cv2.NamedWindow("Bobby #" + i, WindowFlags.Normal);
               

                Cv2.StartWindowThread();
                while (true)
                {
                    int key = Cv2.WaitKey();
                    if (key == 27)
                        break;
                }
                Cv2.DestroyWindow("Bobby #" + i);
                Cv2.DestroyWindow("Bobby" + i);
                ImageEncodingParam param = new ImageEncodingParam(ImwriteFlags.PngStrategy, (int)ImwritePNGFlags.StrategyDefault);
                rawImage.SaveImage("../../../Output/output1.png", new ImageEncodingParam[] { param });
            }
            
            
            //Cv2.ImShow()
        }

        void TrackbarHandlers(int pos, IntPtr userData)
        {
            processed = new Mat();
            moreProcessed = new Mat();
            Cv2.Canny(rawImage, processed, (pos * 25), 245);
            Cv2.ImShow("Bobby" + i, processed);
            Size duck = new Size { Height = 7, Width = 7 };
            Cv2.GaussianBlur(processed, moreProcessed, duck, 0, 0);
            Cv2.ImShow("Bobby #" + i, processed);
            
            
        }
    }
}
