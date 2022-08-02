using System.Globalization;
using Tradility.Data.Repositories.Abstractions;

namespace Tradility.Forms
{
    public interface IRepositoryViewModel
    {
        public IRepository Repository { get; }
        public IColumns Columns { get; }
        public bool WriteToCSV(string filename, CultureInfo cultureInfo);
    }
}
