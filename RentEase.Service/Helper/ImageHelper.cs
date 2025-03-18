using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace RentEase.Service.Helper
{

    public interface IImageHelper
    {
        void ConvertToJpg(string inputPath, string outputPath);
    }
    public class ImageHelper : IImageHelper
    {

        public ImageHelper()
        {
        }

        public void ConvertToJpg(string inputPath, string outputPath)
        {
            using (var image = Image.Load(inputPath))
            {
                image.Mutate(x => x.AutoOrient());
                image.Save(outputPath, new JpegEncoder { Quality = 90 });
            }
        }

    }
}
