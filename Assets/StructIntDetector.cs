using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructIntDetector : MonoBehaviour {

    private List<SpriteRenderer> rends_to_ignore_dyn;
    private Structure structure;
    void Start () {
        rends_to_ignore_dyn = new List<SpriteRenderer>();
        structure = this.GetComponentInParent<Structure>();
    }
	
	void Update () {
		
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player" && collision.tag != "ghost" )
        {
            
            SpriteRenderer rend = collision.gameObject.GetComponent<SpriteRenderer>();
            if (rend != null && rend.sortingOrder > Settings.max_order_in)
            {
                print(collision.gameObject.name);
                rends_to_ignore_dyn.Add(rend);
                //structure.CheckRenderer(rend);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Player" && collision.tag != "ghost")
        {
            print("out : "+collision.gameObject.name);
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

}
