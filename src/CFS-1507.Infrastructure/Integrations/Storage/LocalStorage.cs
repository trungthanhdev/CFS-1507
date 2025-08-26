using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CFS_1507.Infrastructure.Integrations
{
    public class LocalStorage : ILocalStorage
    {
        //1: path docker volumn
        private readonly string _basePath = "/data/Uploads";
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
        public async Task<string> SaveImageAsync(IFormFile imageFile, string folder)
        {
            //2: validate valid imageFile
            if (imageFile == null || imageFile.Length <= 0)
            {
                throw new BadHttpRequestException("Image not found or invalid!");
            }

            string extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException($"Image format invalid. Just upload: {string.Join(", ", AllowedExtensions)}!");
            }
            //3: validate folder in docker 
            string targetFolder = string.IsNullOrWhiteSpace(folder) ? _basePath : Path.Combine(_basePath, folder);
            // if not existed => create folder
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }
            // if existed => save imageFile
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            string fullPath = Path.Combine(targetFolder, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            //4: return link
            string relativePath = string.IsNullOrEmpty(folder) ? $"images/{fileName}" : $"images/{folder}/{fileName}";

            return relativePath.Replace("\\", "/");
        }
    }
}