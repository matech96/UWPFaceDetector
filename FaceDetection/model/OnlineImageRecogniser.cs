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
        private readonly string personGroupId = "admins";


        public async void TrainFaces(string[] trainImages)
        {
            //await faceServiceClient.CreatePersonGroupAsync(personGroupId, "Admins");
            CreatePersonResult admin = await faceServiceClient.CreatePersonInPersonGroupAsync(personGroupId, "Akos");

            foreach (string imagePath in trainImages)
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    await faceServiceClient.AddPersonFaceInPersonGroupAsync(personGroupId, admin.PersonId, s);
                }
            }

            await faceServiceClient.TrainPersonGroupAsync(personGroupId);
        }

        private async Task AsyncWaitTrainer()
        {
            TrainingStatus trainingStatus = null;
            while (true)
            {
                trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

                if (trainingStatus.Status != Status.Running)
                {
                    break;
                }

                await Task.Delay(1000);
            }
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
        public async Task<string[]> RecogniseFaces(Face[] faces)
        {
            await AsyncWaitTrainer();
            List<string> names = new List<string>();
            foreach (Face face in faces)
            {
                Guid[] faceIds = faces.Select(f => f.FaceId).ToArray();
                IdentifyResult[] results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);
                foreach (IdentifyResult identifyResult in results)
                {
                    if (identifyResult.Candidates.Length != 0)
                    {
                        Guid candidateId = identifyResult.Candidates[0].PersonId;
                        Person person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                        names.Add(person.Name);
                    }
                    else
                    {
                        names.Add("Unknown");
                    }
                }
            }
            return names.ToArray();
        }
    }
}
