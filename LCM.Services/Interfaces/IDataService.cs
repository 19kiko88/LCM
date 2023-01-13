using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace LCM.Services.Interfaces
{
    public interface IDataService
    {
        public Task<int> InsertS18(DataTable dt, string updateUser);
        public Task<int> InsertB18(DataTable dt, string updateUser);
        public Task ExportPkBs18(DataTable dt);        
    }
}
