# cpu_emulator
Simple 4 register CPU emulator and cache simulator written in C#.

## NOTE: .NET 8.0 is required to run the project

The program can be built and run using the "run.sh" script.
You need to first make the script executable.
### Linux
```bash
chmod +x run.sh

```
For info on how to run the program, do the following:
```bash
./run.sh -h

```

In order to run the tests you can either use the run.sh script with the -t argument or use 
```bash
dotnet test

```

# Example:
```
./run.sh -in cpu_test.txt -o cpu_test
./run.sh -in cache_test.txt -o cache_test

```
