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

        public void Run()
        {            
            for (int i = 0; i< 8; i++)
            {
                rawImage = new Mat("../../../Pictures/default" + i + ".png", ImreadModes.Grayscale);
                rawImage.Line(0, 0, 10, 10, new Scalar { Val0 = 10, Val1 = 10, Val2 = 10, Val3 = 10 });

                int threshhold = 100;
                Mat processed = new Mat();

                Cv2.Canny(rawImage, processed, 150, 245);

                Cv2.NamedWindow("Bobby", WindowFlags.Normal);
                Cv2.ImShow("Bobby", rawImage);

                // Reasonable window size time.
                Cv2.ResizeWindow("Bobby", new Size { Height = rawImage.Height / 2, Width = rawImage.Width / 2 });
                Cv2.MoveWindow("Bobby", 0, 0);
            
                Cv2.CreateTrackbar("Lower Canny", "Bobby", ref threshhold, 100, TrackbarHandlers);

                Size duck = new Size(7, 7);

                Cv2.Canny(rawImage, rawImage, 150, 245);
                Cv2.GaussianBlur(rawImage, rawImage, duck, 0, 0);
                Cv2.NamedWindow("Bobby #" + i, WindowFlags.Normal);
                Cv2.ImShow("Bobby #" + i, rawImage);
                Cv2.StartWindowThread();
                while (true)
                {
                    int key = Cv2.WaitKey();
                    if (key == 27)
                        break;
                }
                Cv2.DestroyWindow("Bobby #" + i);
                ImageEncodingParam param = new ImageEncodingParam(ImwriteFlags.PngStrategy, (int)ImwritePNGFlags.StrategyDefault);
                rawImage.SaveImage("../../../Output/output1.png", new ImageEncodingParam[] { param });
            }
            
            
            //Cv2.ImShow()
        }

        void TrackbarHandlers(int pos, IntPtr userData)
        {
            Cv2.Canny(rawImage, processed, pos, 245);

        }
    }
}
