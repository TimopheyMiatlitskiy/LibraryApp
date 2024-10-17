namespace LibraryApp.Models
{
    public class Book
    {
        public int Id { get; set; }
        public required string ISBN { get; set; }
        public required string Title { get; set; }
        public required string Genre { get; set; }
        public required string Description { get; set; }


        // Связь с автором (один ко многим)
        public int AuthorId { get; set; }
        public required Author Author { get; set; }
        public DateTime? ReturnAt { get; set; }
        public DateTime? BorrowedAt { get; set; }
        public string? BorrowedByUserId { get; set; } 

        public string? ImagePath { get; set; }
    }
}