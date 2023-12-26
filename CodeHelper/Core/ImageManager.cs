using System.Net.Http.Headers;
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
            _domen = configuration["Domen"];
            _serverImagesPath = Path.Combine(configuration["DomenImages"], "i/");
            Console.WriteLine(_serverImagesPath);
            Console.WriteLine("End init ImageManager");
        }

        public string SaveImage(IFormFile image)
        {
            string filePath;
            var imageId = Guid.NewGuid().ToString().Replace("-", "_");
            var imagePath = imageId + image.FileName;

            using (var client = new HttpClient())
            {
                using var multipartFormDataContent = new MultipartFormDataContent();

                var fileStreamContent = new StreamContent(image.OpenReadStream());
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);

                multipartFormDataContent.Add(fileStreamContent, name: "image", fileName: image.FileName);
                multipartFormDataContent.Add(new StringContent(imagePath), "imagePath");

                var response = client.PostAsync(Path.Combine(_serverImagesPath, "Upload"), multipartFormDataContent).Result;

                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.RequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("---> Image uploaded");
                }
                else
                {
                    Console.WriteLine("---> Error");
                }
            }

            filePath = Path.Combine(_domen, "i/", imageId + image.FileName);
            return filePath;
        }

        public void RemoveImages(string oldContent, string newContent)
        {
            var regex = new Regex($"<img(.*?) src=\"{_domen + "i/"}(.*?)\"");

            var images = regex.Matches(oldContent);
            var newImages = regex.Matches(newContent);

            using var multipartFormDataContent = new MultipartFormDataContent();

            foreach (Match image in images)
            {
                var imageName = image.Groups[2].Value;

                if (!newImages.Any(a => a.Groups[2].Value == imageName))
                {
                    Console.WriteLine("ImageName: " + imageName);
                    multipartFormDataContent.Add(new StringContent(imageName));
                }
            }

            if (multipartFormDataContent.Count() > 0)
            {
                using (var client = new HttpClient())
                {
                    var response = client.PostAsync(Path.Combine(_serverImagesPath, "Delete"), multipartFormDataContent).Result;

                    Console.WriteLine(response.StatusCode);
                    Console.WriteLine(response.RequestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("---> Image deleted");
                    }
                    else
                    {
                        Console.WriteLine("---> Error");
                    }
                }
            }
        }
    }
}
