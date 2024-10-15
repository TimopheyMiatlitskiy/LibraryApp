using AutoMapper;
using LibraryApp.Models;
using LibraryApp.DTOs;

namespace LibraryApp.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookDto>()
                            .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                            .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src =>
                                !string.IsNullOrEmpty(src.ImagePath) ? $"/images/{Path.GetFileName(src.ImagePath)}" : null));

            CreateMap<BookDto, Book>();
            CreateMap<Author, AuthorDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ReverseMap();
        }
    }
}
