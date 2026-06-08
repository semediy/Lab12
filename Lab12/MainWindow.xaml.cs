using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Lab12
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;

        private Line hourHand;
        private Line minuteHand;
        private Line secondHand;

        private double centerX = 300;
        private double centerY = 300;
        private double radius = 220;

        public MainWindow()
        {
            InitializeComponent();

            DrawClockFace();
            CreateHands();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            UpdateClock();
        }

        private void DrawClockFace()
        {
            Ellipse circle = new Ellipse
            {
                Width = radius * 2,
                Height = radius * 2,
                Stroke = Brushes.Black,
                StrokeThickness = 4
            };

            Canvas.SetLeft(circle, centerX - radius);
            Canvas.SetTop(circle, centerY - radius);

            MainCanvas.Children.Add(circle);

            for (int i = 0; i < 60; i++)
            {
                double angle = i * 6 * Math.PI / 180;

                double r1 = radius - 10;
                double r2 = radius - 30;

                if (i % 5 != 0)
                    r2 = radius - 20;

                Line mark = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = (i % 5 == 0) ? 3 : 1
                };

                mark.X1 = centerX + r1 * Math.Sin(angle);
                mark.Y1 = centerY - r1 * Math.Cos(angle);

                mark.X2 = centerX + r2 * Math.Sin(angle);
                mark.Y2 = centerY - r2 * Math.Cos(angle);

                MainCanvas.Children.Add(mark);
            }
        }

        private void CreateHands()
        {
            hourHand = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 6
            };

            minuteHand = new Line
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 4
            };

            secondHand = new Line
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };

            MainCanvas.Children.Add(hourHand);
            MainCanvas.Children.Add(minuteHand);
            MainCanvas.Children.Add(secondHand);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateClock();
        }

        private void UpdateClock()
        {
            DateTime now = DateTime.Now;

            double secondAngle = now.Second * 6;

            double minuteAngle =
                now.Minute * 6 +
                now.Second * 0.1;

            double hourAngle =
                (now.Hour % 12) * 30 +
                now.Minute * 0.5;

            SetHand(secondHand, radius - 50, secondAngle);
            SetHand(minuteHand, radius - 70, minuteAngle);
            SetHand(hourHand, radius - 110, hourAngle);
        }

        private void SetHand(Line hand,
                             double length,
                             double angle)
        {
            double rad = angle * Math.PI / 180;

            hand.X1 = centerX;
            hand.Y1 = centerY;

            hand.X2 = centerX + length * Math.Sin(rad);
            hand.Y2 = centerY - length * Math.Cos(rad);
        }

        private void SaveButton_Click(object sender,
                                      RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "PNG Image|*.png";

            if (dialog.ShowDialog() == true)
            {
                RenderTargetBitmap bitmap = new RenderTargetBitmap((int)MainCanvas.ActualWidth, (int)MainCanvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                bitmap.Render(MainCanvas);

                PngBitmapEncoder encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                using (FileStream fs = new FileStream(dialog.FileName,FileMode.Create))
                {
                    encoder.Save(fs);
                }

                MessageBox.Show("Зображення збережено!");
            }
        }
    }
}