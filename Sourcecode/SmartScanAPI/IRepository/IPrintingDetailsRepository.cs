using Entities;

namespace IRepository
{
    public interface IPrintingDetailsRepository
    {
        Task<bool> SavePrintingDetailsAsync(List<PrintingDetailsRequest> printingDetailsRequest);
        Task<int> SavePrintedData(SavePrintedDataRequest request);
        Task<IEnumerable<PrintingDetailsResponse>> GetPrintingDetails();
        Task<int> SaveMasterPrinting(MasterPrintingRequest request);
        Task<IEnumerable<PrintList>> GetPrintList(string type, int createdBy);
    }
}
