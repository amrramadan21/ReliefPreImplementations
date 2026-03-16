using Amazon.SimpleEmailV2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Relief.Domain.Contracts;
using Relief.Domain.Entities.Users;
using Relief.Domain.Exceptions;
using Relief.ServiceAbstraction.Interfaces.Files;
using Relief.ServiceAbstraction.Interfaces.Profiles;
using Relief.Services.Implementations.Profiles.Specifications;
using Shared.IdentityDTOs;
using Shared.ProfileDTOs;
using System.Security.Claims;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IFileService _fileService;
    private readonly IUnitOfWork _unitOfWork;


    public ProfileService(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IFileService fileService,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _fileService = fileService;
        _unitOfWork = unitOfWork;
    }

    // =========================
    // Get My Profile
    // =========================
    public async Task<object> GetMyProfileAsync()
    {
        var userId = GetCurrentUserId();
        return await GetUserProfileAsync(userId);
    }

    // =========================
    // Get User Profile by Id
    // =========================
    public async Task<object> GetUserProfileAsync(Guid userId)
    {
        var user = await _userManager.Users
            .Include(x => x.ProfilePhoto) //Uploaded profile photo
            .Include(x => x.Address)
            .Include(x => x.CareHomeUser)
            .Include(x => x.IndividualCareHomeUser)
            .Include(x => x.PswUser)
                .ThenInclude(x => x.ProofIdentityFile)
            .Include(x => x.PswUser)
                .ThenInclude(x => x.InsuranceFile)
            .Include(x => x.PswUser)
                .ThenInclude(x => x.PswCertificateFile)
            .Include(x => x.PswUser)
                .ThenInclude(x => x.CVFile)
            .Include(x => x.PswUser)
                .ThenInclude(x => x.ImmunizationRecordFile)
            .Include(x => x.PswUser)
                .ThenInclude(x => x.CriminalRecordFile)
            .Include(x => x.PswUser)
                .ThenInclude(x => x.FirstAidOrCPRFile)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            throw new NotFoundException("User not found");

        var findRole = await _userManager.GetRolesAsync(user);
        var role = findRole.FirstOrDefault() ?? "Unknown";
        // =========================
        // PSW Profile
        // =========================
        if (user.PswUser != null)
        {
            return new PswProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName!,
                LastName = user.LastName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                DateOfBirth = user.BirthOfDate,
                Gender = user.Gender.ToString(),
                Role = role,
                Address = MapAddress(user),

                ProfilePhoto = MapFile(user.ProfilePhoto), //Upload


                ProofIdentityType = user.PswUser.ProofIdentityType,
                VerificationStatus = user.PswUser.VerificationStatus.ToString(),
                RejectionReason = user.PswUser.VerificationRejectionReason,

                ProofIdentityFile = MapFile(user.PswUser.ProofIdentityFile),
                InsuranceFile = MapFile(user.PswUser.InsuranceFile),
                PswCertificateFile = MapFile(user.PswUser.PswCertificateFile),
                CVFile = MapFile(user.PswUser.CVFile),
                ImmunizationRecordFile = MapFile(user.PswUser.ImmunizationRecordFile),
                CriminalRecordFile = MapFile(user.PswUser.CriminalRecordFile),
                FirstAidOrCPRFile = MapFile(user.PswUser.FirstAidOrCPRFile)
            };
        }

        // =========================
        // CareHome Profile
        // =========================
        if (user.CareHomeUser != null)
        {
            return new CareHomeProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName!,
                LastName = user.LastName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                DateOfBirth = user.BirthOfDate,
                Gender = user.Gender.ToString(),
                Role = role,
                Address = MapAddress(user),

                ProfilePhoto = MapFile(user.ProfilePhoto), //upload

                BusinessLicense = user.CareHomeUser.BusinessLicense,
                LegalName = user.CareHomeUser.LegalName,
                VaccinationPolicy = user.CareHomeUser.VaccinationPolicy
            };
        }

        // =========================
        // Individual Profile
        // =========================
        if (user.IndividualCareHomeUser != null)
        {
            return new CareHomeProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName!,
                LastName = user.LastName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                DateOfBirth = user.BirthOfDate,
                Gender = user.Gender.ToString(),
                Role = role,
                Address = MapAddress(user),
                ProfilePhoto = MapFile(user.ProfilePhoto), //upload

                // Individual ماعندوش البيانات دي
                BusinessLicense = "Individual",
                LegalName = $"{user.FirstName} {user.LastName}",
                VaccinationPolicy = "N/A"
            };
        }
        // return admin profile
        return new CareHomeProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName!,
            LastName = user.LastName!,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            DateOfBirth = user.BirthOfDate,
            Gender = user.Gender.ToString(),
            Role = role,
            Address = MapAddress(user)
        };
        
        //throw new NotFoundException("Profile type not found");
    }

    // =========================
    // Update Profile
    // =========================
    public async Task UpdateProfileAsync(UpdateProfileDto dto)
    {
        var userId = GetCurrentUserId();

        var spec = new UserWithFullProfileSpec(userId);
        var userRepo = _unitOfWork.GetRepository<ApplicationUser, Guid>();
        var user = await userRepo.GetByIdAsync(spec);
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Unknown";

        bool reVerificationNeeded = false;


        if (user is null)
            throw new NotFoundException("User not found.");

        // ── Collect old files to delete AFTER the FK update ──
        var oldFilesToCleanUp = new List<FileMetadata>();

        // ══════════════════════════════════════════════════════
        //  COMMON PROPERTIES
        // ══════════════════════════════════════════════════════

        if (dto.FirstName is not null)
            user.FirstName = dto.FirstName;

        if (dto.LastName is not null)
            user.LastName = dto.LastName;

        if (dto.PhoneNumber is not null)
            user.PhoneNumber = dto.PhoneNumber;

        if (dto.Address is not null)
        {
            if (user.Address is null)
            {
                user.Address = new Address
                {
                    ApartmentNumber = dto.Address.ApartmentNumber ?? -1,
                    Street = dto.Address.Street ?? string.Empty,
                    City = dto.Address.City ?? string.Empty,
                    State = dto.Address.State ?? string.Empty,
                    Country = dto.Address.Country ?? string.Empty,
                    PostalCode = dto.Address.PostalCode ?? string.Empty
                };
            }
            else
            {
                if (dto.Address.ApartmentNumber is not null)
                    user.Address.ApartmentNumber = dto.Address.ApartmentNumber.Value;
                if (dto.Address.Street is not null)
                    user.Address.Street = dto.Address.Street;
                if (dto.Address.City is not null)
                    user.Address.City = dto.Address.City;
                if (dto.Address.State is not null)
                    user.Address.State = dto.Address.State;
                if (dto.Address.Country is not null)
                    user.Address.Country = dto.Address.Country;
                if (dto.Address.PostalCode is not null)
                    user.Address.PostalCode = dto.Address.PostalCode;
            }
        }

        // ── Profile Photo ────────────────────────────────────
        if (dto.ProfilePhoto is not null)
        {
            if (user.ProfilePhoto is not null)
                oldFilesToCleanUp.Add(user.ProfilePhoto);

            var newPhoto = await _fileService.UploadFileAsync(
                dto.ProfilePhoto, userId, "profiles/photos");

            user.ProfilePhotoId = newPhoto.Id;
            if(role == "PSW")
                reVerificationNeeded = true;
        }

        // ══════════════════════════════════════════════════════
        //  PSW-ONLY PROPERTIES
        // ══════════════════════════════════════════════════════

        bool hasPswFields =
            dto.ProofIdentityType is not null ||
            dto.ProofIdentityFile is not null ||
            dto.InsuranceFile is not null ||
            dto.PswCertificateFile is not null ||
            dto.CVFile is not null ||
            dto.ImmunizationRecordFile is not null ||
            dto.CriminalRecordFile is not null ||
            dto.FirstAidOrCPRFile is not null;

        if (hasPswFields)
        {

            if (!roles.Contains("PSW"))
                throw new BadRequestException(
                    "Only users with the PSW role can update PSW-specific properties.");

            var psw = user.PswUser
                ?? throw new NotFoundException("PSW profile not found for this user.");

            if (dto.ProofIdentityType is not null)
            {
                psw.ProofIdentityType = dto.ProofIdentityType;
                reVerificationNeeded = true;
            }

            if (dto.ProofIdentityFile is not null)
            {
                if (psw.ProofIdentityFile is not null)
                    oldFilesToCleanUp.Add(psw.ProofIdentityFile);

                var f = await _fileService.UploadFileAsync(
                    dto.ProofIdentityFile, userId, "psw/proof-identity");
                psw.ProofIdentityFileId = f.Id;
                reVerificationNeeded = true;
            }

            if (dto.InsuranceFile is not null)
            {
                if (psw.InsuranceFile is not null)
                    oldFilesToCleanUp.Add(psw.InsuranceFile);

                var f = await _fileService.UploadFileAsync(
                    dto.InsuranceFile, userId, "psw/insurance");
                psw.InsuranceFileId = f.Id;

            }

            if (dto.PswCertificateFile is not null)
            {
                if (psw.PswCertificateFile is not null)
                    oldFilesToCleanUp.Add(psw.PswCertificateFile);

                var f = await _fileService.UploadFileAsync(
                    dto.PswCertificateFile, userId, "psw/certificate");
                psw.PswCertificateFileId = f.Id;
                reVerificationNeeded = true;
            }

            if (dto.CVFile is not null)
            {
                if (psw.CVFile is not null)
                    oldFilesToCleanUp.Add(psw.CVFile);

                var f = await _fileService.UploadFileAsync(
                    dto.CVFile, userId, "psw/cv");
                psw.CVFileId = f.Id;
            }

            if (dto.ImmunizationRecordFile is not null)
            {
                if (psw.ImmunizationRecordFile is not null)
                    oldFilesToCleanUp.Add(psw.ImmunizationRecordFile);

                var f = await _fileService.UploadFileAsync(
                    dto.ImmunizationRecordFile, userId, "psw/immunization");
                psw.ImmunizationRecordFileId = f.Id;
                reVerificationNeeded = true;
            }

            if (dto.CriminalRecordFile is not null)
            {
                if (psw.CriminalRecordFile is not null)
                    oldFilesToCleanUp.Add(psw.CriminalRecordFile);

                var f = await _fileService.UploadFileAsync(
                    dto.CriminalRecordFile, userId, "psw/criminal-record");
                psw.CriminalRecordFileId = f.Id;
                reVerificationNeeded = true;
            }

            if (dto.FirstAidOrCPRFile is not null)
            {
                if (psw.FirstAidOrCPRFile is not null)
                    oldFilesToCleanUp.Add(psw.FirstAidOrCPRFile);

                var f = await _fileService.UploadFileAsync(
                    dto.FirstAidOrCPRFile, userId, "psw/first-aid-cpr");
                psw.FirstAidOrCPRFileId = f.Id;
            }
            if(reVerificationNeeded)
            {
                psw.VerificationStatus = Relief.Domain.Enums.VerificationStatus.Pending;
                psw.VerificationRejectionReason = null;
            }
        }

        
        await _unitOfWork.SaveChangesAsync();


        if (oldFilesToCleanUp.Count > 0)
        {
            var fileRepo = _unitOfWork.GetRepository<FileMetadata, Guid>();

            foreach (var oldFile in oldFilesToCleanUp)
            {
                // Delete from S3
                await _fileService.DeleteFileAsync(oldFile.StoredPath);

                // Delete metadata row from DB
                fileRepo.Delete(oldFile);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }



    // =========================
    // Upload Profile photo
    // =========================
    public async Task UploadProfilePhotoAsync(IFormFile file)
    {
        var userId = GetCurrentUserId();

        var user = await _userManager.Users.Include(x => x.PswUser)
            .FirstOrDefaultAsync(x => x.Id == userId);

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Unknown";

        if (user == null)
            throw new NotFoundException("User not found");

        var fileRepo = _unitOfWork.GetRepository<FileMetadata, Guid>();

        FileMetadata? oldMetadata = null;

        if (user.ProfilePhotoId.HasValue)
        {
            oldMetadata = await fileRepo.GetByIdAsync(user.ProfilePhotoId.Value);
            user.ProfilePhotoId = null;  // Remove FK reference first
            await _userManager.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        if (oldMetadata != null)
        {
            await _fileService.DeleteFileAsync(oldMetadata.StoredPath);  // Delete from S3
            fileRepo.Delete(oldMetadata);  // Delete from DB
            await _unitOfWork.SaveChangesAsync();
        }

        var folder = "profiles/photos";
        var metadata = await _fileService.UploadFileAsync(file, userId, folder);

        user.ProfilePhotoId = metadata.Id;
        if(role == "PSW")
        {
            user.PswUser.VerificationStatus = Relief.Domain.Enums.VerificationStatus.Pending;
            user.PswUser.VerificationRejectionReason = null;
        }
        await _userManager.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveProfilePhotoAsync()
    {
        var userId = GetCurrentUserId();

        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            throw new NotFoundException("User not found");

        if (!user.ProfilePhotoId.HasValue)
            throw new NotFoundException("No profile photo to remove");

        var fileRepo = _unitOfWork.GetRepository<FileMetadata, Guid>();
        var oldMetadata = await fileRepo.GetByIdAsync(user.ProfilePhotoId.Value);

        // Clear reference first
        user.ProfilePhotoId = null;
        await _userManager.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        if (oldMetadata != null)
        {
            await _fileService.DeleteFileAsync(oldMetadata.StoredPath);
            fileRepo.Delete(oldMetadata);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<string> GetProfilePhotoUrlAsync()
    {
        var userId = GetCurrentUserId();

        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            throw new NotFoundException("User not found");

        if (!user.ProfilePhotoId.HasValue)
            throw new NotFoundException("No profile photo uploaded");

        var fileRepo = _unitOfWork.GetRepository<FileMetadata, Guid>();
        var metadata = await fileRepo.GetByIdAsync(user.ProfilePhotoId.Value);

        if (metadata == null)
            throw new NotFoundException("Profile photo metadata not found");

        // Return a temporary presigned URL (valid for 30 min by default)
        return _fileService.GetPresignedUrl(metadata.StoredPath);
    }

    // =========================
    // Helpers
    // =========================

    private Guid GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
            throw new UnauthorizedException("HttpContext is not available");

        var user = httpContext.User;

        var userId =
            user.FindFirst("userId")?.Value ??
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            user.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("UserId claim not found in token");

        return Guid.Parse(userId);
    }


    private AddressDTO MapAddress(ApplicationUser user)
    {
        if (user.Address == null)
            return null;

        return new AddressDTO
        {
            ApartmentNumber = user.Address.ApartmentNumber,
            Street = user.Address.Street,
            City = user.Address.City,
            State = user.Address.State,
            Country = user.Address.Country,
            PostalCode = user.Address.PostalCode
        };
    }

    private FileDto? MapFile(FileMetadata? file)
    {
        if (file == null)
            return null;

        return new FileDto
        {
            Id = file.Id,
            FileName = file.OriginalFileName,
            Url = _fileService.GetPresignedUrl(file.StoredPath),
            ContentType = file.ContentType
        };
    }
}