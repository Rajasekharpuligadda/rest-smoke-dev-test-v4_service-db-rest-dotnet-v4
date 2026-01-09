using System.Net;
using System.Text.Json;
using ServiceDbRestDotnetV4.Entities;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ServiceDbRestDotnetV4.Core.Exceptions
{
    public class ExceptionFilter: IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "application/json";

            var error = new ErrorEntity
            {
                Message = $"{context.Exception.InnerException?.Message}{context.Exception.Message}"
            };

            if (context.Exception is BusinessException exception)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;

            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            response.WriteAsync(JsonSerializer.Serialize(error, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }
    }
}
