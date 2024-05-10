#!/bin/bash

command="dotnet run --project Simulation/Simulation.csproj --"

for arg in "$@"; do
    command+=" $arg"
done

eval $command
