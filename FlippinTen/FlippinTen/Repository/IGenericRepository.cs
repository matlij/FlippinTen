using System.Threading.Tasks;

namespace FlippinTen.Repository
{
    public interface IGenericRepository
    {
        Task<T> GetAsync<T>(string requestUri);
    }
}