using UnityEngine;
using UnityEngine.InputSystem;

namespace EvtModule
{
    public class KeyPressedObserable : EvtObserable
    {
        private enum Operator
        {
            Equal = 0,
        }

        [SerializeField]
        private char targetChar;

        [SerializeField]
        private Operator operatorValue;

        private void Awake()
        {
            if (Keyboard.current != null)
                Keyboard.current.onTextInput += OnPress;
        }

        private void OnDestroy()
        {
            if (Keyboard.current != null)
                Keyboard.current.onTextInput -= OnPress;
        }

        public void OnPress(char c)
        {
            bool isFullfilled = false;
            if (operatorValue == Operator.Equal)
                isFullfilled = c.Equals(targetChar);

            if (isFullfilled)
                Notify();
        }
    }
}
