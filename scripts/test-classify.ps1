[CmdletBinding()]
param(
  [Parameter(Mandatory = $false)]
  [string] $BaseUrl = "https://localhost:44303"
)

$uri = "$BaseUrl/api/trades/risk/classify"
$tradesPath = Join-Path $PSScriptRoot "trades.json"

if (-not (Test-Path -LiteralPath $tradesPath)) {
  throw "Arquivo trades.json não encontrado em: $tradesPath"
}

Write-Host "Reading payload from $tradesPath" -ForegroundColor Cyan
$body = [System.IO.File]::ReadAllText($tradesPath)

Write-Host "POST $uri" -ForegroundColor Cyan
$tmp = New-TemporaryFile
try {
  [System.IO.File]::WriteAllText($tmp.FullName, $body, (New-Object System.Text.UTF8Encoding($false)))
  curl.exe -sS -X POST $uri -H "Content-Type: application/json" --data-binary "@$($tmp.FullName)"
}
finally {
  Remove-Item -LiteralPath $tmp.FullName -Force -ErrorAction SilentlyContinue
}