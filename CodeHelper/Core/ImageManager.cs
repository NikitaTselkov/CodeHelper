using System.Text.RegularExpressions;

namespace CodeHelper.Core
{
    public class ImageManager
    {
        private string _domen;
        private string _serverImagesPath;

        public ImageManager(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _domen = configuration["domen"];
            _serverImagesPath = environment.WebRootPath;
        }

        public string SaveImage(IFormFile image)
        {
            string filePath;
            var imageId = Guid.NewGuid().ToString().Replace("-", "_");
            var serverImagesPath = Path.Combine(_serverImagesPath, "Images", imageId + image.FileName);

            using (var s = new FileStream(serverImagesPath, FileMode.Create))
            {
                image.CopyTo(s);
            }

            filePath = Path.Combine(_domen, "Images", imageId + image.FileName);
            return filePath;
        }

        public void RemoveImages(string oldContent, string newContent)
        {
            var regex = new Regex($"<img(.*?) src=\"{_domen}(.*?)\"");

            var images = regex.Matches(oldContent);
            var newImages = regex.Matches(newContent);

            foreach (Match image in images)
            {
                var imageName = image.Groups[2].Value;

                if (!newImages.Any(a => a.Groups[2].Value == imageName))
                {
                    var serverImagesPath = _serverImagesPath + imageName;
                    File.Delete(serverImagesPath);
                }
            }
        }
    }
}
