using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace INVEST.Web.Middlewares
{
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);

            var accept = context.Request.Headers.Accept.ToString();
            var wantsJson = accept.Contains("application/json", StringComparison.OrdinalIgnoreCase);

            if (wantsJson)
            {
                var problem = new ProblemDetails
                {
                    Title = "An unexpected error occurred.",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment()
                        ? exception.Message
                        : null,
                    Extensions =
                    {
                        ["traceId"] = context.TraceIdentifier
                    }
                };

                context.Response.StatusCode = problem.Status.Value;
                await context.Response.WriteAsJsonAsync(problem, cancellationToken);
                return true;
            }

            context.Response.Redirect($"/Error?traceId={context.TraceIdentifier}");
            return true;
        }
    }
}