using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Light2D;

public class LampEvent : MonoBehaviour {
    public float inter_clign_min = 0.3f;
    public float inter_clign_max = 1.5f;
    public float alpha_clignote;
    public int paranormal_level = 0;
    public GameObject light_c;

    private LightSprite light_sprite;
    private bool ghost_in = false;
    private float initial_a;
    private bool clignote_on = false;
    private float clignote_time;
    private AudioSource source;
    private LockController lock_cam;
    protected bool added_param_level = false;
    private bool is_on;
    void Start () {
        light_sprite = this.GetComponentInChildren<LightSprite>();
        initial_a = light_sprite.Color.a;
        source = this.GetComponent<AudioSource>();
        lock_cam = GameObject.Find("Player").GetComponentInChildren<LockController>();

    }
	
	void Update () {
        if(ghost_in)
        {
            if(clignote_time > 0)
            {
                clignote_time -= Time.deltaTime;
                if(clignote_time <= 0)
                {
                    clignote_time = Random.Range(inter_clign_min, inter_clign_max);
                    clignote_on = !clignote_on;
                    light_sprite.Color = new Color(1,1,1, alpha_clignote);
                }
            }
            if(clignote_on)
                Clignote();
        }
        else
        {
            RewindParanLevel();
        }
            
    }

    private void Clignote(float minus_max = 0.1f)
    {
        UpParanLevel(1);
        float random_a = Random.Range(0, initial_a - minus_max);
        Color c = new Color(1, 1, 1, random_a);
        light_sprite.Color = c;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "ghost")
        {
            if(light_c.activeInHierarchy)
                source.Play();
            clignote_time = Random.Range(inter_clign_min, inter_clign_max);
            clignote_on = true;
            ghost_in = true;
        }
        if(collision.tag == "Lock")
        {
            lock_cam.SetHauntedLevel(paranormal_level);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "ghost")
        {
            source.Stop();
            ghost_in = false;
            light_sprite.Color = new Color(1, 1, 1, initial_a);
        }
        if (collision.tag == "Lock")
        {
            lock_cam.SetHauntedLevel(-paranormal_level);
        }
    }
    private void RewindParanLevel()//Met paranormal_level à 0
    {
        if (paranormal_level > 0)
            lock_cam.SetHauntedLevel(-paranormal_level);
        paranormal_level = 0;
    }

    private void UpParanLevel(int level)
    {
        added_param_level = false;
        paranormal_level = level;
        if (paranormal_level <= 0)
            print("error UpParanLevel");
    }

    public void ActiveLight()
    {
        is_on = true;
        light_c.SetActive(true);
    }
    public void ShutdownLight()
    {
        is_on = false;
        light_c.SetActive(false);
    }

    public bool IsActive()
    {
        return is_on;
    }
}
