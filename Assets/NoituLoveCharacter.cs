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

    public BoxCollider2D attackCollider;

    public NoituLoveCharacter.State charState;


    public float runSpeed = 7;
    public float dashSpeed = 12;

    public float gravity = -40;
    public float terminalVelocity = -12;
    public int damage;
    public float timeBetweenDamage;
    public float attacksLastFor;

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
    private Hitbox _hitbox;

    private float attackTimer;
    private float hitboxTimer;



    private void Awake()
    {
        readInput = true;
        _controller = GetComponent<CharacterController2D>();
        _collider = GetComponent<BoxCollider2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _afterimages = GetComponent<Afterimages>();
        _hitbox = GetComponentInChildren<Hitbox>();
        attackCollider.enabled = true;
    }

    // Use this for initialization
    void Start () {
        velocity = _controller.velocity;
	}
	
	// Update is called once per frame
	void Update () {
        attackTimer += Time.deltaTime;
        hitboxTimer += Time.deltaTime;
        

        if (readInput)
        {
            if (Input.GetAxis("Horizontal") < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
                velocity.x = runSpeed * -1;
            }
            else if (Input.GetAxis("Horizontal") > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                velocity.x = runSpeed;
            }
            else
            {
                velocity.x = 0f;
            }
        }

        if (isDashing)
        {
            if (dashTarget != null && (dashTarget.position - transform.position).magnitude > distanceToDashTo)
            {
                if (dashTarget.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
                }


                _afterimages.StartImages();
                readInput = false;
                _disableGravity = true;
                Vector3 movementDirection = dashTarget.position - this.transform.position;
                movementDirection = movementDirection.normalized;
                movementDirection *= dashSpeed;
                velocity = movementDirection;
                velocity.z = 0f;

            }
            else
            {

                if (Input.GetMouseButton(0))
                {
                    velocity.x = 0;
                    velocity.y = 0;

                    if (hitboxTimer > timeBetweenDamage && !_hitbox.sendDamage)
                    {
                        attackTimer = 0;
                        _hitbox.StartAttack();
                    }

                    if (_hitbox.sendDamage && attackTimer > attacksLastFor)
                    {
                        hitboxTimer = 0;
                        _hitbox.EndAttack();
                    }
                }
                else
                {
                    readInput = true;
                    isDashing = false;
                    _disableGravity = false;
                    _afterimages.StopImages();
                    _hitbox.EndAttack();
                }
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
