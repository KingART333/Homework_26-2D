using UnityEngine;

public class CharacterView : MonoBehaviour
{
    private readonly int VelocityXKey =
        Animator.StringToHash("VelocityX");

    private readonly int VelocityYKey =
        Animator.StringToHash("VelocityY");

    private readonly int IsGroundedKey =
        Animator.StringToHash("IsGrounded");

    private readonly int IsWallSlidingKey =
        Animator.StringToHash("IsWallSliding");

    private readonly int IsDeadKey =
        Animator.StringToHash("IsDead");

    [SerializeField] private Character _character;
    [SerializeField] private Animator _animator;

    private void Update()
    {
        if (_character.IsDead)
        {
            _animator.SetBool(IsDeadKey, true);
            return;
        }

        _animator.SetFloat(
            VelocityXKey,
            Mathf.Abs(_character.Velocity.x)
        );

        _animator.SetFloat(
            VelocityYKey,
            _character.Velocity.y
        );

        _animator.SetBool(
            IsGroundedKey,
            _character.isGrounded()
        );

        _animator.SetBool(
            IsWallSlidingKey,
            _character.IsWallSlidingState
        );

        _animator.SetBool(
            IsDeadKey,
            false
        );
    }
}