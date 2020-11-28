using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using System.Windows.Forms;

namespace Лабораторная_2
{
    class Func
    {
        public Image<Bgr, byte> sourceImage; //глобальная переменная
        public Image<Bgr, byte> binarImage;

        public void Source(string fileName)
        {
            sourceImage = new Image<Bgr, byte>(fileName);

        }

        public Image<Bgr, byte> Binarization(int thresholdValue)
        {
            var resultImage = sourceImage.CopyBlank();

            var grayImage = sourceImage.Convert<Gray, byte>();
            int kernelSize = 3;
            var bluredImage = grayImage.SmoothGaussian(kernelSize);

            var threshold = new Gray(thresholdValue);
            var color = new Gray(255);
            var binarizedImage = bluredImage.ThresholdBinary(threshold, color);

            resultImage = binarizedImage.Convert<Bgr, byte>();
            binarImage = resultImage;
            return resultImage;
        }

        public Image<Bgr, byte> Circuits(int thresholdValue)
        {
            if (binarImage == null)
            {
                binarImage = Binarization(thresholdValue);
            }

            var resultImage = binarImage.Convert<Gray, byte>();
            
            // shapes
            var contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(
                resultImage, // исходное чёрно-белое изображение
                contours, // найденные контуры
                null, // объект для хранения иерархии контуров (в данном случае не используется)
                RetrType.List, // структура возвращаемых данных (в данном случае список)
                ChainApproxMethod.ChainApproxSimple); // метод аппроксимации (сжимает горизонтальные,
                                                       //вертикальные и диагональные сегменты
                                                       //и оставляет только их конечные точки)

            var contoursImage = sourceImage.CopyBlank(); //создание "пустой" копии исходного изображения
            for (int i = 0; i < contours.Size; i++)
            {
                var points = contours[i].ToArray();
                contoursImage.Draw(points, new Bgr(Color.GreenYellow), 2); // отрисовка точек
            }
            return contoursImage;
        }

        public Image<Bgr, byte> Search(int thresholdValue, int minArea, Label label)
        {
            if (binarImage == null)
            {
                binarImage = Binarization(thresholdValue);
            }

            var resultImage = binarImage.Convert<Gray, byte>();

            int triangle = 0;
            int rectangle = 0;
            int circleС = 0;

            // shapes
            var contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(
                resultImage, // исходное чёрно-белое изображение
                contours, // найденные контуры
                null, // объект для хранения иерархии контуров (в данном случае не используется)
                RetrType.List, // структура возвращаемых данных (в данном случае список)
                ChainApproxMethod.ChainApproxSimple); // метод аппроксимации (сжимает горизонтальные,
                                                      //вертикальные и диагональные сегменты
                                                      //и оставляет только их конечные точки)

            var contoursImage = sourceImage.Copy();
            for (int i = 0; i < contours.Size; i++)
            {
                var approxContour = new VectorOfPoint();

                CvInvoke.ApproxPolyDP(contours[i], approxContour, CvInvoke.ArcLength(contours[i], true) * 0.05, true);
                var points = approxContour.ToArray();

                if (CvInvoke.ContourArea(approxContour, false) > minArea)
                {
                    if (approxContour.Size == 3)
                    {
                        triangle++;
                        var pointsTri = approxContour.ToArray();
                        contoursImage.Draw(new Triangle2DF(pointsTri[0], pointsTri[1], pointsTri[2]), new Bgr(Color.GreenYellow), 2);
                    }
                }

                if (isRectangle(points))
                {
                    if (CvInvoke.ContourArea(approxContour, false) > minArea)
                    {
                        rectangle++;
                        contoursImage.Draw(CvInvoke.MinAreaRect(approxContour), new Bgr(Color.Purple), 2);
                    }
                }

            }

            List<CircleF> circles = new List<CircleF>(CvInvoke.HoughCircles(resultImage, 
                HoughModes.Gradient, 
                1.0, 
                250, 
                100, 
                50, 
                5, 
                500));

            foreach (CircleF circle in circles)
            {
                circleС++;
                contoursImage.Draw(circle, new Bgr(Color.Pink), 2);
            }

            label.Text = "Количество треугольников = " + triangle + "\nКоличество прямоугольников = " + rectangle + "\nКоличество кругов = " + circleС;
            return contoursImage;
        }

        private bool isRectangle(Point[] points)
        {
            int delta = 10; // максимальное отклонение от прямого угла
            LineSegment2D[] edges = PointCollection.PolyLine(points, true);
            for (int i = 0; i < edges.Length; i++)
            {
                double angle = Math.Abs(edges[(i + 1) %
                    edges.Length].GetExteriorAngleDegree(edges[i]));
                if (angle < 90 - delta || angle > 90 + delta) // еслиуголнепрямой
                {
                    return false;
                }
            }
            return true;
        }

        public Image<Bgr, byte> SearchColor()
        {
            var hsvImage = sourceImage.Convert<Hsv, byte>();
            var hueChannel = hsvImage.Split()[0];
            byte color = 30; // соответствует желтому тону в Emgu.CV
            byte rangeDelta = 10; // величина разброса цвета
            var resultImage = hueChannel.InRange(new Gray(color - rangeDelta), new Gray(color + rangeDelta)); // выделение цвета

            return resultImage.Convert<Bgr, byte>();
        }
    }
}