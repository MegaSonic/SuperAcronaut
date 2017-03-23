using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour {

    public bool triggered;
    public bool secondTrigger;
    public List<GameObject> objectsToTrigger = new List<GameObject>();

    public GameObject tweenObject1;
    public Transform tween1;

    public GameObject tweenObject2;
    public Transform tween2;

    public GameObject wall;

	// Update is called once per frame
	void Update () {
		if (triggered)
        {
            bool checkAll = false;

            foreach (GameObject go in objectsToTrigger)
            {
                if (go.activeSelf == false)
                {
                    checkAll = true;
                }
                else
                {
                    checkAll = false;
                }

                
            }

            if (checkAll && !secondTrigger)
            {
                secondTrigger = true;
                tweenObject1.SetActive(true);
                tweenObject2.SetActive(true);
                LeanTween.move(tweenObject1, tween1, 0.5f);
                LeanTween.move(tweenObject2, tween2, 0.5f);
            }
        }

        if (secondTrigger && wall.activeSelf == true && tweenObject1.activeSelf == false && tweenObject2.activeSelf == false)
        {
            wall.SetActive(false);
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!triggered)
            {
                triggered = true;
                foreach (GameObject go in objectsToTrigger)
                {
                    go.SetActive(true);
                }
            }
        }
    }
}
