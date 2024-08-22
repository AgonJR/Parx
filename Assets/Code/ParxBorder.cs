using System;
using UnityEngine;

public class ParxBorder : MonoBehaviour
{
    public MeshRenderer rendererRef;
    [Space]
    public Material matNut;
    public Material matOff;
    public Material matOn ;

    public void Toggle(int t)
    {
        rendererRef.material = t == 1 ? matOn : t > 1 ? matOff : matNut;
    }
}