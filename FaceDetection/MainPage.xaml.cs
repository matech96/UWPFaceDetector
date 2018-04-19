using FaceDetection.model;
using FaceDetection.viewModel;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace FaceDetection
{
    public sealed partial class MainPage : Page
    {
        ImageRecogniserViewModel viewModel = new ImageRecogniserViewModel();
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void ButtonClickImageSource(object sender, RoutedEventArgs _)
        {
            ProcessingRing.IsActive = true;
            SourceType type = SourceType.Camera;
            if (sender == ButtonFile)
            {
                type = SourceType.File;
            }
            BitmapSource imageSource = await viewModel.OpenPhoto(type);
            if (imageSource != null)
            {
                FacePhoto.Source = imageSource;
                try
                {
                    FaceDrawer.Children.Clear();
                    List<NameBox> nameBoxes = await viewModel.GetNameBoxes();
                    FaceDrawer.Width = FacePhoto.ActualWidth;
                    FaceDrawer.Height = FacePhoto.ActualHeight;
                    foreach (NameBox nameBox in nameBoxes)
                    {
                        FaceDrawer.Children.Add(nameBox);
                    }
                }
                catch (Exception e)
                {
                    var dialog = new MessageDialog(e.ToString());
                    await dialog.ShowAsync();
                }
            }
            ProcessingRing.IsActive = false;

        }

        private async void FacePhoto_ImageOpened(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Image Opened!");
            await dialog.ShowAsync();
        }
    }
}
