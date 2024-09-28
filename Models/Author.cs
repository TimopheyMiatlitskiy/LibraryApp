namespace LibraryApp.Models
{
    public class Author
    {
        public int Id { get; set; } // Уникальный идентификатор автора
        public string FirstName { get; set; } // Имя автора
        public string LastName { get; set; } // Имя автора
        public string FullName => $"{FirstName} {LastName}";
        public DateTime DateOfBirth { get; set; } // Дата рождения автора
        public string Country { get; set; } // Страна автора

        // Связь с книгами (один автор может написать много книг)
        public List<Book> Books { get; set; } = new List<Book>();
    }
}