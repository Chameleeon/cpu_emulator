#!/bin/bash

if [[ "$1" == "-t" ]]; then
    shift
    dotnet test
else
    dotnet run --project Simulation/Simulation.csproj -- "$@"
fi
