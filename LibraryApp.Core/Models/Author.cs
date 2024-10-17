using System.Text.Json.Serialization;

namespace LibraryApp.Models
{
    public class Author
    {
        public int Id { get; set; } // Уникальный идентификатор автора
        public required string FirstName { get; set; } // Имя автора
        public required string LastName { get; set; } // Имя автора
        public string FullName => $"{FirstName} {LastName}";
        public DateTime DateOfBirth { get; set; } // Дата рождения автора
        public required string Country { get; set; } // Страна автора

        // Связь с книгами (один автор может написать много книг)
        [JsonIgnore]
        public List<Book> Books { get; set; } = new List<Book>();
    }
}