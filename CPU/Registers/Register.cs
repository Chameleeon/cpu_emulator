public class Register64 : IRegister
{
    private long _value;

    public long Value
    {
        get { return this._value; }
        set { this._value = value; }
    }

    public void Reset()
    {
        this._value = 0;
    }
}

