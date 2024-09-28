namespace LibraryApp.Models
{
    public class Book
    {
        public int Id { get; set; } // Уникальный идентификатор книги
        public string ISBN { get; set; } // Международный стандартный книжный номер (ISBN)
        public string Title { get; set; } // Название книги
        public string Genre { get; set; } // Жанр книги
        public string Description { get; set; } // Краткое описание книги


        // Связь с автором (один ко многим)
        public int AuthorId { get; set; } // Внешний ключ для связи с автором
        public Author? Author { get; set; } // Навигационное свойство

        public DateTime ReturnAt { get; set; } // Дата возврата книги
        public DateTime BorrowedAt { get; set; } // Дата когда взяли книгу
        public string? BorrowedByUserId { get; set; } // ID пользователя, взявшего книгу 
    }
}