using System.Collections.Concurrent;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.WebHost.UseKestrel();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

app.UseHttpsRedirection();

var todosApi = app.MapGroup("/images");

todosApi.MapGet("/", () => GetImages());
todosApi.MapGet("/{name}", (string name) => GetImageByName(name) is Image img
        ? Results.Bytes(File.ReadAllBytes(img.path))
        : Results.NotFound());
app.Run();



ConcurrentBag<Image> GetImages()
{
    var path = Path.Combine(Environment.CurrentDirectory, "Data");
    var images = new ConcurrentBag<Image>();

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

public record Image(string path);

[JsonSerializable(typeof(ConcurrentBag<Image>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
