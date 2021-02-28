using Accounts.Domain.Entities;
using Accounts.Domain.Validation;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Accounts.Application.MediatR.Base
{
    public abstract class AbstractRequestHandler<T> : IRequestHandler<T, Response> where T : IRequest<Response>
    {
        internal abstract HandleResponse HandleIt(T request, CancellationToken cancellationToken);

        public Task<Response> Handle(T request, CancellationToken cancellationToken)
        {
            Response response = new Response();

            if (EqualityComparer<T>.Default.Equals(request, default(T)))
                return Task.FromResult(response);

            try
            {
                var result = HandleIt(request, cancellationToken);

                if (result != null && result?.Error == null)
                {
                    response.Content = result.Content;
                    response.Id = result.ContentIdentification;
                }
                else if (result != null && result?.Error != null)
                {
                    response.Error = result.Error;
                }
            }
            catch (DomainValidationException ex)
            {
                var validationProblems = string.Join("/n", ex.ValidationErrors.Select(e => e.Message));
                var logMsg = $"Domain validation errors occurred.\n" +
                             $"Handler: {typeof(T).Name}Handler\n" +
                             $"Request Object: {JsonConvert.SerializeObject(request, Formatting.Indented)}\n" +
                             $"Validation Problems: {validationProblems}";


                response.DomainValidationMessages = ex.ValidationErrors;
            }
            catch (Exception ex)
            {
                var logMsg = $"Error in handler.\n" +
                             $"Handler: {typeof(T).Name}Handler\n" +
                             $"Request Object: {JsonConvert.SerializeObject(request, Formatting.Indented)}\n" +
                             $"Exception: {GetFullMessage(ex)}";

                throw ex;
            }

            return Task.FromResult(response);
        }

        public string GetFullMessage(Exception ex)
        {
            return ex.InnerException == null
                 ? ex.Message
                 : ex.Message + " --> " + GetFullMessage(ex.InnerException);
        }
    }
}
