using UnityEngine;
using System.Collections;
using Prime31;

public enum State
{
    IDLE = 0,
    RUNNING = 1,
    JUMPING = 2,
    DASHING = 3,
    AIRDASHING = 4,
    DIVING = 5,
    WALLCLINGING = 6,
    VAIRDASHING = 7
}

public class Player : MonoBehaviour {

    private enum WallJumpState
    {
        OFF = 0,
        RIGHT = 1,
        LEFT = 2
    }

    // public PlayerState state;

    public State state;

    public float runSpeed = 7;
    public float jumpHeight = 10;
    public float dashSpeed = 12;
    public float dashTime = 0.4f;
    public float airDashTime = 0.4f;
    public float vairDashTime = 0.3f;
    public float vairDashSpeed = -7f;
    public float diveVerticalSpeed = 15;
    public float diveHorizontalSpeed = 3;
    public float wallSlideRate = -10;
    public float wallJumpX = 5;
    public float wallJumpY = 7;
    public float wallJumpDecayRate = 30;
    public float wallDashJumpDecayRate = 100;
    public float gravity = -40;
    public float terminalVelocity = -12;

    public bool dashJump = false;
    public bool usedDoubleJump = false;

    private CharacterController2D _controller;
    private bool _releasedJump = false;
    private Vector3 _velocity;
    private bool _jumpedThisFrame = false;
    private float _dashTimer;
    private bool _disableGravity = false;
    private Afterimages _afterimages;
    private float _airDashTimer;
    private bool _usedAirDash;
    private float _temporaryWallJumpAccel;
    private bool _usedVairDash;
    private float _vairDashTimer;

    private WallJumpState _wallJumpState;
    

    // Use this for initialization
    void Start () {
        _velocity = _controller.velocity;
	}

    void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        _afterimages = GetComponent<Afterimages>();
    }
	
	// Update is called once per frame
	void Update () {
        // Debug.Log(_controller.collisionState.ToString());

        if (_controller.isGrounded)
        {
            _usedAirDash = false;
            usedDoubleJump = false;
            _usedVairDash = false;
        }

        switch (state)
        {
            case State.IDLE:
                _releasedJump = false;
                _velocity.x = 0;

                if (!_controller.isGrounded)
                {
                    state = State.JUMPING;
                    _releasedJump = true;
                    break;
                }
                else
                {
                    _afterimages.StopImages();
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        _velocity.y -= 3f;
                        _controller.ignoreOneWayPlatformsThisFrame = true;
                    }
                }

                if (Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Horizontal") > 0)
                {
                    state = State.RUNNING;
                }

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    if (_controller.isGrounded)
                    {
                        state = State.JUMPING;
                        StartJump(ref _velocity);
                    }
                }
                break;
            case State.RUNNING:
                _releasedJump = false;

                if (!_controller.isGrounded)
                {
                    state = State.JUMPING;
                    _releasedJump = true;
                    break;
                }
                else
                {
                    _afterimages.StopImages();
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        _velocity.y -= 3f;
                        _controller.ignoreOneWayPlatformsThisFrame = true;
                    }
                }

                if (Input.GetAxis("Horizontal") < 0)
                {
                    _velocity.x = -1 * runSpeed;
                    if (transform.localScale.x > 0f)
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
                else if (Input.GetAxis("Horizontal") > 0)
                {
                    _velocity.x = runSpeed;
                    if (transform.localScale.x < 0f)
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    state = State.IDLE;
                    _velocity.x = 0;
                }

                if (Input.GetButton("Dash"))
                {
                    state = State.DASHING;
                    StartDash();
                }

                if (Input.GetButtonDown("Jump"))
                {
                    state = State.JUMPING;
                    StartJump(ref _velocity);
                }

                break;

            case State.JUMPING:
                if (_controller.isGrounded)
                {
                    _afterimages.StopImages();
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        _velocity.y -= 3f;
                        _controller.ignoreOneWayPlatformsThisFrame = true;
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        state = State.DIVING;
                        StartDive();
                        break;
                    }
                }

                

                if (Input.GetAxis("Horizontal") < 0 )
                {
                    if (_wallJumpState == WallJumpState.LEFT)
                    {
                        _wallJumpState = WallJumpState.OFF;
                    }

                    if (_wallJumpState == WallJumpState.RIGHT && _temporaryWallJumpAccel > 0)
                    {
                        _velocity.x = _temporaryWallJumpAccel;
                        if (dashJump)
                        {
                            _temporaryWallJumpAccel -= (Time.deltaTime * wallDashJumpDecayRate);
                        }
                        else
                        {
                            _temporaryWallJumpAccel -= (Time.deltaTime * wallJumpDecayRate);
                        }
                    }
                    else if (dashJump)
                    {
                        _velocity.x = -dashSpeed;
                    }
                    else
                    {
                        _velocity.x = -runSpeed;
                    }

                    if (_controller.isGrounded)
                    {
                        state = State.RUNNING;
                        dashJump = false;
                        _wallJumpState = WallJumpState.OFF;
                    }

                    

                    if (transform.localScale.x > 0f)
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

                    if (_controller.collisionState.left && _controller.velocity.y < 0)
                    {
                        state = State.WALLCLINGING;
                        break;
                    }

                }
                else if (Input.GetAxis("Horizontal") > 0 )
                {
                    if (_wallJumpState == WallJumpState.RIGHT)
                    {
                        _wallJumpState = WallJumpState.OFF;
                    }

                    if (_wallJumpState == WallJumpState.LEFT && _temporaryWallJumpAccel < 0)
                    {
                        _velocity.x = _temporaryWallJumpAccel;
                        if (dashJump)
                        {
                            _temporaryWallJumpAccel += (Time.deltaTime * wallDashJumpDecayRate);
                        }
                        else
                        {
                            _temporaryWallJumpAccel += (Time.deltaTime * wallJumpDecayRate);
                        }
                        
                    }
                    else if (dashJump)
                    {
                        _velocity.x = dashSpeed;
                    }
                    else
                    {
                        _velocity.x = runSpeed;
                    }

                    if (_controller.isGrounded)
                    {
                        state = State.RUNNING;
                        dashJump = false;
                        _wallJumpState = WallJumpState.OFF;
                    }
                    if (transform.localScale.x < 0f)
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    if (_controller.collisionState.right && _controller.velocity.y < 0)
                    {
                        state = State.WALLCLINGING;
                        break;
                    }
                }
                else
                {
                    _velocity.x = 0;
                    if (_controller.isGrounded)
                    {
                        state = State.IDLE;
                        dashJump = false;
                        _wallJumpState = WallJumpState.OFF;
                        break;
                    }
                }

                if (Input.GetAxis("Vertical") > 0 && Input.GetButtonDown("Dash") && !_usedVairDash)
                {
                    state = State.VAIRDASHING;
                    StartVairDash();
                    break;
                }

                if (Input.GetButtonDown("Dash") && !_usedAirDash)
                {
                    StartAirDash();
                    state = State.AIRDASHING;
                    break;
                }

                if (Input.GetButtonUp("Jump") && !_releasedJump && _controller.velocity.y > 0)
                {
                    _velocity.y = 0;
                    _releasedJump = true;
                    break;
                }

                if (Input.GetButtonDown("Jump") && !usedDoubleJump)
                {
                    StartJump(ref _velocity);
                    state = State.JUMPING;
                    usedDoubleJump = true;
                }


                break;

            case State.DASHING:
                _releasedJump = false;
                _dashTimer -= Time.deltaTime;

                if (!_controller.isGrounded)
                {
                    state = State.JUMPING;
                    _releasedJump = true;
                    dashJump = true;
                    break;
                }
                else
                {
                    if (Input.GetAxis("Vertical") > 0)
                    {
                        _velocity.y -= 3f;
                        _controller.ignoreOneWayPlatformsThisFrame = true;
                        dashJump = true;
                    }
                }

                bool releasedDash = false;

                if (_dashTimer < 0f)
                {
                    releasedDash = true;   
                }


                if (Input.GetAxis("Horizontal") == 0 || Input.GetButtonUp("Dash"))
                {
                    releasedDash = true;
                    _dashTimer = dashTime;
                }

                if (Input.GetAxis("Horizontal") < 0)
                {
                    _velocity.x = -1 * dashSpeed;
                    if (transform.localScale.x > 0f)
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    if (releasedDash)
                        state = State.RUNNING;
                }
                else if (Input.GetAxis("Horizontal") > 0)
                {
                    _velocity.x = dashSpeed;
                    if (transform.localScale.x < 0f)
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    if (releasedDash)
                        state = State.RUNNING;
                }
                else
                {
                    state = State.IDLE;
                    _velocity.x = 0;
                }

                if (Input.GetButtonDown("Jump"))
                {
                    state = State.JUMPING;
                    dashJump = true;
                    StartJump(ref _velocity);
                }

                break;

            case State.DIVING:
                if (_controller.isGrounded)
                {
                    dashJump = false;
                    _disableGravity = false;
                    _afterimages.StopImages();
                    if (Input.GetAxis("Horizontal") > 0)
                    {
                        state = State.RUNNING;
                    }
                    else if (Input.GetAxis("Horizontal") < 0)
                    {
                        state = State.RUNNING;
                    }
                    else
                    {
                        state = State.IDLE;
                    }
                }

                if (Input.GetButtonDown("Jump") && !usedDoubleJump)
                {
                    state = State.JUMPING;
                    StartJump(ref _velocity);
                    usedDoubleJump = true;
                    _disableGravity = false;

                    if (!dashJump)
                    {
                        _afterimages.StopImages();
                    }

                }

                break;

            case State.AIRDASHING:
                _disableGravity = true;
                //_velocity.x = dashSpeed;
                _velocity.y = 0;

                _airDashTimer -= Time.deltaTime;

                bool releasedAirDash = false;

                if (_airDashTimer < 0f)
                {
                    releasedAirDash = true;
                }


                if (Input.GetButtonUp("Dash"))
                {
                    releasedAirDash = true;
                    _dashTimer = dashTime;
                }

                if (releasedAirDash)
                {
                    state = State.JUMPING;
                    _disableGravity = false;
                }

                if (Input.GetAxis("Vertical") < 0 && Input.GetButtonDown("Dash") && !_usedVairDash)
                {
                    state = State.VAIRDASHING;
                    StartVairDash();
                    break;
                }

                if (Input.GetAxis("Horizontal") > 0 && _controller.collisionState.right)
                {
                    state = State.WALLCLINGING;
                }
                else if (Input.GetAxis("Horizontal") < 0 && _controller.collisionState.left)
                {
                    state = State.WALLCLINGING;
                }

                break;

            case State.WALLCLINGING:
                dashJump = false;
                usedDoubleJump = false;
                _usedAirDash = false;
                _usedVairDash = false;
                _releasedJump = false;
                _velocity.y = wallSlideRate;
                _disableGravity = true;

                if (Input.GetAxis("Horizontal") < 0)
                {
                    _velocity.x = -runSpeed;
                }
                else if (Input.GetAxis("Horizontal") > 0)
                {
                    _velocity.x = runSpeed;
                }

                if (_controller.isGrounded)
                {
                    state = State.IDLE;
                    _disableGravity = false;
                    break;
                }

                if (!_controller.collisionState.right && !_controller.collisionState.left)
                {
                    state = State.JUMPING;
                    _disableGravity = false;
                    break;
                }

                if (_controller.collisionState.right)
                {
                    if (Input.GetAxis("Horizontal") <= 0)
                    {
                        state = State.JUMPING;
                        _disableGravity = false;
                        break;
                    }
                }
                else if (_controller.collisionState.left)
                {
                    if (Input.GetAxis("Horizontal") >= 0)
                    {
                        state = State.JUMPING;
                        _disableGravity = false;
                        break;
                    }

                    
                }

                if (Input.GetButtonDown("Jump"))
                {
                    state = State.JUMPING;

                    if (Input.GetButton("Dash"))
                    {
                        dashJump = true;
                    }

                    if (_controller.collisionState.left)
                    {
                        if (!dashJump)
                        {
                            _temporaryWallJumpAccel = wallJumpX;
                            _afterimages.StopImages();
                        }
                        else
                        {
                            _temporaryWallJumpAccel = dashSpeed;
                            _afterimages.StartImages();
                        }
                        _wallJumpState = WallJumpState.RIGHT;
                    }
                    else if (_controller.collisionState.right)
                    {
                        if (!dashJump)
                        {
                            _temporaryWallJumpAccel = -wallJumpX;
                            _afterimages.StopImages();
                        }
                        else
                        {
                            _temporaryWallJumpAccel = -dashSpeed;
                            _afterimages.StartImages();
                        }
                        _wallJumpState = WallJumpState.LEFT;
                    }
                    _disableGravity = false;
                    StartJump(ref _velocity);
                    break;
                }
                break;
            case State.VAIRDASHING:
                _velocity.y = 0;
                _vairDashTimer -= Time.deltaTime;

                if (_vairDashTimer <= 0)
                {
                    state = State.JUMPING;
                    _velocity.y = vairDashSpeed;
                    _disableGravity = false;
                }

                if (Input.GetAxis("Horizontal") != 0 && Input.GetButtonDown("Dash") && !_usedAirDash)
                {
                    state = State.AIRDASHING;
                    StartAirDash();
                    break;
                }

                break;

            default:
                break;

                
        }


        if (!_disableGravity)
        {
            _velocity.y += gravity * Time.deltaTime;
            if (_velocity.y < terminalVelocity)
            {
                _velocity.y = terminalVelocity;
            }
        }
        
        _velocity *= Time.deltaTime;
        _controller.move(_velocity);
        _velocity = _controller.velocity;
    }


    public void StartJump(ref Vector3 velocity)
    {
        velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
        // _jumpedThisFrame = true;
    }

    public void StartWallJump(ref Vector3 velocity)
    {
        velocity.y = Mathf.Sqrt(2f * wallJumpY * -gravity);
        // _jumpedThisFrame = true;
    }

    public void StartDash()
    {
        _dashTimer = dashTime;
        _afterimages.StartImages();
    }

    public void StartDive()
    {
        _disableGravity = true;
        _velocity.y = diveVerticalSpeed;
        _afterimages.StartImages();
        if (Input.GetAxis("Horizontal") > 0)
        {
            _velocity.x = diveHorizontalSpeed;
            if (transform.localScale.x < 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            _velocity.x = -diveHorizontalSpeed;
            if (transform.localScale.x > 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            _velocity.x = 0;
        }
    }

    public void StartAirDash()
    {
        _usedAirDash = true;
        _airDashTimer = airDashTime;
        _afterimages.StartImages();
        dashJump = true;

        if (Input.GetAxis("Horizontal") > 0)
        {
            _velocity.x = dashSpeed;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            _velocity.x = -dashSpeed;
        }
        else
        {
            if (transform.localScale.x < 0)
            {
                _velocity.x = -dashSpeed;
            }
            else
            { 
                _velocity.x = dashSpeed;
            }
        }
    }

    public void StartVairDash()
    {
        // _afterimages.StartImages();
        _usedVairDash = true;
        // dashJump = true;
        _vairDashTimer = vairDashTime;
        _disableGravity = true;

        _velocity.x = 0;
        _velocity.y = 0;
    }
}
