namespace Gdac.Onboarding.Api.Middleware;

public class SecurityHeadersMiddleware(RequestDelegate next)
{
    private const string SwaggerCsp =
        "default-src 'self'; script-src 'self' 'unsafe-inline'; " +
        "style-src 'self' 'unsafe-inline'; img-src 'self' data:; connect-src 'self'";

    private const string ApiCsp = "default-src 'none'";

    public async Task InvokeAsync(HttpContext context)
    {
        var isSwagger = context.Request.Path.StartsWithSegments("/swagger");

        context.Response.Headers["X-Frame-Options"]           = "DENY";
        context.Response.Headers["X-Content-Type-Options"]    = "nosniff";
        context.Response.Headers["Referrer-Policy"]           = "strict-origin-when-cross-origin";
        context.Response.Headers["Content-Security-Policy"]   = isSwagger ? SwaggerCsp : ApiCsp;
        context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";

        await next(context);
    }
}
