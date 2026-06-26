using UnityEngine;

public class Button3DElement : MonoBehaviour
{
    public TextMesh txtSymbol;
    public TextMesh txtAtomicNumber;
    public TextMesh txtNameSubtitle;
    public MeshRenderer rendererRef;

    [Space]

    public AudioClip sfxClick;
    public AudioClip sfxDblClick;

    [Space]

    public Material matUnlocked;

    [Space]

    public ElementData eData;

    [Space]

    private AudioSource  _arRef;

    private float _pingDurr = -1f;
    private float _pingLerp = -1f;
    private Color _pingStartColor;
    private Color _pingEndColour;
    private float _lastClickTime;
    private Material _matDefault;
    private MeshRenderer _mshRdr;

    void Awake()
    {
        _mshRdr = rendererRef;
        _matDefault = _mshRdr.material;
        _arRef = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (_pingLerp >= 0)
        {
            float pingElapsed = 1.0f - (_pingLerp / _pingDurr);
            Color lerpedC = Color.Lerp(_pingStartColor, _pingEndColour, pingElapsed);

            _pingLerp -= Time.deltaTime;
            SetTextColor(lerpedC);
        }
        else if (_pingEndColour == Color.clear && !eData.Unlocked)
        {
            SetText_AtomicNumber(string.Empty);
            SetText_Symbol(string.Empty);
            SetText_Name(string.Empty);
        }
    }

    void OnMouseEnter()
    {
        if (eData.Unlocked) 
        {
            SetText_Name(eData.DisplayName);
            SetTextColor(Color.cyan);
            _pingLerp = -1.0f;
        }
        else
        {
            PingTextColor(Color.white, 0.13f);
        }
    }

    void OnMouseOver() 
    { 
        DetectClicks();
    }

    void DetectClicks()
    {
        if ( Input.GetMouseButtonDown(0) )
        { 
            float timeSinceLastClick = Time.time - _lastClickTime;

            if (timeSinceLastClick <= 0.2f)
            {
                if (!eData.Unlocked)
                {
                    // TODO: Double Click = Display Hint 
                }
            }

            _lastClickTime = Time.time;

            if (eData.Unlocked) 
            {
                eData.DisplayNameIndex = (eData.DisplayNameIndex + 1) % eData.Names.Count;
                PingTextColor(Color.gold, Color.cyan, 0.5f);
                SetText_Name(eData.DisplayName);
                _arRef.PlayOneShot(sfxClick);
            }
            else if (!eData.Unlocked)
            {
                _arRef.pitch = 0.96f;
                _arRef.PlayOneShot(sfxClick);
                SetText_AtomicNumber(eData.Number.ToString());
                SetText_Symbol(eData.Symbol);
                SetTextColor(Color.white, true);
            }
        }
    }

    void OnMouseExit()
    {
        if (eData.Unlocked)
            PingTextColor(Color.cyan, Color.white, 0.5f);
        else
            PingTextColor(Color.white, Color.clear, 0.5f);

        SetText_Name(string.Empty);
    }

    public void SetTextColor(Color c, bool endLerp = false)
    {
        txtSymbol.color = c;
        txtAtomicNumber.color = c;
        txtNameSubtitle.color = c;

        if (endLerp)
        {
            _pingEndColour = c; 
            _pingLerp = -1.0f;
        }
    }

    public void PingTextColor(Color start, Color end, float duration)
    {
        _pingStartColor = start; 
        _pingEndColour = end;
        _pingLerp = duration;
        _pingDurr = duration;
    }


    public void PingTextColor(Color end, float duration)
    {
        if (_pingLerp >= 0) // Changing mid-lerp
        {
            float pingElapsed = 1.0f - (_pingLerp / _pingDurr);
            _pingStartColor = Color.Lerp(_pingStartColor, _pingEndColour, pingElapsed); 
        }

        _pingEndColour = end;
        _pingLerp = duration;
        _pingDurr = duration;
    }

    public void SetText_Symbol(string txt)
    {
        txtSymbol.text = txt;
    }

    public void SetText_AtomicNumber(string txt)
    {
        txtAtomicNumber.text = txt;
    }

    public void SetText_Name(string name)
    {
        txtNameSubtitle.text = TextDetector.CapitalizeFirstLetter(name);
    }

    public void UpdateMaterial()
    {
        _mshRdr.material = eData.Unlocked ? matUnlocked : _matDefault;
    }
}
