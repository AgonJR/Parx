using UnityEngine;
using UnityEngine.Events;

public class Button3DElementMenu : MonoBehaviour
{
    public MeshRenderer rendererRef;

    [Space]

    public AudioClip sfxClick;
    public UnityEvent onClick;

    [Space]

    private AudioSource _arRef;

    void Start()
    { 
        _arRef = GetComponent<AudioSource>();
    }

    void OnMouseEnter()
    {
        // ---
        // Highlight
    }

    void OnMouseOver() 
    { 
        DetectClicks();
    }

    void DetectClicks()
    {
        if ( Input.GetMouseButtonDown(0) )
        { 
            _arRef.PlayOneShot(sfxClick);
            onClick.Invoke();
        }
    }

    void OnMouseExit()
    {
        // ---
    }

}
