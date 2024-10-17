namespace LibraryApp.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public required string ISBN { get; set; }
        public required string Title { get; set; }
        public required string Genre { get; set; }
        public required string Description { get; set; }
        public int AuthorId { get; set; }
        public DateTime? ReturnAt { get; set; }
        public DateTime? BorrowedAt { get; set; }
        public string? BorrowedByUserId { get; set; }
        public string? ImagePath { get; set; }
    }
}
