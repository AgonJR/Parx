using System;
using TMPro;
using UnityEngine;

public class Button3D : MonoBehaviour
{

    public bool clickable = true;
    public EventName Event;

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

    public enum EventName
    {
        Next,
        Upgrade,
        AutoDie
    }

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

            if ( clickable ) 
            {
                switch(Event)
                {
                    case EventName.Next:    ClickEvent_NextButton   (); break;
                    case EventName.Upgrade: ClickEvent_UpgradeButton(); break;
                    case EventName.AutoDie: ClickEvent_ToggleAutoDie(); break;
                }
            }
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

        if ( Event == EventName.Next ) _textRef.text = clickable ? "►" : " -";
    }


    public void ClickEvent_NextButton()
    {
        Enable(false);
        Parx.instance.RegenerateGrid();
    }

    public void ClickEvent_UpgradeButton()
    {
        Enable(false);
        Parx.instance.gridSize++;
        ParxManager.instance.RegenerateBoard = true;
    }

    public void ClickEvent_ToggleAutoDie()
    {
        Parx.instance.autoDie = !Parx.instance.autoDie;
        _textRef.text = Parx.instance.autoDie ? "[x]" : "[ ]";
    }
}
