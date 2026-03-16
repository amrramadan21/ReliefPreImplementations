using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Relief.Domain.Contracts;
using Relief.Domain.Entities;
using Relief.Domain.Entities.Users;
using Relief.Domain.Enums;
using Relief.Domain.Exceptions;
using Relief.ServiceAbstraction.Interfaces.Users;
using Shared.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Relief.Services.Implementations.Users
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly bool _skipEmailVerification;
        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration config,
            IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _userManager = userManager;
            _config = config;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _skipEmailVerification = config.GetValue<bool>("AWS:SkipEmailVerification");
        }

        // =========================================================
        // Register
        // =========================================================
        public async Task<RegisterResponseDTO> RegisterUserAsync(RegisterDTO dto, string role)
        {
            if (!Enum.TryParse<Gender>(dto.Gender, true, out var parsedGender))
                throw new BadRequestException($"'{dto.Gender}' is not a valid gender.");

            if (dto.Address == null)
                throw new BadRequestException("Address is required.");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                BirthOfDate = dto.DateOfBirth,
                Gender = parsedGender,
                Address = new Address
                {
                    ApartmentNumber = dto.Address.ApartmentNumber??-1,
                    Street = dto.Address.Street?? string.Empty,
                    City = dto.Address.City ?? string.Empty,
                    State = dto.Address.State ?? string.Empty,
                    PostalCode = dto.Address.PostalCode ?? string.Empty,
                    Country = dto.Address.Country ?? string.Empty
                }
            };

            return await RegisterUserCoreAsync(user, dto, dto.Password, role);
        }

        // =========================================================
        // Verification code
        // =========================================================

        public async Task<AuthResponseDTO> VerifyEmailAsync(VerifyEmailDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new BadRequestException("User not found.");

            if (user.EmailConfirmed)
                throw new BadRequestException("Email is already verified.");

            if (user.EmailVerificationCode != dto.Code)
                throw new BadRequestException("Invalid verification code.");

            if (user.VerificationCodeExpiry < DateTime.UtcNow)
                throw new BadRequestException("Verification code has expired. Please request a new one.");

            // ✅ Mark email as confirmed
            user.EmailConfirmed = true;
            user.EmailVerificationCode = null;
            user.VerificationCodeExpiry = null;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                throw new BadRequestException(
                    string.Join("; ", updateResult.Errors.Select(e => e.Description)));

            // ✅ Return token after successful verification
            return await BuildTokenAsync(user);
        }

        // =========================================================
        // ✅ NEW: Resend Verification Code
        // =========================================================
        public async Task<RegisterResponseDTO> ResendVerificationCodeAsync(ResendCodeDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new BadRequestException("User not found.");

            if (user.EmailConfirmed)
                throw new BadRequestException("Email is already verified.");

            // ✅ Rate limit: prevent spamming
            if (user.VerificationCodeExpiry.HasValue
                && user.VerificationCodeExpiry.Value > DateTime.UtcNow.AddMinutes(13))
            {
                throw new BadRequestException(
                    "Please wait at least 2 minutes before requesting a new code.");
            }

            var code = GenerateVerificationCode();

            user.EmailVerificationCode = code;
            user.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(15);

            await _userManager.UpdateAsync(user);
            await _emailService.SendVerificationCodeAsync(user.Email!, code);

            return new RegisterResponseDTO
            {
                Message = "Verification code resent. Please check your email.",
                Email = user.Email!
            };
        }

        // =========================================================
        // Login
        // =========================================================
        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new UnauthorizedException("Invalid email or password.");

            var ok = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!ok)
                throw new UnauthorizedException("Invalid email or password.");

            // ✅ Auto-verify users created before email verification feature
            if (!user.EmailConfirmed && user.EmailVerificationCode == null)
            {
                // Old user (no verification code was ever sent)
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }

            if (!user.EmailConfirmed)
                throw new UnauthorizedException("Please verify your email before logging in.");

            return await BuildTokenAsync(user);
        }

        // =========================================================
        // Token Builder
        // =========================================================
        private async Task<AuthResponseDTO> BuildTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "";

            var jwt = _config.GetSection("Jwt");

            var key = jwt["Key"] ?? throw new Exception("JWT Key not configured.");
            var issuer = jwt["Issuer"] ?? throw new Exception("JWT Issuer not configured.");
            var audience = jwt["Audience"] ?? throw new Exception("JWT Audience not configured.");

            var expiryMinutes = int.Parse(jwt["ExpiryMinutes"] ?? "60");

            var claims = new List<Claim>
        {
            new Claim("userId", user.Id.ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
        };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            if (role.Equals("PSW", StringComparison.OrdinalIgnoreCase))
            {
                var pswRepo = _unitOfWork.GetRepository<PswUser, Guid>();

                var psw = await pswRepo.GetByIdAsync(user.Id);

                return new PswAuthResponseDTO
                {
                    Token = tokenString,
                    ExpiresAtUtc = expires,
                    Email = user.Email ?? "",
                    Role = role,
                    UserId = user.Id,
                    VerficationStatus = psw.VerificationStatus.ToString()
                };
            }

            return new AuthResponseDTO
            {
                Token = tokenString,
                ExpiresAtUtc = expires,
                Email = user.Email ?? "",
                Role = role,
                UserId = user.Id
            };
        }

        // =========================================================
        // ✅ NEW: Generate 6-Digit Code
        // =========================================================
        private string GenerateVerificationCode()
        {
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var code = (Math.Abs(BitConverter.ToInt32(bytes, 0)) % 900000 + 100000);
            return code.ToString();
        }

        // =========================================================
        // Logout Logic
        // =========================================================
        public async Task LogoutAsync()
        {
            // JWT is stateless
            // logout is handled on the client by removing the token
            await Task.CompletedTask;
        }

        // =========================================================
        // Core Register Logic
        // =========================================================
        private async Task<RegisterResponseDTO> RegisterUserCoreAsync(
            ApplicationUser user,
            RegisterDTO dto,
            string password,
            string role)
        {
            var existing = await _userManager.FindByEmailAsync(user.Email ?? "");

            if (existing != null)
                throw new ConflictException("Email is already registered.");

            var verificationCode = GenerateVerificationCode();
            user.EmailVerificationCode = verificationCode;
            user.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(5);
            user.EmailConfirmed = false;

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var create = await _userManager.CreateAsync(user, password);

                if (!create.Succeeded)
                    throw new BadRequestException(string.Join("; ", create.Errors.Select(e => e.Description)));

                var addRole = await _userManager.AddToRoleAsync(user, role);

                if (!addRole.Succeeded)
                    throw new BadRequestException(string.Join("; ", addRole.Errors.Select(e => e.Description)));

                switch (role)
                {
                    case "CareHome":

                        var careHomeRepo = _unitOfWork.GetRepository<CareHomeUser, Guid>();

                        await careHomeRepo.AddAsync(new CareHomeUser
                        {
                            Id = user.Id,
                            LegalName = dto.LegalName ?? "Pending",
                            BusinessLicense = dto.BusinessLicense??"Pending",
                            VaccinationPolicy = "Pending"
                        });

                        break;

                    case "PSW":

                        var pswRepo = _unitOfWork.GetRepository<PswUser, Guid>();

                        await pswRepo.AddAsync(new PswUser
                        {
                            ApplicationUserId = user.Id,
                            VerificationStatus = VerificationStatus.None,
                            VerificationRejectionReason = "",
                        });
                        break;

                    case "Individual":

                        var indRepo = _unitOfWork.GetRepository<IndividualCareHomeUser, Guid>();

                        await indRepo.AddAsync(new IndividualCareHomeUser
                        {
                            Id = user.Id
                        });

                        break;

                    default:
                        throw new BadRequestException($"Role '{role}' is not supported.");
                }

                await _unitOfWork.SaveChangesAsync();

                transaction.Complete();
            }

            if (_skipEmailVerification)
            {
                // ✅ Auto-verify the user (skip email)
                user.EmailConfirmed = true;
                user.EmailVerificationCode = null;
                user.VerificationCodeExpiry = null;
                await _userManager.UpdateAsync(user);

                return new RegisterResponseDTO
                {
                    Message = "Registration successful. Email auto-verified.",
                    Email = user.Email!
                };
            }

            await _emailService.SendVerificationCodeAsync(user.Email!, verificationCode);

            return new RegisterResponseDTO
            {
                Message = "Registration successful. Please check your email for verification code.",
                Email = user.Email!
            };
        }

     
    }
}
