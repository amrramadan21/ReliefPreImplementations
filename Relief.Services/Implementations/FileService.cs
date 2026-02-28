using Microsoft.AspNetCore.Http;
using Relief.Domain.Contracts;
using Relief.Domain.Entities.Users;
using Relief.Domain.Exceptions;
using Relief.ServiceAbstraction.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Relief.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly IUnitOfWork _unitOfWork;

        // 👇 ممكن تعدلهم حسب احتياجك
        private readonly string[] _allowedExtensions =
            { ".jpg", ".jpeg", ".png", ".pdf" };

        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public FileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FileMetadata> UploadFileAsync(
            IFormFile file,
            Guid ownerId,
            string folderPath)
        {
            // =============================
            // Basic Validation
            // =============================

            if (file == null)
                throw new BadRequestException("File is required.");

            if (file.Length == 0)
                throw new BadRequestException("Uploaded file is empty.");

            if (file.Length > MaxFileSize)
                throw new BadRequestException("File size exceeds 5 MB limit.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!_allowedExtensions.Contains(extension))
                throw new BadRequestException("File type is not allowed.");

            if (string.IsNullOrWhiteSpace(folderPath))
                throw new BadRequestException("Invalid folder path.");

            // =============================
            // Create Directory Safely
            // =============================

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var storedFileName = $"{Guid.NewGuid()}{extension}";
            var storedPath = Path.Combine(folderPath, storedFileName);

            // =============================
            // Save File
            // =============================

            try
            {
                using (var stream = new FileStream(storedPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch
            {
                throw new Exception("Error while saving the file.");
            }

            // =============================
            // Save Metadata
            // =============================

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
