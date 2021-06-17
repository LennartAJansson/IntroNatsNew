$registryHost = $env:REGISTRYHOST
$buildId = "latest"

#[System.IO.Path]::GetRandomFileName()
#[System.IO.Path]::GetTempFileName()
#[System.Environment]::SetEnvironmentVariable('variable', 'value', [System.EnvironmentVariableTarget]::Machine)
$temp = [System.IO.Path]::GetTempPath()
$guid = [System.Guid]::NewGuid().ToString()
$tempDir = "$temp$guid"
New-Item -Name $guid -Path $temp -ItemType Directory
$tempDir

Copy-Item -Path .\deploy\* -Destination $tempDir -Recurse

$files = Get-ChildItem -Path $tempDir\*.yaml -Recurse -Force

foreach ($file in $files) {
	$content = Get-Content -Path $file
	$content = $content -replace '#{Build.BuildId}#', 'latest'
	$content | Set-Content -Path $file
}

#kubectl delete -k $tempDir/all

kubectl apply -k $tempDir/all

Remove-Item $tempDir -recurse -force
