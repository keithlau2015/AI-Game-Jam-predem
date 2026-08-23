using System;
public class EquipmentException : Exception
{
    private string message = "[EquipmentSystem] Error Code: %s";
    public override string Message {
        get { 
            return message; 
        }
    }

    public EquipmentException(string message) : base(message) {
        this.message = string.Format(this.message, message);
    }
}