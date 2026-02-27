using Relief.Domain.Contracts;
using Relief.Domain.Entities;
using Relief.Domain.Entities.Users;
using Relief.ServiceAbstraction.Interfaces;
using Shared.IdentityDTOs;

namespace Relief.Services.Implementations
{
    public class PswService : IPswService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public PswService(
            IUnitOfWork unitOfWork,
            IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task CompleteProfileAsync(
            Guid userId,
            CompletePswProfileDto dto)
        {
            var pswRepo = _unitOfWork.GetRepository<PswUser, Guid>();

            var psw = await pswRepo.GetByIdAsync(userId);

            if (psw == null)
                throw new Exception("PSW not found");

            // ===============================
            // Upload Files
            // ===============================

            var proofFile = await _fileService.UploadFileAsync(
                dto.ProofIdentityFile,
                userId,
                "psw/proof");

            var certFile = await _fileService.UploadFileAsync(
                dto.PswCertificateFile,
                userId,
                "psw/certificate");

            var cvFile = await _fileService.UploadFileAsync(
                dto.CVFile,
                userId,
                "psw/cv");

            var immFile = await _fileService.UploadFileAsync(
                dto.ImmunizationRecordFile,
                userId,
                "psw/immunization");

            var criminalFile = await _fileService.UploadFileAsync(
                dto.CriminalRecordFile,
                userId,
                "psw/criminal");

            FileMetadata? cprFile = null;

            if (dto.FirstAidOrCPRFile != null)
            {
                cprFile = await _fileService.UploadFileAsync(
                    dto.FirstAidOrCPRFile,
                    userId,
                    "psw/cpr");
            }

            // ===============================
            // Update PSW
            // ===============================

            psw.ProofIdentityType = dto.ProofIdentityType;
            psw.WorkStatus = dto.WorkStatus;

            psw.ProofIdentityFileId = proofFile.Id;
            psw.PswCertificateFileId = certFile.Id;
            psw.CVFileId = cvFile.Id;
            psw.ImmunizationRecordFileId = immFile.Id;
            psw.CriminalRecordFileId = criminalFile.Id;
            psw.FirstAidOrCPRFileId = cprFile?.Id;

            // Auto verification
            psw.IsProfileCompleted = true;
            psw.IsVerified = true;

            pswRepo.Update(psw);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}