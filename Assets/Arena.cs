using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour {

    public bool triggered;
    public List<GameObject> objectsToTrigger = new List<GameObject>();

	
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!triggered)
            {
                foreach (GameObject go in objectsToTrigger)
                {
                    go.SetActive(true);
                }
            }
        }
    }
}
