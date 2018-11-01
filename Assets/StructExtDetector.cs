using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructExtDetector : MonoBehaviour {

    private List<SpriteRenderer> rends_to_ignore_dyn;
    void Start () {
        rends_to_ignore_dyn = new List<SpriteRenderer>();
    }
	
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player" && collision.tag != "ghost")
        {

            SpriteRenderer rend = collision.gameObject.GetComponent<SpriteRenderer>();
            if (rend != null && rend.sortingOrder > Settings.max_order_in)
            {
                rends_to_ignore_dyn.Add(rend);
                rend.enabled = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Player" && collision.tag != "ghost")
        {
            SpriteRenderer rend = collision.gameObject.GetComponent<SpriteRenderer>();
            if (rend != null)
            {
                rends_to_ignore_dyn.Remove(rend);
                rend.enabled = true;
            }
        }
    }
    //---------------------------------------------------------------------------------------------------------
    public List<SpriteRenderer> GetRenderers()
    {
        return rends_to_ignore_dyn;
    }
    public bool CheckMatch(SpriteRenderer rend)
    {
        return rends_to_ignore_dyn.Contains(rend);
    }
}
