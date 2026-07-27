using LibraryManagementClassLib.Data;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly LibraryManagementAPIDbContext _context;

        public CategoryService(LibraryManagementAPIDbContext context)
        {
            _context = context;
        }
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = _context.Categories.Select(c => new CategoryDto { Name = c.Name }).ToList();
            return categories;
        }
    }
}
