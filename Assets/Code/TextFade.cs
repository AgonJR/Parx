using UnityEngine;

public class TextFade : MonoBehaviour
{
    private TextMesh _textMesh;

    private float _fadeDuration = -1f;
    private float _fadeProgress = -1f;

    private Color _interimColor;

    private bool _fadeIn = false;

    void Start()
    {
        _textMesh = GetComponent<TextMesh>();
        _interimColor = _textMesh.color;
    }

    void Update()
    {
        if ( _fadeProgress < _fadeDuration)
        {

            _fadeProgress += Time.deltaTime;

            if (_fadeIn) 
                _interimColor.a = Mathf.Lerp(0, 1, _fadeProgress/_fadeDuration);
            else 
                _interimColor.a = Mathf.Lerp(1, 0, _fadeProgress/_fadeDuration);


            _textMesh.color = _interimColor;
        }
        else
        {
            _fadeProgress = _fadeDuration;
        }
    }

    public void FadeOut(float duration)
    {
        _fadeIn = false;
        _fadeDuration = duration;
        _fadeProgress = 0.0f;
    }

    public void FadeIn(float duration)
    {
        _fadeIn = true;
        _fadeDuration = duration;
        _fadeProgress = 0.0f;
    }
}
