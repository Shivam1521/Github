using IRepository;
using Repository.Data;

namespace Repository
{
    public class WrapperRepository(DapperContext context) : IWrapperRepository
    {
        private IUserRepository? _user;
        public IUserRepository User => _user ??= new UserRepository(context);

        private IPageRepository? _page;
        public IPageRepository Page => _page ??= new PageRepository(context);

        private IRoleRepository? _role;
        public IRoleRepository Role => _role ??= new RoleRepository(context);

        private IListRepository? _list;
        public IListRepository List => _list ??= new ListRepository(context);

        private IPrintingDetailsRepository? _print;
        public IPrintingDetailsRepository PrintingDetails => _print ??= new PrintingDetailsRepository(context);

        private IUnitRepository? _unit;
        public IUnitRepository Unit => _unit ??= new UnitRepository(context);

        private IUtilsRepository? _utils;
        public IUtilsRepository Utils => _utils ??= new UtilsRepository(context);

        private IPlantWarehouseRepository? _plantWarehouse;
        public IPlantWarehouseRepository PlantWarehouse => _plantWarehouse ??= new PlantWarehouseRepository(context);

        private IERPRepository? _erp;
        public IERPRepository ERP => _erp ??= new ERPRepository(context);

        private ISerialUploadRepository? _srupload;
        public ISerialUploadRepository SrUpload => _srupload ??= new SerialUploadRepository(context);

        public IPrintJobRepository? _printJob;
        public IPrintJobRepository PrintJob => _printJob ??= new PrintJobRepository(context);

        private IReportRepository? _allReport;
        public IReportRepository AllReport => _allReport ??= new ReportRepository(context);
        private IItemRepository? _item;
        public IItemRepository Item => _item ??= new ItemRepository(context);

        private IScratchRepository? _scratch;
        public IScratchRepository Scratch => _scratch ??= new ScratchRepository(context);

        private IQRAuthenticationRepository? _qrAuthentication;
        public IQRAuthenticationRepository QRAuthentication => _qrAuthentication ??= new QRAuthenticationRepository(context);

        private IPreprintedQRJobRepository? _preprintedQRJob;
        public IPreprintedQRJobRepository PreprintedQRJob => _preprintedQRJob ??= new PreprintedQRJobRepository(context);

        public IPeripheralDevicesRepository? _peripheralDevices;
        public IPeripheralDevicesRepository PeripheralDevices => _peripheralDevices ??= new PeripheralDevicesRepository(context);

        private IPickSlipRepository? _pickSlip;
        public IPickSlipRepository PickSlip => _pickSlip ??= new PickSlipRepository(context);

        private ICartonLedgerRepository? _cartonLedger;
        public ICartonLedgerRepository CartonLedger => _cartonLedger ??= new CartonLedgerRepository(context);

        private IQCRepository? _qc;
        public IQCRepository QC => _qc ??= new QCRepository(context);

        private ITrackingRepository? _tracking;
        public ITrackingRepository Tracking => _tracking ??= new TrackingRepository(context);

        private IDashboardRepository? _dashboard;
        public IDashboardRepository Dashboard => _dashboard ??= new DashboardRepository(context);

        private IRefreshTokenRepository? _refreshToken;
        public IRefreshTokenRepository RefreshToken => _refreshToken ??= new RefreshTokenRepository(context);

        private IQRRespository? _qr;
        public IQRRespository QR => _qr ??= new QRRespository(context);
    }
}
