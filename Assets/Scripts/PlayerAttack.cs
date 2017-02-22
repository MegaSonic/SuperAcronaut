using UnityEngine;
using System.Collections;
using Prime31;

public class PlayerAttack : MonoBehaviour {

    private CharacterController2D _controller;
    private Player _player;

    private bool canAttack = false;

    void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        _controller.onTriggerEnterEvent += Attack;
        _controller.onTriggerStayEvent += Attack;
        _player = GetComponent<Player>();
    }

    void Attack(Collider2D col)
    {
        if (col.gameObject.GetInterface<IHealth>() != null)
        {
            if (canAttack)
            {
                var attackable = col.gameObject.GetInterface<IHealth>();
                attackable.Damage(1);
            }
            else
            {
                _player.StartHurt();
            }
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (_player.state == State.DASHING || _player.state == State.AIRDASHING || _player.state == State.DIVING)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
	}
}
