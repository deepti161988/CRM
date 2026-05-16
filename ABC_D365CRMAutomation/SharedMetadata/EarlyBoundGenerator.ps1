
# Early Bound Generator Script

Write-Host "----------------------------------------"
Write-Host "Checking PAC authentication..."
Write-Host "----------------------------------------"

pac auth list

Write-Host ""
Write-Host "----------------------------------------"
Write-Host "Selecting environment..."
Write-Host "----------------------------------------"

pac org select --environment https://org843d3fcc.crm.dynamics.com

Write-Host ""
Write-Host "----------------------------------------"
Write-Host "Generating Early Bound Classes..."
Write-Host "----------------------------------------"

pac modelbuilder build -o ".\Model" -stf ".\builderSettings.json"

Write-Host ""
Write-Host "----------------------------------------"
Write-Host "Early Bound Generation Completed!"
Write-Host "Output folder: .\Model"
Write-Host "----------------------------------------"