using UnityEngine;

public class ParxBorder : MonoBehaviour
{
    public Material matOff;
    public Material matOn ;

    private MeshRenderer _renderer;

    void Start()
    {
        _renderer = gameObject.GetComponent<MeshRenderer>();
    }

    public void Toggle(bool o)
    {
        _renderer.material = o ? matOn : matOff;
    }
}