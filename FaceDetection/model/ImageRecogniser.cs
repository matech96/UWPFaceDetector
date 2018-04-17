using Microsoft.ProjectOxford.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetection.model
{
    class ImageRecogniser
    {
        private readonly IFaceServiceClient faceServiceClient = 
            new FaceServiceClient("d0e2334e2d1646adbf8ca5856d957445", "https://westcentralus.api.cognitive.microsoft.com/face/v1.0");
    }
}
