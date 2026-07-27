using Azure.Core;
using LibraryManagementClassLib.Data;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Entities;
using LibraryManagementClassLib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Implementation;

public class BookService : IBookService
{
    private readonly LibraryManagementAPIDbContext _context;

    public BookService(LibraryManagementAPIDbContext context)
    {
        _context = context;
    }

    public async Task<List<BookDto>> AdminGetAllBooksAsync(BookQueryDto queryDto)
    {
        var booksQuery = _context.Books.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.CourseCode))
        {
            booksQuery = booksQuery.Where(b => b.Subjects.Any(s => s.SemesterCode.ToLower().Substring(0, 3).Contains(queryDto.CourseCode)));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.SearchTerm))
        {
            booksQuery = booksQuery.Where(b => b.Title.ToLower().Contains(queryDto.SearchTerm) ||
                                               b.ISBN.ToLower().Contains(queryDto.SearchTerm) ||
                                               b.Subjects.Any(s => s.Code.ToLower().Contains(queryDto.SearchTerm)) ||
                                               b.Authors.Any(ba => ba.FirstName.ToLower().Contains(queryDto.SearchTerm) || ba.LastName.ToLower().Contains(queryDto.SearchTerm)));
        }

        var totalCount = await booksQuery.CountAsync();

        if (!string.IsNullOrWhiteSpace(queryDto.SortBy))
        {
            if (queryDto.SortBy.Equals("title", StringComparison.OrdinalIgnoreCase))
            {
                booksQuery = queryDto.IsDescending
                    ? booksQuery.OrderByDescending(b => b.Title)
                    : booksQuery.OrderBy(b => b.Title);
            }
            if (queryDto.SortBy.Equals("isbn", StringComparison.OrdinalIgnoreCase))
            {
                booksQuery = queryDto.IsDescending
                    ? booksQuery.OrderByDescending(b => b.ISBN)
                    : booksQuery.OrderBy(b => b.ISBN);
            }
            if (queryDto.SortBy.Equals("availableCopies", StringComparison.OrdinalIgnoreCase))
            {
                booksQuery = queryDto.IsDescending
                    ? booksQuery.OrderByDescending(b => b.AvailableCopies)
                    : booksQuery.OrderBy(b => b.AvailableCopies);
            }


        }

        var pageNumber = queryDto.PageNumber <= 0 ? 1 : queryDto.PageNumber;
        var pageSize = queryDto.PageSize <= 0 ? 10 : queryDto.PageSize;

        var skipNumber = (pageNumber - 1) * pageSize;
        booksQuery = booksQuery.Skip(skipNumber).Take(pageSize);

        var books = await booksQuery
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                ISBN = b.ISBN,
                TotalCopies = b.AvailableCopies,
                Authors = b.Authors.Select(a => new AuthorDto
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName
                }).ToList(),
                Categories = b.Categories.Select(c => c.Name).ToList(),
                Publisher = b.Publisher.Name,
                PublisherAddress = b.Publisher.Address ?? "",
                ResultCount = totalCount,
            })
            .ToListAsync();
        return books;
    }

    public async Task<List<BookDto>> UserGetAllBooksAsync(BookQueryDto queryDto)
    {
        var booksQuery = _context.Books.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.CourseCode))
        {
            booksQuery = booksQuery.Where(b => b.Subjects.Any(s => s.SemesterCode.ToLower().Substring(0, 3).Contains(queryDto.CourseCode)));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.SearchTerm))
        {
            booksQuery = booksQuery.Where(b => b.Title.ToLower().Contains(queryDto.SearchTerm) ||
                                               b.ISBN.ToLower().Contains(queryDto.SearchTerm) ||
                                               b.Subjects.Any(s => s.Code.ToLower().Contains(queryDto.SearchTerm)) ||
                                               b.Authors.Any(ba => ba.FirstName.ToLower().Contains(queryDto.SearchTerm) || ba.LastName.ToLower().Contains(queryDto.SearchTerm)));
        }

        var totalCount = await booksQuery.CountAsync();

        if (!string.IsNullOrWhiteSpace(queryDto.SortBy))
        {
            if (queryDto.SortBy.Equals("title", StringComparison.OrdinalIgnoreCase))
            {
                booksQuery = queryDto.IsDescending
                    ? booksQuery.OrderByDescending(b => b.Title)
                    : booksQuery.OrderBy(b => b.Title);
            }
            if (queryDto.SortBy.Equals("isbn", StringComparison.OrdinalIgnoreCase))
            {
                booksQuery = queryDto.IsDescending
                    ? booksQuery.OrderByDescending(b => b.ISBN)
                    : booksQuery.OrderBy(b => b.ISBN);
            }
            if (queryDto.SortBy.Equals("availableCopies", StringComparison.OrdinalIgnoreCase))
            {
                booksQuery = queryDto.IsDescending
                    ? booksQuery.OrderByDescending(b => b.AvailableCopies)
                    : booksQuery.OrderBy(b => b.AvailableCopies);
            }


        }

        var pageNumber = queryDto.PageNumber <= 0 ? 1 : queryDto.PageNumber;
        var pageSize = queryDto.PageSize <= 0 ? 10 : queryDto.PageSize;

        var skipNumber = (pageNumber - 1) * pageSize;
        booksQuery = booksQuery.Skip(skipNumber).Take(pageSize);

        var books = await booksQuery
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                ISBN = b.ISBN,
                TotalCopies = b.TotalCopies,
                Authors = b.Authors.Select(a => new AuthorDto
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName
                }).ToList(),
                Categories = b.Categories.Select(c => c.Name).ToList(),
                Publisher = b.Publisher.Name,
                PublisherAddress = b.Publisher.Address ?? "",
                AvailableCopies = b.AvailableCopies,
                ResultCount = totalCount,
            })
            .ToListAsync();
        return books;
    }

    public async Task<string> AddBookAsync(BookDto request)
    {
        var bookExists = await _context.Books.Where(b => b.ISBN == request.ISBN).FirstOrDefaultAsync();
        if (bookExists == null)
        {
            var book = new Book
            {
                Title = request.Title,
                ISBN = request.ISBN,
                TotalCopies = request.TotalCopies,
                AvailableCopies = request.TotalCopies
            };
            foreach (var requestAuthor in request.Authors)
            {
                var author = await HandleAuthorAsync(requestAuthor);
                book.Authors.Add(author);
            }

            foreach (var requestCategory in request.Categories)
            {
                var category = await HandleCategoryAsync(requestCategory);
                book.Categories.Add(category);
            }

            var publisher = await HandlePublisherAsync(request.Publisher, request.PublisherAddress);
            book.Publisher = publisher;

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return "New book was added";
        }
        else if (bookExists != null)
        {
            bookExists.TotalCopies += request.TotalCopies;
            bookExists.AvailableCopies += request.TotalCopies;
            await _context.SaveChangesAsync();
            return $"{request.TotalCopies} copies of {request.Title} was added to inventory. New available copies: {bookExists.AvailableCopies}";
        }

        return "An error occurred while adding the book";
    }

    public async Task<bool> UpdateBookAsync(int? bookId, BookDto bookDto)
    {
        var existingBook = await _context.Books
            .Include(b => b.Authors)
            .Include(b => b.Categories)
            .Include(b => b.Subjects)
            .Include(b => b.Publisher)
            .FirstOrDefaultAsync(b => b.Id == bookId);

        if (existingBook == null)
            return false;

        // Update basic properties
        existingBook.Title = bookDto.Title;
        existingBook.ISBN = bookDto.ISBN;
        existingBook.TotalCopies = bookDto.TotalCopies;
        existingBook.AvailableCopies = bookDto.TotalCopies;

        #region Authors

        var requestedAuthors = bookDto.Authors
            .Select(a => $"{a.FirstName}|{a.LastName}")
            .ToHashSet();

        var existingAuthors = await _context.Authors
            .Where(a => requestedAuthors.Contains(a.FirstName + "|" + a.LastName))
            .ToListAsync();

        var authorLookup = existingAuthors.ToDictionary(
            a => $"{a.FirstName}|{a.LastName}");

        existingBook.Authors.Clear();

        foreach (var dto in bookDto.Authors)
        {
            var key = $"{dto.FirstName}|{dto.LastName}";

            if (!authorLookup.TryGetValue(key, out var author))
            {
                author = new Author
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };

                _context.Authors.Add(author);
                authorLookup[key] = author;
            }

            existingBook.Authors.Add(author);
        }

        #endregion

        #region Categories

        var existingCategories = await _context.Categories
            .Where(c => bookDto.Categories.Contains(c.Name))
            .ToListAsync();

        var categoryLookup = existingCategories.ToDictionary(c => c.Name);

        existingBook.Categories.Clear();

        foreach (var name in bookDto.Categories)
        {
            if (!categoryLookup.TryGetValue(name, out var category))
            {
                category = new Category
                {
                    Name = name
                };

                _context.Categories.Add(category);
                categoryLookup[name] = category;
            }

            existingBook.Categories.Add(category);
        }

        #endregion

        #region Subjects

        var subjectIds = bookDto.SubjectIds;

        var subjects = await _context.Subjects
            .Where(s => subjectIds.Contains(s.Id))
            .ToListAsync();

        existingBook.Subjects.Clear();

        foreach (var subject in subjects)
        {
            existingBook.Subjects.Add(subject);
        }

        #endregion

        // Publisher
        existingBook.Publisher = await HandlePublisherAsync(bookDto.Publisher, "");

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteBookAsync(int? bookId)
    {
        var existingBook = await _context.Books.FindAsync(bookId);
        if (existingBook == null)
        {
            return false;
        }
        _context.Books.Remove(existingBook);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<AdminBookViewDto> AdminGetBookById(int bookId)
    {
        var availableBook = await _context.Books
            .Include(b => b.Subjects)
            .Where(b => b.Id == bookId)
            .Select(b => new AdminBookViewDto
            {
                Id = b.Id,
                Title = b.Title,
                ISBN = b.ISBN,
                TotalCopies = b.TotalCopies,
                Authors = b.Authors.Select(a => new AuthorDto
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName
                }).ToList(),
                Categories = b.Categories.Select(c => c.Name).ToList(),
                Publisher = b.Publisher.Name,
                PublisherAddress = b.Publisher.Address ?? "",
                Subjects = b.Subjects.Select(s => new SubjectDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Code = s.Code
                }).ToList(),
                AvailableCopies = b.AvailableCopies
            })
            .FirstOrDefaultAsync();
        if (availableBook == null)
        {
            return null;
        }
        return availableBook;
    }

    private async Task<Author> HandleAuthorAsync(AuthorDto authorDto)
    {
        var author = await _context.Authors
            .FirstOrDefaultAsync(a => a.FirstName == authorDto.FirstName && a.LastName == authorDto.LastName);
        if (author == null)
        {
            author = new Author
            {
                FirstName = authorDto.FirstName,
                LastName = authorDto.LastName
            };
            _context.Authors.Add(author);
        }
        return author;
    }

    private async Task<Category> HandleCategoryAsync(string categoryName)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == categoryName);
        if (category == null)
        {
            category = new Category
            {
                Name = categoryName
            };
            _context.Categories.Add(category);
        }
        return category;
    }

    private async Task<Publisher> HandlePublisherAsync(string publisherName, string publisherAddress)
    {
        var publisher = await _context.Publishers
            .FirstOrDefaultAsync(p => p.Name == publisherName);
        if (publisher == null)
        {
            publisher = new Publisher
            {
                Name = publisherName,
                Address = publisherAddress
            };
            _context.Publishers.Add(publisher);
        }
        return publisher;
    }

    //public async Task<string> BulkAddBooksAsync(List<AddBookDto> bookDtos)
    //{
    //    foreach (var bookDto in bookDtos)
    //    {
    //        await AddBookAsync(bookDto);
    //    }
    //    return "Bulk insert completed successfully";
    //}

    public async Task<string> AddBooksAsync(
    List<AddBookDto> requests)
    {
        if (requests == null || requests.Count == 0)
        {
            return "No books provided.";
        }

        // --------------------------------------------------
        // 1. Normalize input
        // --------------------------------------------------

        foreach (var request in requests)
        {
            request.ISBN = request.ISBN.Trim();

            request.Title = request.Title.Trim();

            request.Publisher = request.Publisher?.Trim();

            request.Authors = request.Authors?
                .Select(a => new AuthorDto
                {
                    FirstName = a.FirstName.Trim(),
                    LastName = a.LastName.Trim()
                })
                .ToList() ?? new();

            request.Categories = request.Categories?
                .Select(c => c.Trim())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new();
        }

        // --------------------------------------------------
        // 2. Get all ISBNs from request
        // --------------------------------------------------

        var isbnList = requests
            .Select(x => x.ISBN)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();


        // --------------------------------------------------
        // 3. Load existing books in ONE DB query
        // --------------------------------------------------

        var existingBooks = await _context.Books
            .Where(b => isbnList.Contains(b.ISBN))
            .ToListAsync();

        var existingBooksByIsbn = existingBooks
            .ToDictionary(
                b => b.ISBN,
                StringComparer.OrdinalIgnoreCase);


        // --------------------------------------------------
        // 4. Collect all authors
        // --------------------------------------------------

        var authorKeys = requests
            .SelectMany(x => x.Authors)
            .Select(a => new
            {
                FirstName = a.FirstName,
                LastName = a.LastName
            })
            .Distinct()
            .ToList();


        // --------------------------------------------------
        // 5. Load existing authors
        // --------------------------------------------------

        // 5. Load only potentially matching authors

        var firstNames = authorKeys
            .Select(x => x.FirstName)
            .Distinct()
            .ToList();

        var lastNames = authorKeys
            .Select(x => x.LastName)
            .Distinct()
            .ToList();

        var existingAuthors = await _context.Authors
            .Where(a =>
                firstNames.Contains(a.FirstName) &&
                lastNames.Contains(a.LastName))
            .ToListAsync();

        var authorsByKey = existingAuthors
            .ToDictionary(
                a => $"{a.FirstName.Trim().ToLower()}|{a.LastName.Trim().ToLower()}",
                StringComparer.OrdinalIgnoreCase);


        // --------------------------------------------------
        // 6. Collect categories
        // --------------------------------------------------

        var categoryNames = requests
            .SelectMany(x => x.Categories)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();


        // --------------------------------------------------
        // 7. Load existing categories
        // --------------------------------------------------

        var existingCategories = await _context.Categories
            .Where(c => categoryNames.Contains(c.Name))
            .ToListAsync();

        var categoriesByName = existingCategories
            .ToDictionary(
                c => c.Name,
                StringComparer.OrdinalIgnoreCase);


        // --------------------------------------------------
        // 8. Collect publishers
        // --------------------------------------------------

        var publisherNames = requests
            .Select(x => x.Publisher)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();


        // --------------------------------------------------
        // 9. Load existing publishers
        // --------------------------------------------------

        var existingPublishers = await _context.Publishers
            .Where(p => publisherNames.Contains(p.Name))
            .ToListAsync();

        var publishersByName = existingPublishers
            .ToDictionary(
                p => p.Name,
                StringComparer.OrdinalIgnoreCase);


        // --------------------------------------------------
        // 10. Load all subjects in ONE query
        // --------------------------------------------------

        var subjectIds = requests
            .SelectMany(x => x.SubjectIds)
            .Distinct()
            .ToList();

        var subjects = await _context.Subjects
            .Where(s => subjectIds.Contains(s.Id))
            .ToListAsync();

        var subjectsById = subjects
            .ToDictionary(s => s.Id);


        // --------------------------------------------------
        // 11. Track newly created entities in memory
        // --------------------------------------------------

        var newAuthors = new Dictionary<string, Author>(
            StringComparer.OrdinalIgnoreCase);

        var newCategories = new Dictionary<string, Category>(
            StringComparer.OrdinalIgnoreCase);

        var newPublishers = new Dictionary<string, Publisher>(
            StringComparer.OrdinalIgnoreCase);


        // --------------------------------------------------
        // 12. Process books in memory
        // --------------------------------------------------

        foreach (var request in requests)
        {
            // ----------------------------------------------
            // Existing book
            // ----------------------------------------------

            if (existingBooksByIsbn.TryGetValue(
                    request.ISBN,
                    out var existingBook))
            {
                existingBook.TotalCopies += request.TotalCopies;

                existingBook.AvailableCopies += request.TotalCopies;

                continue;
            }


            // ----------------------------------------------
            // New book
            // ----------------------------------------------

            var book = new Book
            {
                Title = request.Title,
                ISBN = request.ISBN,
                TotalCopies = request.TotalCopies,

                // Don't trust availableCopies from request
                AvailableCopies = request.TotalCopies
            };


            // ----------------------------------------------
            // Authors
            // ----------------------------------------------

            foreach (var requestAuthor in request.Authors)
            {
                var authorKey =
                    $"{requestAuthor.FirstName.Trim().ToLower()}|" +
                    $"{requestAuthor.LastName.Trim().ToLower()}";


                // Existing author
                if (!authorsByKey.TryGetValue(
                        authorKey,
                        out var author))
                {
                    // Newly created author in this batch
                    if (!newAuthors.TryGetValue(
                            authorKey,
                            out author))
                    {
                        author = new Author
                        {
                            FirstName = requestAuthor.FirstName,
                            LastName = requestAuthor.LastName
                        };

                        newAuthors.Add(authorKey, author);
                    }
                }

                book.Authors.Add(author);
            }


            // ----------------------------------------------
            // Categories
            // ----------------------------------------------

            foreach (var categoryName in request.Categories)
            {
                if (!categoriesByName.TryGetValue(
                        categoryName,
                        out var category))
                {
                    if (!newCategories.TryGetValue(
                            categoryName,
                            out category))
                    {
                        category = new Category
                        {
                            Name = categoryName
                        };

                        newCategories.Add(
                            categoryName,
                            category);
                    }
                }

                book.Categories.Add(category);
            }


            // ----------------------------------------------
            // Publisher
            // ----------------------------------------------

            if (!string.IsNullOrWhiteSpace(request.Publisher))
            {
                if (!publishersByName.TryGetValue(
                        request.Publisher,
                        out var publisher))
                {
                    if (!newPublishers.TryGetValue(
                            request.Publisher,
                            out publisher))
                    {
                        publisher = new Publisher
                        {
                            Name = request.Publisher,
                            Address = request.PublisherAddress
                        };

                        newPublishers.Add(
                            request.Publisher,
                            publisher);
                    }
                }

                book.Publisher = publisher;
            }


            // ----------------------------------------------
            // Subjects
            // ----------------------------------------------

            foreach (var subjectId in request.SubjectIds.Distinct())
            {
                if (subjectsById.TryGetValue(
                        subjectId,
                        out var subject))
                {
                    book.Subjects.Add(subject);
                }
            }


            // ----------------------------------------------
            // Add book
            // ----------------------------------------------

            _context.Books.Add(book);

            // Important:
            // Add new book to dictionary so another request
            // with same ISBN in this batch won't create
            // another Book entity.

            existingBooksByIsbn.Add(
                request.ISBN,
                book);
        }


        // --------------------------------------------------
        // 13. ONE database save
        // --------------------------------------------------

        await _context.SaveChangesAsync();


        return $"{requests.Count} books processed successfully.";
    }
}

