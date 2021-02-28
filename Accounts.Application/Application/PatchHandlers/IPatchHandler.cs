using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Application.Application.PatchHandlers
{
    public interface IPatchHandler
    {
        void Handle(IEnumerable<JsonPatch> patches, long parentId);
    }
}
