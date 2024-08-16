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

    public void Init(int x, int y, int clr = 0)
    {
        _renderer = gameObject.GetComponentInChildren<MeshRenderer>();

        _x = x;
        _y = y;

        _renderer.material = (x+y)%2==0 ? matC1 : matC2;
        if(clr>0)_renderer.material.color = colors[clr];
    }

    private int clrIndx = 1;
    void OnMouseOver() 
    { 
        if (Input.GetMouseButtonDown(0))
        {
            if ( Input.GetKey(KeyCode.LeftShift) )
            {
                _renderer.material.color = colors[clrIndx];
                clrIndx++;
                if ( clrIndx >= colors.Length ) clrIndx = 0;
            }
            else
            {
                Parx.instance.PlaceTree(_x, _y, 3);
            }
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
}
