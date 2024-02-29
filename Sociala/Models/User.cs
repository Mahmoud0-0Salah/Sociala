﻿namespace Sociala.Models
{
    public class User
    {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Password { get; set; }
            public string Photo { get; set; }
            public bool IsActive { get; set; }
            public string ActiveKey { get; set; }
    }
}
