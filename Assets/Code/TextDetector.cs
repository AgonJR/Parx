using UnityEngine;

public class TextDetector : MonoBehaviour
{
    public TextMesh OnScreenTextObj;

    [Space]

    public AudioClip sfxTyping;
    public AudioClip sfxFound;
    public AudioClip sfxError;

    [Space]

    private string _currentInput = string.Empty;
    private AudioSource _arRef;

    void Start()
    {
        _arRef = GetComponent<AudioSource>();
    }

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
                    _arRef.pitch += _arRef.pitch <= 1.3 ? 0.05f : 0f;
                    _arRef.PlayOneShot(sfxTyping);
                }
            }
            
            ProcessText();
        }
    }

    private void ProcessText()
    {
        if ( _currentInput.Length > 2 ) // Let player type a couple of letters first
        {
            if (NameGame.Manager.CheckPrefix(_currentInput))
            {
                if (NameGame.Manager.UnlockElement(_currentInput))
                {
                    _arRef.pitch = 1.0f;
                    _arRef.PlayOneShot(sfxFound);
                    _currentInput = string.Empty;
                }
            }
            else
            {
                _arRef.pitch = 1.0f;
                _arRef.PlayOneShot(sfxError);
                _currentInput = string.Empty;
            }
        }

        string displayTxt = _currentInput;

        if (displayTxt.Length > 0) // Capitalize First Letter
        {
            displayTxt = char.ToUpper(displayTxt[0]).ToString() + displayTxt[1..];
        }

        OnScreenTextObj.text = displayTxt;
    }
    
}
