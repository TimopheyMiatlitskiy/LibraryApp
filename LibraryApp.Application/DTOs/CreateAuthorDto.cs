namespace LibraryApp.DTOs
{
    public class CreateAuthorDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required DateTime DateOfBirth { get; set; }
        public required string Country { get; set; }
    }
}
