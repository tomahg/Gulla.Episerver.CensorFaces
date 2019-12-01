using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace Gulla.EpiserverCensorFaces.Core
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ImageEventInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var events = context.Locate.ContentEvents();
            events.CreatingContent += CreatingContent;
        }

        private static void CreatingContent(object sender, ContentEventArgs e)
        {
            if (e.Content is ImageData imageData)
            {
                CensorFaceHelper.CensorBinaryData(imageData.BinaryData);
            }
        }

        public void Uninitialize(InitializationEngine context)
        {

        }
    }
}