using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour {

    private NoituLoveCharacter player;

    public bool sendDamage;

    public List<Transform> enemiesInAttack;


    private void Awake()
    {
        player = GetComponentInParent<NoituLoveCharacter>();
        enemiesInAttack = new List<Transform>();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay2D(Collider2D collision)
    {
        

        if (collision.transform.root.tag != "Player")
        {
            if (sendDamage)
            {
                IDamageable damageable = collision.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    if (!enemiesInAttack.Contains(collision.transform))
                    {
                        enemiesInAttack.Add(collision.transform);
                        damageable.Damage(player.damage);
                    }
                }
            }
        }
    }

    public void StartAttack()
    {
        enemiesInAttack.Clear();
        sendDamage = true;
    }

    public void EndAttack()
    {
        
        sendDamage = false;
    }
}
