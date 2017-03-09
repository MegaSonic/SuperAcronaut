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
        // Get the Character Controller and add the Absorb event to Collider Enter and Collider Stay events
        _controller = GetComponent<CharacterController2D>();
        _controller.onTriggerEnterEvent += Absorb;
        _controller.onTriggerStayEvent += Absorb;
        _player = GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
        // If the Player State is a state that lets them absorb...
        if (_player.state == State.DASHING || _player.state == State.AIRDASHING || _player.state == State.DIVING || _player.state == State.VAIRDASHING)
            canAbsorb = true;
        else
        {
            canAbsorb = false;
        }
	}

    void Absorb(Collider2D col)
    {
        // If the game object we're colliding with is Absorbable
        if (col.gameObject.GetInterface<IAbsorbable>() != null)
        {
            // And the player can absorb
            if (canAbsorb)
            {
                // Get the object as an absorbable interface
                var absorbable = col.gameObject.GetInterface<IAbsorbable>();
                currentStored += absorbable.absorbAmount;
                if (currentStored > maxStored) currentStored = maxStored;
                // Now that we've absorbed that object, destroy it. We can modify this in the future if we don't want absorbable objects to necessarily be destroyed
                // Right now this is thinking of absorbable objects as bullets in the world, but later on we could have something like energy enemies that can't normally be damaged but can be absorbed
                // To add functionality like that, we'd add a BeAbsorbed() method or something to the IAbsorbable interface which would handle any special behavior an object might have when it gets absorbed
                Destroy(col.gameObject);
            }
            else
            {
                // Otherwise, hurt the player and destroy the conflicting game object
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
