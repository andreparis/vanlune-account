using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Application.Application.PatchHandlers
{
    public class JsonPatch
    {
        public string Op { get; set; }
        public string Path { get; set; }
        public string[] Value { get; set; }
    }
}
