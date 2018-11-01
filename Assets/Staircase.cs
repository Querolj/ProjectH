using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staircase : MonoBehaviour {
    public bool sense = true; //true = left to right(up);

    private EdgeCollider2D edge;
    private PlayerController player;
    private Transform first_step;
    private bool is_colliding = false;
    private bool first_check = false;//Si le joueur pop a un endroit gênant par rapport à l'escalier (pour retire l'edge)
    void Start () {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        if (player == null)
            print("player null");
        edge = this.GetComponent<EdgeCollider2D>();
        first_step = GameObject.Find(this.name + "FirstStep").GetComponent<Transform>();
    }
	
	void Update () {
        float side = player.GetCenterPoint().x - first_step.position.x;
        if (!first_check)
        {
            float dist_vert = player.GetCenterPoint().y - first_step.localPosition.y;
            if (((sense && side > 0) || (!sense && side < 0)) && !(dist_vert > 1))
            {
                edge.enabled = false;
            }
            first_check = true;
        }

        
        if (Input.GetAxis("Vertical") > 0 && !edge.enabled)
        {
            if((sense && side < 0) || (!sense && side > 0))
            edge.enabled = true;
        }
        else if (Input.GetAxis("Vertical") <= 0 && edge.enabled && !is_colliding)
        {
            TryDisableEdge(side);
        }
            
	}

    private void TryDisableEdge(float side)
    {
        float dist_vert = player.GetCenterPoint().y - first_step.localPosition.y;
        if (((sense && side < 0) || (!sense && side > 0)) && !(dist_vert > 1))//dist_vert pb?
        {
            edge.enabled = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player" && is_colliding)
        {
            //player.VerticalFreeze(true);
            if(sense)
                player.OnStairCaseLR = true;
            else
                player.OnStairCaseRL = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (sense)
                player.OnStairCaseLR = false;
            else
                player.OnStairCaseRL = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
            is_colliding = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            is_colliding = false;
            if (sense)
                player.OnStairCaseLR = false;
            else
                player.OnStairCaseRL = false;
        }
            
    }

}
