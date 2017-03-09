using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;


public class NoituLoveCharacter : MonoBehaviour {

    public enum State
    {
        IDLE = 0,
        DASHING = 1,
        ATTACKING = 2,
        RUNNING = 3
    }

    public NoituLoveCharacter.State charState;


    public float runSpeed = 7;
    public float dashSpeed = 12;

    public float gravity = -40;
    public float terminalVelocity = -12;

    public Vector3 velocity;
    public bool _disableGravity;
    public bool readInput;
    public bool isDashing;

    public float distanceToDashTo;

    public Transform dashTarget;

    private CharacterController2D _controller;
    private BoxCollider2D _collider;
    private SpriteRenderer _sprite;
    private Afterimages _afterimages;


    private void Awake()
    {
        readInput = true;
        _controller = GetComponent<CharacterController2D>();
        _collider = GetComponent<BoxCollider2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _afterimages = GetComponent<Afterimages>();
    }

    // Use this for initialization
    void Start () {
        velocity = _controller.velocity;
	}
	
	// Update is called once per frame
	void Update () {

        if (readInput)
        {
            if (Input.GetAxis("Horizontal") < 0)
            {
                _sprite.flipX = true;
                velocity.x = runSpeed * -1;
            }
            else if (Input.GetAxis("Horizontal") > 0)
            {
                _sprite.flipX = false;
                velocity.x = runSpeed;
            }
            else
            {
                velocity.x = 0f;
            }
        }

        if (isDashing)
        {
            _afterimages.StartImages();
            readInput = false;
            _disableGravity = true;
            Vector3 movementDirection = dashTarget.position - this.transform.position;
            movementDirection = movementDirection.normalized;
            movementDirection *= dashSpeed;
            velocity = movementDirection;
            velocity.z = 0f;

            if ((dashTarget.position - transform.position).magnitude < distanceToDashTo)
            {
                readInput = true;
                isDashing = false;
                _disableGravity = false;
                _afterimages.StopImages();
            }
        }

        if (!_disableGravity)
        {
            velocity.y += gravity * Time.deltaTime;
            if (velocity.y < terminalVelocity)
            {
                velocity.y = terminalVelocity;
            }
        }

        velocity *= Time.deltaTime;
        _controller.move(velocity);
        velocity = _controller.velocity;
    }
}
