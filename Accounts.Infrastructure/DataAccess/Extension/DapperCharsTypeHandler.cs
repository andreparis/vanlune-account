using Accounts.Domain.Entities;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Accounts.Infrastructure.DataAccess.Extension
{
    public class DapperCharsTypeHandler : SqlMapper.TypeHandler<IEnumerable<Character>>
    {
        public override void SetValue(IDbDataParameter parameter, IEnumerable<Character> value)
        {
            parameter.Value = value.ToString();
        }

        public override IEnumerable<Character> Parse(object value)
        {
            return JsonConvert.DeserializeObject<IEnumerable<Character>>((string)value);
        }
    }
}
