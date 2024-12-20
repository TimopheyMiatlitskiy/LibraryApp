﻿using LibraryApp.Data;
using LibraryApp.Interfaces;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Repositories
{
    public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(LibraryContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Author>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _dbSet.Include(a => a.Books)
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToListAsync();
        }

        public async Task<Author?> GetAuthorByIdAsync(int id)
        {
            return await _dbSet.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Author?> FindByNameAsync(string firstName, string lastName)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.FirstName == firstName && a.LastName == lastName);
        }
    }
}
