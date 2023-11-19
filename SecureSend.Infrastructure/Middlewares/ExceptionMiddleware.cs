using Microsoft.AspNetCore.Http;
using SecureSend.Domain.Exceptions;
using System.Text.Json;
using SecureSend.Domain.Entities;

namespace SecureSend.Infrastructure.Middlewares
{
    internal sealed class ExceptionMiddleware: IMiddleware
    {

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (SecureSendNotFoundException ex)
            {
                context.Response.StatusCode = 404;
                await WriteToResponse(context, ex);
            }
            catch (InvalidPasswordException ex)
            {
                context.Response.StatusCode = 401;
                await WriteToResponse(context, ex);
            }
            catch (SecureSendException ex)
            {
                context.Response.StatusCode = 400;
                await WriteToResponse(context, ex);

            }
            catch (Exception) 
            {
                context.Response.StatusCode = 500;
                context.Response.Headers.Add("content-type", "application/json");
                await context.Response.WriteAsync(JsonSerializer.Serialize(new {Message = "Something went wrong."}));

            }
        }

        private async Task WriteToResponse(HttpContext context, Exception ex)
        {
            context.Response.Headers.Add("content-type", "application/json");

            var errorCode = ToUnderscoreCase(ex.GetType().Name.Replace("Exception", string.Empty));
            var json = JsonSerializer.Serialize(new { ErrorCode = errorCode, ex.Message });
            await context.Response.WriteAsync(json);
        }

        public static string ToUnderscoreCase(string value)
            => string.Concat((value ?? string.Empty).Select((x, i) => i > 0 && char.IsUpper(x) && !char.IsUpper(value![i - 1]) ? $"_{x}" : x.ToString())).ToLower();
    }
  
}
