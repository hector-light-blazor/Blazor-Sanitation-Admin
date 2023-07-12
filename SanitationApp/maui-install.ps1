#Requires -RunAsAdministrator

<#
    .SYNOPSIS
    Install the MAUI .NET workload for a specific branch of the .NET MAUI repository.

    .DESCRIPTION
    This script will install the .NET MAUI workload for a specific branch of the .NET
    MAUI repository. It downloads the prepared rollback files and NuGet config files
    and then installs the workloads for a particular .NET SDK band.

    .PARAMETER MauiBranch
    TODO

    .PARAMETER Rollback
    TODO

    .PARAMETER NuGetConfig
    TODO

    .PARAMETER SdkVersion
    TODO

    .PARAMETER Workloads
    TODO

    .INPUTS
    None.

    .OUTPUTS
    None.

    .EXAMPLE
    PS> .\maui-install.ps1
#>

# iwr https://aka.ms/dotnet/maui/maui-install.ps1 -OutFile maui-install.ps1;
# .\maui-install.ps1 -b 'release/6.0.2xx-preview14' -v '6.0.200-preview'

param (
    [Alias("b", "branch")]
    [Parameter(Mandatory = $true)] [string] $MauiBranch,
    [Alias("v", "version")]
    [Parameter(Mandatory = $true)] [string] $SdkVersion,
    [Parameter()] [string] $Rollback,
    [Parameter()] [string] $NuGetConfig,
    [Parameter()] [string] $GlobalNuGetConfig,
    [Parameter()] [switch] $SkipUpdateGlobalNuGetConfig,
    [Parameter(ValueFromRemainingArguments = $true)] [string[]] $Workloads = @('maui','android','ios','maccatalyst','macos','tvos')
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

$releaseBranchPrefix = 'release/'

if (-not $Rollback) {
    Write-Host "No rollback file provided, trying to detect..."
    $branch = $MauiBranch
    if ($branch.StartsWith($releaseBranchPrefix)) {
        $branch = $branch.Substring($releaseBranchPrefix.Length)
    }
    $Rollback = "https://maui.blob.core.windows.net/metadata/rollbacks/$($branch).json"
}
Write-Host "Using rollback file: $($Rollback)"

if (-not $NuGetConfig) {
    Write-Host "No NuGet config file provided, trying to detect..."
    $NuGetConfig = "https://raw.githubusercontent.com/dotnet/maui/$($MauiBranch)/NuGet.config"
}
Write-Host "Using NuGet config file: $($NuGetConfig)"

if (-not $GlobalNuGetConfig) {
    Write-Host "No global NuGet config file provided, trying to detect..."
    if ($IsLinux -or $IsMacOS) {
        $GlobalNuGetConfig = Join-Path "$env:HOME" (Join-Path ".nuget" (Join-Path "NuGet" "NuGet.Config"))
    } else {
        $GlobalNuGetConfig = Join-Path "$env:AppData" (Join-Path "NuGet" "NuGet.Config")
    }
}
Write-Host "Using global NuGet config file: $($GlobalNuGetConfig)"

try {
    $workingDir = Join-Path ([IO.Path]::GetTempPath()) (Join-Path "dotnet-workload-install" ([Guid]::NewGuid()))
    $rollbackFile = Join-Path $workingDir 'rollback.json'
    $nugetConfigFile = Join-Path $workingDir 'nuget.config'
    $globalJsonFile = Join-Path $workingDir 'global.json'

    New-Item $workingDir -ItemType Directory -Force -ErrorAction Ignore | Out-Null

    Push-Location $workingDir

    Write-Host "Generating global.json file..."
    Set-Content -Path $globalJsonFile -Value "{
        `"sdk`": {
            `"version`": `"$SdkVersion`",
            `"rollForward`": `"patch`",
            `"allowPrerelease`": true
        }
    }"

    Write-Host "Downloading nuget.config file..."
    Invoke-WebRequest -Uri $NuGetConfig -OutFile $nugetConfigFile

    Write-Host "Downloading rollback.json file..."
    Invoke-WebRequest -Uri $Rollback -OutFile $rollbackFile

    Write-Host "Using .NET SDK version:"
    dotnet --version

    if (-not $?) {
        exit 1
    }

    Write-Host "Installing .NET workloads..."
    dotnet workload install @Workloads --from-rollback-file $rollbackFile --configfile $nugetConfigFile --skip-sign-check

    if (-not $?) {
        exit 1
    }

    Write-Host "Removing newer sdk-manifest bands..."
    $sdks = (dotnet --list-sdks)
    $usingVersion = (dotnet --version)
    $sdkPath = [regex]::Match($sdks, '.*\[(.+)\]')
    if ($sdkPath.Success) {
        $manifestsFolder = Join-Path $sdkPath.Groups[1].Value (Join-Path ".." "sdk-manifests")
        $toRemove = Get-ChildItem $manifestsFolder | Where-Object { $_.Name -gt $usingVersion }
        Write-Host "Removing SDK band folders for workload manifests..."
        $toRemove | Remove-Item -Force -Recurse | Out-Null
    } else {
        Write-Error "Unable to complete patch."
        exit 1
    }

    if (-not $SkipUpdateGlobalNuGetConfig) {
        Write-Host "Updating global NuGet config file with required sources..."
        $globalNuGetConfigXml = [xml] (Get-Content $GlobalNuGetConfig)
        $mauiNuGetConfigXml = [xml] (Get-Content $nugetConfigFile)

        $globalAllSources = $globalNuGetConfigXml.configuration.packageSources.add

        $mauiDisabledSources = $mauiNuGetConfigXml.configuration.disabledPackageSources.add | Where-Object { $_.value -eq 'true' } | Select-Object -ExpandProperty key
        $mauiAllSources = $mauiNuGetConfigXml.configuration.packageSources.add | Where-Object { $_.key -notin $mauiDisabledSources }

        Write-Host "Removing old .NET MAUI NuGet sources..."
        foreach ($source in $globalAllSources) {
            if ($source.key.StartsWith('maui-generated-')) {
                $removed = $globalNuGetConfigXml.configuration.packageSources.RemoveChild($source)
            }
        }

        Write-Host "Adding new .NET MAUI NuGet sources..."
        foreach ($source in $mauiAllSources) {
            $newSource = $globalNuGetConfigXml.CreateElement("add")
            $newSourceKey = $globalNuGetConfigXml.CreateAttribute("key")
            $newSourceKey.InnerText = "maui-generated-$($source.key)"
            $added = $newSource.Attributes.Append($newSourceKey)
            $newSourceValue = $globalNuGetConfigXml.CreateAttribute("value")
            $newSourceValue.InnerText = $source.value
            $added = $newSource.Attributes.Append($newSourceValue)
            $added = $globalNuGetConfigXml.configuration.packageSources.AppendChild($newSource)
        }
        $globalNuGetConfigXml.Save($GlobalNuGetConfig)
    } else {
        Write-Warning "Not updating global NuGet config due to SkipUpdateGlobalNuGetConfig switch."
    }

} finally {
    Pop-Location

    Write-Host "Cleaning up..."
    Remove-Item $workingDir -Force -Recurse -ErrorAction Ignore | Out-Null
    Remove-Item $MyInvocation.InvocationName -Force -Recurse -ErrorAction Ignore | Out-Null
}

Write-Host "All done!"
