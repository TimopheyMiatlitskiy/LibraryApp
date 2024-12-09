namespace LibraryApp.DTOs
{
    public class GetAuthorsDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
