using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffLight : Event {
    public LampEvent lamp;
    public Sprite On;
    public Sprite Off;


    protected override void PlayerAction()
    {
        SwitchLight();
    }
    protected override void GhostAct()
    {
        SwitchLight();
    }

    private void SwitchLight()
    {
        PlayEventClip();
        if (lamp.IsActive())
        {
            lamp.ShutdownLight();
            rend.sprite = Off;
        }
        else
        {
            lamp.ActiveLight();
            rend.sprite = On;
        }
    }

    

}
