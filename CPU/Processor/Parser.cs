using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class Parser
{
    private Dictionary<string, byte> opcodes = new Dictionary<string, byte>();

    public Parser()
    {
        using (StreamReader reader = new StreamReader("config/InstructionSet.cnf"))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string pattern = @"(?<opcode>0x([a-fA-F0-9_]){2})\s+(?<instruction>[a-zA-Z0-9_]+\s*[r|m|imm]{0,1}\s*[r|m|imm]{0,1})";
                Regex instruction = new Regex(pattern, RegexOptions.IgnoreCase);
                Match match = instruction.Match(line);
                if (match.Success)
                {
                    opcodes.Add(match.Groups["instruction"].Value, Convert.ToByte(match.Groups["opcode"].Value, 16));
                    Console.WriteLine("Opcode: " + match.Groups["opcode"].Value);
                    Console.WriteLine("Instruction: " + match.Groups["instruction"].Value);
                }
            }
        }
    }

    public void ParseInstructions(String path)
    {
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Regex instruction;
                    string pattern;

                    pattern = @"(?<instruction>[a-zA-Z0-9_]+)\s*(?<operand1>[a-zA-Z0-9_]+)\s*,\s*(?<operand2>[a-zA-Z0-9_]+)";
                    instruction = new Regex(pattern, RegexOptions.IgnoreCase);
                    Match match = instruction.Match(line);
                    if (match.Success)
                    {

                        Console.WriteLine("Instruction: " + match.Groups["instruction"].Value);
                        Console.WriteLine("Operand1: " + match.Groups["operand1"].Value);
                        Console.WriteLine("Operand2: " + match.Groups["operand2"].Value);
                    }
                }
            }


        }
    }
}
