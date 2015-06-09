# Generate load for the .NET Lab web sites

function Test([String] $url)
{
	Write-Host "Testing URL: $url"
	Invoke-WebRequest $url | select StatusCode, StatusDescription, RawContentLength, RawContent
}

for($i = 0; $i -lt 200; $i++)
{
	Test "http://localhost:2100/"
	Test "http://localhost:2100/healthcheck.aspx"
	Test "http://localhost:2100/logstats"
	Test "http://localhost:2100/searchstats"
	Test "http://localhost:2100/robots/robots.aspx"
	Test "http://localhost:2100/Ajax/healthcheck.aspx"
	Test "http://localhost:2101/"
	Test "http://localhost:2101/healthcheck.aspx"

	# Ping AJAX end-point
	Test "http://localhost:2100/Ajax/AjaxLabService.svc/CheckData?value=%22Weather%20Seattle%22"

	Start-Sleep -Seconds 1

	# Generate load with multiple URL's on the API site
	$keys1 = @("San Francisco", "Boston", "Seattle", "Moscow", "Paris", "Las Vegas", "New York", "Tokio", "Amsterdam", "London")
	$keys2 = @("ginormous", "confuzzled", "woot", "chillax", "cognitive displaysia", "gripton", "phonecrastinate", "slickery", "snirt", "lingweenie")
	$keys3 = @("human", "robot", "monkey")

	foreach ($key3 in $keys3)
	{
		foreach ($key2 in $keys2)
		{
			foreach ($key1 in $keys1)
			{
				$url = "http://localhost:2101/$key1/$key2/$key3"
				Test $url

			}
		}

		Test "http://localhost:2101/AppDynamics/DotNet/Monitoring"

		Start-Sleep -Seconds 1
	}

	Start-Sleep -Seconds 5
}
