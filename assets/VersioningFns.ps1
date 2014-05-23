Function Get-Semver
{
    param(
        [string] $path = ".",
        [switch] $recurse,
        [string] $versionFile="version.txt",
        [string]$branch,
        [string]$buildCounter,
        [string]$rev,
        [switch]$nugetCompatible,
        [string]$stableBranchRegex="^(prod|production|master|brewmaster|brewmaster-prod)$"
        )

    $version = Get-Version -path $path -recurse:$recurse
    $localBuild = ($branch -eq "")

    if ($localBuild)
    {
        $preRel="alpha"
    } else {
        if (-Not ($branch -match $stableBranchRegex)) {$preRel="beta"}        
    }
    
    $packageVer=$(Build-Semver -version $version -preRel $preRel -buildCounter $buildCounter -rev $rev -nugetCompatible:$nugetCompatible)
    
    return $packageVer
}

Function Bump-Version 
{
    param(
        [string]$part = "patch",
        [string]$path= ".",
        [switch]$recurse
        )
    $versionFile = Get-VersionFile -path $path -recurse:$recurse
    if ($versionFile -eq $null) {throw "Cannot find version file"}
    $version=(cat $versionFile)
    $newVersion=Incr-Version -version $version -part $part
    Set-Content $versionFile -Value $newVersion
    TC-Message "Bumped from $version to $newVersion in $versionFile"
    return $newVersion
}

Function Incr-Version()
{
    param(
        [Parameter(Mandatory=$true)]
        [string] $version ="",
        [Parameter(Mandatory=$true)]
        [string] $part
        )
    if ($version -eq "") {
        write-warning "Version not passed"
    }
    $number,$pre,$build=$version -split '[\+\-]'
    $major,$minor,$patch=$number.Split(".")
    
    switch -wildcard ($part) 
    {
        "ma*" {$incrementMajor = $True}
        "mi*" {$incrementMinor = $True}
        "pa*" {$incrementPatch = $True}
        default {}
    }
    if ($incrementPatch) {$patch=([int]$patch) + 1}
    if ($incrementMajor) 
    {
        $major=([int]$major) + 1
        $minor=0
        $patch=0
    }
    if ($incrementMinor) 
    { 
        $minor=([int]$minor) + 1
        $patch=0
    }
    $version=($major,$minor,$patch) -join "."
   
    return $version
}

Function Get-VersionFile()
{
    param(
        [string] $versionFile="version.txt",
        [Switch] $recurse,
        [string] $path="."
        )
    if (-Not $recurse) {
        if (Test-Path "$path\$versionFile") { $foundPath= "$path\$versionFile" }
    } else {
        $file = (ls $versionFile -recurse) | Sort-object FullName.Length| Select-Object -first 1
        if ($file -ne $null) {$foundPath = $file.FullName }
    }
    return $foundPath
}

Function Get-Version()
{
    param(
        [string] $versionFile="version.txt",
        [Switch] $recurse,
        [string] $default,
        [string] $path="."
    )
    $file = Get-VersionFile -path $path -recurse:$recurse
    if ($file -eq $null) {
        return $default
    }
    return (cat $file)
}

Function Build-Semver
{
    param(
        [string] $version,
        [string] $preRel,
        [string] $rev,
        [string] $buildCounter,
        [Switch] $nugetCompatible
        )
    if ($preRel -ne "") {$version="$version-$preRel" }
    if ($nugetCompatible -and $preRel -eq "") {return $version}

    if ($rev -ne $null -and $rev.Length -gt 7) {
        $rev=$rev.Substring(0,7)
    }
    if ($nugetCompatible)
    {
        if ($buildCounter -ne "") {$version="$($version)B$($buildCounter)R$rev" }
    } else 
    {
        if ($buildCounter -ne "") {$version="$version+B$($buildCounter)R$rev" }
    }
    
    return $version
}

Function TC-Message
{
    param(
        [string] $message
        )
    write-host "##teamcity[message text='$message']"
} 