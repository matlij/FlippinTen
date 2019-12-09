using System.Threading.Tasks;

namespace FlippinTen.Core.Interfaces
{
    public interface IGenericRepository
    {
        Task<T> GetAsync<T>(string requestUri);
        Task<T> PostAsync<T>(string requestUri, T body);
    }
}