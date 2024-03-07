using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using API.Interfaces;

namespace API.Services
{
    public class UserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task UpdateUserPasswordSalt(int userId)
        {
           var user = await _context.Users.FindAsync(userId);

           if (user != null)
           {
              using var hmac = new HMACSHA512();
              user.PasswordSalt = hmac.Key;

              _context.Users.Update(user);
              await _context.SaveChangesAsync();
            }
        }
    }
}