using app.Server.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace app.Server.Service
{
    public class ImageService(IConfiguration configuration) : IImageService
    {
        public async Task<string> SaveAsync(IFormFile file)
        {
            // webp - максимлано стискає
            using MemoryStream ms = new();
            await file.CopyToAsync(ms);
            var bytes = ms.ToArray();
            var imageName = await SaveBytesAsync(bytes);
            return imageName;
        }

        private async Task<string> SaveBytesAsync(byte[] bytes)
        {
            string imageName = $"{Guid.NewGuid()}.webp";
            //var imagesDir = configuration["ImagesDir"] ?? "images";
            var sizes = configuration.GetSection("ImageSizes").Get<int[]>()
                ?? new[] { 50, 100, 200, 400, 800, 1200 };

            Task[] tasks = sizes
                .AsParallel()
                .Select(s => SaveByteNameAsync(bytes, imageName, s))
                .ToArray();

            await Task.WhenAll(tasks);
            return imageName;
        }

        private async Task SaveByteNameAsync(byte[] bytes, string name, int size)
        {
            var path = Path.Combine(configuration["ImagesDir"] ?? "images", $"{size}_{name}");
            using var image = Image.Load(bytes);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(size, size),
                Mode = ResizeMode.Max
            }));
            await image.SaveAsWebpAsync(path);
        }

        public async Task<string> SaveImageFromUrlAsync(string imageUrl)
        {
            using var httpClient = new HttpClient();
            var bytes = await httpClient.GetByteArrayAsync(imageUrl);
            var imageName = await SaveBytesAsync(bytes);
            return imageName;
        }
    }
 }
