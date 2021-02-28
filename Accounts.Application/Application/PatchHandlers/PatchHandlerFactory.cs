using MediatR;
using System.Collections.Generic;

namespace Accounts.Application.Application.PatchHandlers
{
    public static class PatchHandlerFactory
    {
        public static IPatchHandler GetInstance(string path, IMediator mediator)
        {
            var patchHandlers = new Dictionary<string, IPatchHandler>() { };

            if (!patchHandlers.ContainsKey(path))
                throw new System.Exception($"PatchHandler for path {path} not found");

            return patchHandlers[path];
        }
    }
}
