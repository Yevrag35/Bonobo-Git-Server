using MG.Extensions.Strings;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Yev.Bonobo.Database;
using Yev.Bonobo.Database.Entities;

namespace Yev.Bonobo.Middleware;

public sealed class DbUserMiddleware
{
    private readonly RequestDelegate _next;

    public DbUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        if (true == httpContext.User.Identity?.IsAuthenticated)
        {
            string? oid = httpContext.User.FindFirstValue("oid");
            if (Guid.TryParse(oid, out Guid userId))
            {
                var dbCtx = httpContext.RequestServices.GetRequiredService<GitDbContext>();
                var query = dbCtx.Users.AsNoTracking()
                    .Where(x => x.EntraIdObjectId == userId);

                var user = await query.FirstOrDefaultAsync(httpContext.RequestAborted).ConfigureAwait(false);
                if (user is null)
                {
                    user = new UserModel
                    {
                        EntraIdObjectId = userId,
                        UserName = httpContext.User.FindFirstValue("preferred_username") ?? oid,
                    };
                    dbCtx.Users.Add(user);
                    int changes = await dbCtx.SaveChangesAsync(httpContext.RequestAborted).ConfigureAwait(false);
                    Debug.Assert(changes > 0);
                }

                httpContext.Items.Add(nameof(UserModel), user);
            }
        }

        await _next(httpContext).ConfigureAwait(false);
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
/// <summary>
///	Adds the DbUserMiddleware middleware class to the HTTP request pipeline.
/// </summary>
public static class DbUserMiddlewareExtensions
{
    public static IApplicationBuilder UseDbUserMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<DbUserMiddleware>();
    }
}