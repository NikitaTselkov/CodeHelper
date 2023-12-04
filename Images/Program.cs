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
todosApi.MapGet("/{name}", (string name) => GetImages().FirstOrDefault(a => Path.GetFileName(a.path) == name) is { } img
        ? Results.File(File.ReadAllBytes(img.path))
        : Results.NotFound());
app.Run();

List<Image> GetImages()
{
    var images = new List<Image>();
    var path = Path.Combine(Environment.CurrentDirectory, "Data");

    foreach (var image in Directory.GetFiles(path))
    {
        images.Add(new(image));
    }

    return images;
}

public record Image(string path);

[JsonSerializable(typeof(List<Image>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
