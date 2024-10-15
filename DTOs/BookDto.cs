﻿namespace LibraryApp.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }
        public DateTime? ReturnAt { get; set; }
        public DateTime? BorrowedAt { get; set; }
        public string? BorrowedByUserId { get; set; }
        public string? ImagePath { get; set; }
    }
}