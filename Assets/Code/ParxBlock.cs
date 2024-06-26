using UnityEngine;
using TMPro;
using System;

public class ParxBlock : MonoBehaviour
{
    [Header("References")]
    public TMP_Text textComponent;
    public GameObject  frontPlate;

    public Material matC1;
    public Material matC2;

    private MeshRenderer _renderer;

    private int _x;
    private int _y;

    public void SetText(string text)
    {
        textComponent.text = text;
    }

    public void Init(int x, int y)
    {
        _renderer = gameObject.GetComponentInChildren<MeshRenderer>();

        _x = x;
        _y = y;

        _renderer.material = (x+y)%2==0 ? matC1 : matC2;
    }

    void OnMouseOver() 
    { 
        if (Input.GetMouseButtonDown(0))
        {
            Parx.instance.PlaceTree(_x, _y, 3);
        }
    }

    public void ToggleFrontPlate(bool tgl)
    {
        frontPlate.SetActive(tgl);
    }
}
