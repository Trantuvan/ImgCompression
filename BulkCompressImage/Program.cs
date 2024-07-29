using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

var currentDir = new DirectoryInfo(AppContext.BaseDirectory);
var imgFolder = currentDir?.Parent?.Parent?.Parent;

if (imgFolder is null)
{
  // Replace with current username
  imgFolder = new DirectoryInfo("C:\\Users\\test\\source\\repos\\BulkCompressImage\\BulkCompressImage");
}

var inputDir = Directory.CreateDirectory(Path.Combine(imgFolder.FullName, "OriginalImg"));
var outputDir = Directory.CreateDirectory(Path.Combine(imgFolder.FullName, "CompressImg"));
int compressionQuality = 70;

if (Directory.GetFiles(inputDir.FullName, "*.*", SearchOption.AllDirectories).Length == 0)
{
  Console.WriteLine("Please Input Image to OriginalImg folder");
  return;
}

foreach (var existFile in outputDir.EnumerateFiles())
{
  existFile.Delete();
  Console.WriteLine($"Deleted {existFile}");
}

Console.WriteLine($"Deleted complete");

foreach (var file in Directory.GetFiles(inputDir.FullName, "*.*", SearchOption.AllDirectories))
{
  if (IsImageFile(file))
  {
    var outputFile = Path.Combine(outputDir.FullName, Path.GetFileName(file));
    await CompressImageAsync(file, outputFile, compressionQuality);
    Console.WriteLine($"Compressed {file} to {outputFile}");
  }
}

Console.WriteLine("Compression complete");

static bool IsImageFile(string filePath)
{
  string extension = Path.GetExtension(filePath).ToLower();
  return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif";
}

static async Task CompressImageAsync(string inputFile, string outputFile, int quality)
{
  using var stream = File.OpenRead(inputFile);
  IImageFormat originalFormat = await Image.DetectFormatAsync(stream);
  IImageEncoder encoder = CreateImageEncoder(originalFormat.Name.ToLower(), quality);
  using var image = await Image.LoadAsync(stream);
  image.Mutate(x => x.Resize(1597, 2129));
  image.Save(outputFile, encoder);
}

static IImageEncoder CreateImageEncoder(string imgFormat, int quality) =>
imgFormat switch
{
  "jpeg" => new JpegEncoder { Quality = quality },
  "jpg" => new JpegEncoder { Quality = quality },
  "png" => new PngEncoder { CompressionLevel = PngCompressionLevel.Level7 },
  "gif" => new GifEncoder { ColorTableMode = GifColorTableMode.Global, },
  _ => throw new ArgumentException("Invalid string value for command", nameof(imgFormat)),
};