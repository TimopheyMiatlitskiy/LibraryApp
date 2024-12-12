using AutoMapper;
using LibraryApp.Models;
using LibraryApp.DTOs;

namespace LibraryApp.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Маппинг книги
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.ImagePath) ? $"/images/{Path.GetFileName(src.ImagePath)}" : null))
                .ReverseMap();

            // Маппинг создания книги
            CreateMap<CreateBookDto, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Игнорируем Id
                .ForMember(dest => dest.Author, opt => opt.Ignore()); // Игнорируем вложенные сущности

            CreateMap<UpdateBookDto, Book>()
                .ForMember(dest => dest.Author, opt => opt.Ignore()); // Игнорируем вложенные сущности

            CreateMap<BookIdDto, Book>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<BookISBNDto, Book>()
                .ForMember(dest => dest.ISBN, opt => opt.MapFrom(src => src.ISBN));

            // Маппинг автора
            CreateMap<Author, AuthorDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ReverseMap();

            CreateMap<CreateAuthorDto, Author>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<UpdateAuthorDto, Author>();

            // Маппинг для получения списка книг/авторов
            CreateMap<GetBooksDto, Book>();
            CreateMap<GetAuthorsDto, Author>();

            // Маппинг для авторизации
            CreateMap<LoginRequest, ApplicationUser>();
            CreateMap<RegisterRequest, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<ResetPasswordRequest, ApplicationUser>();
            CreateMap<RefreshTokenRequest, ApplicationUser>();

            // Маппинг для загрузки изображений
            CreateMap<UploadImageDto, Book>()
                .ForMember(dest => dest.ImagePath, opt => opt.Ignore()); // Игнорируем путь до файла
        }
    }
}
