using System.Diagnostics;

namespace Yev.Bonobo.Startup;

internal static class AppStartup
{
    internal static WebApplicationBuilder CreateWebBuilder(string[] arguments)
    {
        Span<string> argSpan = arguments.AsSpan();
        ParsedEnvironment environment = EnvironmentExtensions.UseEnvironmentFromArgs(ref argSpan);

        if (argSpan.Length != arguments.Length)
        {
            arguments = argSpan.ToArray();
        }

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = arguments,
            ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
            EnvironmentName = environment.EnvironmentName,
        });

        builder.Configuration.AddEnvironmentVariables()
                             .AddJsonFile(environment.JsonName);

        Environment.SetEnvironmentVariable("AppEnvName", environment.EnvironmentName);

        return builder;
    }
}

internal static class EnvironmentExtensions
{
    private const string STARTING = "--environment=";

    /// <inheritdoc cref="HostingHostBuilderExtensions.UseEnvironment(IHostBuilder, string)"/>
    public static ParsedEnvironment UseEnvironmentFromArgs(ref Span<string> arguments)
    {
        string envName = GetEnvironmentName();
        int index = 0;

        ParsedEnvironment? pev = null;
        foreach (ReadOnlySpan<char> line in arguments)
        {
            if (line.IsWhiteSpace() || !line.StartsWith(STARTING, StringComparison.OrdinalIgnoreCase))
            {
                index++;
                continue;
            }
            else if (STARTING.Length == line.Length)
            {
                Debug.Fail("No environment name provided.");
                index++;
                continue;
            }

            ReadOnlySpan<char> parsedEnv = line.Slice(STARTING.Length).Trim();
            CheckEnvironmentArgument(parsedEnv);
            ParsedNameEqualsSetValue(parsedEnv, envName);

            pev = new(parsedEnv, envName);
        }

        if (index < arguments.Length)
        {
            arguments = CutOutEnvironment(arguments, index);
        }

        return pev ?? new ParsedEnvironment(envName, envName);
    }

    [Conditional("DEBUG")]
    private static void CheckEnvironmentArgument(ReadOnlySpan<char> envName)
    {
        StringComparison c = StringComparison.OrdinalIgnoreCase;
        if (envName.Equals(Environments.Development, c))
        {
            return;
        }

        if (envName.Equals(Environments.Production, c))
        {
            return;
        }

        if (envName.Equals(Environments.Staging, c))
        {
            return;
        }

        Debug.Fail("Invalid environment name provided.");
    }
    private static Span<string> CutOutEnvironment(Span<string> arguments, int index)
    {
        Span<string> argsAtIndex = arguments.Slice(index);
        for (int i = 0; i < argsAtIndex.Length; i++)
        {
            argsAtIndex[i] = i < argsAtIndex.Length - 1
                ? argsAtIndex[i + 1]
                : null!;
        }

        return arguments.Slice(0, arguments.Length - 1);
    }
    private static string GetEnvironmentName()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", EnvironmentVariableTarget.Process)
            ?? Environments.Production;
    }
    [Conditional("DEBUG")]
    private static void ParsedNameEqualsSetValue(ReadOnlySpan<char> parsed, ReadOnlySpan<char> environmentName)
    {
        Debug.Assert(environmentName.Equals(parsed, StringComparison.OrdinalIgnoreCase),
            "The parsed argument line and the environment variable do not line up.");
    }
}