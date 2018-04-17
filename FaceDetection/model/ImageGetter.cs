using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;

namespace FaceDetection.model
{
    public class ImageGetter
    {
        public static async Task<SoftwareBitmap> FromCamera()
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.AllowCropping = false;
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;

            StorageFile photoFile = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photoFile == null)
            {
                throw new NoImageException();
            }
            IRandomAccessStream fileStream = null;
            using (fileStream = await photoFile.OpenAsync(FileAccessMode.Read))
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
                return sourceBitmap;
            }
        }
    }
}
