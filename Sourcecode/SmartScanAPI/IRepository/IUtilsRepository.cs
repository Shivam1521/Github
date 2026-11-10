using Entities;

namespace IRepository
{
    public interface IUtilsRepository
    {
        public Task<PincodeDetails> GetPincodeDetails(string pinCode);
    }
}
