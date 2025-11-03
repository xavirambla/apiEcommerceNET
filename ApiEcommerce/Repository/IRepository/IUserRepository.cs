using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Repository.IRepository;
/*
=============
🏆 Ejercicio 
=============
*/
// 1. Crear una interfaz llamada IUserRepository.
//
// 2. Incluir los siguientes métodos en la interfaz:
//
//    - GetUsers
//        → Devuelve todos los usuarios en ICollection del tipo User.
//
//    - GetUser
//        → Recibe un id y devuelve un solo objeto User o null si no se encuentra.
//
//    - IsUniqueUser
//        → Recibe un nombre de usuario y devuelve un bool indicando si el nombre de usuario es único.
//
//    - Login
//        → Recibe un objeto UserLoginDto y devuelve un UserLoginResponseDto de forma asíncrona (Task).
//
//    - Register
//        → Recibe un objeto CreateUserDto y devuelve un objeto User de forma asíncrona (Task).

public interface IUserRepository
{
    public ICollection<ApplicationUser> GetUsers();
    public ApplicationUser? GetUser(string userId);   
    public bool IsUniqueUser(string username);
    public Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto); 

    public Task<UserDataDto> Register(CreateUserDto createUserDto);
}


