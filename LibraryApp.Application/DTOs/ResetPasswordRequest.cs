﻿namespace LibraryApp.DTOs
{
    public class ResetPasswordRequest
    {
        public required string Email { get; set; }
        public required string NewPassword { get; set; }
    }
}