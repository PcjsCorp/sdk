// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace Microsoft.DotNet.Cli.Installer.Windows;

/// <summary>
/// A class that swallows all logging - used for code paths where logging setup operations isn't valuable, like <c>dotnet --info<c>.
/// </summary>
internal class NullInstallerLogger : ISynchronizingLogger
{
    public string LogPath => string.Empty;

    public void AddNamedPipe(string pipeName)
    {

    }

    public void LogMessage(string message)
    {

    }
}
