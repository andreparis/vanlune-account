using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Domain.Entities
{
    public class FacebookLogin
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
        [JsonProperty("data_access_expiration_time")]
        public string DataAccessExpTime { get; set; }
        public int ExpiresIn { get; set; }
    }

    public class FacebookResponse
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public dynamic Error { get; set; }
    }
}
