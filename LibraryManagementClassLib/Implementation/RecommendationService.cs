using LibraryManagementClassLib.Data;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Entities;
using LibraryManagementClassLib.Helpers;
using LibraryManagementClassLib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Implementation
{
    public class RecommendationService : IRecommendationService
    {
        private readonly LibraryManagementAPIDbContext _context;

        public RecommendationService(LibraryManagementAPIDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookDto>?> GetUserRecommendations(int userId)
        {
            // 1. Load all books with their related data
            var books = await _context.Books
                .Include(b => b.Subjects)
                .Include(b => b.Categories)
                .Include(b => b.Authors)
                .Include(b => b.Publisher)
                .ToListAsync();

            // 2. Build feature representations for every book
            var featureBooks = BuildBookFeatures(books);

            // 3. Build the global vocabulary (all unique features)
            var vocabulary = BuildVocabulary(featureBooks);

            // 4. Convert each book's features into a weighted vector
            var bookVectors = BuildBookVectors(featureBooks, vocabulary);

            // 5. Retrieve the user's interaction history
            var borrowedBooks = await _context.BookIssues
                .Where(b => b.UserId == userId)
                .Select(b => b.BookId)
                .Distinct()
                .ToListAsync();

            var wishlistBooks = await _context.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.WishList.Books)
                .Select(b => b.Id)
                .ToListAsync();

            bool isColdStart = !borrowedBooks.Any() && !wishlistBooks.Any();

            // 6. Handle cold-start scenario
            if (isColdStart)
            {
                return await GetColdStartRecommendations(userId);
            }

            // 7. Build the user's preference vector from borrowed and wishlisted books
            var userVector = BuildUserVector(bookVectors, borrowedBooks, wishlistBooks);

            // 8. Compute similarity scores for all books not already interacted with
            var excludedBookIds = borrowedBooks.Union(wishlistBooks).ToHashSet();
            var recommendations = new List<BookRecommendationDto>();

            foreach (var book in bookVectors)
            {
                if (excludedBookIds.Contains(book.BookId))
                    continue;

                double similarity = RecommendationHelper.CosineSimilarity(userVector, book.Vector);
                if (similarity > 0)
                {
                    recommendations.Add(new BookRecommendationDto
                    {
                        BookId = book.BookId,
                        Similarity = similarity
                    });
                }
            }

            // 9. Order by similarity and take top 10
            var topBookIds = recommendations
                .OrderByDescending(r => r.Similarity)
                .Take(10)
                .Select(r => r.BookId)
                .ToList();

            // 10. Fetch the full book entities and preserve order
            var recommendedBooks = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                .Include(b => b.Subjects)
                .Include(b => b.Publisher)
                .Where(b => topBookIds.Contains(b.Id))
                .ToListAsync();

            var orderedBooks = topBookIds
                .Join(recommendedBooks,
                    id => id,
                    book => book.Id,
                    (id, book) => book)
                .ToList();

            // 11. Map to DTOs and return
            return MapBooks(orderedBooks);
        }

        private List<BookFeatureDto> BuildBookFeatures(List<Book> books)
        {
            return books.Select(book =>
            {
                var features = new List<string>();

                features.AddRange(book.Subjects
                    .Select(s => $"subject:{s.Name.Trim().ToLowerInvariant()}"));

                features.AddRange(book.Categories
                    .Select(c => $"category:{c.Name.Trim().ToLowerInvariant()}"));

                features.AddRange(book.Authors
                    .Select(a => $"author:{a.FirstName.Trim().ToLowerInvariant()} {a.LastName.Trim().ToLowerInvariant()}"));

                if (book.Publisher != null)
                {
                    features.Add($"publisher:{book.Publisher.Name.Trim().ToLowerInvariant()}");
                }

                return new BookFeatureDto
                {
                    BookId = book.Id,
                    Features = features
                };
            }).ToList();
        }

        private List<string> BuildVocabulary(List<BookFeatureDto> featureBooks)
        {
            return featureBooks
                .SelectMany(b => b.Features)
                .Distinct()
                .OrderBy(f => f)
                .ToList();
        }

        private List<BookVectorDto> BuildBookVectors(
            List<BookFeatureDto> featureBooks,
            List<string> vocabulary)
        {
            var featureWeights = new Dictionary<string, double>
                {
                    { "subject", 5 },
                    { "category", 3 },
                    { "author", 2 },
                    { "publisher", 1 }
                };

            var vocabularyLookup = vocabulary
                .Select((feature, index) => new { feature, index })
                .ToDictionary(x => x.feature, x => x.index);

            return featureBooks.Select(book =>
            {
                var vector = new double[vocabulary.Count];

                foreach (var feature in book.Features)
                {
                    if (vocabularyLookup.TryGetValue(feature, out var index))
                    {
                        var featureType = feature.Split(':')[0];
                        vector[index] = featureWeights[featureType];
                    }
                }

                return new BookVectorDto
                {
                    BookId = book.BookId,
                    Vector = vector
                };
            }).ToList();
        }

        private double[] BuildUserVector(
            List<BookVectorDto> bookVectors,
            List<int> borrowedBooks,
            List<int> wishlistBooks)
        {
            var borrowedVectors = bookVectors
                .Where(v => borrowedBooks.Contains(v.BookId))
                .ToList();

            var wishlistVectors = bookVectors
                .Where(v => wishlistBooks.Contains(v.BookId))
                .ToList();

            int vectorSize = bookVectors.First().Vector.Length;
            double[] userVector = new double[vectorSize];

            foreach (var book in borrowedVectors)
            {
                for (int i = 0; i < vectorSize; i++)
                {
                    userVector[i] += book.Vector[i];
                }
            }

            foreach (var book in wishlistVectors)
            {
                for (int i = 0; i < vectorSize; i++)
                {
                    userVector[i] += book.Vector[i] * 0.7;
                }
            }

            int interactionCount = borrowedVectors.Count + wishlistVectors.Count;
            if (interactionCount > 0)
            {
                for (int i = 0; i < vectorSize; i++)
                {
                    userVector[i] /= interactionCount;
                }
            }

            return userVector;
        }

        private async Task<List<BookDto>> GetColdStartRecommendations(int userId)
        {
            var courseId = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.StudentDetail.Course.Id)
                .FirstOrDefaultAsync();

            if (courseId == 0)
                return new List<BookDto>();

            var courseSubjectIds = await _context.Subjects
                .Where(s => s.CourseId == courseId)
                .Select(s => s.Id)
                .ToListAsync();

            var recommendedBooks = await _context.Books
                .Include(b => b.Subjects)
                .Where(b => b.Subjects.Any(s => courseSubjectIds.Contains(s.Id)))
                .Select(book => new
                {
                    Book = book,
                    MatchCount = book.Subjects.Count(s => courseSubjectIds.Contains(s.Id))
                })
                .OrderByDescending(x => x.MatchCount)
                .Select(x => x.Book)
                .Take(10)
                .ToListAsync();

            return MapBooks(recommendedBooks);
        }

        private List<BookDto> MapBooks(List<Book> books)
        {
            return books.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Authors = b.Authors.Select(a => new AuthorDto
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName
                }).ToList(),
                ISBN = b.ISBN,
                Publisher = b.Publisher?.Name,
                SubjectIds = b.Subjects.Select(s => s.Id).ToList(),
            }).ToList();
        }

    }
}
