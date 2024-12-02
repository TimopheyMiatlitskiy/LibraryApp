namespace LibraryApp.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBookRepository Books { get; }
        IAuthorRepository Authors { get; }
        IUserRepository Users { get; }
        Task<int> CompleteAsync();
    }
}
