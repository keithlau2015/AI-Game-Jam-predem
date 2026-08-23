using System;

public class PromptDotController
{
    public event Action<int> onValueChangeEvent;

    public void CallUpdate(int count)
    {
        onValueChangeEvent?.Invoke(count);
    }
}
