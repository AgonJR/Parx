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
    private TextFade _errFade;
    private TextFade _crrFade;

    public void EnableSFX(bool on) => _arRef.enabled = on;

    private float _lastInputTime = 0.0f;
    private float _inputLock = -1.0f;

    void Start()
    {
        _arRef = GetComponent<AudioSource>();
        _errAnimator = OnScreenTextErr.GetComponent<Animator>();
        _crrAnimator = OnScreenTextCrr.GetComponent<Animator>();
        _errFade =  OnScreenTextErr.GetComponent<TextFade>();
        _crrFade =  OnScreenTextCrr.GetComponent<TextFade>();
        _currentInput = string.Empty;
    }

    void Update()
    {
        if (CheckLock()) { if (DetectInput()) { ProcessText(); } }
    }

    private bool DetectInput()
    {
        if (NameGame.Manager.Paused) return CheckForESC();

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
                        PlaySFX(sfxDelete);
                        _currentInput = _currentInput.Substring(0, _currentInput.Length - 1);
                    }
                    else
                        return false;
                }
                else if (c == '\u001b') // Unicode for ESC
                {
                    NameGame.PauseGame(true);
                }
                else if (c == '0') 
                {
                    #if UNITY_EDITOR
                        PlaySFX(sfxFound); // TODO: REMOVE for Release
                        NameGame.Manager.UnlockAll();
                    #endif
                }
                else if (!char.IsLetter(c))
                {
                    PlaySFX(sfxBadIn);
                    return false;
                }
                else
                {
                    _currentInput += c;
                    // _arRef.pitch += _arRef.pitch <= 1.3 ? 0.03f : 0f;
                    // PlaySFX(sfxTyping);
                }
            }
            
            return true;
        }

        return false;
    }

    private bool CheckForESC()
    {
        if ( Input.inputString.Equals(string.Empty) == false )
        {
            foreach (char c in Input.inputString)
            {
                if (c == ' ' || c == '\n' || c == '\u001b')
                {
                    NameGame.PauseGame(false);
                }
            }
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
                if (Input.inputString[0] == '\b') // Backspace
                {
                    // "Fixes" latest mistake if backspace pressed during lock
                    _currentInput = OnScreenTextErr.text.Substring(0, OnScreenTextErr.text.Length - 1);
                    OnScreenTextErr.text = string.Empty;
                    UpdateDisplayText();
                    PlaySFX(sfxDelete);
                }
                else
                {
                    PlaySFX(sfxLocked);
                }

                _inputLock = -1;
                _errFade.Fade(false, 0.25f);
            }

            if ( _inputLock <= 0 )
            {
                _inputLock = -1;
                _errFade.Fade(false, 0.5f);
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
                    _arRef.pitch = 1.0f; PlaySFX(sfxFound);

                    _crrAnimator.gameObject.SetActive(true);
                    OnScreenTextCrr.text = CapitalizeFirstLetter(_currentInput);
                    _crrAnimator.Play("InputCorrect", 0, 0f);
                    _crrFade.Fade(false, 0.7f, 1.3f);

                    _currentInput = string.Empty;
                }
            }
            else
            {
                _arRef.pitch = 1.0f;
                PlaySFX(sfxError);
                _inputLock = ErrorLockDuration;

                _errFade.Fade(true, 0.03f); //Reset Color
                _errAnimator.gameObject.SetActive(true);
                OnScreenTextErr.text = CapitalizeFirstLetter(_currentInput);
                _errAnimator.Play("InputError", 0, 0f);

                _currentInput = string.Empty;
            }
        }

        UpdateDisplayText();
    }

    private void UpdateDisplayText()
    {
        string displayTxt = CapitalizeFirstLetter(_currentInput);
        OnScreenTextObj.text = displayTxt;
    }

    public static string CapitalizeFirstLetter(string txt)
    {
        if (txt.Length > 0) { txt = char.ToUpper(txt[0]).ToString() + txt[1..]; }
        return txt;
    }

    public void PlaySFX(AudioClip sfx)
    {
        if ( NameGame.Manager.SFX ) { _arRef.PlayOneShot(sfx); }
    }

    public void ResetInput()
    {
        _currentInput = string.Empty;
        OnScreenTextObj.text = "Type to Start";
    }
}
