using System.Text.Json.Serialization;

namespace Domain.Contracts.Responses
{
    public class PageData
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    public class PagedApiResponse<T>
    {
        [JsonPropertyName("paging")]
        public PageData Paging { get; set; }

        [JsonPropertyName("data")]
        public IEnumerable<T> Data { get; set; }

        public PagedApiResponse(PageData paging, IEnumerable<T> data)
        {
            Paging = paging;
            Data = data;
        }
    }
}
