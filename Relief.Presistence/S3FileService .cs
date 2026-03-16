using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Relief.Domain.Contracts;
using Relief.Domain.Entities.Users;
using Relief.ServiceAbstraction.Interfaces.Files;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Relief.Domain.Exceptions;

namespace Relief.Presistence
{
    public class S3FileService : IFileService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3Settings _s3Settings;
        private readonly IUnitOfWork _unitOfWork;

        public S3FileService(
            IAmazonS3 s3Client,
            IOptions<S3Settings> s3Settings,
            IUnitOfWork unitOfWork)
        {
            _s3Client = s3Client;
            _s3Settings = s3Settings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<FileMetadata> UploadFileAsync(
            IFormFile file,
            Guid ownerId,
            string folderPath)
        {
            // ─── Validation ───────────────────────────────────
            if (file == null || file.Length == 0)
                throw new BadRequestException("File is required.");

            // ─── Generate unique file name ────────────────────
            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);
            var storedFileName = $"{fileId}{extension}";

            //  Result: "psw/proof/{ownerId}/{guid}.pdf"
            var s3Key = $"{folderPath}/{ownerId}/{storedFileName}";

            // ─── Upload to S3 ────────────────────────────────
            using var stream = file.OpenReadStream();

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = s3Key,
                BucketName = _s3Settings.BucketName,
                ContentType = file.ContentType,

                // Private by default — access via presigned URLs only
                CannedACL = S3CannedACL.Private

            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

            // ─── Save metadata to DB ─────────────────────────
            var metadata = new FileMetadata
            {
                Id = fileId,
                OriginalFileName = file.FileName,
                StoredFileName = storedFileName,
                StoredPath = s3Key,             // S3 key, NOT local path
                ContentType = file.ContentType,
                FileSize = file.Length,
                UploadedAt = DateTime.UtcNow
            };

            var fileRepo = _unitOfWork.GetRepository<FileMetadata, Guid>();
            await fileRepo.AddAsync(metadata);

            return metadata;
        }

        // ─── Generate a temporary download link ──────────────
        public string GetPresignedUrl(string s3Key, int expiryMinutes = 30)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = s3Key,
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Verb = HttpVerb.GET
            };

            return _s3Client.GetPreSignedURL(request);
        }

        

        // ─── Delete file from S3 ─────────────────────────────
        public async Task DeleteFileAsync(string s3Key)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = s3Key
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
        }
    }
}
