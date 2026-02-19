namespace Domain.Interfaces
{
    public interface IObjectStorage
    {
        Task SaveAsync(string folderKey, Stream data, string fileName);
        Task<Stream> OpenReadAsync(string directoryPath, string fileName);
        Task DeleteAsync(string key);
        bool Exists(string key);
    }
}
