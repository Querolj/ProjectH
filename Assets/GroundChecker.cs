using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour {

    private bool on_ground;
    private bool on_mountable;
	void Start () {
		
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "ground")
        {
            on_ground = true;
        }
        if (collision.tag == "mountable")
        {
            on_mountable = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "ground")
        {
            on_ground = false;
        }
        if (collision.tag == "mountable")
        {
            on_mountable = false;
        }
    }

    public bool OnGround
    {
        get { return on_ground; }
    }
    public bool OnMountable
    {
        get { return on_mountable; }
    }
}
