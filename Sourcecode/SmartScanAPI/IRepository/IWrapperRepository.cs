namespace IRepository
{
    public interface IWrapperRepository
    {
        IUserRepository User { get; }
        IPageRepository Page { get; }
        IRoleRepository Role { get; }
        IListRepository List { get; }
        IPrintingDetailsRepository PrintingDetails { get; }
        IUnitRepository Unit { get; }
        IUtilsRepository Utils { get; }
        IPlantWarehouseRepository PlantWarehouse { get; }
        IERPRepository ERP { get; }
        ISerialUploadRepository SrUpload { get; }
        IPrintJobRepository PrintJob { get; }
        IReportRepository AllReport { get; }
        IItemRepository Item { get; }
        IScratchRepository Scratch { get; }
        IQRAuthenticationRepository QRAuthentication { get; }
        IPreprintedQRJobRepository PreprintedQRJob { get; }
        IPeripheralDevicesRepository PeripheralDevices { get; }
        IPickSlipRepository PickSlip { get; }
        ICartonLedgerRepository CartonLedger { get; }
        IQCRepository QC { get; }
        ITrackingRepository Tracking { get; }
        IDashboardRepository Dashboard { get; }
        IRefreshTokenRepository RefreshToken { get; }
        IQRRespository QR { get; }
    }
}
