using FaceDetection.model;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace FaceDetection
{
    public sealed partial class MainPage : Page
    {
        double widthScale = 1;
        double heightScale = 1;
        double orgWidth = 0;
        double orgHeight = 0;
        OnlineImageRecogniser recigniser = new OnlineImageRecogniser();
        List<Tuple<Face, string>> tuples = null;
        public MainPage()
        {
            this.InitializeComponent();
            string[] images = FaceUtilities.GetAdminsImage();
            recigniser.TrainFaces(images);
        }

        private async void ButtonClickImageSource(object sender, RoutedEventArgs e)
        {
            StorageFile photo = null;
            if (sender == ButtonCamera)
            {
                photo = await ImageGetter.FromCamera();
            }
            else if (sender == ButtonFile)
            {
                photo = await ImageGetter.FromDisk();
            }

            if (photo == null) return;
            await ProcessPhoto(photo);

        }

        private async Task ProcessPhoto(StorageFile photo)
        {
            await DrawImageAsync(photo);
            ProcessingRing.IsActive = true;

            Face[] detectedFaces = await recigniser.GetFaces(photo);
            string[] names = await recigniser.RecogniseFaces(detectedFaces);
            ShowDetectedFaces(detectedFaces, names);
            ProcessingRing.IsActive = false;
        }

        private async Task DrawImageAsync(StorageFile photo)
        {
            var stream = await photo.OpenReadAsync();
            BitmapImage imageSource = new BitmapImage();
            await imageSource.SetSourceAsync(stream);
            orgWidth = imageSource.PixelWidth;
            orgHeight = imageSource.PixelHeight;
            FacePhoto.Source = imageSource;
        }

        private void ShowDetectedFaces(Face[] faces, string[] names)
        {
            tuples = new List<Tuple<Face, string>>(faces.Zip(names, Tuple.Create));
            RedrawBoxes();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RedrawBoxes();
        }

        private void RedrawBoxes()
        {
            if (tuples == null) return;

            FaceDrawer.Children.Clear();
            widthScale = FacePhoto.ActualWidth / orgWidth; // TODO: scale calculation might work with outdated data
            heightScale = FacePhoto.ActualHeight / orgHeight;
            FaceDrawer.Width = FacePhoto.ActualWidth;
            FaceDrawer.Height = FacePhoto.ActualHeight;
            foreach (var tuple in tuples)
            {
                Face face = tuple.Item1;
                string name = tuple.Item2;
                Rectangle box = new Rectangle();
                box.Tag = face.FaceRectangle;
                uint width = (uint)(face.FaceRectangle.Width * widthScale);
                box.Width = width;
                uint height = (uint)(face.FaceRectangle.Height * heightScale);
                box.Height = height;
                box.Fill = new SolidColorBrush(Colors.Green);
                box.Stroke = new SolidColorBrush(Colors.Green);
                box.StrokeThickness = 20;
                box.Margin = new Thickness((uint)(face.FaceRectangle.Left * widthScale), (uint)(face.FaceRectangle.Top * heightScale), 0, 0);
                RichTextBlock text = new RichTextBlock();
                text.Width = width;
                text.Height = height;
                text.Margin = new Thickness((uint)(face.FaceRectangle.Left * widthScale), (uint)(face.FaceRectangle.Top * heightScale), 0, 0);
                Paragraph paragraph = new Paragraph();
                Run run = new Run();
                run.Text = name + FaceUtilities.FaceDescription(face);
                paragraph.Inlines.Add(run);
                text.Blocks.Add(paragraph);
                FaceDrawer.Children.Add(box);
                FaceDrawer.Children.Add(text);
            }
        }
    }
}
