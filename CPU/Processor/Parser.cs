using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class Parser
{
    private static Parser _instance;

    public static Parser Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Parser();
            }
            return _instance;
        }
    }

    private Dictionary<string, byte> opcodes = new Dictionary<string, byte>();
    private Dictionary<string, byte> registers = new Dictionary<string, byte>();

    private Parser()
    {
        using (StreamReader reader = new StreamReader("config/InstructionSet.cnf"))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                string pattern = @"(?<opcode>0x([a-fA-F0-9_]){2})\s+(?<instruction>[a-zA-Z0-9_]+\s*[r|m|imm]{0,1}\s*[r|m|imm]{0,1})";
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

    public void ParseInstructions(string path)
    {
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string? line;
                int lineCount = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    lineCount++;
                    Regex instruction;
                    string instructionPattern;

                    instructionPattern = @"(?<instruction>[a-zA-Z0-9_]+)\s*(?<operand1>[a-zA-Z0-9_]+|\[[a-zA-Z0-9_]+\])\s*(,\s*(?<operand2>[a-zA-Z0-9_]+|\[[a-zA-Z0-9_]+\])\s*){0,1}|\s*|(?<comment>#*)";
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
                            }

                            if (match.Groups["operand2"].Value != "")
                            {
                                string operand = ParseOperand(match.Groups["operand2"].Value, encodedInstruction, false);
                                if (operand == "")
                                {
                                    throw new Exception("Error parsing instruction on line " + lineCount + ": " + line + " - Invalid operand: " + match.Groups["operand1"].Value);
                                }
                                inst += " " + operand;
                            }
                            encodedInstruction.Opcode = opcodes[inst];

                            //TODO memory, immediate and register indirect addressing
                        }
                    }
                    else
                    {
                        throw new Exception("Error parsing instruction on line " + lineCount + ": " + line);
                    }
                }
            }
        }
    }

    private string ParseOperand(string operand, Instruction instruction, bool isOperand1)
    {
        string? pattern = @"(?<registerDirect>[rR]{1}[0-9]+)|(?<registerIndirect>\[[rR][0-9]+\])|(?<memoryDirect>\[0x[a-fA-F0-9]{16}\])|(?<immediate>0x[a-fA-F0-9]+|[0-9]+)";
        Regex regex = new Regex(pattern);
        Match match = regex.Match(operand);

        if (match.Success)
        {
            byte MODRR = instruction.MODRR;
            if (match.Groups["registerDirect"].Value != "")
            {
                if (isOperand1)
                {
                    MODRR = (byte)(MODRR | (0x80 | (registers[match.Groups["registerDirect"].Value] << 3)));
                }
                else
                {
                    MODRR = (byte)(MODRR | (0x40 | (registers[match.Groups["registerDirect"].Value])));
                }
                instruction.MODRR = MODRR;
                return "r";
            }
            else if (match.Groups["registerIndirect"].Value != "")
            {
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
}
