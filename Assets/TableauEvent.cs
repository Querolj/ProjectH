using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableauEvent : Event {
    public Sprite empty_frame;
    public float event_dist = 1.1f;
    public TVScript tv;

    private bool ev_activated = false;
    protected override void GhostAct()
    {
        float dist = Mathf.Abs(player.GetPosition().x - t.position.x);
        float dist_vert = Mathf.Abs(player.GetPosition().y - t.position.y);
        if (!entered && !ev_activated && dist >= event_dist && dist_vert < 1f)
        {
            ev_activated = true;
            rend.sprite = empty_frame;
            tv.Haunted = true;
            UpParanLevel(2);
        }
    }

}
