using UnityEngine;

public class Button3DElement : MonoBehaviour
{
    public TextMesh txtSymbol;
    public TextMesh txtAtomicNumber;

    [Space]

    public AudioClip sfxClick;

    [Space]

    private AudioSource  _arRef;

    private float _pingDurr = -1f;
    private float _pingLerp = -1f;
    private Color _pingColor;

    void Awake()
    {
        _arRef = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (_pingLerp >= 0)
        {
            float pingElapsed = 1.0f - (_pingLerp / _pingDurr);
            Color lerpedC = Color.Lerp(_pingColor, Color.white, pingElapsed);

            _pingLerp -= Time.deltaTime;
            SetTextColor(lerpedC);
        }
    }

    void OnMouseOver() 
    { 
        if ( Input.GetMouseButtonDown(0) )
        { _arRef.PlayOneShot(sfxClick); }
        SetTextColor(Color.green);
        _pingLerp = -1.0f;
    }

    void OnMouseExit()
    {
        SetTextColor(Color.white);
    }

    public void SetTextColor(Color c)
    {
        txtSymbol.color = c;
        txtAtomicNumber.color = c;
    }

    public void PingTextColor(Color c, float duration)
    {
        _pingColor = c;
        SetTextColor(c);
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
}
