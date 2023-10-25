﻿using System.ComponentModel.DataAnnotations;

namespace SaleApi.Models.Users
{
    public class LoginUser
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
