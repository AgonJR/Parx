using UnityEngine;

public class Button3DElement : MonoBehaviour
{
    public TextMesh txtSymbol;
    public TextMesh txtAtomicNumber;

    [Space]

    public AudioClip sfxClick;

    [Space]

    private AudioSource  _arRef;

    void Awake()
    {
        _arRef = GetComponent<AudioSource>();
    }

    void OnMouseOver() 
    { 
        if ( Input.GetMouseButtonDown(0) )
        {
            _arRef.PlayOneShot(sfxClick);
        }
        
        txtSymbol.color = Color.green;
        txtAtomicNumber.color = Color.green;
    }

    void OnMouseExit()
    {
        txtSymbol.color = Color.white;
        txtAtomicNumber.color = Color.white;
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
