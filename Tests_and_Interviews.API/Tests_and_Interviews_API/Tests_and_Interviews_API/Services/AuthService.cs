using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using Tests_and_Interviews_API.DTOs;
using Tests_and_Interviews_API.Models.Core;
using Tests_and_Interviews_API.Repositories.Interfaces;
using Tests_and_Interviews_API.Services.Interfaces;

namespace Tests_and_Interviews_API.Services
{
    /// <summary>
    /// Provides login and registration logic.
    /// </summary>
    public class AuthService : IAuthService
    {
        private const string SecretKey =
            "O_CHEIE_SECRET_FOARTE_LUNGA_SI_SIGURA_AICI_12345!";
        private const string Issuer = "UBB-SE-2026";
        private const string Audience = "UBB-SE-Client";

        private readonly IUserRepository userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        public AuthService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <inheritdoc/>
        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            User? user = await this.userRepository.GetByEmailAsync(dto.Email);

            if (user == null)
            {
                return null;
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!passwordValid)
            {
                return null;
            }

            return new AuthResponseDto
            {
                Token = this.GenerateJwt(user),
                Role = user.Role,
                Name = user.Name,
                UserId = user.Id,
            };
        }

        /// <inheritdoc/>
        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            User? existing = await this.userRepository.GetByEmailAsync(dto.Email);

            if (existing != null)
            {
                return null;
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            User newUser = new User(0, dto.Name, dto.Email, hash, dto.Role);
            await this.userRepository.AddAsync(newUser);

            User? created = await this.userRepository.GetByEmailAsync(dto.Email);

            if (created == null)
            {
                return null;
            }

            return new AuthResponseDto
            {
                Token = this.GenerateJwt(created),
                Role = created.Role,
                Name = created.Name,
                UserId = created.Id,
            };
        }

        private string GenerateJwt(User user)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(SecretKey));

            SigningCredentials credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
