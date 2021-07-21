using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApiPractice.Api.Exceptions;
using WebApiPractice.Api.ResponseStructure;

namespace WebApiPractice.Api.Middlewares
{
    /// <summary>
    /// Middle ware to handle exceptions. Log exceptions and return appropriate response code.
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentNullException)
            {
                //This is similar to the validation error but this time the request is null nothing is provided in the request.
                var response = JsonConvert.SerializeObject(new
                {
                    errorCode = ErrorCode.InvalidRequest.Code,
                    message = ErrorCode.InvalidRequest.Message
                });
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(response);
            }
            catch (RequestExecutionException ex)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                // extract the response message from exception.
                var responseMessage = ex.ResponseMessage;
                var errors = responseMessage.Messages.Select(m => new { field = m.Property, message = m.Description });
                var response = JsonConvert.SerializeObject(
                    new
                    {
                        errorCode = responseMessage.Code,
                        message = ErrorCode.Validation.Message,
                        errors
                    });
                await context.Response.WriteAsync(response);
            }
            catch (ResourceNotFoundException ex)
            {
                context.Response.StatusCode = 404;
                context.Response.ContentType = "application/json";
                var response = JsonConvert.SerializeObject(
                    new
                    {
                        errorCode = ErrorCode.ResourceNotFound.Code,
                        message = ex.Message
                    });
                await context.Response.WriteAsync(response);
            }
            catch (ResourceConflictException ex)
            {
                context.Response.StatusCode = 409;
                context.Response.ContentType = "application/json";
                var response = JsonConvert.SerializeObject(
                    new
                    {
                        errorCode = ErrorCode.ResourceConflict.Code,
                        message = ex.Message
                    });
                await context.Response.WriteAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message);
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                var response = JsonConvert.SerializeObject(
                    new
                    {
                        errorCode = ErrorCode.InternalError.Code,
                        message = ErrorCode.InternalError.Message
                    });
                await context.Response.WriteAsync(response);
            }
        }
    }
}
