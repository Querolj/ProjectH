using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenGlass : Event {
    public Sprite brokenGlass;
    public GameObject glass_splash;

    private Transform glass_splash_loc;
    private bool broke = false;
    protected override void Start()
    {
        base.Start();
        glass_splash_loc = this.GetComponentInChildren<Transform>();
    }

    protected override void GhostAct()
    {
        if(!broke)
        {
            rend.sprite = brokenGlass;
            PlayEventClip();
            Instantiate(glass_splash, glass_splash_loc.position, Quaternion.identity);
            broke = true;
            UpParanLevel(1);
        }
    }

}
