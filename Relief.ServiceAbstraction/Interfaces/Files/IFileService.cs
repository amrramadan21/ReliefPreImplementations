using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Relief.Domain.Entities.Users;

namespace Relief.ServiceAbstraction.Interfaces.Files
{
    public interface IFileService
    {
        Task<FileMetadata> UploadFileAsync(
            IFormFile file,
            Guid ownerId,
            string folderPath);

        string GetPresignedUrl(string s3Key, int expiryMinutes = 30);

        Task DeleteFileAsync(string s3Key);
    }
}
