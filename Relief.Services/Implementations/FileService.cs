using Microsoft.AspNetCore.Http;
using Relief.Domain.Contracts;
using Relief.Domain.Entities.Users;
using Relief.ServiceAbstraction.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Relief.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FileMetadata> UploadFileAsync(
            IFormFile file,
            Guid ownerId,
            string folderPath)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            Directory.CreateDirectory(folderPath);

            var extension = Path.GetExtension(file.FileName);
            var storedFileName = $"{Guid.NewGuid()}{extension}";
            var storedPath = Path.Combine(folderPath, storedFileName);

            using (var stream = new FileStream(storedPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var metadata = new FileMetadata
            {
                Id = Guid.NewGuid(),
                OriginalFileName = file.FileName,
                StoredFileName = storedFileName,
                StoredPath = storedPath,
                ContentType = file.ContentType ?? "application/octet-stream",
                FileSize = file.Length,
                UploadedAt = DateTime.UtcNow
            };

            var repo = _unitOfWork.GetRepository<FileMetadata, Guid>();

            await repo.AddAsync(metadata);
            await _unitOfWork.SaveChangesAsync();

            return metadata;
        }
    }
}
