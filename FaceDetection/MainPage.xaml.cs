using FaceDetection.model;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
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

        private async void ButtonClickImageSource(object sender, RoutedEventArgs _)
        {
            ProcessingRing.IsActive = true;
            //try
            //{
                StorageFile photo = null;
                if (sender == ButtonCamera)
                {
                    photo = await ImageGetter.FromCamera();
                }
                else if (sender == ButtonFile)
                {
                    photo = await ImageGetter.FromDisk();
                }

                if (photo != null)
                {
                    await ProcessPhoto(photo);
                }
            //}
            //catch (Exception e)
            //{
            //    var dialog = new MessageDialog(e.ToString());
            //    await dialog.ShowAsync();
            //}
            ProcessingRing.IsActive = false;

        }

        private async Task ProcessPhoto(StorageFile photo)
        {
            await DrawImageAsync(photo);

            Face[] detectedFaces = await recigniser.GetFaces(photo);
            string[] names = await recigniser.RecogniseFaces(detectedFaces);
            ShowDetectedFaces(detectedFaces, names);

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
                Grid box = new Grid();
                box.Tag = face.FaceRectangle;
                uint width = (uint)(face.FaceRectangle.Width * widthScale);
                box.Width = width;
                uint height = (uint)(face.FaceRectangle.Height * heightScale);
                box.Height = height;
                AcrylicBrush brush = new AcrylicBrush();
                brush.BackgroundSource = AcrylicBackgroundSource.Backdrop;
                brush.TintColor = Colors.Black;
                brush.TintOpacity = 0.25;
                box.Background = brush;
                box.Margin = new Thickness((uint)(face.FaceRectangle.Left * widthScale), (uint)(face.FaceRectangle.Top * heightScale), 0, 0);
                box.Padding = new Thickness(5, 5, 5, 5);
                ToolTip toolTip = new ToolTip();
                toolTip.Content = FaceUtilities.FaceDescription(face);
                ToolTipService.SetToolTip(box, toolTip);
                Viewbox viewbox = new Viewbox();
                viewbox.Stretch = Stretch.Uniform;
                viewbox.StretchDirection = StretchDirection.DownOnly;
                TextBlock text = new TextBlock();
                text.Text = name;
                text.TextWrapping = TextWrapping.Wrap;
                text.FontSize = box.Height / 4;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.HorizontalAlignment = HorizontalAlignment.Center;
                viewbox.Child = text;
                box.Children.Add(viewbox);
                FaceDrawer.Children.Add(box);
            }
        }
    }
}
