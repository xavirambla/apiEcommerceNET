
using System;
using System.Security.Claims;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiEcommerce.Repository;

public class UserRepository : IUserRepository
{
    public readonly ApplicationDbContext _db;
    private string? secretKey;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    // Mapster no requiere IMapper

    public UserRepository(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
        _userManager = userManager;
        _roleManager = roleManager;
    }

    
     public ApplicationUser? GetUser(string userId)
    {
        return _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
    }

   
    public ICollection<ApplicationUser> GetUsers()
    {
        return _db.ApplicationUsers.OrderBy(u => u.UserName).ToList();
    }

    public bool IsUniqueUser(string username)
    {
        return  _db.Users.Any(u =>u.UserName!=null && u.UserName.ToLower().Trim() == username.ToLower().Trim()) == false;
        
    }
    // dotnet add package BCrypt.Net-Next --version 4.0.3
    //https://github.com/BcryptNet/bcrypt.net

    public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
    {
        if (string.IsNullOrEmpty(userLoginDto.Username))
        {
            return new UserLoginResponseDto()
            {
                User = null,
                Token = "",
                Message = "Username es requerido"
            };
        }
        var user = await _db.ApplicationUsers.FirstOrDefaultAsync<ApplicationUser>(u => u.UserName != null && u.UserName.ToLower().Trim() == userLoginDto.Username.ToLower().Trim());
        if (user == null)
        {
            return new UserLoginResponseDto()
            {
                User = null,
                Token = "",
                Message = "Username no encontrado"
            };
        }
        if (userLoginDto.Password == null)
        {
            return new UserLoginResponseDto()
            {
                User = null,
                Token = "",
                Message = "Password requerido"

            };
        }
        bool isValid = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);
        if (!isValid)
        {
            return new UserLoginResponseDto()
            {
                User = null,
                Token = "",
                Message = "Credenciales incorrectas"

            };
        }



        /*
                    if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
                    {
                        return new UserLoginResponseDto()
                        {
                            User = null,
                            Token = "",
                            Message = "Credenciales incorrectas"
                        };
                    }
                    */
        // Generar el token
        var handlerToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("SecretKey no está configurada");
        }

        var roles = await _userManager.GetRolesAsync(user);




        var key = System.Text.Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
        {
            Subject = new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim("id", user.Id.ToString()),
                new System.Security.Claims.Claim("username", user.UserName ?? string.Empty),
                new System.Security.Claims.Claim( ClaimTypes.Role, roles.FirstOrDefault()   ?? string.Empty ),
            }),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = handlerToken.CreateToken(tokenDescriptor);
        /*
                return new UserLoginResponseDto()
                {
                    Token = handlerToken.WriteToken(token),

                    User = new UserRegisterDto()
                    {
                        Username = user.Username,
                        Name = user.Name,
                        Role = user.Role,
                        Password = user.Password ?? ""
                    },
                    Message = "Usuario logueado correctamente"
                };
        */
        return new UserLoginResponseDto()
        {
            Token = handlerToken.WriteToken(token),
            User = user.Adapt<UserDataDto>(),
            Message = "Usuario logueado correctamente"
        };



    }

    public async Task<UserDataDto> Register(CreateUserDto createUserDto)
    {
        /*
                string encriptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
                var user = new ApplicationUser()
                {
                    Name = createUserDto.Name,
                    Username = createUserDto.Username ?? "No Username",
                    Password = encriptedPassword,
                    Role = createUserDto.Role
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                return user;
        */
        if (string.IsNullOrEmpty(createUserDto.Username))
        {
            throw new ArgumentNullException("Username es requerido");
        }


        if (createUserDto.Password == null)
        {
            throw new ArgumentNullException("Password es requerido");
        }

        var user = new ApplicationUser()
        {
            UserName = createUserDto.Username,
            Email = createUserDto.Username,
            NormalizedEmail = createUserDto.Username.ToUpper(),
            Name = createUserDto.Name,
        };
        var result = await _userManager.CreateAsync(user, createUserDto.Password);
        if (result.Succeeded)
        {
            var userRole = createUserDto.Role ?? "User";
            var roleExists = await _roleManager.RoleExistsAsync(userRole);
            if (!roleExists)
            {
                var identityRole = new IdentityRole(userRole);
                await _roleManager.CreateAsync(identityRole);

            }
            await _userManager.AddToRoleAsync(user, userRole);
            var createdUser = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == createUserDto.Username);
            return createdUser.Adapt<UserDataDto>();
        }
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            
        throw new ApplicationException($"No se pudo crear el usuario: {errors}");

    }



}
