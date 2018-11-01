using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairCaseFloor : MonoBehaviour {
    public float fading_rate = 0.02f;

    private CapsuleCollider2D player_col;
    private bool entered = false;
    private BoxCollider2D box_col;
    private bool col_enable = true;
    private SpriteRenderer rend;

    //private bool fading_on = false;
    //private bool fading_sense; //true == appear
    private PlayerController player;
    private Transform t;
    void Start () {
        player_col = GameObject.Find("Player").GetComponent<CapsuleCollider2D>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        box_col = GameObject.Find(this.transform.parent.name).GetComponent<BoxCollider2D>();
        t = GameObject.Find(this.transform.parent.name+"/"+this.name+"/FadingPoint").GetComponent<Transform>();
        rend = this.GetComponent<SpriteRenderer>();
    }
	
	void Update () {
		if(entered && col_enable && Input.GetAxis("Vertical") == -1)
        {
            col_enable = false;
            Physics2D.IgnoreCollision(player_col, box_col);
        }
        else if(!col_enable && !entered)
        {
            col_enable = true;
            Physics2D.IgnoreCollision(player_col, box_col, false);
        }
        //float dist = player.GetPosition().y - t.position.y;
        if (player.GetPosition().y - t.position.y > 0)
            Fading(true);
        else
            Fading(false);
 

    }

    private void Fading(bool b)//true = appear
    {
        Color c = rend.color;
        if (b)
        {
            if(c.a + fading_rate >= 1)
            {
                c.a = 1;
            }
            else
                c.a += fading_rate;
        }
        else
        {
            if (c.a - fading_rate <= 0)
            {
                c.a = 0;
            }
            else
                c.a -= fading_rate;
        }
        rend.color = c;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            entered = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            entered = false;
        }
    }
}
