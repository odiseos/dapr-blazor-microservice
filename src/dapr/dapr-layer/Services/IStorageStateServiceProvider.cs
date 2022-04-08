namespace FiveInLine.Dapr.Services
{
    public interface IStorageStateServiceProvider: IDisposable
    {
        IStorageStateService CreateStorageStateService(string storageStateName);
    }
}
