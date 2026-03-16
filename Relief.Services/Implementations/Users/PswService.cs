using Relief.Domain.Contracts;
using Relief.Domain.Entities;
using Relief.Domain.Entities.Users;
using Relief.Domain.Enums;
using Relief.Domain.Exceptions;
using Relief.ServiceAbstraction.Interfaces.Files;
using Relief.ServiceAbstraction.Interfaces.Users;
using Shared.IdentityDTOs;
using System.Transactions;

namespace Relief.Services.Implementations.Users
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
            if (dto == null)
                throw new BadRequestException("Profile data is required.");

            var pswRepo = _unitOfWork.GetRepository<PswUser, Guid>();
            var psw = await pswRepo.GetByIdAsync(userId);

            if (psw == null)
                throw new NotFoundException("PSW not found.");

            // Allow re-upload if verification was rejected
            if (psw.VerificationStatus != VerificationStatus.Rejected)
                throw new ConflictException("Profile is already completed.");

            if (string.IsNullOrWhiteSpace(dto.ProofIdentityType))
                throw new BadRequestException("Proof identity type is required.");

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // ===============================
                // Upload Required Files
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

                psw.ProofIdentityFileId = proofFile.Id;
                psw.PswCertificateFileId = certFile.Id;
                psw.CVFileId = cvFile.Id;
                psw.ImmunizationRecordFileId = immFile.Id;
                psw.CriminalRecordFileId = criminalFile.Id;
                psw.FirstAidOrCPRFileId = cprFile?.Id;

                
                psw.VerificationStatus = VerificationStatus.Pending;
                psw.VerificationRejectionReason = null;

                pswRepo.Update(psw);

                await _unitOfWork.SaveChangesAsync();
                transaction.Complete();
            }
        }
    }
}