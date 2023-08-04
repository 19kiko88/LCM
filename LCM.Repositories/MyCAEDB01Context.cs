using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace LCM.Repositories
{
    /// <summary>
    /// 使用partial重新包裝bulkInsert，避免更新EF Model時把Context的內容清掉
    /// </summary>
    public partial class CAEDB01Context
    {

        public CAEDB01Context()
        {
        }

        /// <summary>
        /// 包裝BulkInsert。
        /// 原本的BulkInsert是Static Method，無法被Mock。故重新包裝為virtual Method，才能進行Mock
        public virtual void BulkInsert<T>(IList<T> entities, BulkConfig bulkConfig = null, Action<decimal> progress = null, Type? type = null) where T : class
        {
            //執行真正的BulkInsert
            ((DbContext)this).BulkInsert<T>(entities, bulkConfig, progress, type);
        }
    }
}
