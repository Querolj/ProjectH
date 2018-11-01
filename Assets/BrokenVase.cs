using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenVase : Event {
    public Sprite broken_vase;
    public float drop_str = 10;

    private bool broken = false;
    private Rigidbody2D body;
    private bool is_dropping = false;
    private bool dropped = false;
    private CapsuleCollider2D caps;
    private bool ignored = false;
    protected override void Start()
    {
        base.Start();
        t = this.GetComponent<Transform>();
        body = this.GetComponent<Rigidbody2D>();
        caps = this.GetComponent<CapsuleCollider2D>();
        
        body.isKinematic = true;
    }

    protected override void Update()
    {
        if(!ignored)
        {
            ignored = true;
            Physics2D.IgnoreCollision(caps, player.GetColl());
        }

        base.Update();
        if(is_dropping && !body.isKinematic)
        {
            is_dropping = false;
            int r = Random.Range(0, 2);
            if (r == 0)
                body.AddForce(Vector2.left * drop_str * GhostState.ghost_strenght_mult);
            else
                body.AddForce(Vector2.right * drop_str * GhostState.ghost_strenght_mult);
        }
    }

    protected override void GhostAct()
    {
        if(!dropped)
        {
            Quaternion q = t.rotation;
            q.z = 0.1f;
            t.rotation = q;
            is_dropping = true;
            body.isKinematic = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "ground")
        {
            Quaternion q = t.rotation;
            q.z = 0;
            t.rotation = q;
            rend.sprite = broken_vase;
            body.constraints = RigidbodyConstraints2D.FreezeAll;
            dropped = true;
            PlayEventClip();
            UpParanLevel(1);
        }
    }

}
