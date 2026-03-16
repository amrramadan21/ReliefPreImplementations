using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relief.Domain.Contracts;
using Relief.Domain.Entities.Users;
using Relief.ServiceAbstraction.Interfaces.Admin;
using Relief.ServiceAbstraction.Interfaces.Files;
using Relief.ServiceAbstraction.Interfaces.Profiles;
using Shared.AdminDTOs;
using Shared.QueryDTOs.Admin;

namespace Relief.Presentation.Controllers
{
    [Authorize(Roles = "PSW,Admin")]
    [ApiController]
    [Route("api/files")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IUnitOfWork _unitOfWork;

        public FileController(IFileService fileService, IUnitOfWork unitOfWork)
        {
            _fileService = fileService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{fileId:guid}/download-url")]
        public async Task<IActionResult> GetDownloadUrl(Guid fileId)
        {
            var fileRepo = _unitOfWork.GetRepository<FileMetadata, Guid>();
            var file = await fileRepo.GetByIdAsync(fileId);

            if (file == null)
                return NotFound("File not found.");

            var url = _fileService.GetPresignedUrl(file.StoredPath, expiryMinutes: 15);

            return Ok(new
            {
                downloadUrl = url,
                fileName = file.OriginalFileName,
                expiresInMinutes = 15
            });
        }
    }
}
