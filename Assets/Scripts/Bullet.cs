using UnityEngine;
using System.Collections;
using System;

public class Bullet : MonoBehaviour, IAbsorbable {

    public int absorbAmount = 1;

    public float speed = 4f;
    public Transform target;


    int IAbsorbable.absorbAmount
    {
        get
        {
            return absorbAmount;
        }
    }

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
	}
}
