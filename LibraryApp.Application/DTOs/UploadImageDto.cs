using Microsoft.AspNetCore.Http;

namespace LibraryApp.DTOs
{
    public class UploadImageDto
    {
        public int Id { get; set; }
        public IFormFile ImageFile { get; set; } = null!;
    }
}
