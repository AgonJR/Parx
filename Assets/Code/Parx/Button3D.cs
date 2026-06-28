using System;
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
    private TextMesh     _textRef;
    private AudioSource  _arRef;

    private bool isNameGame = false;

    public enum EventName
    {
        Next,
        Upgrade,
        AutoDie,
        Toggle,
    }

    void Awake()
    {
        _renderRef  = GetComponent<MeshRenderer>();
        _textRef    = GetComponentInChildren<TextMesh>();
        _arRef      = GetComponent<AudioSource>();
    }

    void Start()
    {
        isNameGame = NameGame.Manager != null;
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
                    case EventName.Toggle:  ClickEvent_ToggleText   (); break;
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

    public void SetUpgradeCD(int cd)
    {
        if ( Event == EventName.Upgrade ) { _textRef.text = cd <= 0 ? "+" : cd.ToString(); }
    }

    public void ClickEvent_NextButton()
    {
        Enable(false);
        Parx.instance.RegenerateGrid();
    }

    public void ClickEvent_UpgradeButton()
    {
        Enable(false);
        Parx.instance.IncSize();
        ParxManager.instance.RegenerateBoard = true;
    }

    public void ClickEvent_ToggleAutoDie()
    {
        Parx.instance.autoDie = !Parx.instance.autoDie;
        _textRef.text = Parx.instance.autoDie ? "[x]" : "[ ]";
    }

    private bool toggledOn = false;
    public void ClickEvent_ToggleText()
    {
        if (isNameGame) return;

        toggledOn = !toggledOn;
        _textRef.text = toggledOn ? "[x]" : "[o]";
    }

    public void SetText_External(string txt)
    {
        _textRef.text = txt;
    }
}
