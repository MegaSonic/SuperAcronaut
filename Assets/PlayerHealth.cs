using UnityEngine;
using System.Collections;
using System;

public class PlayerHealth : MonoBehaviour, IHealth, IKillable {

    public int currentHealth;
    public int maxHealth;

    // Use this for initialization
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Damage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void Kill()
    {
        Debug.Log("Absorb");
    }

    
}
