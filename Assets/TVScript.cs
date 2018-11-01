using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVScript : Event {
    public Sprite spriteOff;
    public Sprite[] spritesOn;
    public Sprite[] spritesHaunted;
    public bool is_on = false;
    public int fps_on = 15;
    public AudioClip tv_on_sound;
    public GameObject light_tv;

    private bool haunted = false;

    protected override void Update()
    {
        base.Update();
        if (is_on)
        {
            if (!light_tv.activeInHierarchy)
                light_tv.SetActive(true);
            int index = (int)(Time.timeSinceLevelLoad * fps_on);
            if (!haunted)
            {
                RewindParanLevel();
                index = index % spritesOn.Length;
                rend.sprite = spritesOn[index];
            }
            else
            {
                UpParanLevel(2);
                index = index % spritesHaunted.Length;
                rend.sprite = spritesHaunted[index];
            }
        }
        else
        {
            if (light_tv.activeInHierarchy)
                light_tv.SetActive(false);
            RewindParanLevel();
        }
            
    }

    protected override void PlayerAction()
    {
        if (is_on)
        {
            is_on = false;
            rend.sprite = spriteOff;
            source.Stop();
        }
        else
        {
            source.Play();
            is_on = true;
        }
    }

    protected override void GhostAct()
    {
        is_on = !is_on;
        if (!is_on)
        {
            source.Stop();
            rend.sprite = spriteOff;
        }
        else
            source.Play();
    }

    public bool Haunted
    {
        get { return haunted; }
        set { haunted = value; }
    }

}
