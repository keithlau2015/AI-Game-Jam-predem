using System;

public class FormulaException : Exception
{
    private string message = "[FormulaModule] Error Code: %s";
    public override string Message
    {
        get
        {
            return message;
        }
    }

    public FormulaException(string message) : base(message)
    {
        this.message = string.Format(this.message, message);
    }
}