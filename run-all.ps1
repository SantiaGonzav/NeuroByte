# ===============================
#  NEUROBYTE - AUTO LAUNCH SCRIPT
# ===============================

Write-Host "==============================="
Write-Host "    Actualizando bases de datos..."
Write-Host "==============================="

dotnet ef database update --project ./AuthService/AuthService.csproj
dotnet ef database update --project ./EquipService/EquipService.csproj
dotnet ef database update --project ./RequestService/RequestService.csproj   

Write-Host "==============================="
Write-Host "   Migraciones completadas"
Write-Host "==============================="

Start-Sleep -Seconds 2

Write-Host "==============================="
Write-Host "    Iniciando microservicios..."
Write-Host "==============================="

Start-Process "dotnet" "run --project ./AuthService/AuthService.csproj"
Start-Process "dotnet" "run --project ./EquipService/EquipService.csproj"
Start-Process "dotnet" "run --project ./RequestService/RequestService.csproj"   

Start-Sleep -Seconds 2

Write-Host "==============================="
Write-Host "    Iniciando frontend (Vite)..."
Write-Host "==============================="

# Cambiamos al directorio del frontend
Set-Location "./WEB-FRONTEND"

# Ejecutamos el frontend con npm run dev en una nueva ventana
Start-Process "cmd" "/k npm run dev"

# Volvemos al directorio raíz (opcional)
Set-Location ".."

Write-Host "==============================="
Write-Host "    Todos los servicios iniciados con éxito"
Write-Host "==============================="

