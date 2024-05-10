namespace UnitTests;

public class ProcessorUnitTests
{
    [Fact]
    public void MOVTest()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/MOVTest.txt", "test_programs/MOVTest");
        p.ExecuteProgram("test_programs/MOVTest");
        foreach (IRegister r in p._registers)
            Assert.Equal(15, p._memory.ReadLong(0x1000));
        Assert.Equal(15, p._registers[2].Value);
    }

    [Fact]
    public void ADDTest()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/ADDTest.txt", "test_programs/ADDTest");
        p.ExecuteProgram("test_programs/ADDTest");
        Assert.Equal(3000, p._registers[0].Value);
        Assert.Equal(35, p._registers[2].Value);
        Assert.Equal(3000, p._registers[1].Value);

    }

    [Fact]
    public void SUBTest()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/SUBTest.txt", "test_programs/SUBTest");
        p.ExecuteProgram("test_programs/SUBTest");
        Assert.Equal(1000, p._registers[0].Value);
        Assert.Equal(1000, p._registers[1].Value);
        Assert.Equal(1000, p._registers[2].Value);
    }

    [Fact]
    public void MULTest()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/MULTest.txt", "test_programs/MULTest");
        p.ExecuteProgram("test_programs/MULTest");
        Assert.Equal(20, p._registers[0].Value);
        Assert.Equal(8, p._registers[1].Value);
        Assert.Equal(1000, p._registers[3].Value);
    }

    [Fact]
    public void DIVTest()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/DIVTest.txt", "test_programs/DIVTest");
        p.ExecuteProgram("test_programs/DIVTest");
        Assert.Equal(5, p._registers[0].Value);
        Assert.Equal(1, p._registers[1].Value);
        Assert.Equal(5, p._registers[3].Value);
    }

    [Fact]
    public void CMPTestEqualR()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/CMPTestEqualR.txt", "test_programs/CMPTestEqualR");
        p.ExecuteProgram("test_programs/CMPTestEqualR");
        Assert.Equal(0x0040, p._flagsRegister.Value & 0x0040);
        Assert.Equal(0x0000, p._flagsRegister.Value & 0x0080);
    }

    [Fact]
    public void CMPTestEqualM()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/CMPTestEqualM.txt", "test_programs/CMPTestEqualM");
        p.ExecuteProgram("test_programs/CMPTestEqualM");
        Assert.Equal(0x0040, p._flagsRegister.Value & 0x0040);
        Assert.Equal(0x0000, p._flagsRegister.Value & 0x0080);
    }

    [Fact]
    public void CMPTestEqualIMM()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/CMPTestEqualIMM.txt", "test_programs/CMPTestEqualIMM");
        p.ExecuteProgram("test_programs/CMPTestEqualIMM");
        Assert.Equal(0x0040, p._flagsRegister.Value & 0x0040);
        Assert.Equal(0x0000, p._flagsRegister.Value & 0x0080);
    }

    [Fact]
    public void CMPTestNotEqualR()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/CMPTestNotEqualR.txt", "test_programs/CMPTestNotEqualR");
        p.ExecuteProgram("test_programs/CMPTestNotEqualR");
        Assert.Equal(0x0000, p._flagsRegister.Value & 0x0040);
        Assert.Equal(0x0080, p._flagsRegister.Value & 0x0080);
    }

    [Fact]
    public void CMPTestNotEqualM()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/CMPTestNotEqualM.txt", "test_programs/CMPTestNotEqualM");
        p.ExecuteProgram("test_programs/CMPTestNotEqualM");
        Assert.Equal(0x0000, p._flagsRegister.Value & 0x0040);
        Assert.Equal(0x0080, p._flagsRegister.Value & 0x0080);
    }

    [Fact]
    public void CMPTestNotEqualIMM()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/CMPTestNotEqualIMM.txt", "test_programs/CMPTestNotEqualIMM");
        p.ExecuteProgram("test_programs/CMPTestNotEqualIMM");
        Assert.Equal(0x0000, p._flagsRegister.Value & 0x0040);
        Assert.Equal(0x0080, p._flagsRegister.Value & 0x0080);
    }

    [Fact]
    public void ANDTest()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/ANDTest.txt", "test_programs/ANDTest");
        p.ExecuteProgram("test_programs/ANDTest");
        Assert.Equal(2, p._registers[0].Value);
        Assert.Equal(1, p._registers[1].Value);
        Assert.Equal(14, p._registers[3].Value);
    }

    [Fact]
    public void ORTest()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/ORTest.txt", "test_programs/ORTest");
        p.ExecuteProgram("test_programs/ORTest");
        Assert.Equal(6, p._registers[0].Value);
        Assert.Equal(7, p._registers[1].Value);
        Assert.Equal(3, p._registers[3].Value);
    }

    [Fact]
    public void NOTTest()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/NOTTest.txt", "test_programs/NOTTest");
        p.ExecuteProgram("test_programs/NOTTest");
        Assert.Equal(-2, p._registers[0].Value);
        Assert.Equal(1, p._registers[1].Value);
    }

    [Fact]
    public void XORTest()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/XORTest.txt", "test_programs/XORTest");
        p.ExecuteProgram("test_programs/XORTest");
        Assert.Equal(7, p._registers[0].Value);
        Assert.Equal(4, p._registers[1].Value);
        Assert.Equal(4, p._registers[3].Value);
    }

    [Fact]
    public void JMPTest()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/JMPTest.txt", "test_programs/JMPTest");
        p.ExecuteProgram("test_programs/JMPTest");
        Assert.Equal(10, p._registers[0].Value);
        Assert.Equal(0, p._registers[1].Value);
        Assert.Equal(25, p._registers[2].Value);
        Assert.Equal(15, p._registers[3].Value);
    }

    [Fact]
    public void JETest()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/JETest.txt", "test_programs/JETest");
        p.ExecuteProgram("test_programs/JETest");
        Assert.Equal(30, p._registers[0].Value);
        Assert.Equal(15, p._registers[2].Value);
        Assert.Equal(5, p._registers[3].Value);
    }

    [Fact]
    public void JNETest()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/JNETest.txt", "test_programs/JNETest");
        p.ExecuteProgram("test_programs/JNETest");
        Assert.Equal(20, p._registers[0].Value);
        Assert.Equal(10, p._registers[1].Value);
        Assert.Equal(15, p._registers[2].Value);
        Assert.Equal(0, p._registers[3].Value);
    }

    [Fact]
    public void JGETest()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/JGETest.txt", "test_programs/JGETest");
        p.ExecuteProgram("test_programs/JGETest");
        Assert.Equal(8, p._registers[2].Value);
        Assert.Equal(0, p._registers[3].Value);
    }

    [Fact]
    public void JLTest()
    {
        Processor p = new Processor();
        Compiler.Instance.Compile("test_programs/JLTest.txt", "test_programs/JLTest");
        p.ExecuteProgram("test_programs/JLTest");
        Assert.Equal(40, p._registers[2].Value);
        Assert.Equal(15, p._registers[1].Value);
    }
}
