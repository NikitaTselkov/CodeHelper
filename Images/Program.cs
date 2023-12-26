using System.Collections.Concurrent;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.WebHost.UseKestrel();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

var todosApi = app.MapGroup("/i");

todosApi.MapGet("/All", GetImages);
todosApi.MapPost("/Upload", PostUpload);
todosApi.MapPost("/Delete", PostDelete);
todosApi.MapGet("/{name}", (string name) => GetImageByName(name) is Image img
        ? Results.Bytes(File.ReadAllBytes(img.path))
        : Results.NotFound());
app.Run();



ConcurrentBag<Image> GetImages()
{
    var path = Path.Combine(Environment.CurrentDirectory, "Data");
    var images = new ConcurrentBag<Image>();

    if (!Directory.Exists(path))
        Directory.CreateDirectory(path);

    var result = Parallel.ForEach(Directory.GetFiles(path), (image) =>
    {
        var img = new Image(image);
        images.Add(img);
    });

    return images;
}

Image GetImageByName(string name)
{
    var path = Path.Combine(Environment.CurrentDirectory, "Data");
    var results = new ConcurrentBag<Image>();

    if (!Directory.Exists(path))
        Directory.CreateDirectory(path);

    var result = Parallel.ForEach(Directory.GetFiles(path), (image, ParallelLoopState) =>
    {
        var img = new Image(image);

        if (Path.GetFileName(image) == name)
        {
            results.Add(img);
            ParallelLoopState.Break();
        }
    });

    return results.FirstOrDefault();
}

async Task PostUpload(HttpContext context)
{
    Console.WriteLine("--PostUpload--");

    var form = context.Request.Form;

    string? imagePath = form["imagePath"];
    Console.WriteLine("imagePath: " + imagePath);
    IFormFileCollection files = form.Files;

    var path = Path.Combine(Environment.CurrentDirectory, "Data");

    if (!Directory.Exists(path))
        Directory.CreateDirectory(path);

    foreach (var file in files)
    {
        string fullPath = $"{path}/{imagePath}";

        Console.WriteLine("fullPath: " + fullPath);

        using (var fileStream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
    }
}

async Task PostDelete(HttpContext context)
{
    Console.WriteLine("--PostDelete--");

    var form = context.Request.Form;
    var path = Path.Combine(Environment.CurrentDirectory, "Data");

    foreach (var item in form)
    {
        for (int i = 0; i < item.Value.Count; i++)
        {
            var serverImagesPath = Path.Combine(path, item.Value[i]);

            Console.WriteLine(serverImagesPath);
            File.Delete(serverImagesPath);
        }
    }
}

public record Image(string path);

[JsonSerializable(typeof(ConcurrentBag<Image>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
