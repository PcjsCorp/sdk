﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.CommandLine;
using Microsoft.DotNet.Cli.Commands.Restore;
using Microsoft.DotNet.Cli.Extensions;

namespace Microsoft.DotNet.Cli.Commands.Pack;

public class PackCommand(
    IEnumerable<string> msbuildArgs,
    bool noRestore,
    string msbuildPath = null) : RestoringCommand(msbuildArgs, noRestore, msbuildPath)
{
    public static PackCommand FromArgs(string[] args, string msbuildPath = null)
    {
        var parser = Parser.Instance;
        var parseResult = parser.ParseFrom("dotnet pack", args);
        return FromParseResult(parseResult, msbuildPath);
    }

    public static PackCommand FromParseResult(ParseResult parseResult, string msbuildPath = null)
    {
        parseResult.ShowHelpOrErrorIfAppropriate();

        var msbuildArgs = new List<string>()
        {
            "-target:pack",
            "--property:_IsPacking=true" // This property will not hold true for MSBuild /t:Publish or in VS.
        };

        IEnumerable<string> slnOrProjectArgs = parseResult.GetValue(PackCommandParser.SlnOrProjectArgument);

        msbuildArgs.AddRange(parseResult.OptionValuesToBeForwarded(PackCommandParser.GetCommand()));

        ReleasePropertyProjectLocator projectLocator = new(parseResult, MSBuildPropertyNames.PACK_RELEASE,
            new ReleasePropertyProjectLocator.DependentCommandOptions(
                    parseResult.GetValue(PackCommandParser.SlnOrProjectArgument),
                    parseResult.HasOption(PackCommandParser.ConfigurationOption) ? parseResult.GetValue(PackCommandParser.ConfigurationOption) : null
                )
        );
        msbuildArgs.AddRange(projectLocator.GetCustomDefaultConfigurationValueIfSpecified());

        msbuildArgs.AddRange(slnOrProjectArgs ?? []);

        bool noRestore = parseResult.HasOption(PackCommandParser.NoRestoreOption) || parseResult.HasOption(PackCommandParser.NoBuildOption);

        return new PackCommand(
            msbuildArgs,
            noRestore,
            msbuildPath);
    }

    public static int Run(ParseResult parseResult)
    {
        parseResult.HandleDebugSwitch();

        return FromParseResult(parseResult).Execute();
    }
}
