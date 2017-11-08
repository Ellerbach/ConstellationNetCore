namespace Constellation
{
    //
    // Summary:
    //     Specifies the level for log message.
    public enum LogLevel
    {
        //
        // Summary:
        //     Debug level (never send to the Constellation hub)
        Debug = 0,
        //
        // Summary:
        //     Information level
        Info = 1,
        //
        // Summary:
        //     Warning level
        Warn = 2,
        //
        // Summary:
        //     Error level
        Error = 3,
        //
        // Summary:
        //     Fatal error level
        Fatal = 4
    }
}