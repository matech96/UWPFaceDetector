using FaceDetection.model;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace FaceDetection.viewModel
{
    class ImageRecogniserViewModel
    {
        StorageFile photo = null;
        OnlineImageRecogniser recigniser = new OnlineImageRecogniser();
        public ImageRecogniserViewModel()
        {
            string[] images = FaceUtilities.GetAdminsImage();
            recigniser.TrainFaces(images);
        }

        public async Task<BitmapImage> OpenPhoto(SourceType type)
        {
            switch (type)
            {
                case SourceType.Camera:
                    this.photo = await ImageGetter.FromCamera();
                    break;
                case SourceType.File:
                    this.photo = await ImageGetter.FromDisk();
                    break;
            }
            if(this.photo == null) { return null; }

            var stream = await photo.OpenReadAsync();
            BitmapImage imageSource = new BitmapImage();
            await imageSource.SetSourceAsync(stream);
            return imageSource;
        }

        public async Task<List<Tuple<Face, string>>> RecogniseFaces()
        {
            Face[] faces = await recigniser.GetFaces(this.photo );
            string[] names = await recigniser.RecogniseFaces(faces);
            return new List<Tuple<Face, string>>(faces.Zip(names, Tuple.Create));
        }

        public async Task<List<NameBox>> GetNameBoxes()
        {
            var tuples = await this.RecogniseFaces();
            List<NameBox> res = new List<NameBox>();
            foreach (var tuple in tuples)
            {
                Face face = tuple.Item1;
                string name = tuple.Item2;
                NameBox nameBox = new NameBox();
                nameBox.Width = face.FaceRectangle.Width;
                nameBox.Height = face.FaceRectangle.Height;
                nameBox.VerticalAlignment = VerticalAlignment.Top;
                nameBox.HorizontalAlignment = HorizontalAlignment.Left;
                nameBox.Margin = new Thickness(face.FaceRectangle.Left, face.FaceRectangle.Top, 0, 0);
                nameBox.NameText = name;
                nameBox.Description = FaceUtilities.FaceDescription(face);
                res.Add(nameBox);
            }
            return res;
        }
    }
}
