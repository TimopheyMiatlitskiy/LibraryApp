namespace LibraryApp.DTOs
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public required DateTime DateOfBirth { get; set; }
        public required string Country { get; set; }
    }
}
