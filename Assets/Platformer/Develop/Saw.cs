using UnityEngine;

public class Saw : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;
    [SerializeField] private float _speed = 0.5f;

    private void Update()
    {
        if (_pointA == null || _pointB == null)
            return;

        float t = Mathf.PingPong(Time.time * _speed, 1f);

        t = Mathf.SmoothStep(0f, 1f, t);

        transform.position = Vector3.Lerp(
            _pointA.position,
            _pointB.position,
            t
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Character>(out Character character))
        {
            character.Die();
        }
    }
}