[CmdletBinding()]
Param()

trap 
{ 
  write-error $_ 
  exit 1 
} 

[string]$Script:ScriptPath = Split-Path $MyInvocation.MyCommand.Path

[string]$Script:SoureFolder = $Script:ScriptPath
[string]$Script:IndexFile = "$($Script:ScriptPath)/index.csv"
[string]$Script:TargetFolder = [System.IO.Path]::GetFullPath("$($Script:SoureFolder)/../src/NLedger.Launchpad/wwwroot/sample-data")

Write-Verbose "Source folder: $($Script:SoureFolder)"
Write-Verbose "Target folder: $($Script:TargetFolder)"
Write-Verbose "Index file: $($Script:IndexFile)"

Write-Verbose "Reading index file..."
if (!(Test-Path -LiteralPath $Script:IndexFile -PathType Leaf)){throw "Index file not found: $($Script:IndexFile)"}
$indexData = Import-Csv -LiteralPath $Script:IndexFile
if (($indexData | Measure-Object).Count -eq 0) {throw "No data in index file"}

$keyIndex = @{}
$fileList = @()

foreach($indexItem in $indexData) {
    [Guid]$sampleID = [Guid]::Parse($indexItem.SampleID)
    [string]$title = $indexItem.Title
    [string]$command = $indexItem.Command
    $itemFiles = $indexItem.Files -split "/"

    if ($sampleID -eq [Guid]::Empty) {throw "Empty SampleID"}
    if (!$title) {throw "Empty Title"}
    if (!$command) {throw "Empty Command"}

    Write-Verbose "SampleID: $sampleID; Title: $title; Command: $command; Files: $itemFiles"
    $null = $itemFiles | Where-Object{!($fileList -contains $_)} | ForEach-Object{$fileList += $_}
    $keyIndex[$sampleID] = [PSCustomObject]@{
        SampleID = $sampleID
        Title = $title
        Command = $command
        Files = $fileList
    }
}

if (($keyIndex.Keys | Measure-Object).Count -ne ($indexData | Measure-Object).Count) { "SampleID values must be unique" }

Write-Verbose "Check referenced files"
$null = $fileList | ForEach-Object{if(!(Test-Path -LiteralPath "$($Script:SoureFolder)/$($_)" -PathType Leaf)){throw "File not found: $($Script:SoureFolder)/$($_)"}}

Write-Verbose "Cleaning target folder"
if(!(Test-Path -LiteralPath $Script:TargetFolder -PathType Container)) {"Target folder not found: $Script:TargetFolder"}
$null = Get-ChildItem -Path $Script:TargetFolder -Include *.* -File -Recurse | ForEach-Object{ $_.Delete()}

Write-Verbose "Write index..."
$null = $keyIndex.Values | ConvertTo-Json | Out-File "$($Script:TargetFolder)/index.json" -Encoding utf8
Write-Verbose "Saved $($Script:TargetFolder)/index.json"

Write-Verbose "Write files..."
foreach($fileItem in $fileList) {
    [string]$fileContent = Get-Content -LiteralPath "$($Script:SoureFolder)/$fileItem" -Raw
    $contentObject = [PSCustomObject]@{
        Content = $fileContent
    }
    $null = $contentObject | ConvertTo-Json | Out-File "$($Script:TargetFolder)/$($fileItem).json" -Encoding utf8

    Write-Verbose "Saved $($Script:TargetFolder)/$($fileItem).json"
}

Write-Verbose "Done"