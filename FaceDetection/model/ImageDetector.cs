using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.FaceAnalysis;
using Windows.Storage;
using Windows.Storage.Streams;

namespace FaceDetection.model
{
    public class ImageDetector
    {
        SoftwareBitmap image;
        public ImageDetector(SoftwareBitmap i_image)
        {
            image = i_image;
        }
        public async Task<IList<DetectedFace>> GetFaces()
        {

            // Use FaceDetector.GetSupportedBitmapPixelFormats and IsBitmapPixelFormatSupported to dynamically
            // determine supported formats
            const BitmapPixelFormat faceDetectionPixelFormat = BitmapPixelFormat.Gray8;

            SoftwareBitmap convertedBitmap = null;
            if (image.BitmapPixelFormat != faceDetectionPixelFormat)
            {
                convertedBitmap = SoftwareBitmap.Convert(image, faceDetectionPixelFormat);
            }
            else
            {
                convertedBitmap = image;
            }
            using (convertedBitmap)
            {
                FaceDetector faceDetector = await FaceDetector.CreateAsync();
                IList<DetectedFace> detectedFaces = await faceDetector.DetectFacesAsync(convertedBitmap);
                return detectedFaces;
            }
        }
    }
}
