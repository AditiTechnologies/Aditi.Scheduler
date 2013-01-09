param($installPath, $toolsPath, $package, $project)

$readmeFile = "https://github.com/AditiTechnologies/Aditi.SignatureAuth"
$DTE.ItemOperations.Navigate($readmeFile, [EnvDTE.vsNavigateOptions]::vsNavigateOptionsNewWindow)