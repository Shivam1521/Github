using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class UtilsRepository(DapperContext context) : IUtilsRepository
    {
        public async Task<PincodeDetails> GetPincodeDetails(string pinCode)
        {
            using var connection = context.CreateConnection();
            var details = await connection.QueryFirstAsync<PincodeDetails>("GetPincodeDetails", new
            {
                pinCode
            }, commandType: CommandType.StoredProcedure);
            return details;
        }
    }
}
