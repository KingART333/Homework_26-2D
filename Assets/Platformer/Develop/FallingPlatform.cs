using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float _fallDelay = 2f;
    [SerializeField] private float _shakeDuration = 0.5f;
    [SerializeField] private float _shakeAmount = 0.05f;
    [SerializeField] private float _fallGravityScale = 3f;

    private Rigidbody2D _rigidbody;
    private Vector3 _startPosition;
    private bool _isTriggered;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _startPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isTriggered)
            return;

        if (collision.collider.TryGetComponent<Character>(out Character character))
        {
            _isTriggered = true;
            StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(_fallDelay - _shakeDuration);

        float elapsed = 0f;

        while (elapsed < _shakeDuration)
        {
            elapsed += Time.deltaTime;

            float offsetX = Random.Range(-_shakeAmount, _shakeAmount);
            float offsetY = Random.Range(-_shakeAmount, _shakeAmount);

            transform.position = _startPosition + new Vector3(offsetX, offsetY, 0f);

            yield return null;
        }

        transform.position = _startPosition;

        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.gravityScale = _fallGravityScale;
    }
}