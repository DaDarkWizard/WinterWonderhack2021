using OpenCvSharp;
using OpenCvSharp.Dnn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterWonderHack
{

    class MaskDetector
    {
        Net netDet;
        Net netRecogn;
        Mat rawImage;
        Mat processed = new Mat();
        Mat moreProcessed = new Mat();
        int i = 0;

        public void Start()
        {
            Console.WriteLine("Loading models...");
            netDet = CvDnn.ReadNetFromCaffe("../../../face_detector.prototxt", "../../../face_detector.caffemodel");
            netRecogn = CvDnn.ReadNetFromTorch("../../../face_recognition.t7");
            
            Console.WriteLine("Models Loaded.");
        }

        public void Run()
        {            
            for (i = 0; i< 8; i++)
            {
                rawImage = new Mat("../../../Pictures/default" + i + ".png");
                rawImage.CvtColor(ColorConversionCodes.RGBA2BGR);
                var rects = detectFaces(rawImage);
                rawImage = new Mat(rawImage, rects[0]);
                Cv2.ImShow("Bobby" + i, rawImage);

                Cv2.NamedWindow("Bobby" + i, WindowFlags.Normal);
                

                // Reasonable window size time.
                
                Cv2.MoveWindow("Bobby" + i, 0, 0);
            


                System.Media.SoundPlayer player = new System.Media.SoundPlayer("../../../honk.wav");
                player.Play();


                while (true)
                {
                    int key = Cv2.WaitKey();
                    if (key == 27) //esc key
                        break;
                }
                Cv2.DestroyWindow("Bobby" + i);
            }
            
            
            //Cv2.ImShow()
        }



        List<Rect> detectFaces(Mat image)
        {
            var blob = CvDnn.BlobFromImage(image, 1, new Size { Height = 192, Width=144 },
                            new Scalar {Val0=104, Val1=117,Val2=123,Val3=0 }, false, false);

            netDet.SetInput(blob);
            var output = netDet.Forward("detection_out");
            
            var x = netDet.GetLayerNames();


            List<Rect> faces = new List<Rect>();

            Console.WriteLine(sizeof(double));

            unsafe
            {
                int i = 0;
                for(float* a = (float*)output.Data.ToPointer(); a + 5 < output.DataEnd.ToPointer(); a += 7)
                {
                    float confidence = ((float*)a)[i + 2];
                    int left = (int)(a[i + 3] * image.Cols);
                    int top = (int)(a[i + 4] * image.Rows);
                    int right = (int)(a[i + 5] * image.Cols);
                    int bottom = (int)(a[i + 6] * image.Rows);
                    left = Math.Min(Math.Max(0, left), image.Cols - 1);
                    right = Math.Min(Math.Max(0, right), image.Cols - 1);
                    bottom = Math.Min(Math.Max(0, bottom), image.Rows - 1);
                    top = Math.Min(Math.Max(0, top), image.Rows - 1);

                    if (confidence > 0.5 && left < right && top < bottom)
                    {
                        faces.Add(new Rect(){ X=left, Y= top, Width= right - left, Height= bottom - top});
                        i += 7;
                    }
                }
            }


            return faces;
        }


    }
}
