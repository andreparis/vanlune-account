using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Application.Application.PatchHandlers
{
    public static class JsonPatchOperationType
    {
        public const string ADD = "add";
        public const string REMOVE = "remove";
        public const string REPLACE = "replace";
        public const string MOVE = "move";
        public const string COPY = "copy";
        public const string TEST = "test";
    }
}
