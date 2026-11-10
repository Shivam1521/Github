using Entities;

namespace IRepository
{
    public interface IPickSlipRepository
    {
        Task<IEnumerable<PickSlipDetails>> GetPickSlipDetails(string pickslipNumber);
        Task<PickSlip> GetPickSlipDetailsForPickup(string pickslipNumber);
        Task<Received> GetDetailsForReceving(string documentNumber);
        Task<PickupQRDetailsResponse> VerifyQRForPickslip(PickupQRDetailsRequestDB request);
        Task<Response> InsertPickSlipDetails(InsertPickSlipRequest request);
        Task<IEnumerable<PickupQRDetails>> GetTempPickslipQRs(string pickslipNumber);

        Task<IEnumerable<PickupQRDetails>> GetTempRecivedQRs(string pickslipNumber);
        Task<IEnumerable<PickSlipReportDetails>> GetPickedslipsDetails(TableRequest request);
        Task<IEnumerable<PickSlipReceiptStatus>> GetPickSlipReceiptStatus(string pickslipNumber);
        Task<DemapPickSlipDetailResponse> DemapPickSlipDetail(DemapPickSlipDetailRequestDB request);

        Task<IEnumerable<PendingPickslipResponse>> GetPendingPickslipsForSubmit(int userId);
        Task<IEnumerable<BulkSubmitPickSlipResult>> BulkSubmitPickSlips(BulkSubmitPickSlipsRequestDB request);
    }
}
