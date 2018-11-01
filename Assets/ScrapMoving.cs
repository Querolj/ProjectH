using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapMoving : Event {
    public float scrap_throw_str = 30;

    private Rigidbody2D body;
    private float rotate_time = 0;
    private CircleCollider2D circ;
    private bool ignored = false;
    protected override void Start()
    {
        base.Start();
        body = this.GetComponent<Rigidbody2D>();
        circ = this.GetComponent<CircleCollider2D>();
    }
    protected override void Update()
    {
        if (!ignored)
        {
            ignored = true;
            Physics2D.IgnoreCollision(circ, player.GetColl());
        }
        base.Update();
        if(rotate_time > 0)
        {
            rotate_time -= Time.deltaTime;
            if(rotate_time <= 0)
            {
                body.constraints = RigidbodyConstraints2D.FreezeRotation;
                RewindParanLevel();
            }
        }
    }

    protected override void GhostAct()
    {
        body.constraints = RigidbodyConstraints2D.None;
        rotate_time = 2f;
        UpParanLevel(1);
        int r = Random.Range(0, 2);
        if (r == 0)
            body.AddForce(Vector2.left * scrap_throw_str);
        else
            body.AddForce(Vector2.right * scrap_throw_str);
    }
}
