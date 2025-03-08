using EPiServer;
using EPiServer.Core;
using Microsoft.AspNetCore.Builder;

namespace Gulla.Episerver.CensorFaces
{
    public static class ServiceCollectionExtensions
    {
        public static IApplicationBuilder UseCensorFaces(this IApplicationBuilder builder, IContentEvents contentEvents)
        {
            contentEvents.CreatingContent += CreatingContent;
            return builder;
        }

        private static void CreatingContent(object sender, ContentEventArgs e)
        {
            if (e.Content is ImageData imageData)
            {
                CensorFaceHelper.CensorBinaryData(imageData.BinaryData);
            }
        }
    }
}
