using UnityEngine;

public class ParxBorder : MonoBehaviour
{
    public MeshRenderer rendererRef;
    [Space]
    public Material matOff;
    public Material matOn ;

    public void Toggle(bool o)
    {
        rendererRef.material = o ? matOn : matOff;
    }
}