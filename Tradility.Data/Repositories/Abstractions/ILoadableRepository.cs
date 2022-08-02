using System.Threading.Tasks;

namespace Tradility.Data.Repositories.Abstractions
{
    public interface ILoadableRepository : IRepository
    {
        Task LoadAsync();
    }
}
