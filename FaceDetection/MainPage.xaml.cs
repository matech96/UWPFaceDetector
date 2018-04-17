using FaceDetection.model;
using Microsoft.ProjectOxford.Face.Contract;
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
using Windows.UI.Xaml.Documents;
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
        //IList<DetectedFace> detectedFaces;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            StorageFile photo = await ImageGetter.FromCamera();
            if (photo == null) return;

            //using (IRandomAccessStream fileStream = await photo.OpenAsync(Windows.Storage.FileAccessMode.Read))
            //{
            //    BitmapImage bitmapImage = new BitmapImage
            //    {
            //        DecodePixelHeight = 720,
            //        DecodePixelWidth = 1280
            //    };
            //    await bitmapImage.SetSourceAsync(fileStream);
            //    FacePhoto.Source = bitmapImage;
            //}
            using (IRandomAccessStream fileStream = await photo.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);

                BitmapTransform transform = new BitmapTransform();
                const float sourceImageHeightLimit = 1280;

                if (decoder.PixelHeight > sourceImageHeightLimit)
                {
                    float scalingFactor = (float)sourceImageHeightLimit / (float)decoder.PixelHeight;
                    transform.ScaledWidth = (uint)Math.Floor(decoder.PixelWidth * scalingFactor);
                    transform.ScaledHeight = (uint)Math.Floor(decoder.PixelHeight * scalingFactor);
                }

                SoftwareBitmap sourceBitmap = await decoder.GetSoftwareBitmapAsync(decoder.BitmapPixelFormat,
                    BitmapAlphaMode.Premultiplied,
                    transform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage);
                ImageBrush brush = new ImageBrush();
                SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
                await bitmapSource.SetBitmapAsync(sourceBitmap);
                brush.ImageSource = bitmapSource;
                brush.Stretch = Stretch.Fill;
                this.FaceDrawer.Background = brush;
            }

            string[] images = FaceUtilities.GetAdminsImage();
            OnlineImageRecogniser recigniser = new OnlineImageRecogniser();
            Face[] detectedFaces = await recigniser.GetFaces(photo);

            ShowDetectedFaces(detectedFaces);


        }

        private void ShowDetectedFaces(Face[] faces)
        {
            if (faces != null)
            {
                double widthScale = 1; // sourceBitmap.PixelWidth / this.FaceDrawer.ActualWidth;
                double heightScale = 1; // sourceBitmap.PixelHeight / this.FaceDrawer.ActualHeight;

                foreach (Face face in faces)
                {
                    // Create a rectangle element for displaying the face box but since we're using a Canvas
                    // we must scale the rectangles according to the image’s actual size.
                    // The original FaceBox values are saved in the Rectangle's Tag field so we can update the
                    // boxes when the Canvas is resized.
                    Rectangle box = new Rectangle
                    {
                        Tag = face.FaceRectangle,
                        Width = (uint)(face.FaceRectangle.Width / widthScale),
                        Height = (uint)(face.FaceRectangle.Height / heightScale),
                        Fill = new SolidColorBrush(Colors.Green),
                        Stroke = new SolidColorBrush(Colors.Green),
                        StrokeThickness = 20,
                        Margin = new Thickness((uint)(face.FaceRectangle.Left / widthScale), (uint)(face.FaceRectangle.Top / heightScale), 0, 0)
                    };
                    RichTextBlock text = new RichTextBlock();
                    //text.Text = FaceUtilities.FaceDescription(face);
                    text.Margin = new Thickness((uint)(face.FaceRectangle.Left / widthScale), (uint)(face.FaceRectangle.Top / heightScale), 0, 0);
                    Paragraph paragraph = new Paragraph();
                    Run run = new Run();
                    run.Text = FaceUtilities.FaceDescription(face);
                    paragraph.Inlines.Add(run);
                    text.Blocks.Add(paragraph);
                    FaceDrawer.Children.Add(box);
                    FaceDrawer.Children.Add(text);
                }
            }
        }
    }
}
