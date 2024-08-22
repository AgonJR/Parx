using UnityEngine;
using TMPro;

public class ParxBlock : MonoBehaviour
{
    [Header("References")]
    public TMP_Text textComponent;
    public GameObject  frontPlate;
    [Space]
    public GameObject markD;
    public GameObject markX;
    [Space]
    public Material matC1;
    public Material matC2;
    public Color emittClr;

    public Color[] colors;

    private MeshRenderer _renderer;

    private MeshRenderer _marX1;
    private MeshRenderer _marX2;

    private int _x;
    private int _y;

    public void SetText(string text)
    {
        textComponent.text = text;
    }

    public void ToggleMarker(int mark)
    {
        markX.SetActive(mark > 0);
        markD.SetActive(mark < 0);
        
        // _renderer.material.SetColor("_EmissionColor", mark != 0 ? emittClr : Color.black);
    }

    public void ColourMarker(Color c)
    {
        _marX1.material.color = c;
        _marX2.material.color = c;
    }

    public void Init(int x, int y, int clr = 0)
    {
        _renderer = gameObject.GetComponentInChildren<MeshRenderer>();

        _marX1 = markX.GetComponentsInChildren<MeshRenderer>()[0];
        _marX2 = markX.GetComponentsInChildren<MeshRenderer>()[1];

        _x = x;
        _y = y;

        _renderer.material = (x+y)%2==0 ? matC1 : matC2;
        if(clr>0)_renderer.material.color = colors[clr];
    }

    void OnMouseOver() 
    { 
        if (Input.GetMouseButtonDown(0))
        {
            if ( Input.GetKey(KeyCode.LeftShift)) 
                 Parx.instance.PlaceMarker(_x, _y, true);
            else Parx.instance.PlaceTree(_x, _y, 3, true);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Parx.instance.PlaceMarker(_x, _y, true);
        }

        ToggleFrontPlate(true);
    }

    void OnMouseExit()
    {
        ToggleFrontPlate(false);
    }

    public void ToggleFrontPlate(bool tgl)
    {
        frontPlate.SetActive(tgl);
    }

    public void TogglerEmission(bool tgl)
    {
        _renderer.material.SetColor("_EmissionColor", tgl ? emittClr : Color.black);
    }
}
