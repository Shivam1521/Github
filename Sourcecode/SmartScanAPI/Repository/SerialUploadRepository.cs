using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class SerialUploadRepository(DapperContext context) : ISerialUploadRepository
    {
        public async Task<ResponseSerial> CreateUploadSerial(SrUpload request)
        {
            try
            {
                using var connection = context.CreateConnection();
                var result = await connection.QueryFirstAsync<ResponseSerial>(
                    "Create_SerialUpload",
                    new
                    {
                        request.ItemCode,
                        request.QtyToBeDispatched,
                        request.CreatedBy,
                        request.Type,
                        request.LineNumber
                    },
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
            catch (Exception ex)
            {
                return new ResponseSerial
                {
                    ResponseMessage = $"An error occurred: {ex.Message}"
                };
            }
        }
    }
}
