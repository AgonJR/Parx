using UnityEngine;
using UnityEngine.Events;

public class Button3DElementMenu : MonoBehaviour
{
    public float holdToActivate = 0.0f;
    public MeshRenderer rendererRef;
    public Transform progressBar;
    public GameObject Highlight;

    [Space]

    public AudioClip sfxClick;
    public UnityEvent onClick;

    [Space]

    private Vector3 _progressZero;
    private AudioSource _arRef;
    private float _pressStart;
    private bool _pressed;

    void Start()
    { 
        _arRef = GetComponent<AudioSource>();
        _progressZero = new Vector3(0, 1, 1);
    }

    void FixedUpdate()
    {
        if (_pressed && holdToActivate > 0)
        {
            float elapsed = Time.time - _pressStart;

            float progress = Mathf.Min(elapsed/holdToActivate, 1);
            progressBar.localScale = new Vector3(progress, 1, 1);

            if (elapsed > holdToActivate) Activate();
        }
    }

    void OnMouseEnter()
    {
        Highlight.SetActive(true);
    }

    void OnMouseOver() 
    { 
        DetectClicks();
    }

    void DetectClicks()
    {
        if ( Input.GetMouseButtonDown(0) )
        { 
            _pressed = true;
            _pressStart = Time.time;
            if (holdToActivate <= 0) Activate();
        }

        if ( Input.GetMouseButtonUp(0) )
        {
            _pressed = false;
            if(progressBar != null)
                progressBar.localScale = _progressZero;
        }
    }

    private void Activate()
    {
        _pressed = false;
        onClick.Invoke();
        if ( NameGame.Manager.SFX) 
            _arRef.PlayOneShot(sfxClick);
    }

    void OnMouseExit()
    {
        _pressed = false;
        Highlight.SetActive(false);
        if (progressBar != null) progressBar.localScale = _progressZero;
    }

}
