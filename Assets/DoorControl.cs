using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : Event {
    public Sprite opened;
    public Sprite closed;
    public Sprite semi_opened;

    public bool state = false; //false = closed
    public Vector2 offset_opened;
    public Vector2 size_opened;
    public Vector2 offset_closed;
    public Vector2 size_closed;
    [Space]
    public AudioClip open_sound;
    public AudioClip close_sound;
    public AudioClip semi_sound;
    public int key_code = -1;
    public bool looked = false;

    private BoxCollider2D box;
    private BoxCollider2D box_trigger;
    private SpriteRenderer light_obs;

    protected override void Start () {
        base.Start();
        box = GameObject.Find(this.name + "/Collider").GetComponent<BoxCollider2D>();
        light_obs = GameObject.Find(this.name + "/LightObstacle").GetComponent<SpriteRenderer>();
        box_trigger = this.GetComponent<BoxCollider2D>();
        if (state)
        {
            light_obs.enabled = false;
            box.enabled = false;
        }
    }

    protected override void PlayerAction()
    {
        if (!looked)
        {
            DoorAction();
            RewindParanLevel();
        }
        else if (player.UseKey(key_code))
        {
            player.Warning("Key used", false);
            DoorAction();
            RewindParanLevel();
            looked = false;
        }
        else
            player.Warning("No Key");
    }

    protected override void GhostAct()
    {
        if(!looked)
            SemiOpened();
    }
    private void DoorAction()
    {
        light_obs.color = new Color(1, 1, 1, 1);
        if (!state)
        {
            source.PlayOneShot(open_sound);
            box_trigger.offset = offset_opened;
            box_trigger.size = size_opened;
            light_obs.enabled = false;
            box.enabled = false;
            state = true;
            rend.sprite = opened;
            rend.sortingOrder = -2;
        }
        else
        {
            source.PlayOneShot(close_sound);
            box_trigger.offset = offset_closed;
            box_trigger.size = size_closed;
            light_obs.enabled = true;
            box.enabled = true;
            state = false;
            rend.sprite = closed;
            rend.sortingOrder = 0;
        }
    }
    private void SemiOpened()
    {
        UpParanLevel(1);
        source.PlayOneShot(semi_sound);
        light_obs.color = new Color(1, 1, 1, 0.3f);
        rend.sprite = semi_opened;
    }

    
    
}
