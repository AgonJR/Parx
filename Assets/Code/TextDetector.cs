using UnityEngine;

public class TextDetector : MonoBehaviour
{
    public TextMesh OnScreenTextObj;
    private string _currentInput = "";

    void Update()
    {
        DetectInput();
    }

    private void DetectInput()
    {
        if ( Input.inputString.Equals(string.Empty) == false )
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b' && _currentInput.Length > 0) // Backspace
                {
                    _currentInput = _currentInput.Substring(0, _currentInput.Length - 1);
                }
                else if (c == '\n' || c == '\r') // Enter/Return
                {
                    _currentInput = "";
                }
                else
                {
                    _currentInput += c;
                }
            }
            
            ProcessText(_currentInput);
        }
    }

    private void ProcessText(string playerInput)
    {
        string displayTxt = playerInput;

        if (displayTxt.Length > 0) // Capitalize First Letter
        {
            displayTxt = char.ToUpper(displayTxt[0]).ToString() + displayTxt[1..];
        }

        // TODO: Replace with blinking cursor (separate script?)
        while(displayTxt.Length < 3)
        {
            displayTxt += '_';
        }

        OnScreenTextObj.text = displayTxt;
    }
}
