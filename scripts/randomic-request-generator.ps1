$data = @()

for ($i = 0; $i -lt 100000; $i++) {
    $value = Get-Random -Minimum 500000 -Maximum 2000000
    $sector = @("Public", "Private") | Get-Random
    $clientId = "CLI" + (Get-Random -Minimum 1 -Maximum 1000).ToString("D3")

    $data += [PSCustomObject]@{
        value = $value
        clientSector = $sector
        clientId = $clientId
    }
}

$data | ConvertTo-Json -Depth 3 | Out-File "trades.json"