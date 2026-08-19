using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private int _checkpointId;

    private bool _activated;

    private void Awake()
    {
        int lastCheckpoint = Character.LastCheckpointId;

        if (_checkpointId <= lastCheckpoint && lastCheckpoint > 0)
        {
            ActivateImmediately();
        }
        else
        {
            ResetAnimation();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_activated)
            return;

        if (!other.TryGetComponent<Character>(out Character character))
            return;

        _activated = true;

        character.SetCheckpoint(
            transform.position,
            _checkpointId
        );

        PlayActivationAnimation();

        Debug.Log(
            "Checkpoint reached: " + _checkpointId
        );
    }

    private void PlayActivationAnimation()
    {
        if (_animator == null)
            return;

        _animator.speed = 1f;
        _animator.Play("Checkpoint", 0, 0f);
    }

    private void ActivateImmediately()
    {
        _activated = true;

        if (_animator == null)
            return;

        _animator.Play("Checkpoint", 0, 1f);

        _animator.speed = 0f;
    }

    private void ResetAnimation()
    {
        if (_animator == null)
            return;

        _animator.Play("Checkpoint", 0, 0f);

        _animator.speed = 0f;
    }
}