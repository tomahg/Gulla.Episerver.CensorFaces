using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace Gulla.EpiserverCensorFaces.Core
{
    public static class CensorFaceHelper
    {
        private static IFaceClient _client;
        private static IConfiguration _configuration;
        private static string _subscriptionKey;
        private static string _endpoint;

        private const string RecognitionModel = Microsoft.Azure.CognitiveServices.Vision.Face.Models.RecognitionModel.Recognition04;
        private const float CensorWidthModifier = 0.25f;
        private const float CensorHeightModifier = 4;

        private static IConfiguration Configuration => _configuration ??= ServiceLocator.Current.GetInstance<IConfiguration>();
        private static string Endpoint => _endpoint ??= Configuration.GetValue<string>("Gulla:EpiserverCensorFaces:CognitiveServices:Endpoint");
        private static string SubscriptionKey => _subscriptionKey ??= Configuration.GetValue<string>("Gulla:EpiserverCensorFaces:CognitiveServices:SubscriptionKey");
        private static IFaceClient Client => _client ??= new FaceClient(new ApiKeyServiceClientCredentials(SubscriptionKey)) { Endpoint = Endpoint };


        public static void CensorBinaryData(Blob imageBlob)
        {
            var detectedFaces = GetFaces(imageBlob.OpenRead());
            if (!detectedFaces.Any()) return;

            var image = Image.Load(new MemoryStream(imageBlob.ReadAllBytes()));
            foreach (var face in detectedFaces)
            {
                CensorFace(face, image);
            }

            UpdateImageBlob(imageBlob, image);
        }

        private static IList<DetectedFace> GetFaces(Stream image)
        {
            var task = Task.Run(async () => await DetectFaces(image));
            return task.Result;
        }

        private static async Task<IList<DetectedFace>> DetectFaces(Stream image)
        {
            return await Client.Face.DetectWithStreamAsync(image,
                recognitionModel: RecognitionModel,
                returnFaceLandmarks: true);
        }

        private static void CensorFace(DetectedFace face, Image image)
        {
            image.Mutate(ctx => ctx.DrawLine(Color.Black, GetCensorHeight(face), GetCensorLeftPoint(face), GetCensorRightPoint(face)));
        }

        private static float GetCensorHeight(DetectedFace face)
        {
            return CensorHeightModifier * (float)Math.Max(
                       Math.Abs(face.FaceLandmarks.EyeLeftTop.Y - face.FaceLandmarks.EyeLeftBottom.Y),
                       Math.Abs(face.FaceLandmarks.EyeRightTop.Y - face.FaceLandmarks.EyeRightBottom.Y)
                   );
        }

        private static PointF GetCensorLeftPoint(DetectedFace face)
        {
            var rightEyeX = face.FaceLandmarks.EyeRightOuter.X;
            var leftEyeX = face.FaceLandmarks.EyeLeftOuter.X;
            var rightEyeY = face.FaceLandmarks.EyeRightOuter.Y;
            var leftEyeY = face.FaceLandmarks.EyeLeftOuter.Y;

            return new PointF(
                (float)leftEyeX - CensorWidthModifier * (float)(rightEyeX - leftEyeX),
                (float)leftEyeY - CensorWidthModifier * (rightEyeY < leftEyeY ? 1 : -1) * (float)(rightEyeY - leftEyeY));
        }

        private static PointF GetCensorRightPoint(DetectedFace face)
        {
            var rightEyeX = face.FaceLandmarks.EyeRightOuter.X;
            var leftEyeX = face.FaceLandmarks.EyeLeftOuter.X;
            var rightEyeY = face.FaceLandmarks.EyeRightOuter.Y;
            var leftEyeY = face.FaceLandmarks.EyeLeftOuter.Y;

            return new PointF(
                (float)rightEyeX + CensorWidthModifier * (float)(rightEyeX - leftEyeX),
                (float)rightEyeY + CensorWidthModifier * (leftEyeY < rightEyeY ? 1 : -1) * (float)(leftEyeY - rightEyeY));
        }

        private static void UpdateImageBlob(Blob imageBlob, Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.Metadata.DecodedImageFormat);
                var imageBytes = ms.ToArray();

                using (var ws = imageBlob.OpenWrite())
                {
                    ws.Write(imageBytes, 0, imageBytes.Length);
                }
            }
        }
    }
}