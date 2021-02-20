using OpenCvSharp;
using OpenCvSharp.Dnn;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WinterWonderHack
{

    class MaskDetector
    {
        Net netDet;
        Net netRecogn;
        Mat rawImage;
        VideoCapture video;
        Mat processed = new Mat();
        Mat moreProcessed = new Mat();
        int i = 0;
        DateTime lastPlayed = DateTime.Now;
        

        public void Start()
        {
            Console.WriteLine("Loading models...");
            netDet = CvDnn.ReadNetFromCaffe("../../../face_detector.prototxt", "../../../face_detector.caffemodel");
            netRecogn = CvDnn.ReadNetFromTorch("../../../face_recognition.t7");
            video = VideoCapture.FromCamera(0);
            rawImage = new Mat(new Size(video.FrameWidth, video.FrameHeight), MatType.CV_8UC4);
            Console.WriteLine("Models Loaded.");
        }

        public async void Run()
        {            
            while(true)
            {
                int[] topAveColors = new int[3];
                int[] botAveColors = new int[3];
                int totalDiff = 0;
                System.Media.SoundPlayer player = new System.Media.SoundPlayer("../../../honk.wav");

                rawImage = video.RetrieveMat();
                
                //rawImage = new Mat(fileImage);
                rawImage.CvtColor(ColorConversionCodes.RGBA2BGR);
                var rects = detectFaces(rawImage);
                if(rects.Count < 1)
                {
                    Console.WriteLine("Ope");
                    continue;
                }
                processed = new Mat(rawImage, rects[0]);

                Mat top = new Mat(processed, new Rect() { X = processed.Width / 4, Y = processed.Height / 20,
                    Width = processed.Width / 2, Height = processed.Height / 3 });
                
                Mat bottom = new Mat(processed, new Rect()
                {
                    X = processed.Width / 4,
                    Y = processed.Height - (processed.Height / 20) - (processed.Height / 3),
                    Width = processed.Width / 2,
                    Height = processed.Height / 3
                });

                //Cv2.ImShow("Top", top);
                //Cv2.NamedWindow("Top");
                //Cv2.MoveWindow("Top", processed.Width, 0);

                //Cv2.ImShow("Bottom", bottom);
                //Cv2.NamedWindow("Bottom");
                //Cv2.MoveWindow("Bottom", processed.Width, top.Height + 60);

                //Cv2.ImShow("Bobby" + i, processed);
                //Cv2.NamedWindow("Bobby" + i, WindowFlags.Normal);
                //Cv2.MoveWindow("Bobby" + i, 0, 0);

                topAveColors = colorAverage(top);
                botAveColors = colorAverage(bottom);

                for (int p = 0; p < 3; p++)
                    totalDiff += Math.Abs(topAveColors[p] - botAveColors[p]);


                Console.WriteLine(totalDiff);
                if (totalDiff <= 100)
                {
                    if ((DateTime.Now - lastPlayed).Seconds > .5)
                    { 
                        player.Play();
                        lastPlayed = DateTime.Now;
                    }
                    //player.O
                }

                //while (true)
                //{
                    //int key = Cv2.WaitKey();
                    //if (key == 27) //esc key
                    //    break;
                //}
                //Cv2.DestroyWindow("Bobby" + i);
                //await Task.Delay(1);
                Cv2.DestroyAllWindows();
            }
            Console.WriteLine("Out!");
            Console.Read();
            Thread.Sleep(500);
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

            unsafe
            {
                int i = 0;
                for(float* a = (float*)output.Data.ToPointer(); a + 5 < output.DataEnd.ToPointer(); a += 7)
                {

                    float val1 = a[i];
                    float val2 = a[i + 1];
                    float confidence = ((float*)a)[i + 2];
                    int left = (int)(a[i + 3] * image.Cols);
                    int top = (int)(a[i + 4] * image.Rows);
                    int right = (int)(a[i + 5] * image.Cols);
                    int bottom = (int)(a[i + 6] * image.Rows);
                    left = Math.Min(Math.Max(0, left), image.Cols - 1);
                    right = Math.Min(Math.Max(0, right), image.Cols - 1);
                    bottom = Math.Min(Math.Max(0, bottom), image.Rows - 1);
                    top = Math.Min(Math.Max(0, top), image.Rows - 1);

                    if (confidence > 0.5 && left < right && top < bottom && val1 == 0 && val2 == 1)
                    {
                        faces.Add(new Rect(){ X=left, Y= top, Width= right - left, Height= bottom - top});
                        i += 7;
                    }
                }
            }


            return faces;
        }
        
        int[] colorAverage(Mat image)
        {
            int[] colors = new int[3];
            Mat aveColor = new Mat(image.Rows, image.Cols, MatType.CV_8UC1);
            Scalar final = new Scalar();

            for (int k = 0; k < image.Rows; k++)
            {
                for (int m = 0; m < image.Cols; m++)
                {
                    Vec3b pix = image.At<Vec3b>(k, m);

                    for(int n=0; n<3; n++) 
                        colors[n] += pix[n];

                }
            }
            for (int n = 0; n < 3; n++) colors[n] /= (image.Rows*image.Cols);

            //System.Media.SoundPlayer player = new System.Media.SoundPlayer("../../../honk.wav");
            //player.Play();

            return colors;
        }

    }
}
