using System.Text.RegularExpressions;

namespace CodeHelper.Core
{
    public class ImageManager
    {
        private string _domen;
        private string _serverImagesPath;

        public ImageManager(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Console.WriteLine("Init ImageManager");
            _domen = configuration["DomenImages"];
            _serverImagesPath = GetServerImagesPath(environment.WebRootPath);
            Console.WriteLine("End init ImageManager");
        }

        private string GetServerImagesPath(string path)
        {
            var parent = Directory.GetParent(path)?.FullName;

            if (string.IsNullOrWhiteSpace(parent))
                return string.Empty;

            if (Directory.GetParent(parent)?.Name == "CodeHelper")
            {
                var p = Directory.GetParent(parent);

                return Path.Combine(p.FullName, "Images", "Data");
            }

            GetServerImagesPath(parent);

            return string.Empty;
        }

        public string SaveImage(IFormFile image)
        {
            string filePath;
            var imageId = Guid.NewGuid().ToString().Replace("-", "_");
            var serverImagesPath = Path.Combine(_serverImagesPath, imageId + image.FileName);

            using (var s = new FileStream(serverImagesPath, FileMode.Create))
            {
                image.CopyTo(s);
            }

            filePath = Path.Combine(_domen, "Images/", imageId + image.FileName);
            return filePath;
        }

        public void RemoveImages(string oldContent, string newContent)
        {
            var regex = new Regex($"<img(.*?) src=\"{_domen + "Images/"}(.*?)\"");

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
