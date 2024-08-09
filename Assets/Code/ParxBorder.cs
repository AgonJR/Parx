using UnityEngine;

public class ParxBorder : MonoBehaviour
{
    public MeshRenderer rendererRef;
    [Space]
    public Material matNut;
    public Material matOff;
    public Material matOn ;

    public void Toggle(bool o)
    {
        rendererRef.material = o ? matOn : matOff;
    }

    public void Extinguish()
    {
        rendererRef.material = matNut;
    }
}