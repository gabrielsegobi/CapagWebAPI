namespace Domain.Contracts.Json
{
    public class JsonResponse<T>
    {
        public List<T> Data { get; set; } = new();
    }
}
