using Tradility.Data.Models;
using Tradility.Data.Repositories;

namespace Tradility.Forms.CashReports
{
    public class CashReportsViewModel : BaseRepositoryViewModel<CashReportModel>
    {
        public CashReportColumns CashReportColumns { get; set; }

        public CashReportsViewModel(CashReportRepository repository) : base(repository, new CashReportColumns())
        {
            CashReportColumns = Columns as CashReportColumns;
        }
    }
}
