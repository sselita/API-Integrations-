
using System.Collections.Generic;
using System.Data.SqlClient;

namespace RestWebApi.DbFactory
{
    public interface ICustomDbFactory
    {
        List<T> PrepareMasterDataGroupObject<T>(SqlDataReader reader) where T : new();
     
    }
}