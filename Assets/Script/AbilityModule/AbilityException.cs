using System;

public class AbilityException : Exception
{
    private string message = "[AbilityModule] Error Code: %s";
    public override string Message
    {
        get
        {
            return message;
        }
    }

    public AbilityException(string message) : base(message)
    {
        this.message = string.Format(this.message, message);
    }
}
