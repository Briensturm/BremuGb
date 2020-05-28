using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BremuGb.IntegrationTests
{
    static internal class TestHelper
    {
        static void WriteScreenToImageFile(byte[] data, string path)
        {
            var image = Image.LoadPixelData<Rgb24>(data, 160, 144);
            image.Save(path);
        }
    }
}
