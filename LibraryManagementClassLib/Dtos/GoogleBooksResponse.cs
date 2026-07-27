using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace LibraryManagementClassLib.Dtos
{
    public class GoogleBooksResponse
    {
        [JsonPropertyName("items")]
        public List<GoogleBookItem>? Items { get; set; }
    }

    public class GoogleBookItem
    {
        [JsonPropertyName("volumeInfo")]
        public VolumeInfo? VolumeInfo { get; set; }
    }

    public class VolumeInfo
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("authors")]
        public List<string>? Authors { get; set; }

        [JsonPropertyName("publisher")]
        public string? Publisher { get; set; }

        [JsonPropertyName("industryIdentifiers")]
        public List<IndustryIdentifier>? IndustryIdentifiers { get; set; }
    }

    public class IndustryIdentifier
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }
    }
}
