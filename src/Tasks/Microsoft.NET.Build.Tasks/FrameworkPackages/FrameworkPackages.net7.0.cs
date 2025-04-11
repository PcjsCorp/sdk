namespace Microsoft.ComponentDetection.Detectors.NuGet;

using static global::NuGet.Frameworks.FrameworkConstants.CommonFrameworks;

/// <summary>
/// Framework packages for net7.0.
/// </summary>
internal partial class FrameworkPackages
{
    internal static class NETCoreApp70
    {
        internal static FrameworkPackages Instance { get; } = new(Net70, FrameworkNames.NetCoreApp, NETCoreApp60.Instance)
        {
            { "System.Collections.Immutable", "7.0.0" },
            { "System.Diagnostics.DiagnosticSource", "7.0.2" },
            { "System.Formats.Asn1", "7.0.0" },
            { "System.Net.Http.Json", "7.0.1" },
            { "System.Reflection.Metadata", "7.0.2" },
            { "System.Security.AccessControl", "6.0.1" },
            { "System.Text.Encoding.CodePages", "7.0.0" },
            { "System.Text.Encodings.Web", "7.0.0" },
            { "System.Text.Json", "7.0.4" },
            { "System.Threading.Channels", "7.0.0" },
            { "System.Threading.Tasks.Dataflow", "7.0.0" },
        };

        internal static FrameworkPackages AspNetCore { get; } = new(Net70, FrameworkNames.AspNetCoreApp, NETCoreApp60.AspNetCore)
        {
            { "Microsoft.AspNetCore", "7.0.0" },
            { "Microsoft.AspNetCore.Antiforgery", "7.0.0" },
            { "Microsoft.AspNetCore.Authentication", "7.0.0" },
            { "Microsoft.AspNetCore.Authentication.Abstractions", "7.0.0" },
            { "Microsoft.AspNetCore.Authentication.Cookies", "7.0.0" },
            { "Microsoft.AspNetCore.Authentication.Core", "7.0.0" },
            { "Microsoft.AspNetCore.Authentication.OAuth", "7.0.0" },
            { "Microsoft.AspNetCore.Authorization", "7.0.0" },
            { "Microsoft.AspNetCore.Authorization.Policy", "7.0.0" },
            { "Microsoft.AspNetCore.Components", "7.0.0" },
            { "Microsoft.AspNetCore.Components.Authorization", "7.0.0" },
            { "Microsoft.AspNetCore.Components.Forms", "7.0.0" },
            { "Microsoft.AspNetCore.Components.Server", "7.0.0" },
            { "Microsoft.AspNetCore.Components.Web", "7.0.0" },
            { "Microsoft.AspNetCore.Connections.Abstractions", "7.0.0" },
            { "Microsoft.AspNetCore.CookiePolicy", "7.0.0" },
            { "Microsoft.AspNetCore.Cors", "7.0.0" },
            { "Microsoft.AspNetCore.Cryptography.Internal", "7.0.0" },
            { "Microsoft.AspNetCore.Cryptography.KeyDerivation", "7.0.0" },
            { "Microsoft.AspNetCore.DataProtection", "7.0.0" },
            { "Microsoft.AspNetCore.DataProtection.Abstractions", "7.0.0" },
            { "Microsoft.AspNetCore.DataProtection.Extensions", "7.0.0" },
            { "Microsoft.AspNetCore.Diagnostics", "7.0.0" },
            { "Microsoft.AspNetCore.Diagnostics.Abstractions", "7.0.0" },
            { "Microsoft.AspNetCore.Diagnostics.HealthChecks", "7.0.0" },
            { "Microsoft.AspNetCore.HostFiltering", "7.0.0" },
            { "Microsoft.AspNetCore.Hosting", "7.0.0" },
            { "Microsoft.AspNetCore.Hosting.Abstractions", "7.0.0" },
            { "Microsoft.AspNetCore.Hosting.Server.Abstractions", "7.0.0" },
            { "Microsoft.AspNetCore.Html.Abstractions", "7.0.0" },
            { "Microsoft.AspNetCore.Http", "7.0.0" },
            { "Microsoft.AspNetCore.Http.Abstractions", "7.0.0" },
            { "Microsoft.AspNetCore.Http.Connections", "7.0.0" },
            { "Microsoft.AspNetCore.Http.Connections.Common", "7.0.0" },
            { "Microsoft.AspNetCore.Http.Extensions", "7.0.0" },
            { "Microsoft.AspNetCore.Http.Features", "7.0.0" },
            { "Microsoft.AspNetCore.Http.Results", "7.0.0" },
            { "Microsoft.AspNetCore.HttpLogging", "7.0.0" },
            { "Microsoft.AspNetCore.HttpOverrides", "7.0.0" },
            { "Microsoft.AspNetCore.HttpsPolicy", "7.0.0" },
            { "Microsoft.AspNetCore.Identity", "7.0.0" },
            { "Microsoft.AspNetCore.Localization", "7.0.0" },
            { "Microsoft.AspNetCore.Localization.Routing", "7.0.0" },
            { "Microsoft.AspNetCore.Metadata", "7.0.0" },
            { "Microsoft.AspNetCore.Mvc", "7.0.0" },
            { "Microsoft.AspNetCore.Mvc.Abstractions", "7.0.0" },
            { "Microsoft.AspNetCore.Mvc.ApiExplorer", "7.0.0" },
            { "Microsoft.AspNetCore.Mvc.Core", "7.0.0" },
            { "Microsoft.AspNetCore.Mvc.Cors", "7.0.0" },
            { "Microsoft.AspNetCore.Mvc.DataAnnotations", "7.0.0" },
            { "Microsoft.AspNetCore.Mvc.Formatters.Json", "7.0.0" },
            { "Microsoft.AspNetCore.Mvc.Formatters.Xml", "7.0.0" },
            { "Microsoft.AspNetCore.Mvc.Localization", "7.0.0" },
            { "Microsoft.AspNetCore.Mvc.Razor", "7.0.0" },
            { "Microsoft.AspNetCore.Mvc.RazorPages", "7.0.0" },
            { "Microsoft.AspNetCore.Mvc.TagHelpers", "7.0.0" },
            { "Microsoft.AspNetCore.Mvc.ViewFeatures", "7.0.0" },
            { "Microsoft.AspNetCore.OutputCaching", "7.0.0" },
            { "Microsoft.AspNetCore.RateLimiting", "7.0.0" },
            { "Microsoft.AspNetCore.Razor", "7.0.0" },
            { "Microsoft.AspNetCore.Razor.Runtime", "7.0.0" },
            { "Microsoft.AspNetCore.RequestDecompression", "7.0.0" },
            { "Microsoft.AspNetCore.ResponseCaching", "7.0.0" },
            { "Microsoft.AspNetCore.ResponseCaching.Abstractions", "7.0.0" },
            { "Microsoft.AspNetCore.ResponseCompression", "7.0.0" },
            { "Microsoft.AspNetCore.Rewrite", "7.0.0" },
            { "Microsoft.AspNetCore.Routing", "7.0.0" },
            { "Microsoft.AspNetCore.Routing.Abstractions", "7.0.0" },
            { "Microsoft.AspNetCore.Server.HttpSys", "7.0.0" },
            { "Microsoft.AspNetCore.Server.IIS", "7.0.0" },
            { "Microsoft.AspNetCore.Server.IISIntegration", "7.0.0" },
            { "Microsoft.AspNetCore.Server.Kestrel", "7.0.0" },
            { "Microsoft.AspNetCore.Server.Kestrel.Core", "7.0.0" },
            { "Microsoft.AspNetCore.Server.Kestrel.Transport.Quic", "7.0.0" },
            { "Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets", "7.0.0" },
            { "Microsoft.AspNetCore.Session", "7.0.0" },
            { "Microsoft.AspNetCore.SignalR", "7.0.0" },
            { "Microsoft.AspNetCore.SignalR.Common", "7.0.0" },
            { "Microsoft.AspNetCore.SignalR.Core", "7.0.0" },
            { "Microsoft.AspNetCore.SignalR.Protocols.Json", "7.0.0" },
            { "Microsoft.AspNetCore.StaticFiles", "7.0.0" },
            { "Microsoft.AspNetCore.WebSockets", "7.0.0" },
            { "Microsoft.AspNetCore.WebUtilities", "7.0.0" },
            { "Microsoft.Extensions.Caching.Abstractions", "7.0.0" },
            { "Microsoft.Extensions.Caching.Memory", "7.0.0" },
            { "Microsoft.Extensions.Configuration", "7.0.0" },
            { "Microsoft.Extensions.Configuration.Abstractions", "7.0.0" },
            { "Microsoft.Extensions.Configuration.Binder", "7.0.0" },
            { "Microsoft.Extensions.Configuration.CommandLine", "7.0.0" },
            { "Microsoft.Extensions.Configuration.EnvironmentVariables", "7.0.0" },
            { "Microsoft.Extensions.Configuration.FileExtensions", "7.0.0" },
            { "Microsoft.Extensions.Configuration.Ini", "7.0.0" },
            { "Microsoft.Extensions.Configuration.Json", "7.0.0" },
            { "Microsoft.Extensions.Configuration.KeyPerFile", "7.0.0" },
            { "Microsoft.Extensions.Configuration.UserSecrets", "7.0.0" },
            { "Microsoft.Extensions.Configuration.Xml", "7.0.0" },
            { "Microsoft.Extensions.DependencyInjection", "7.0.0" },
            { "Microsoft.Extensions.DependencyInjection.Abstractions", "7.0.0" },
            { "Microsoft.Extensions.Diagnostics.HealthChecks", "7.0.0" },
            { "Microsoft.Extensions.Diagnostics.HealthChecks.Abstractions", "7.0.0" },
            { "Microsoft.Extensions.Features", "7.0.0" },
            { "Microsoft.Extensions.FileProviders.Abstractions", "7.0.0" },
            { "Microsoft.Extensions.FileProviders.Composite", "7.0.0" },
            { "Microsoft.Extensions.FileProviders.Embedded", "7.0.0" },
            { "Microsoft.Extensions.FileProviders.Physical", "7.0.0" },
            { "Microsoft.Extensions.FileSystemGlobbing", "7.0.0" },
            { "Microsoft.Extensions.Hosting", "7.0.0" },
            { "Microsoft.Extensions.Hosting.Abstractions", "7.0.0" },
            { "Microsoft.Extensions.Http", "7.0.0" },
            { "Microsoft.Extensions.Identity.Core", "7.0.0" },
            { "Microsoft.Extensions.Identity.Stores", "7.0.0" },
            { "Microsoft.Extensions.Localization", "7.0.0" },
            { "Microsoft.Extensions.Localization.Abstractions", "7.0.0" },
            { "Microsoft.Extensions.Logging", "7.0.0" },
            { "Microsoft.Extensions.Logging.Abstractions", "7.0.0" },
            { "Microsoft.Extensions.Logging.Configuration", "7.0.0" },
            { "Microsoft.Extensions.Logging.Console", "7.0.0" },
            { "Microsoft.Extensions.Logging.Debug", "7.0.0" },
            { "Microsoft.Extensions.Logging.EventLog", "7.0.0" },
            { "Microsoft.Extensions.Logging.EventSource", "7.0.0" },
            { "Microsoft.Extensions.Logging.TraceSource", "7.0.0" },
            { "Microsoft.Extensions.ObjectPool", "7.0.0" },
            { "Microsoft.Extensions.Options", "7.0.0" },
            { "Microsoft.Extensions.Options.ConfigurationExtensions", "7.0.0" },
            { "Microsoft.Extensions.Options.DataAnnotations", "7.0.0" },
            { "Microsoft.Extensions.Primitives", "7.0.0" },
            { "Microsoft.Extensions.WebEncoders", "7.0.0" },
            { "Microsoft.JSInterop", "7.0.0" },
            { "Microsoft.Net.Http.Headers", "7.0.0" },
            { "System.Diagnostics.EventLog", "7.0.0" },
            { "System.IO.Pipelines", "7.0.0" },
            { "System.Security.Cryptography.Pkcs", "7.0.0" },
            { "System.Security.Cryptography.Xml", "7.0.0" },
            { "System.Threading.RateLimiting", "7.0.0" },
        };

        internal static FrameworkPackages WindowsDesktop { get; } = new(Net70, FrameworkNames.WindowsDesktopApp, NETCoreApp60.WindowsDesktop)
        {
            { "Microsoft.Win32.Registry.AccessControl", "7.0.0" },
            { "Microsoft.Win32.SystemEvents", "7.0.0" },
            { "System.CodeDom", "7.0.0" },
            { "System.Configuration.ConfigurationManager", "7.0.0" },
            { "System.Diagnostics.EventLog", "7.0.0" },
            { "System.Diagnostics.PerformanceCounter", "7.0.0" },
            { "System.Drawing.Common", "7.0.0" },
            { "System.IO.Packaging", "7.0.0" },
            { "System.Resources.Extensions", "7.0.0" },
            { "System.Security.Cryptography.Pkcs", "7.0.0" },
            { "System.Security.Cryptography.ProtectedData", "7.0.0" },
            { "System.Security.Cryptography.Xml", "7.0.0" },
            { "System.Security.Permissions", "7.0.0" },
            { "System.Threading.AccessControl", "7.0.0" },
            { "System.Windows.Extensions", "7.0.0" },
        };

        internal static void Register() => FrameworkPackages.Register(Instance, AspNetCore, WindowsDesktop);
    }
}
