using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddelware
    {
        private readonly IHostEnvironment _env;
        
        private readonly ILogger<ExceptionMiddelware> _logger;
        private readonly RequestDelegate _next;
        public ExceptionMiddelware(RequestDelegate next,ILogger<ExceptionMiddelware> logger,
        IHostEnvironment env)
        {
            _env = env;
            _next = next;
            _logger = logger;
            
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }

            catch(Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                context.Response.ContentType="appliaction/json";
                context.Response.StatusCode=(int) HttpStatusCode.InternalServerError;

                var response=_env.IsDevelopment()
                 ? new ApiException(context.Response.StatusCode,ex.Message,ex.StackTrace?.ToString())
                 :new ApiException(context.Response.StatusCode,"Internal server error");

                var options=new JsonSerializerOptions{PropertyNamingPolicy=JsonNamingPolicy.CamelCase};
                var json=JsonSerializer.Serialize(response,options);
                await context.Response.WriteAsync(json);

            }
        }
    }
}