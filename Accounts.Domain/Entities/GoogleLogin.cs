using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Domain.Entities
{
    public class GoogleLogin
    {
        public string AccessToken { get; set; }
        public ProfileObj ProfileObj { get; set; }
    }

    public class ProfileObj
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class GoogleResponse
    {
        [JsonProperty("access_type")]
        public string AccessType { get; set; }
        public string Email { get; set; }
        [JsonProperty("verified_email")]
        public bool VerifiedEmail { get; set; }
    }
}
