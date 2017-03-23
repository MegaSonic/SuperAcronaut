using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class NoituEnemy : MonoBehaviour, IDamageable {

    public int health;
    public int maxHealth;

    public int healthBarLength;

    public int deathForce;
    public float gravity;
    public bool isDead;
    public Vector3 velocity;
    public bool useGravity;

    private CharacterController2D _controller;
    private Rigidbody2D _rigid;
    private NoituLoveCharacter _player;

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
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<NoituLoveCharacter>();
        maxHealth = health;

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

        if (_player.dashTarget == this.gameObject.transform)
        {
            _player.StopDash();
            _player.dashTarget = null;
        }
        useGravity = true;
        this.GetComponent<Clickable>().enabled = false;
        _controller.platformMask = LayerMask.GetMask("DeadEnemies");
        velocity = UnityEngine.Random.onUnitSphere * deathForce;
        _controller.velocity = velocity;
    }
}


