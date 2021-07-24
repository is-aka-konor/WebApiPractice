using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Exceptions;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Api.ValidationFlow.Interfaces;

namespace WebApiPractice.Api.ValidationFlow
{
    /// <summary>
    /// Pipeline behavior for validation contract
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ContractValidationPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IValidationContract where TResponse : new()
    {
        private readonly IEnumerable<IValidationContractHandler> _validationContractHandlers;

        public ContractValidationPipeline(IEnumerable<IValidationContractHandler> validationContractHandlers)
        {
            _validationContractHandlers = validationContractHandlers;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            //1. Get all the IValidationContract interfaces implemented by the request.
            var requestValidationContracts = request
                .GetType()
                .GetInterfaces()
                .Where(m => m.GetInterfaces().Any(s => s == typeof(IValidationContract)))
                .Select(m => m)
                .ToArray();

            // filter the _validationContractHandlers with the validation handlers implemented by the request.
            // order the handlersToRun in the same sequence as they are implemented in the request.
            var handlersToRun = new List<IValidationContractHandler>();
            var validationFailures = new List<ErrorMessage>();

            foreach (var validationContract in requestValidationContracts)
            {
                // find the handler for the request contract.
                var handler = _validationContractHandlers.SingleOrDefault(
                        m => m.GetValidationContractType() == validationContract);

                // if our request has implemented a contract but there is no validationContractHandler found in DI, throw exception
                if (handler == null)
                {
                    var msg = $"Cannot find ValidationContractHandler for contract: {validationContract} and request: {typeof(TRequest)}";
                    throw new Exception(msg);
                }

                handlersToRun.Add(handler);
            }

            // run the contract validation handlers 1 by 1.
            foreach (var handler in handlersToRun)
            {
                var response = await handler.Handle(request, cancellationToken).ConfigureAwait(false);

                // exist if the handler should be aborted on failure and response has errors.
                if (handler.AbortOnFailure() && response.Count > 0)
                    throw new RequestExecutionException(response);

                validationFailures.AddRange(response);
            }

            if (validationFailures.Count > 0)
                throw new RequestExecutionException(validationFailures);

            return await next().ConfigureAwait(false);
        }
    }
}
