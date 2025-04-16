// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CommandLine;
using System.CommandLine.Completions;
using Microsoft.Extensions.EnvironmentAbstractions;
using NuGet.Versioning;
using Microsoft.DotNet.Cli.Extensions;
using System.CommandLine.Parsing;
using NuGet.Packaging.Core;

namespace Microsoft.DotNet.Cli.Commands.Package.Add;

internal static class PackageAddCommandParser
{
    public static PackageIdentity ParsePackageIdentity(ArgumentResult packageArgResult)
    {
        // per the Arity of the CmdPackageArgument's Arity, we should have exactly one token - 
        // this is a safety net for if we change the Arity in the future and forget to update this parser.
        if (packageArgResult.Tokens.Count != 1)
        {
            throw new ArgumentException($"Expected exactly one token, but got {packageArgResult.Tokens.Count}.");
        }
        var token = packageArgResult.Tokens[0].Value;
        var indexOfAt = token.IndexOf('@');
        if (indexOfAt == -1)
        {
            // no version specified, so we just return the package id
            return new PackageIdentity(token, null);
        }
        // we have a version specified, so we need to split the token into id and version
        else
        {
            var id = token[0..indexOfAt];
            var versionString = token[(indexOfAt + 1)..];
            if (SemanticVersion.TryParse(versionString, out var version))
            {
                return new PackageIdentity(id, new NuGetVersion(version.Major, version.Minor, version.Patch, version.ReleaseLabels, version.Metadata));
            }
            else
            {
                throw new ArgumentException(string.Format(CliCommandStrings.InvalidSemVerVersionString, versionString));
            }
        };
    }

    public static readonly Argument<PackageIdentity> CmdPackageArgument = new DynamicArgument<PackageIdentity>(CliCommandStrings.CmdPackage)
    {
        Description = CliCommandStrings.CmdPackageDescription,
        Arity = ArgumentArity.ExactlyOne,
        CustomParser = ParsePackageIdentity,

    }.AddCompletions((context) =>
    {
        // we should take --prerelease flags into account for version completion
        var allowPrerelease = context.ParseResult.GetValue(PrereleaseOption);
        return QueryNuGet(context.WordToComplete, allowPrerelease, CancellationToken.None).Result.Select(packageId => new CompletionItem(packageId));
    });

    public static readonly Option<string> VersionOption = new DynamicForwardedOption<string>("--version", "-v")
    {
        Description = CliCommandStrings.CmdVersionDescription,
        HelpName = CliCommandStrings.CmdVersion
    }.ForwardAsSingle(o => $"--version {o}")
        .AddCompletions((context) =>
        {
            // we can only do version completion if we have a package id
            if (context.ParseResult.GetValue(CmdPackageArgument) is PackageIdentity packageId && !packageId.HasVersion)
            {
                // we should take --prerelease flags into account for version completion
                var allowPrerelease = context.ParseResult.GetValue(PrereleaseOption);
                return QueryVersionsForPackage(packageId.Id, context.WordToComplete, allowPrerelease, CancellationToken.None)
                    .Result
                    .Select(version => new CompletionItem(version.ToNormalizedString()));
            }
            else
            {
                return [];
            }
        });

    public static readonly Option<string> FrameworkOption = new ForwardedOption<string>("--framework", "-f")
    {
        Description = CliCommandStrings.PackageAddCmdFrameworkDescription,
        HelpName = CliCommandStrings.PackageAddCmdFramework
    }.ForwardAsSingle(o => $"--framework {o}");

    public static readonly Option<bool> NoRestoreOption = new("--no-restore", "-n")
    {
        Description = CliCommandStrings.PackageAddCmdNoRestoreDescription,
        Arity = ArgumentArity.Zero
    };

    public static readonly Option<string> SourceOption = new ForwardedOption<string>("--source", "-s")
    {
        Description = CliCommandStrings.PackageAddCmdSourceDescription,
        HelpName = CliCommandStrings.PackageAddCmdSource
    }.ForwardAsSingle(o => $"--source {o}");

    public static readonly Option<string> PackageDirOption = new ForwardedOption<string>("--package-directory")
    {
        Description = CliCommandStrings.CmdPackageDirectoryDescription,
        HelpName = CliCommandStrings.CmdPackageDirectory
    }.ForwardAsSingle(o => $"--package-directory {o}");

    public static readonly Option<bool> InteractiveOption = CommonOptions.InteractiveOption().ForwardIfEnabled("--interactive");

    public static readonly Option<bool> PrereleaseOption = new ForwardedOption<bool>("--prerelease")
    {
        Description = CliStrings.CommandPrereleaseOptionDescription,
        Arity = ArgumentArity.Zero
    }.ForwardAs("--prerelease");

    private static readonly Command Command = ConstructCommand();

    public static Command GetCommand()
    {
        return Command;
    }

    private static Command ConstructCommand()
    {
        Command command = new("add", CliCommandStrings.PackageAddAppFullName);

        VersionOption.Validators.Add(DisallowVersionIfPackageIdentityHasVersionValidator);
        command.Arguments.Add(CmdPackageArgument);
        command.Options.Add(VersionOption);
        command.Options.Add(FrameworkOption);
        command.Options.Add(NoRestoreOption);
        command.Options.Add(SourceOption);
        command.Options.Add(PackageDirOption);
        command.Options.Add(InteractiveOption);
        command.Options.Add(PrereleaseOption);
        command.Options.Add(PackageCommandParser.ProjectOption);

        command.SetAction((parseResult) => new PackageAddCommand(parseResult, parseResult.GetValue(PackageCommandParser.ProjectOption)).Execute());

        return command;
    }

    private static void DisallowVersionIfPackageIdentityHasVersionValidator(OptionResult result)
    {
        if (result.Parent.GetValue(CmdPackageArgument) is PackageIdentity identity && identity.HasVersion)
        {
            result.AddError(CliCommandStrings.ValidationFailedDuplicateVersion);
        }
    }

    public static async Task<IEnumerable<string>> QueryNuGet(string packageStem, bool allowPrerelease, CancellationToken cancellationToken)
    {
        try
        {
            var downloader = new NuGetPackageDownloader.NuGetPackageDownloader(packageInstallDir: new DirectoryPath());
            var versions = await downloader.GetPackageIdsAsync(packageStem, allowPrerelease, cancellationToken: cancellationToken);
            return versions;
        }
        catch (Exception)
        {
            return [];
        }
    }

    internal static async Task<IEnumerable<NuGetVersion>> QueryVersionsForPackage(string packageId, string versionFragment, bool allowPrerelease, CancellationToken cancellationToken)
    {
        try
        {
            var downloader = new NuGetPackageDownloader.NuGetPackageDownloader(packageInstallDir: new DirectoryPath());
            var versions = await downloader.GetPackageVersionsAsync(new(packageId), versionFragment, allowPrerelease, cancellationToken: cancellationToken);
            return versions;
        }
        catch (Exception)
        {
            return [];
        }
    }
}
