using UnityEngine;

public class TextFade : MonoBehaviour
{
    private TextMesh _textMesh;

    private float _fadeDuration = -1f;
    private float _fadeProgress = -1f;
    private float _fadeDelay = 0.0f;

    private Color _interimColor;

    private bool _fadeIn = false;

    void Start()
    {
        _textMesh = GetComponent<TextMesh>();
        _interimColor = _textMesh.color;
    }

    void Update()
    {
        if (_fadeDelay > 0) { _fadeDelay = Mathf.Max(0, _fadeDelay - Time.deltaTime); }

        if ( _fadeProgress < _fadeDuration)
        {
            _fadeProgress += _fadeDelay > 0 ? 0 : Time.deltaTime;

            if (_fadeIn) 
                _interimColor.a = Mathf.Lerp(0, 1, _fadeProgress/_fadeDuration);
            else 
                _interimColor.a = Mathf.Lerp(1, 0, _fadeProgress/_fadeDuration);

            _textMesh.color = _interimColor;
        }
    }

    public void Fade(bool fadeIn, float duration, float delay = 0.0f)
    {
        _fadeIn = fadeIn;
        _fadeDelay = delay;
        _fadeDuration = duration;
        _fadeProgress = 0.0f;
    }
}
