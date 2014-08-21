function Get-Prediction() { 

	<#

		.SYNOPSIS
		Ask the oracle, and the oracle will answer.

	#>

	param
	(

		[Parameter(Mandatory=$true)]
		[string] $Question

	)

	$ErrorActionPreference = "Stop";
	Set-StrictMode -Version "Latest";

	# see http://en.wikipedia.org/wiki/Magic_8-Ball
	$predictions = @(
		# yes
		"It is certain",
		"It is decidedly so",
		"Without a doubt",
		"Yes definitely",
		"You may rely on it",
		"As I see it, yes",
 		"Most likely",
		"Outlook good",
		"Yes",
		# maybe
		"Signs point to yes",
		"Reply hazy try again",
		"Ask again later",
		"Better not tell you now",
		"Cannot predict now",
		"Concentrate and ask again",
		# no
		"Don't count on it",
		"My reply is no",
		"My sources say no",
		"Outlook not so good",
		"Very doubtful"
	);

	$prediction = $predictions | Get-Random;

	return "You asked: $question`r`n" +
	       "Magic 8-ball says: $prediction";

}
