using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Implementation
{
    public class GoogleBooksService : IGoogleBooksService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GoogleBooksService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<BookDto?> GetBookByIsbnAsync(string isbn)
        {
            var apiKey = _configuration["GoogleBooks:ApiKey"];

            var url =
                $"https://www.googleapis.com/books/v1/volumes" +
                $"?q=isbn:{isbn}" +
                $"&key={apiKey}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var googleResponse = JsonSerializer.Deserialize<GoogleBooksResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            var book = googleResponse?.Items?.FirstOrDefault()?.VolumeInfo;

            if (book == null)
                return null;

            return new BookDto
            {
                Title = book.Title,
                ISBN = book.IndustryIdentifiers?
                    .FirstOrDefault(i => i.Type == "ISBN_13")?.Identifier
                    ?? isbn,
                Publisher = book.Publisher,
                PublisherAddress = null,
                Authors = book.Authors?
                    .Select(author =>
                    {
                        var parts = author.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        return new AuthorDto
                        {
                            FirstName = parts.FirstOrDefault() ?? "",
                            LastName = parts.Length > 1
                                ? string.Join(" ", parts.Skip(1))
                                : ""
                        };
                    })
                    .ToList() ?? new List<AuthorDto>()
            };
        }
    }
}
