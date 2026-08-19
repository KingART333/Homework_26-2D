using UnityEngine;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour
{
    private const string HorizontalAxisName = "Horizontal";

    private int _lastWallJumpSide;
    private bool _isWallJumping;
    private bool _insideAreaEffector;

    private static Vector3 _checkpointPosition;
    private static int _lastCheckpointId;
    private static bool _hasCheckpoint;

    public static int LastCheckpointId => _lastCheckpointId;

    [Header("References")]
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private ObstacleChecker _groundChecker;
    [SerializeField] private ObstacleChecker _ceilChecker;
    [SerializeField] private ObstacleChecker _leftWallChecker;
    [SerializeField] private ObstacleChecker _rightWallChecker;

    [Header("Movement")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _gravity = 20f;
    [SerializeField] private float _yVelocityForJump = 10f;

    [Header("Wall Slide")]
    [SerializeField] private float _wallSlideSpeed = 2f;

    [Header("Wall Jump")]
    [SerializeField] private float _wallJumpHorizontalVelocity = 12f;
    [SerializeField] private float _wallJumpVerticalVelocity = 9f;

    [Header("Death")]
    [SerializeField] private float _deathAnimationDuration = 1f;

    private Vector2 _velocity;
    private bool _jumpPressed;

    public bool IsDead { get; private set; }

    private bool IsTouchingLeftWall =>
        _leftWallChecker.IsTouches();

    private bool IsTouchingRightWall =>
        _rightWallChecker.IsTouches();

    private bool IsTouchingWall =>
        IsTouchingLeftWall || IsTouchingRightWall;

    private bool IsWallSliding =>
        !_isWallJumping &&
        !_insideAreaEffector &&
        !isGrounded() &&
        _velocity.y < 0f &&
        IsTouchingWall;

    public bool IsWallSlidingState =>
        IsWallSliding;

    public Vector2 Velocity =>
        _rigidbody.velocity;

    private Quaternion TurnRight =>
        Quaternion.identity;

    private Quaternion TurnLeft =>
        Quaternion.Euler(0f, 180f, 0f);

    private void Awake()
    {
        if (_hasCheckpoint)
        {
            transform.position = _checkpointPosition;
        }
    }

    private void Update()
    {
        if (IsDead)
            return;

        float xInput =
            Input.GetAxisRaw(HorizontalAxisName);

        _jumpPressed =
            Input.GetKeyDown(KeyCode.Space);

        HandleHorizontalMovement(xInput);
        HandleGravity();
        HandleJump();
        HandleCeil();

        HandleWallJumpState();

        ApplyVelocity();

        transform.rotation =
            GetRotationFrom(_rigidbody.velocity);
    }

    public bool isGrounded()
    {
        return _groundChecker.IsTouches();
    }

    private void HandleHorizontalMovement(float xInput)
    {
        if (_isWallJumping)
        {
            float launchVelocityX =
                _lastWallJumpSide == -1
                    ? _wallJumpHorizontalVelocity
                    : -_wallJumpHorizontalVelocity;

            bool velocityUntouchedByInput =
                Mathf.Approximately(
                    _velocity.x,
                    launchVelocityX
                );

            if (Mathf.Approximately(xInput, 0f))
            {
                if (velocityUntouchedByInput)
                    return;

                _velocity.x = 0f;
                return;
            }

            _velocity.x = xInput * _speed;

            return;
        }

        _velocity.x = xInput * _speed;
    }

    private void HandleGravity()
    {
        if (_insideAreaEffector)
            return;

        if (isGrounded() && _velocity.y <= 0f)
        {
            _velocity.y = 0f;

            _isWallJumping = false;
            _lastWallJumpSide = 0;

            return;
        }

        if (IsWallSliding)
        {
            _velocity.y = -_wallSlideSpeed;
            return;
        }

        _velocity.y -=
            _gravity * Time.deltaTime;
    }

    private void HandleJump()
    {
        if (!_jumpPressed)
            return;

        if (isGrounded())
        {
            _velocity.y = _yVelocityForJump;

            if (_insideAreaEffector)
            {
                _rigidbody.velocity =
                    new Vector2(
                        _rigidbody.velocity.x,
                        _yVelocityForJump
                    );
            }

            return;
        }

        if (IsWallSliding &&
            IsTouchingLeftWall)
        {
            if (_lastWallJumpSide == -1)
                return;

            PerformWallJump(
                _wallJumpHorizontalVelocity,
                -1
            );

            return;
        }

        if (IsWallSliding &&
            IsTouchingRightWall)
        {
            if (_lastWallJumpSide == 1)
                return;

            PerformWallJump(
                -_wallJumpHorizontalVelocity,
                1
            );
        }
    }

    private void PerformWallJump(
        float horizontalVelocity,
        int wallSide)
    {
        _velocity = new Vector2(
            horizontalVelocity,
            _wallJumpVerticalVelocity
        );

        _isWallJumping = true;
        _lastWallJumpSide = wallSide;

        if (_insideAreaEffector)
        {
            _rigidbody.velocity = _velocity;
        }
    }

    private void HandleWallJumpState()
    {
        if (!_isWallJumping)
            return;

        if (_lastWallJumpSide == -1 &&
            IsTouchingRightWall)
        {
            _isWallJumping = false;
        }

        if (_lastWallJumpSide == 1 &&
            IsTouchingLeftWall)
        {
            _isWallJumping = false;
        }
    }

    private void HandleCeil()
    {
        if (!_ceilChecker.IsTouches())
            return;

        _velocity.y =
            Mathf.Min(0f, _velocity.y);

        if (_insideAreaEffector)
        {
            _rigidbody.velocity =
                new Vector2(
                    _rigidbody.velocity.x,
                    Mathf.Min(
                        0f,
                        _rigidbody.velocity.y
                    )
                );
        }
    }

    private void ApplyVelocity()
    {
        if (_insideAreaEffector)
        {
            _rigidbody.velocity =
                new Vector2(
                    _velocity.x,
                    _rigidbody.velocity.y
                );

            return;
        }

        _rigidbody.velocity = _velocity;
    }

    private Quaternion GetRotationFrom(
        Vector2 velocity)
    {
        if (velocity.x > 0f)
            return TurnRight;

        if (velocity.x < 0f)
            return TurnLeft;

        return transform.rotation;
    }

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        if (other.GetComponent<AreaEffector2D>() != null)
        {
            _insideAreaEffector = true;
        }
    }

    private void OnTriggerExit2D(
        Collider2D other)
    {
        if (other.GetComponent<AreaEffector2D>() != null)
        {
            _insideAreaEffector = false;

            _velocity.y =
                _rigidbody.velocity.y;
        }
    }


    public void SetCheckpoint(
        Vector3 position,
        int checkpointId)
    {
        _checkpointPosition = position;
        _hasCheckpoint = true;

        _checkpointPosition = position;
        _lastCheckpointId = checkpointId;
        _hasCheckpoint = true;
    }


    public void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        _velocity = Vector2.zero;

        _rigidbody.velocity =
            Vector2.zero;

        _rigidbody.simulated = false;

        Invoke(
            nameof(RestartLevel),
            _deathAnimationDuration
        );
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}