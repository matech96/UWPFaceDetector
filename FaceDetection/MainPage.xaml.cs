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
                FaceDrawer.Children.Clear();
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

            widthScale = FacePhoto.ActualWidth / orgWidth; // TODO: scale calculation might work with outdated data
            heightScale = FacePhoto.ActualHeight / orgHeight;
            FaceDrawer.Width = FacePhoto.ActualWidth;
            FaceDrawer.Height = FacePhoto.ActualHeight;
            foreach (var tuple in tuples)
            {
                Face face = tuple.Item1;
                string name = tuple.Item2;
                NameBox nameBox = new NameBox();
                nameBox.Width = face.FaceRectangle.Width;// * widthScale;
                nameBox.Height = face.FaceRectangle.Height;// * heightScale;
                nameBox.VerticalAlignment = VerticalAlignment.Top;
                nameBox.HorizontalAlignment = HorizontalAlignment.Left;
                nameBox.Margin = new Thickness(face.FaceRectangle.Left /** widthScale*/, face.FaceRectangle.Top /** heightScale*/, 0, 0);
                nameBox.NameText = name;
                nameBox.Description = FaceUtilities.FaceDescription(face);
                FaceDrawer.Children.Add(nameBox);
            }
        }
    }
}
