using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Domain.Entities
{
    public class Entity
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
