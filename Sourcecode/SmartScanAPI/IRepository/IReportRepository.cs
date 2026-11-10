using Entities;

namespace IRepository
{
    public interface IReportRepository
    {
        Task<IEnumerable<SerialReport>> GetSerialUploadReport(TableRequest request);
        Task<IEnumerable<PrintJobReport>> GetPrintJobReport(string Srno);
    }
}
