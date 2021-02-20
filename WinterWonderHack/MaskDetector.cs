﻿using OpenCvSharp;
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
            var img = new Mat("../../../Pictures/maskless0.png");
            img.CvtColor(ColorConversionCodes.RGBA2BGR);
            detectFaces(img);
            Console.WriteLine("Models Loaded.");
        }

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
                    if (key == 27) //esc key
                        break;
                }
                Cv2.DestroyWindow("Bobby $" + i);
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
            Mat veryProcessed = new Mat();
 
                
            Size duck = new Size { Height = 7, Width = 7 };
            Size goose = new Size { Height = 3, Width = 3 };
            
            Cv2.Threshold(rawImage, processed, 96, 128, (ThresholdTypes) 1);
            Cv2.Canny(processed, moreProcessed, (pos * 25), 245);
            Cv2.Canny(rawImage, processed, (pos * 25), 245);

            Cv2.MorphologyEx(processed, processed, (MorphTypes)4, Cv2.GetStructuringElement((MorphShapes)0, goose));
            Cv2.MorphologyEx(moreProcessed, moreProcessed, (MorphTypes)4, Cv2.GetStructuringElement((MorphShapes)0, goose));
            //Cv2.GaussianBlur(processed, processed, duck, 0, 5);
            //Cv2.GaussianBlur(moreProcessed, moreProcessed, duck, 0, 5);
            veryProcessed = findDifference(processed, moreProcessed);

            //Console.WriteLine(processed.Depth());

            /*RotatedRect mask = Cv2.MinAreaRect((MatType.CV_32S) processed);
            
            Point2f[] maskCorners = new Point2f[4];
            maskCorners = mask.Points();
            for (int n=0; n<4; n++)
            {
                processed.Line((Point)maskCorners[n], (Point)maskCorners[(n + 1) % 4], 2, 2);
            }*/


            Cv2.ImShow("Bobby" + i, processed);
            Cv2.ImShow("Bobby #" + i, moreProcessed);
            Cv2.ImShow("Bobby $" + i, veryProcessed);
            
        }

        Mat findDifference(Mat image0, Mat image1)
        {
            Mat diff = new Mat();
            Cv2.Absdiff(image0, image1, diff);

            Mat difference = new Mat( diff.Rows,diff.Cols,MatType.CV_8UC1);

            for(int k=0; k<image0.Rows; k++)
            {
                for(int m=0; m<image0.Cols; m++)
                {
                    Vec3b pix = diff.At<Vec3b>(k, m);
                    double dist = Math.Sqrt(pix[0]^2 + pix[1]^2 + pix[2]^2);
                    if (dist > 10.0) difference.At<byte>(k, m) = 255;
                }
            }
            return difference; 
            
            System.Media.SoundPlayer player = new System.Media.SoundPlayer("../../../honk.wav");
            player.Play();
        }

        List<Rect> detectFaces(Mat image)
        {
            var blob = CvDnn.BlobFromImage(image, 1, new Size { Height = 192, Width=144 },
                            new Scalar {Val0=104, Val1=117,Val2=123,Val3=0 }, false, false);

            netDet.SetInput(blob);
            var output = netDet.Forward();

            List<Rect> faces = new List<Rect>();

            Console.WriteLine("Output: Width={0}, Height={1}", output.Size().Width, output.Size().Height);

            /*
            //IntPtr i;
            int n = output.Size();

            for (var i = 0, n = output.data32F.length; i < n; i += 7) {
                var confidence = output.data32F[i + 2];
                var left = output.data32F[i + 3] * img.cols;
                var top = output.data32F[i + 4] * img.rows;
                var right = output.data32F[i + 5] * img.cols;
                var bottom = output.data32F[i + 6] * img.rows;
                left = Math.min(Math.max(0, left), img.cols - 1);
                right = Math.min(Math.max(0, right), img.cols - 1);
                bottom = Math.min(Math.max(0, bottom), img.rows - 1);
                top = Math.min(Math.max(0, top), img.rows - 1);

                if (confidence > 0.5 && left < right && top < bottom)
                {
                    faces.push({ x: left, y: top, width: right - left, height: bottom - top})
    }
            }
            blob.delete();
            out.delete();

            unsafe
            {
                output.ForEachAsInt32((int* value, int* position) =>
                {
                    Console.WriteLine("{0} hailing from output[{1}]", *value, *position);
                });
            }
            */

            return faces;
        }


    }
}
