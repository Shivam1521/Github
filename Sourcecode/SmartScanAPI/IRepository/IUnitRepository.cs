using Entities;

namespace IRepository
{
    public interface IUnitRepository
    {
        Task<Response> CreateUpdateUnit(UnitMasterRequest request);
        Task<IEnumerable<UnitMasterResponse>> GetUnits(TableRequest request);
        Task<IEnumerable<DDL>> GetUnitsDDL();
    }
}
