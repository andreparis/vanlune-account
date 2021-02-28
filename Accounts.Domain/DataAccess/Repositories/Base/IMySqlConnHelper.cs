using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Accounts.Domain.DataAccess.Repositories
{
    public interface IMySqlConnHelper
    {
        DbConnection MySqlConnection();
    }
}
