using UnityEngine;

public class ParxBorder : MonoBehaviour
{
    public Light lightRef;
    public MeshRenderer rendererRef;
    [Space]
    public Material matOff;
    public Material matOn ;

    public void Toggle(bool o)
    {
        rendererRef.material = o ? matOn : matOff;
        lightRef.color = rendererRef.material.color;
    }
}