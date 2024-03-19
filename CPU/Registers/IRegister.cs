/**
 *<summary>
 * Interface for registers. Contains basic register operations such as setting and retrieving values, as well as reseting the register
 * </summary>
 */
public interface IRegister
{
    /** <summary>
     * Holds the value of the register. Can be set and retrieved.
     * </summary>
     */
    long Value { get; set; }

    /**
     * <summary>
     * Resets the register value to 0
     * </summary>
     */
    void Reset();
}
