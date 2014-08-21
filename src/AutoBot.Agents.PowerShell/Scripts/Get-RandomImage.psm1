function Get-RandomImage()
{

	<#

	.SYNOPSIS
    Returns a random image from a Google image search for a given search term.

	.DESCRIPTION
    Returns a random image from a Google image search for a given search term.

	.NOTES
    Name: Get-RandomImage
    Author: Steve Garrett, Lloyd Holman
    DateCreated: 21/11/2011

	.EXAMPLE
    Get-RandomImage coolio

	#>

	param
	(

		[Parameter(Mandatory=$true)]
		[string] $term

	)
	
	$ErrorActionPreference = "Stop";
	Set-StrictMode -Version "Latest";

	try
	{

		$url = "https://ajax.googleapis.com/ajax/services/search/images?v=1.0&q=$term";
		$html = (new-object System.Net.WebClient).DownloadString($url);
		#write-host $html;

		$regex = [regex] """url"":""([^""]+)""";
		$result = $regex.Matches($html) | get-random | % { $_.Groups[1].Value };

		return ($result | Out-String);

	}
	catch [Exception]
	{

		$ex = $_.psbase.Exception;
		write-host $ex.ToString();

	}

}
