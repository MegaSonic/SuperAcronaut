using UnityEngine;
using System.Collections;
using Prime31;

public class PlayerAbsorb : MonoBehaviour {

    private CharacterController2D _controller;
    private Player _player;

    public bool canAbsorb;

    public int currentStored;
    public int maxStored;

	// Use this for initialization
	void Awake () {
        _controller = GetComponent<CharacterController2D>();
        _controller.onTriggerEnterEvent += Absorb;
        _controller.onTriggerStayEvent += Absorb;
        _player = GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
        if (_player.state == State.DASHING || _player.state == State.AIRDASHING || _player.state == State.DIVING || _player.state == State.VAIRDASHING)
            canAbsorb = true;
        else
        {
            canAbsorb = false;
        }
	}

    void Absorb(Collider2D col)
    {
        if (col.gameObject.GetInterface<IAbsorbable>() != null)
        {
            if (canAbsorb)
            {
                var absorbable = col.gameObject.GetInterface<IAbsorbable>();
                currentStored += absorbable.absorbAmount;
                if (currentStored > maxStored) currentStored = maxStored;
                Destroy(col.gameObject);
            }
            else
            {
                _player.StartHurt();
                Destroy(col.gameObject);
            }
        }
    }

    void EnableAbsorb()
    {
        canAbsorb = true;
    }

    void DisableAbsorb()
    {
        canAbsorb = false;
    }

    void SetAbsorb(bool state)
    {
        canAbsorb = state;
    }
}
