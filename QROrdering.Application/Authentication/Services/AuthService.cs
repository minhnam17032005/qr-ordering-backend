using QROrdering.Application.Authentication.DTOs;
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Application.Exceptions;
using QROrdering.Domain.Entities.Identity;

namespace QROrdering.Application.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        public async Task<RegisterResponse> RegisterAsync(
            RegisterRequest request)
        {
            // 1. Normalize input
            var fullName = request.FullName.Trim();
            var username = request.Username.Trim();
            var email = request.Email.Trim().ToLowerInvariant();

            // 2. Check duplicate username
            var usernameExists =
                await _userRepository.ExistsByUsernameAsync(username);

            // 3. Check duplicate email
            var emailExists =
                await _userRepository.ExistsByEmailAsync(email);

            // 4. Reject if username or email already exists
            if (usernameExists || emailExists)
            {
                throw new ConflictException(
                   "Username or email already exists.");
            }

            // 5. Hash password
            var passwordHash = _passwordService.Hash(request.Password);

            // 6. Create User
            var user = new User
            {
                FullName = fullName,
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                IsActive = true
            };

            // 7. Save User
            await _userRepository.AddAsync(user);

            // 8. Return response
            return new RegisterResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email
            };
        }
    }
}