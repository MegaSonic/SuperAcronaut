using UnityEngine;
using System.Collections;
using System;
using Prime31;

public class Enemy : MonoBehaviour, IHealth, IKillable {
    public int health;
    public int maxHealth;

    private Vector3 _velocity;

    public float deathGravity = -10;
    public float deathJumpHeight = 5;

    public bool isDead = false;

    private CharacterController2D _controller;

    void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        _velocity = _controller.velocity;
    }

    public void Damage(int amount)
    {
        health -= amount;
        if (health <= 0) Kill();
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }

    public void Kill()
    {
        _controller.platformMask = LayerMask.GetMask("CameraPlatform");
        Transform cameraPlatform = GameObject.FindGameObjectWithTag("CameraPlatform").transform;
        transform.SetParent(cameraPlatform, true);
        _velocity.y = Mathf.Sqrt(2f * deathJumpHeight * -deathGravity);
        gameObject.layer = 12;
        
        isDead = true;
        
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        


        if (isDead)
        {
            if (!_controller.isGrounded)
            {
                _velocity.y += deathGravity * Time.deltaTime;
            }
            else
            {
 //               _velocity.y = 0;
            }
        }
        else
        {
            if (!_controller.isGrounded)
            {
                _velocity.y += deathGravity * Time.deltaTime;
            }
            else
            {
                // _velocity.y = 0;
            }
        }

        _velocity *= Time.deltaTime;
        _controller.move(_velocity);
        _velocity = _controller.velocity;
    }
}
