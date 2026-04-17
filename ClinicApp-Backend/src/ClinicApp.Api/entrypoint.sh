#!/bin/bash
set -e

echo "Iniciando aplicação ClinicApp.Api..."
exec dotnet ClinicApp.Api.dll
