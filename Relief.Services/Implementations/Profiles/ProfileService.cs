using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Relief.Domain.Entities.Users;
using Relief.Domain.Exceptions;
using Relief.ServiceAbstraction.Interfaces.Files;
using Relief.ServiceAbstraction.Interfaces.Profiles;
using Shared.IdentityDTOs;
using Shared.ProfileDTOs;
using System.Security.Claims;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IFileService _fileService;

    public ProfileService(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IFileService fileService)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _fileService = fileService;
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

                Address = MapAddress(user),

                ProfilePhoto = MapFile(user.ProfilePhoto), //Upload


                ProofIdentityType = user.PswUser.ProofIdentityType,
                WorkStatus = user.PswUser.WorkStatus,
                IsProfileCompleted = user.PswUser.IsProfileCompleted,
                IsVerified = user.PswUser.IsVerified,

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

                Address = MapAddress(user),

                // Individual ماعندوش البيانات دي
                BusinessLicense = "Individual",
                LegalName = $"{user.FirstName} {user.LastName}",
                VaccinationPolicy = "N/A"
            };
        }

        throw new NotFoundException("Profile type not found");
    }

    // =========================
    // Update Profile
    // =========================
    public async Task UpdateProfileAsync(UpdateProfileDto dto)
    {
        var userId = GetCurrentUserId();

        var user = await _userManager.Users
            .Include(x => x.Address)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            throw new NotFoundException("User not found");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.PhoneNumber = dto.PhoneNumber;

        if (user.Address != null && dto.Address != null)
        {
            user.Address.ApartmentNumber = dto.Address.ApartmentNumber;
            user.Address.Street = dto.Address.Street;
            user.Address.City = dto.Address.City;
            user.Address.State = dto.Address.State;
            user.Address.Country = dto.Address.Country;
            user.Address.PostalCode = dto.Address.PostalCode;
        }

        await _userManager.UpdateAsync(user);
    }

    // =========================
    // Upload Profile photo
    // =========================
    public async Task UploadProfilePhotoAsync(IFormFile file)
    {
        var userId = GetCurrentUserId();

        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            throw new NotFoundException("User not found");

        var folder = Path.Combine("uploads", "profile-photos", userId.ToString());

        var metadata = await _fileService.UploadFileAsync(file, userId, folder);

        user.ProfilePhotoId = metadata.Id;

        await _userManager.UpdateAsync(user);
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
            Url = file.StoredPath
        };
    }
}