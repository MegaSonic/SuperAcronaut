using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour {

#region Public fields
#endregion

#region Serialized Unity Inspector fields
#endregion

#region Private Fields
#endregion

#region Unity Lifecycle
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            Debug.Log("Not player!");
            col.gameObject.SetActive(false);
        }
        else if (col.gameObject.tag == "Player")
        {
            Debug.Log("Player!");
            col.gameObject.transform.position = col.gameObject.GetComponent<NoituLoveCharacter>().lastGroundedPosition;
        }
    }
#endregion

#region Public Methods
#endregion

#region Private Methods
#endregion

#region Private Structures
#endregion

}
