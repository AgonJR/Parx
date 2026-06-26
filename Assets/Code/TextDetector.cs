using UnityEngine;

public class TextDetector : MonoBehaviour
{
    public TextMesh OnScreenTextObj;
    public TextMesh OnScreenTextErr;
    public TextMesh OnScreenTextCrr;

    [Space]

    public float ErrorLockDuration;

    [Space] 
    [Header("Audio")]
    public AudioClip sfxTyping;
    public AudioClip sfxDelete;
    public AudioClip sfxLocked;
    public AudioClip sfxFound;
    public AudioClip sfxError;
    public AudioClip sfxBadIn;

    [Space]

    private string _currentInput = string.Empty;
    private Animator _errAnimator;
    private Animator _crrAnimator;
    private AudioSource _arRef;

    private float _lastInputTime = 0.0f;
    private float _inputLock = -1.0f;

    void Start()
    {
        _arRef = GetComponent<AudioSource>();
        _errAnimator = OnScreenTextErr.GetComponent<Animator>();
        _crrAnimator = OnScreenTextCrr.GetComponent<Animator>();
        _currentInput = string.Empty;
    }

    void Update()
    {
        if (CheckLock()) { if (DetectInput()) { ProcessText(); } }
    }

    private bool DetectInput()
    {
        if ( Input.inputString.Equals(string.Empty) == false )
        {
            _errAnimator.gameObject.SetActive(false); // Clear previous error
            _crrAnimator.gameObject.SetActive(false); // Clear previous answer

            if ( Time.time - _lastInputTime > 1.0f ) // Reset pitch between pauses
            { _arRef.pitch = 1.0f; } _lastInputTime = Time.time;

            foreach (char c in Input.inputString)
            {
                if (c == '\b') // Backspace
                {
                    if (_currentInput.Length > 0)
                    {
                        _arRef.PlayOneShot(sfxDelete);
                        _currentInput = _currentInput.Substring(0, _currentInput.Length - 1);
                    }
                    else
                        return false;
                }
                else if (c == '\u001b') // Unicode for Escape
                {
                    // TODO: open a pause menu instead, with an option to reset
                    _arRef.PlayOneShot(sfxDelete);
                    NameGame.Manager.Reset();
                }
                else if (c == '0') 
                {
                    // TODO: REMOVE THIS, it's for testing only
                    _arRef.PlayOneShot(sfxFound);
                    NameGame.Manager.UnlockAll();
                }
                else if (!char.IsLetter(c))
                {
                    _arRef.PlayOneShot(sfxBadIn);
                    return false;
                }
                else
                {
                    _currentInput += c;
                    // _arRef.pitch += _arRef.pitch <= 1.3 ? 0.03f : 0f;
                    // _arRef.PlayOneShot(sfxTyping);
                }
            }
            
            return true;
        }

        return false;
    }

    private bool CheckLock()
    {
        if (_inputLock > 0)
        {
            _inputLock -= Time.deltaTime;

            if (!Input.inputString.Equals(string.Empty))
            {
                _inputLock = -1;
                // TODO: error message fadeout here
                _arRef.PlayOneShot(sfxLocked);
            }

            return false;
        }

        return true;
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

                    _crrAnimator.gameObject.SetActive(true);
                    OnScreenTextCrr.text = CapitalizeFirstLetter(_currentInput);
                    _crrAnimator.Play("InputCorrect", 0, 0f);

                    _currentInput = string.Empty;
                }
            }
            else
            {
                _arRef.pitch = 1.0f;
                _arRef.PlayOneShot(sfxError);
                _inputLock = ErrorLockDuration;

                _errAnimator.gameObject.SetActive(true);
                OnScreenTextErr.text = CapitalizeFirstLetter(_currentInput);
                _errAnimator.Play("InputError", 0, 0f);

                _currentInput = string.Empty;
            }
        }

        string displayTxt = CapitalizeFirstLetter(_currentInput);

        OnScreenTextObj.text = displayTxt;
    }

    private string CapitalizeFirstLetter(string txt)
    {
        if (txt.Length > 0) { txt = char.ToUpper(txt[0]).ToString() + txt[1..]; }
        return txt;
    }

}
