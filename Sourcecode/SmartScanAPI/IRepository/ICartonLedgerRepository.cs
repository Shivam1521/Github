using Entities;
using System.Data;

namespace IRepository
{
    public interface ICartonLedgerRepository
    {
        Task<ReceiveCartonResponse> UpdateReceiveStatus(ReceiveCarton request);
        Task<Response> SubmitReceiveCartons(string pickSlipNumber);
        Task<Response> UpdateDispatchStatus(DispatchCarton request);
        Task<IEnumerable<ReceiveCartonResult>> UpdateReceiveStatusBulk(DataTable table, int performedBy);

        Task<List<DispatchCartonResult>> DispatchFullCartons(List<DispatchCartonTVP> cartons, DispatchCartonsRequest request, int performedBy);

        Task<List<DispatchCartonResult>> Dispatchverify(List<DispatchCartonTVP> cartons, DispatchCartonsRequest request, int performedBy);

        Task<Response> SubmitDispatchFullCartons(string pickSlipNumber,int performedBy);
    }
}
