function Get-Help()
{

	<#

	.SYNOPSIS
    Details all available AutoBot commands.

	.DESCRIPTION
    Returns a list of names of all *.psm1 PowerShell Modules in the AutoBot\Scripts folder and optionally gives
	detailed help on a given script

	.NOTES
    Name: Get-Help
    Author: Lloyd Holman
    DateCreated: 2011/11/09

	.EXAMPLE
    Get-Help

	Description
	------------
	Returns a list of names of all *.psm1 PowerShell Modules in the AutoBot\Scripts folder

	.EXAMPLE
    Get-Help Set-Profile
	Description
	------------
	Returns PowerShell native get-help detail for the Set-Profile AutoBot script module

	#>

	[cmdletbinding()]
    param
	(

			[Parameter(Position = 0, Mandatory = $false )]
			[string] $modulename,

			[Parameter(Position = 1, Mandatory = $false)]
			[switch] $examples,

			[Parameter(Position = 2, Mandatory = $false)]
			[switch] $detailed,

			[Parameter(Position = 3, Mandatory = $False )]
			[switch] $full

	)

	try
	{

		if( [string]::IsNullOrEmpty($modulename) )
		{
			$scripts = Get-ChildItem -Recurse -Exclude get-help.psm1 -Include *.psm1 | % { [System.IO.Path]::GetFileNameWithoutExtension($_); };
			$result = "Word! I have the following scripts installed and ready to run.`r`n" +
					  "`r`n" +
					  [string]::Join("`r`n", $scripts) +
					  "`r`n" +
					  "For information about running an installed script use get-help <scriptname>`r`n" +
					  "e.g. `"@AutoBot get-help set-profile`" `r`n" +
					  "Find more scripts at https://github.com/lholman/AutoBot-Scripts/tree/master/src/Scripts" ;
			return $result;
		}

		#write-host "$modulename provided";
		if( $modulename -eq "Get-Help" )
		{
			return $null;
		}

		Microsoft.PowerShell.Core\Import-Module ".\Scripts\$modulename.psm1";

		if( $PSBoundParameters.ContainsKey("examples") )
		{
			$result = Microsoft.PowerShell.Core\Get-Help $moduleName -examples;
			return $result;
		}
		elseif( $PSBoundParameters.ContainsKey("detailed") )
		{
			$result = Microsoft.PowerShell.Core\Get-Help $moduleName -detailed;
			return $result;
		}
		elseif( $PSBoundParameters.ContainsKey("full") )
		{
			$result = Microsoft.PowerShell.Core\Get-Help $moduleName -full;
			return $result;
		}
		else
		{
			$result = Microsoft.PowerShell.Core\Get-Help $moduleName;
			return $result;
		}

	}
	catch [Exception]
	{

		$ex = $_.psbase.Exception;
		write-host $ex.ToString();

	}

}