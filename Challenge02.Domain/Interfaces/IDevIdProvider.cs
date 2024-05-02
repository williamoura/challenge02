namespace Challenge02.Domain.Interfaces
{
    public interface IDevIdProvider
    {
        Task<int> GetNextIdAsync();
    }
}