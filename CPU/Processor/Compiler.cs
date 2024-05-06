using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class Compiler
{
    private static Compiler _instance;

    public static Compiler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Compiler();
            }
            return _instance;
        }
    }

    private Dictionary<string, byte> opcodes = new Dictionary<string, byte>();
    private Dictionary<string, byte> registers = new Dictionary<string, byte>();

    private Compiler()
    {
        using (StreamReader reader = new StreamReader("config/InstructionSet.cnf"))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                string pattern = @"(?<opcode>0x([a-fA-F0-9_]){2})\s+(?<instruction>[a-zA-Z0-9_]+(\s+(r|m|imm)){0,1}(\s+(r|m|imm)){0,1})";
                Regex instruction = new Regex(pattern);
                Match match = instruction.Match(line);
                if (match.Success)
                {
                    opcodes.Add(match.Groups["instruction"].Value, Convert.ToByte(match.Groups["opcode"].Value, 16));
                }
            }
        }

        using (StreamReader reader = new StreamReader("config/Registers.cnf"))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                string pattern = @"\s*(?<register>[rR][0-9]+)\s+(?<code>[01]{3})\s*";
                Regex instruction = new Regex(pattern);
                Match match = instruction.Match(line);
                if (match.Success)
                {
                    registers.Add(match.Groups["register"].Value, Convert.ToByte(match.Groups["code"].Value, 2));
                }
            }
        }

    }

    public void Compile(string input, string output)
    {
        if (File.Exists(input))
        {
            using (StreamReader reader = new StreamReader(input))
            {
                string? line;
                int lineCount = 0;
                FileStream fs = new FileStream(output, FileMode.Create, FileAccess.Write);

                while ((line = reader.ReadLine()) != null)
                {
                    lineCount++;
                    Regex instruction;
                    string instructionPattern;

                    instructionPattern = @"(?<instruction>[a-zA-Z0-9_]+)(\s+(?<operand1>[a-zA-Z0-9_]+|\[[a-zA-Z0-9_]+\])\s*(,\s*(?<operand2>[a-zA-Z0-9_]+|\[[a-zA-Z0-9_]+\])\s*){0,1}){0,1}|\s*|(?<comment>#*)";
                    instruction = new Regex(instructionPattern);
                    Match match = instruction.Match(line);
                    if (match.Success)
                    {
                        Instruction encodedInstruction = new Instruction();

                        if (match.Groups["instruction"].Value != "")
                        {
                            string inst = match.Groups["instruction"].Value;

                            if (match.Groups["operand1"].Value != "")
                            {
                                string operand = ParseOperand(match.Groups["operand1"].Value, encodedInstruction, true);
                                if (operand == "")
                                {
                                    throw new Exception("Error parsing instruction on line " + lineCount + ": " + line + " - Invalid operand: " + match.Groups["operand1"].Value);
                                }
                                inst += " " + operand;
                                if (operand == "m")
                                {
                                    encodedInstruction.HasAddress = true;
                                    encodedInstruction.MemoryAddress = long.Parse(match.Groups["operand1"].Value.Substring(3, match.Groups["operand1"].Value.Length - 4), System.Globalization.NumberStyles.HexNumber);
                                }
                                else if (operand == "imm")
                                {
                                    encodedInstruction.HasImmediate = true;
                                    if (match.Groups["operand1"].Value.StartsWith("0x"))
                                    {
                                        encodedInstruction.ImmediateValue = long.Parse(match.Groups["operand1"].Value.Substring(2), System.Globalization.NumberStyles.HexNumber);
                                    }
                                    else
                                    {
                                        encodedInstruction.ImmediateValue = long.Parse(match.Groups["operand1"].Value);
                                    }
                                }
                            }

                            if (match.Groups["operand2"].Value != "")
                            {
                                string operand = ParseOperand(match.Groups["operand2"].Value, encodedInstruction, false);
                                if (operand == "")
                                {
                                    throw new Exception("Error parsing instruction on line " + lineCount + ": " + line + " - Invalid operand: " + match.Groups["operand1"].Value);
                                }

                                inst += " " + operand;

                                if (operand == "m")
                                {
                                    encodedInstruction.HasAddress = true;
                                    encodedInstruction.MemoryAddress = long.Parse(match.Groups["operand2"].Value.Substring(3, match.Groups["operand2"].Value.Length - 4), System.Globalization.NumberStyles.HexNumber);
                                }
                                else if (operand == "imm")
                                {
                                    encodedInstruction.HasImmediate = true;
                                    if (match.Groups["operand2"].Value.StartsWith("0x"))
                                    {
                                        encodedInstruction.ImmediateValue = long.Parse(match.Groups["operand2"].Value.Substring(2), System.Globalization.NumberStyles.HexNumber);
                                    }
                                    else
                                    {
                                        encodedInstruction.ImmediateValue = long.Parse(match.Groups["operand2"].Value);
                                    }
                                }
                            }
                            try
                            {
                                encodedInstruction.Opcode = opcodes[inst];
                            }
                            catch (KeyNotFoundException e)
                            {
                                Console.WriteLine("Error parsing instruction on line " + lineCount + ": " + line);
                                Environment.Exit(1);
                            }
                            byte[] instructionBytes = encodedInstruction.ToByteArray();
                            WriteInstructionToFile(fs, instructionBytes);
                        }
                    }
                    else
                    {
                        throw new Exception("Error parsing instruction on line " + lineCount + ": " + line);
                    }
                }
                fs.Close();
            }
        }
    }

    private string ParseOperand(string operand, Instruction instruction, bool isOperand1)
    {
        string? pattern = @"(\[(?<registerIndirect>[rR][0-9]+)\])|(?<registerDirect>[rR]{1}[0-9]+)|(?<memoryDirect>\[0x[a-fA-F0-9]{0,16}\])|(?<immediate>0x[a-fA-F0-9]+|[0-9]+)";
        Regex regex = new Regex(pattern);
        Match match = regex.Match(operand);

        if (match.Success)
        {
            byte MODRR = instruction.MODRR;
            if (match.Groups["registerDirect"].Value != "")
            {
                string register = match.Groups["registerDirect"].Value.ToLower();
                if (isOperand1)
                {
                    try
                    {
                        MODRR = (byte)(MODRR | (0x80 | (registers[register] << 3)));
                    }
                    catch (KeyNotFoundException e)
                    {
                        Console.WriteLine("Unknown register: " + register);
                        Environment.Exit(1);
                    }
                }
                else
                {
                    try
                    {
                        MODRR = (byte)(MODRR | (0x40 | (registers[register])));
                    }
                    catch (KeyNotFoundException e)
                    {
                        Console.WriteLine("Unknown register: " + register);
                        Environment.Exit(1);
                    }
                }
                instruction.MODRR = MODRR;
                return "r";
            }
            else if (match.Groups["registerIndirect"].Value != "")
            {
                string register = match.Groups["registerIndirect"].Value.ToLower();
                if (isOperand1)
                {
                    try
                    {
                        MODRR = (byte)(MODRR | (registers[register] << 3));
                    }
                    catch (KeyNotFoundException e)
                    {
                        Console.WriteLine("Unknown register: " + register);
                        Environment.Exit(1);
                    }
                }
                else
                {
                    try
                    {
                        MODRR = (byte)(MODRR | (registers[register]));
                    }
                    catch (KeyNotFoundException e)
                    {
                        Console.WriteLine("Unknown register: " + register);
                        Environment.Exit(1);
                    }
                }
                instruction.MODRR = MODRR;
                return "r";
            }
            else if (match.Groups["memoryDirect"].Value != "")
            {
                return "m";
            }

            else if (match.Groups["immediate"].Value != "")
            {
                return "imm";
            }
        }
        return "";
    }

    private void WriteInstructionToFile(FileStream fs, byte[] instruction)
    {
        try
        {
            fs.Write(instruction, 0, instruction.Length);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error while writing to file: " + e.Message);
            Environment.Exit(1);
        }
    }
}
