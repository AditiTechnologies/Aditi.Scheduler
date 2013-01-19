param($installPath, $toolsPath, $package, $project)

$readmeFile = "http://www.aditicloud.com/nugets/gettingstarted"
$DTE.ItemOperations.Navigate($readmeFile, [EnvDTE.vsNavigateOptions]::vsNavigateOptionsNewWindow)