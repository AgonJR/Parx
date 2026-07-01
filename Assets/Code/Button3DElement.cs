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
    private float _timeOnEnter;

    // -----------------------------------------------------------------------------------------------

    void Awake()
    {
        _mshRdr = rendererRef;
        _matDefault = _mshRdr.material;
        _arRef = GetComponent<AudioSource>();
        
        txtSymbol.gameObject.SetActive(true);
        txtAtomicNumber.gameObject.SetActive(true);
        txtNameSubtitle.gameObject.SetActive(true);

        // Zoom Data

        _scaleAtStart = rendererRef.transform.localScale;
        _scaleAtZoom = _scaleAtStart * 1.39f;

        _posAtStart = rendererRef.transform.localPosition;
        _posAtZoom = new Vector3(_posAtStart.x, _posAtStart.y + 3.1f, _posAtStart.z);

        _colorBeforeZoom = matUnlocked.color;
        _colorAfterZoom = _colorBeforeZoom; 
        _colorAfterZoom.a = 1.0f;

        SetText_Name(string.Empty);
        SetTextColor(Color.clear);
    }

    void FixedUpdate()
    {
        UpdateTextColor();
        UpdateZoom();
    }

    private void UpdateTextColor()
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

    private void UpdateZoom()
    {
        if (_zoomLerp >= 0)
        {
            float elapsed = 1.0f - (_zoomLerp / _zoomLerpDuration);

            Color lerpedColor;
            Vector3 lerpedScl;
            Vector3 lerpedPos;

            if (_zoomUp)
            {
                lerpedColor = Color.Lerp(_colorBeforeZoom, _colorAfterZoom, elapsed);
                lerpedScl = Vector3.Lerp(_scaleAtStart, _scaleAtZoom, elapsed);
                lerpedPos = Vector3.Lerp(_posAtStart, _posAtZoom, elapsed);
            }
            else
            {
                lerpedColor = Color.Lerp(_colorAfterZoom, _colorBeforeZoom, elapsed);
                lerpedScl = Vector3.Lerp(_scaleAtZoom, _scaleAtStart, elapsed);
                lerpedPos = Vector3.Lerp(_posAtZoom, _posAtStart, elapsed);
            }

            rendererRef.transform.localScale = lerpedScl;
            rendererRef.transform.localPosition = lerpedPos;
            rendererRef.material.color = lerpedColor;

            _zoomLerp -= Time.deltaTime;

            if (_zoomLerp <= 0 && !_zoomUp)
            {
                rendererRef.material = eData.Unlocked ? matUnlocked : _matDefault;
            }
        }
    }

    void OnMouseEnter()
    {
        _timeOnEnter = Time.time;

        if (eData.Unlocked) 
        {
            SetText_Name(eData.DisplayName);
            Zoom(true);
        }
        else
        {
            SetText_Symbol(eData.Symbol);
            PingTextColor(Color.white, _pingEndColour == Color.clear ? 0.31f : 1.13f);
        }
    }

    void OnMouseExit()
    {
        if (eData.Unlocked)
        {
            Zoom(false);
            if ( Time.time < (_timeOnEnter + 0.3f) )
                PingTextColor(Color.cyan, Color.white, 0.631f);
        }
        else
        {
            PingTextColor(Color.white, Color.clear, 0.31f);
        }

        SetText_Name(string.Empty);
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
            bool doubleClick = timeSinceLastClick <= 0.369f;

            _lastClickTime = Time.time;

            if (eData.Unlocked) 
            {
                if (NameGame.Manager.SFX)
                { 
                    _arRef.pitch = eData.Names.Count > 1 ? 1.13f : 1.01f;
                    _arRef.PlayOneShot(sfxClick);
                }

                eData.DisplayNameIndex = (eData.DisplayNameIndex + 1) % eData.Names.Count;
                PingTextColor(eData.Names.Count > 1 ? Color.gold : Color.cyan, Color.white, 0.31f);
                SetText_Name(eData.DisplayName);
            }
            else if (!eData.Unlocked)
            {
                if (NameGame.Manager.SFX) 
                { 
                    _arRef.pitch = doubleClick ? 0.99f : 0.96f;
                    _arRef.PlayOneShot(sfxClick);
                }
                
                NameGame.ShowHint(eData.Hints[eData.HintsIndex], eData.Number);
                if (!NameGame.Manager.HintDisplayed) 
                { 
                    eData.HintsIndex = (eData.HintsIndex + 1) % eData.Hints.Count;
                }

                SetTextColor(Color.white, true);
            }
        }
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

    // Zoom,
    // - increases tile size, ascends above others
    // - material made opaque
    
    private bool _zoomUp;
    private float _zoomLerp = -1f;
    private float _zoomLerpDuration = 1f;
    private Vector3 _scaleAtStart = Vector3.zero;
    private Vector3 _scaleAtZoom = Vector3.zero;
    private Vector3 _posAtStart = Vector3.zero;
    private Vector3 _posAtZoom = Vector3.zero;
    private Color _colorBeforeZoom;
    private Color _colorAfterZoom;

    private void Zoom(bool up, float duration = 0.13f)
    {
        _zoomUp = up;
        _zoomLerp = duration;
        _zoomLerpDuration = duration;
        rendererRef.shadowCastingMode = up ? UnityEngine.Rendering.ShadowCastingMode.On 
                                           : UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    public void ResetZoom()
    {
        _zoomLerp = -1f;
        _zoomLerpDuration = 1f;
        rendererRef.transform.localScale = _scaleAtStart;
        rendererRef.transform.localPosition = _posAtStart;
    }
}
