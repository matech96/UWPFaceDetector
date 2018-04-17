using FaceDetection.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.FaceAnalysis;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FaceDetection
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        IList<DetectedFace> detectedFaces;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Brush brush = new SolidColorBrush(Colors.Green);
            FaceDrawer.Background = brush;
            SoftwareBitmap sourceBitmap = null;
            using (sourceBitmap = await ImageGetter.FromCamera())
            {
                ImageDetector detector = new ImageDetector(sourceBitmap);
                detectedFaces = await detector.GetFaces();
                ShowDetectedFaces(sourceBitmap, detectedFaces);
            }


        }

        private async void ShowDetectedFaces(SoftwareBitmap sourceBitmap, IList<DetectedFace> faces)
        {
            ImageBrush brush = new ImageBrush();
            SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
            await bitmapSource.SetBitmapAsync(sourceBitmap);
            brush.ImageSource = bitmapSource;
            brush.Stretch = Stretch.Fill;
            this.FaceDrawer.Background = brush;

            if (detectedFaces != null)
            {
                double widthScale = 1; // sourceBitmap.PixelWidth / this.FaceDrawer.ActualWidth;
                double heightScale = 1; // sourceBitmap.PixelHeight / this.FaceDrawer.ActualHeight;

                foreach (DetectedFace face in detectedFaces)
                {
                    // Create a rectangle element for displaying the face box but since we're using a Canvas
                    // we must scale the rectangles according to the image’s actual size.
                    // The original FaceBox values are saved in the Rectangle's Tag field so we can update the
                    // boxes when the Canvas is resized.
                    Rectangle box = new Rectangle();
                    box.Tag = face.FaceBox;
                    box.Width = (uint)(face.FaceBox.Width / widthScale);
                    box.Height = (uint)(face.FaceBox.Height / heightScale);
                    box.Fill = new SolidColorBrush(Colors.Green);
                    box.Stroke = new SolidColorBrush(Colors.Green);
                    box.StrokeThickness = 20;
                    box.Margin = new Thickness((uint)(face.FaceBox.X / widthScale), (uint)(face.FaceBox.Y / heightScale), 0, 0);

                    FaceDrawer.Children.Add(box);
                }
            }
        }
    }
}
