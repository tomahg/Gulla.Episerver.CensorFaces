using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EPiServer.Framework.Blobs;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Image = System.Drawing.Image;

namespace EpiserverSite1.Business
{
    public static class CensorFaceHelper
    {
        private static IFaceClient _client;

        private const string SubscriptionKey = "{{xxx}}";
        private const string Endpoint = "https://{{xxx}}.cognitiveservices.azure.com/";
        private const string RecognitionModel = "recognition_02";

        private const float CensorWidthModifier = 0.25f;
        private const float CensorHeightModifier = 4;

        private static IFaceClient Client =>
            _client ?? (_client = new FaceClient(new ApiKeyServiceClientCredentials(SubscriptionKey))
            {
                Endpoint = Endpoint
            });

        public static void CensorBinaryData(Blob imageBlob)
        {
            var detectedFaces = GetFaces(imageBlob.OpenRead());
            if (!detectedFaces.Any()) return;

            var image = Image.FromStream(new MemoryStream(imageBlob.ReadAllBytes()));
            var graphics = Graphics.FromImage(image);
            foreach (var face in detectedFaces)
            {
                CensorFace(face, graphics);
            }

            UpdateImageBlob(imageBlob, image);
        }
        private static IList<DetectedFace> GetFaces(Stream image)
        {
            var task = Task.Run(async () => await CensorFaceHelper.DetectFaces(image));
            return task.Result;
        }

        private static void CensorFace (DetectedFace face, Graphics graphics)
        {
            graphics.DrawLine(GetCensorPen(face), GetCensorLeftPoint(face), GetCensorRightPoint(face));
        }

        private static Pen GetCensorPen(DetectedFace face)
        {
            return new Pen(new SolidBrush(Color.Black), GetCensorHeight(face));
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
            var leftEyeY = face.FaceLandmarks.EyeLeftOuter.Y;
            var rightEyeY = face.FaceLandmarks.EyeRightOuter.Y;

            return new PointF(
                (float)leftEyeX - CensorWidthModifier * (float)(rightEyeX - leftEyeX),
                (float)leftEyeY - CensorWidthModifier * (rightEyeY < leftEyeY ? 1 : -1) * (float)(rightEyeY - leftEyeY));
        }

        private static PointF GetCensorRightPoint(DetectedFace face)
        {
            var rightEyeX = face.FaceLandmarks.EyeRightOuter.X;
            var leftEyeX = face.FaceLandmarks.EyeLeftOuter.X;
            var leftEyeY = face.FaceLandmarks.EyeLeftOuter.Y;
            var rightEyeY = face.FaceLandmarks.EyeRightOuter.Y;

            return new PointF(
                (float)rightEyeX + CensorWidthModifier * (float)(rightEyeX - leftEyeX),
                (float)rightEyeY + CensorWidthModifier * (leftEyeY < rightEyeY ? 1 : -1) * (float)(leftEyeY - rightEyeY));
        }

        private static async Task<IList<DetectedFace>> DetectFaces(Stream image)
        {
            return await Client.Face.DetectWithStreamAsync(image,
                recognitionModel: RecognitionModel,
                returnFaceLandmarks: true);
        }

        private static void UpdateImageBlob(Blob imageBlob, Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                var imageBytes = ms.ToArray();

                using (var ws = imageBlob.OpenWrite())
                {
                    ws.Write(imageBytes, 0, imageBytes.Length);
                }
            }
        }
    }
}