namespace Client.Services.IServices
{
    public interface ILocalStorageManager
    {
        Task<T> GetItem<T>(string key);
        Task SetItem<T>(string key, T value);
        Task RemoveItem(string key);
    }
}
