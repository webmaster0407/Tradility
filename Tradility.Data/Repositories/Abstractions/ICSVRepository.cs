using System.Threading.Tasks;

namespace Tradility.Data.Repositories.Abstractions
{
    public interface ICSVRepository : IRepository
    {
        public Task LoadAsync(string filePath);
    }
}
