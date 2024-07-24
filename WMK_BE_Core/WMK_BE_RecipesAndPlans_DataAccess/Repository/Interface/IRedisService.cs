using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement
{
    public interface IRedisService
    {
         Task<bool> SetValueAsync<T>(string key, T value,TimeSpan timeOut);
    Task<T> GetValueAsync<T>(string key);
        //clear cache
        Task<bool> RemoveAsync(string key);

    }
}
