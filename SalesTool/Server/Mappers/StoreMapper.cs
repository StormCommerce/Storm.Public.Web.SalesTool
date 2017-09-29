using Enferno.Public.Web.SalesTool.Server.Models;
using Enferno.StormApiClient.Applications;

namespace Enferno.Public.Web.SalesTool.Server.Mappers
{
    public static class StoreMapper
    {
        public static StoreModel MapToStoreModel(Store store)
        {
            if (store == null)
                return null;
            return new StoreModel
            {
                Id = store.Id,
                Code = store.Code,
                Name = store.Name,
                Phone = store.Phone,
                Address = new AddressModel
                {
                    Address1 = store.Address
                }
            };
        }
    }
}