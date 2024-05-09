public class Processor : IProcessor
{
    private Dictionary<byte, Action<byte, byte, byte>> _regRegOps = new Dictionary<byte, Action<byte, byte, byte>>();
    private Dictionary<byte, Action<byte, long>> _regMemOps = new Dictionary<byte, Action<byte, long>>();
    private Dictionary<byte, Action<byte, long>> _regImmOps = new Dictionary<byte, Action<byte, long>>();
    private Dictionary<byte, Action<byte>> _singleOperandRegOps = new Dictionary<byte, Action<byte>>();
    private Dictionary<byte, Action<byte, long>> _singleOperandMemOps = new Dictionary<byte, Action<byte, long>>();
    private Dictionary<byte, Action<byte, long>> _singleOperandImmOps = new Dictionary<byte, Action<byte, long>>();
    private Dictionary<byte, Action<byte>> _noOperandOps = new Dictionary<byte, Action<byte>>();

    private bool _halted = false;

    public IRegister[] _registers { get; set; }
    public IRegister _flagsRegister = new Register64();

    public long _programCounter { get; set; }

    public IMemSpace _memory { get; set; }

    public Processor(int pageSize = 4096, int registerCount = 4)
    {
        _memory = new Memory(pageSize);
        _programCounter = 0;
        _registers = new Register64[registerCount];
        for (int i = 0; i < _registers.Length; i++)
        {
            _registers[i] = new Register64();
        }
        InitializeInstructions();
    }
    private long _nextFreeAddress = 0;

    private int LoadProgram(string input)
    {
        int programMemory = 0;
        try
        {
            using (FileStream fs = new FileStream(input, FileMode.Open, FileAccess.Read))
            {
                int bufferSize = 4096;
                byte[] buffer = new byte[bufferSize];
                int bytesRead;

                while ((bytesRead = fs.Read(buffer, 0, bufferSize)) > 0)
                {
                    programMemory += bytesRead;
                    _memory.WriteBytes(_nextFreeAddress, buffer);
                    byte[] bytes = _memory.ReadBytes(_nextFreeAddress, bytesRead);
                    _nextFreeAddress += bytesRead;
                }
            }
            return programMemory;
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
            return 0;
        }
    }

    public void ExecuteProgram(string input)
    {
        int instruction = 0;
        int programSize = LoadProgram(input);
        StreamWriter addressWriter = new StreamWriter(input + "_mem_access.txt");
        while (_programCounter < programSize)
        {
            byte nextInstructionOpcode = _memory.ReadByte(_programCounter);
            byte nextInstructionMODRR = _memory.ReadByte(_programCounter + 1);

            if (_regRegOps.ContainsKey(nextInstructionOpcode))
            {
                _regRegOps[nextInstructionOpcode](nextInstructionMODRR, (byte)((nextInstructionMODRR & 0x38) >> 3), (byte)(nextInstructionMODRR & 0x07));
                _programCounter += 2;
            }
            else if (_regMemOps.ContainsKey(nextInstructionOpcode))
            {
                long address = _memory.ReadLong(_programCounter + 2);
                _regMemOps[nextInstructionOpcode](nextInstructionMODRR, address);
                WriteAddressToFile(addressWriter, address);
                _programCounter += 10;
            }
            else if (_regImmOps.ContainsKey(nextInstructionOpcode))
            {
                _regImmOps[nextInstructionOpcode](nextInstructionMODRR, _memory.ReadLong(_programCounter + 2));
                _programCounter += 10;
            }
            else if (_singleOperandRegOps.ContainsKey(nextInstructionOpcode))
            {
                _singleOperandRegOps[nextInstructionOpcode](nextInstructionMODRR);
                _programCounter += 2;
            }
            else if (_singleOperandMemOps.ContainsKey(nextInstructionOpcode))
            {
                long address = _memory.ReadLong(_programCounter + 2);
                _singleOperandMemOps[nextInstructionOpcode](nextInstructionMODRR, address);
                WriteAddressToFile(addressWriter, address);
                _programCounter += 10;

            }
            else if (_singleOperandImmOps.ContainsKey(nextInstructionOpcode))
            {
                _singleOperandImmOps[nextInstructionOpcode](nextInstructionMODRR, _memory.ReadLong(_programCounter + 2));
                _programCounter += 10;
            }
            else if (_noOperandOps.ContainsKey(nextInstructionOpcode))
            {
                _noOperandOps[nextInstructionOpcode](nextInstructionMODRR);
                _programCounter += 2;
            }
            else
            {
                Console.WriteLine("Opcode not found: " + nextInstructionOpcode);
                Environment.Exit(1);
            }

            instruction++;
        }
        addressWriter.Close();
    }

    public void RegisterRegRegInstruction(byte opcode, Action<byte, byte, byte> operation)
    {
        _regRegOps[opcode] = operation;
    }

    public void RegisterRegTwoOperandOperation(byte opcode, Action<byte, long> operation, bool immediate = false)
    {
        if (immediate)
        {
            _regImmOps[opcode] = operation;
        }
        else
        {
            _regMemOps[opcode] = operation;
        }
    }

    public void RegisterSingleOperandRegOperation(byte opcode, Action<byte> operation)
    {
        _singleOperandRegOps[opcode] = operation;
    }

    public void RegisterSingleOperandOperation(byte opcode, Action<byte, long> operation, bool immediate = false)
    {
        if (immediate)
        {
            _singleOperandImmOps[opcode] = operation;
        }
        else
        {
            _singleOperandMemOps[opcode] = operation;
        }
    }

    public void RegisterNoOperandOperation(byte opcode, Action<byte> operation)
    {
        _noOperandOps[opcode] = operation;
    }

    // register all the necessary operations
    private void InitializeInstructions()
    {
        //MOV
        RegisterRegRegInstruction(0x00, (modrr, r1, r2) =>
        {
            // direct and indirect addressing
            long val;
            if ((modrr & 0x40) == 0x40)
            {
                val = _registers[r2].Value;
            }
            else
            {
                val = _memory.ReadLong(_registers[r2].Value);
            }
            if ((modrr & 0x80) == 0x80)
            {
                _registers[r1].Value = val;
            }
            else
            {
                _memory.WriteLong(_registers[r1].Value, val);
            }
        });
        RegisterRegTwoOperandOperation(0x01, (modrr, addr) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            if ((modrr & 0x80) == 0x80)
            {
                _registers[register].Value = _memory.ReadLong(addr);
            }
            else
            {
                _memory.WriteLong(_registers[register].Value, _memory.ReadLong(addr));
            }
        });
        RegisterRegTwoOperandOperation(0x02, (modrr, addr) =>
        {
            byte register = (byte)((modrr & 0x07));
            if ((modrr & 0x40) == 0x40)
            {
                _memory.WriteLong(addr, _registers[register].Value);
            }
            else
            {
                _memory.WriteLong(addr, _memory.ReadLong(_registers[register].Value));
            }
        });
        RegisterRegTwoOperandOperation(0x03, (modrr, imm) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            if ((modrr & 0x80) == 0x80)
            {
                _registers[register].Value = imm;
            }
            else
            {
                _memory.WriteLong(_registers[register].Value, imm);
            }
        }, true);

        //ADD
        RegisterRegRegInstruction(0x04, (modrr, r1, r2) => { _registers[r1].Value += _registers[r2].Value; });
        RegisterRegTwoOperandOperation(0x05, (modrr, addr) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            _registers[register].Value += _memory.ReadLong(addr);
        });
        RegisterRegTwoOperandOperation(0x06, (modrr, imm) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            _registers[register].Value += imm;
        }, true);

        //SUB
        RegisterRegRegInstruction(0x07, (modrr, r1, r2) => { _registers[r1].Value -= _registers[r2].Value; });
        RegisterRegTwoOperandOperation(0x08, (modrr, addr) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            _registers[register].Value -= _memory.ReadLong(addr);
        });
        RegisterRegTwoOperandOperation(0x09, (modrr, imm) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            _registers[register].Value -= imm;
        }, true);

        //MUL
        RegisterRegRegInstruction(0x0A, (modrr, r1, r2) => { _registers[r1].Value *= _registers[r2].Value; });
        RegisterRegTwoOperandOperation(0x0B, (modrr, addr) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            _registers[register].Value *= _memory.ReadLong(addr);
        });
        RegisterRegTwoOperandOperation(0x0C, (modrr, imm) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            _registers[register].Value *= imm;
        }, true);

        //DIV
        RegisterRegRegInstruction(0x0D, (modrr, r1, r2) => { _registers[r1].Value /= _registers[r2].Value; });
        RegisterRegTwoOperandOperation(0x0E, (modrr, addr) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            _registers[register].Value /= _memory.ReadLong(addr);
        });
        RegisterRegTwoOperandOperation(0x0F, (modrr, imm) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            _registers[register].Value /= imm;
        }, true);

        //CMP
        RegisterRegRegInstruction(0x10, (modrr, r1, r2) =>
        {
            long result = _registers[r1].Value - _registers[r2].Value;
            SetCMPFlags(result);
        });
        RegisterRegTwoOperandOperation(0x11, (modrr, addr) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            long result = _registers[register].Value - _memory.ReadLong(addr);
            SetCMPFlags(result);
        });

        RegisterRegTwoOperandOperation(0x12, (modrr, imm) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            long result = _registers[register].Value - imm;
            SetCMPFlags(result);
        }, true);

        //AND
        RegisterRegRegInstruction(0x13, (modrr, r1, r2) => { _registers[r1].Value &= _registers[r2].Value; });
        RegisterRegTwoOperandOperation(0x14, (modrr, addr) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            _registers[register].Value &= _memory.ReadLong(addr);
        });
        RegisterRegTwoOperandOperation(0x15, (modrr, imm) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            _registers[register].Value &= imm;
        }, true);

        //OR
        RegisterRegRegInstruction(0x16, (modrr, r1, r2) => { _registers[r1].Value |= _registers[r2].Value; });
        RegisterRegTwoOperandOperation(0x17, (modrr, addr) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            _registers[register].Value |= _memory.ReadLong(addr);
        });
        RegisterRegTwoOperandOperation(0x18, (modrr, imm) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            _registers[register].Value |= imm;
        }, true);

        //NOT
        RegisterSingleOperandRegOperation(0x19, (modrr) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            _registers[register].Value = ~_registers[register].Value;
        });

        //XOR
        RegisterRegRegInstruction(0x1A, (modrr, r1, r2) => { _registers[r1].Value ^= _registers[r2].Value; });
        RegisterRegTwoOperandOperation(0x1B, (modrr, addr) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            _registers[register].Value ^= _memory.ReadLong(addr);
        });
        RegisterRegTwoOperandOperation(0x1C, (modrr, imm) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            _registers[register].Value ^= imm;
        }, true);

        //JMP
        RegisterSingleOperandRegOperation(0x1D, (modrr) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            _programCounter = _registers[register].Value - 2;
        });
        RegisterSingleOperandOperation(0x1E, (modrr, addr) => { _programCounter = _memory.ReadLong(addr) - 10; });
        RegisterSingleOperandOperation(0x1F, (modrr, imm) => { _programCounter = imm - 10; }, true);

        //JE
        RegisterSingleOperandRegOperation(0x20, (modrr) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            if ((_flagsRegister.Value & 0x0040) == 0x0040) _programCounter = _registers[register].Value - 2;
        });
        RegisterSingleOperandOperation(0x21, (modrr, addr) => { if ((_flagsRegister.Value & 0x0040) == 0x0040) _programCounter = _memory.ReadLong(addr) - 10; });
        RegisterSingleOperandOperation(0x22, (modrr, imm) => { if ((_flagsRegister.Value & 0x0040) == 0x0040) _programCounter = imm - 10; }, true);

        //JNE
        RegisterSingleOperandRegOperation(0x23, (modrr) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            if ((_flagsRegister.Value & 0x0040) != 0x0040) _programCounter = _registers[register].Value - 2;
        });
        RegisterSingleOperandOperation(0x24, (modrr, addr) => { if ((_flagsRegister.Value & 0x0040) != 0x0040) _programCounter = _memory.ReadLong(addr) - 10; });
        RegisterSingleOperandOperation(0x25, (modrr, imm) => { if ((_flagsRegister.Value & 0x0040) != 0x0040) _programCounter = imm - 10; }, true);

        //JGE
        RegisterSingleOperandRegOperation(0x26, (modrr) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            if ((_flagsRegister.Value & 0x0080) != 0x0080 || (_flagsRegister.Value & 0x0040) == 0x0040) _programCounter = _registers[register].Value;
        });
        RegisterSingleOperandOperation(0x27, (modrr, addr) => { if ((_flagsRegister.Value & 0x0080) != 0x0080 || (_flagsRegister.Value & 0x0040) == 0x0040) _programCounter = _memory.ReadLong(addr); });
        RegisterSingleOperandOperation(0x28, (modrr, imm) => { if ((_flagsRegister.Value & 0x0080) != 0x0080 || (_flagsRegister.Value & 0x0040) == 0x0040) _programCounter = imm; }, true);

        //JL
        RegisterSingleOperandRegOperation(0x29, (modrr) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            if ((_flagsRegister.Value & 0x0040) == 0x0040) _programCounter = _registers[register].Value - 2;
        });
        RegisterSingleOperandOperation(0x2A, (modrr, addr) => { if ((_flagsRegister.Value & 0x0080) == 0x0080) _programCounter = _memory.ReadLong(addr) - 10; });
        RegisterSingleOperandOperation(0x2B, (modrr, imm) => { if ((_flagsRegister.Value & 0x0080) == 0x0080) _programCounter = imm - 10; }, true);

        //WRITE
        RegisterSingleOperandRegOperation(0x2C, (modrr) =>
        {
            byte register = (byte)((modrr & 0x38) >> 3);
            Console.WriteLine((char)_registers[register].Value);
        });

        //READ
        RegisterSingleOperandRegOperation(0x2D, (modrr) =>
        {
            string? input = Console.ReadLine();
            if (input != null)
            {
                byte register = (byte)((modrr & 0x38) >> 3);
                _registers[register].Value = input[0];
            }
        });

        //HALT
        RegisterNoOperandOperation(0x2E, (modrr) =>
        {
            Console.WriteLine("Processor Halted. Press any key to interrupt.");
            _halted = true;
            Console.ReadKey();
            _halted = false;
        });

    }

    private void SetCMPFlags(long value)
    {
        if (value == 0)
        {
            // set ZERO flag, mask 0x0040 
            _flagsRegister.Value = (byte)(_flagsRegister.Value | 0x0040);
            // clear SIGN flag, mask 0x0080
            _flagsRegister.Value = (byte)(_flagsRegister.Value & 0xFF7F);
        }
        else if (value < 0)
        {
            // set SIGN flag, mask 0x0080
            _flagsRegister.Value = (byte)(_flagsRegister.Value | 0x0080);
            // clear ZERO flag, mask 0x0040
            _flagsRegister.Value = (byte)(_flagsRegister.Value & 0xFFBF);
        }
        else
        {
            // clear SIGN flag, mask 0x0080
            _flagsRegister.Value = (byte)(_flagsRegister.Value & 0xFF7F);
            // clear ZERO flag, mask 0x0040
            _flagsRegister.Value = (byte)(_flagsRegister.Value & 0xFFBF);
        }
    }

    private void WriteAddressToFile(StreamWriter sw, long address)
    {
        try
        {
            sw.Write(address.ToString("X8") + "\n");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error while writing to file: " + e.Message);
            Environment.Exit(1);
        }
    }
}
