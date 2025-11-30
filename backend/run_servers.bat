@echo off
echo Starting OrchestratorService...
start "" ".\OrchestratorService\bin\Debug\OrchestratorService.exe"
echo Starting Proxy...
start "" ".\Proxy\bin\Debug\Proxy.exe"

pause