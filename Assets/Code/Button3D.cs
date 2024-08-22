using System;
using TMPro;
using UnityEngine;

public class Button3D : MonoBehaviour
{

    public bool clickable = true;

    [Space]

    public Material matClickable;
    public Material matUnClickbl;

    [Space]

    public AudioClip sfxClickable;
    public AudioClip sfxUnClickbl;

    [Space]

    private MeshRenderer _renderRef;
    private TMP_Text     _textRef;
    private AudioSource  _arRef;


    void Start()
    {
        _renderRef  = GetComponent<MeshRenderer>();
        _textRef    = GetComponentInChildren<TMP_Text>();
        _arRef      = GetComponent<AudioSource>();
    }

    void OnMouseOver() 
    { 
        if ( Input.GetMouseButtonDown(0) )
        {
            _arRef.PlayOneShot(clickable ? sfxClickable : sfxUnClickbl);
            if ( clickable ) ClickEvent_NextButton();
        }

        _textRef.color = clickable ? Color.green : Color.red;
    }

    void OnMouseExit()
    {
        _textRef.color = Color.white;
    }


    public void Enable(bool toggle)
    {
        clickable = toggle;

        _renderRef.material = clickable ? matClickable : matUnClickbl;
    }


    public void ClickEvent_NextButton()
    {
        Enable(false);
        Parx.instance.RegenerateGrid();
    }
}
