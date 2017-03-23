using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class NoituEnemy : MonoBehaviour, IDamageable {

    public int health;
    public int deathForce;
    public float gravity;
    public bool isDead;
    public Vector3 velocity;
    public bool useGravity;

    private CharacterController2D _controller;
    private Rigidbody2D _rigid;

    public void Damage(int damage)
    {
        if (!isDead)
        {
            Debug.Log("Damaged!");
            health -= damage;

            if (health <= 0) Die();
        }
    }

    // Use this for initialization
    void Start () {
		
	}

    private void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        _rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (useGravity)
        {
            velocity.y += gravity * Time.deltaTime;
            velocity *= Time.deltaTime;
            _controller.move(velocity);
            velocity = _controller.velocity;
        }
    }

    public void Die()
    {
        isDead = true;
        Destroy(this.gameObject);
    }
}
