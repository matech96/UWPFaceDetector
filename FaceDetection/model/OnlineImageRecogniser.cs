using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace FaceDetection.model
{
    public class OnlineImageRecogniser
    {
        private readonly IFaceServiceClient faceServiceClient =
            new FaceServiceClient("d0e2334e2d1646adbf8ca5856d957445", "https://westcentralus.api.cognitive.microsoft.com/face/v1.0");
        
        public OnlineImageRecogniser()
        {
        }

        public async Task<Face[]> GetFaces(StorageFile image)
        {
            IEnumerable<FaceAttributeType> faceAttributes =
                new FaceAttributeType[] { FaceAttributeType.Gender,
                    FaceAttributeType.Age,
                    FaceAttributeType.Smile,
                    FaceAttributeType.Emotion,
                    FaceAttributeType.Glasses,
                    FaceAttributeType.Hair };
            using (Stream imageFileStream = await image.OpenStreamForReadAsync())
            {
                Face[] faces = await faceServiceClient.DetectAsync(imageFileStream,
                    returnFaceId: true,
                    returnFaceLandmarks: false,
                    returnFaceAttributes: faceAttributes);
                return faces;
            }
        }
    }
}
