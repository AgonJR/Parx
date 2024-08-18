using System;
using TMPro;
using UnityEngine;

public class Button3D : MonoBehaviour
{

    public bool clickable = true;

    [Space]
    public Material matClickable;
    public Material matUnClickbl;

    [SerializeField]
    private MeshRenderer _renderRef;
    [SerializeField]
    private TMP_Text _textRef;

    
    
    void Start()
    {
        _renderRef = GetComponent<MeshRenderer>();
        _textRef = GetComponentInChildren<TMP_Text>();
    }


    void OnMouseOver() 
    { 
        if ( clickable && Input.GetMouseButtonDown(0) )
        {
            ClickEvent_NextButton();
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
