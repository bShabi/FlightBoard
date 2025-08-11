using FlightBoard.Models.Enum;
using FlightBoard.Models.Exceptions;
using FlightBoard.Models.Response;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace FlightBoard.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api") == false)
            {
                await _next(context);
            }
            else
            {
                var originalResponseBodyStream = context.Response.Body;
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        context.Response.Body = memoryStream;
                        await _next.Invoke(context);

                        memoryStream.Seek(0, SeekOrigin.Begin);
                        var bodyAsText = await new StreamReader(memoryStream).ReadToEndAsync();
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        context.Response.Body = originalResponseBodyStream;

                        var jsonString = WrappSuccess(bodyAsText);
                        HandleResponseAsync(context, jsonString);
                    }
                }
                catch (Exception ex)
                {
                    context.Response.Body = originalResponseBodyStream;
                    var jsonString = WrappException(ex);
                    HandleResponseAsync(context, jsonString);
                }
            }
        }

        private string WrappException(Exception exception)
        {
            eError errorCode = eError.GENERAL_ERROR;

            if (exception is BaseException)
            {
                errorCode = ((BaseException)exception).ErrorCode;
            }

            var jsonObj = new
            {
                Header = new
                {
                    ReturnCode = errorCode,
                    ReturnCodeMessage = $"{exception.Message}"
                }
            };

            string res = JsonConvert.SerializeObject(jsonObj);
            Activity.Current?.SetTag("otel.status_code", "ERROR");
            Activity.Current?.SetTag("otel.status_description", "error = " + res);
            return res;
        }

        private string WrappSuccess(string payload)
        {
            dynamic body = JsonConvert.DeserializeObject(payload);

            var res = new BaseResponse
            {
                Header = new ResponseHeader
                {
                    ReturnCode = ((int)eError.SUCCESS).ToString(),
                    ReturnCodeMessage = eError.SUCCESS.ToString()
                },
                Body = body
            };
            return JsonConvert.SerializeObject(res);
        }

        private async void HandleResponseAsync(HttpContext context, string jsonString)
        {
            context.Response.StatusCode = context.Response.StatusCode;
            context.Response.ContentType = "application/json";
            context.Response.ContentLength = jsonString != null ? Encoding.UTF8.GetByteCount(jsonString) : 0;
            await context.Response.WriteAsync(jsonString);
        }
    }
}
