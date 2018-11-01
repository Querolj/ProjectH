using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Event : MonoBehaviour {
    public float avoid_ghostev_cd = 10f;
    public float chance_ghost_to_act = 1;
    public bool player_action = true;
    public AudioClip event_clip;
    public int paranormal_level = 0;

    [Space]
    [Space]
    protected SpriteRenderer rend;
    protected bool entered = false;
    protected PlayerController player;
    protected bool action_initiated = false;
    protected float avoid_ghostev_timer = 0;
    protected AudioSource source;
    protected Transform t;

    private int init_plevel;
    private LockController lock_cam;
    protected bool added_param_level = false;
    protected virtual void Start () {
        avoid_ghostev_cd = avoid_ghostev_cd * GhostState.ghost_active_mult;
        rend = this.GetComponent<SpriteRenderer>();
        GameObject p = GameObject.Find("Player");
        player = p.GetComponent<PlayerController>();
        lock_cam = p.GetComponentInChildren<LockController>();
        source = this.GetComponent<AudioSource>();
        t = this.GetComponent<Transform>();
        init_plevel = paranormal_level;
    }

    protected virtual void Update () {
        if (avoid_ghostev_timer > 0)
            avoid_ghostev_timer -= Time.deltaTime;

        if (player_action && entered && Input.GetButtonDown("Jump") && !player.IsDoingAction())
        {
            action_initiated = true;
            player.StartAction();
        }

        if (action_initiated && !player.IsDoingAction())
        {
            action_initiated = false;
            PlayerAction();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            entered = true;
            OnPlayerEnter();
        }
        else if (collision.tag == "ghost" && avoid_ghostev_timer <= 0)
        {
            avoid_ghostev_timer = avoid_ghostev_cd;
            float r = Random.Range(0f, 1f);
            if (r <= chance_ghost_to_act * GhostState.chance_mult)
                GhostAct();
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            entered = false;
        }
        if (collision.tag == "Lock")
        {
            //paranormal_level = init_plevel;
            lock_cam.SetHauntedLevel(-paranormal_level);
            added_param_level = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Lock" && !added_param_level)
        {
            lock_cam.SetHauntedLevel(paranormal_level);
            added_param_level = true;
        }
    }

    protected virtual void GhostAct()
    {

    }

    protected virtual void PlayerAction()
    {

    }

    protected virtual void OnPlayerEnter()
    {

    }

    protected virtual void PlayEventClip(bool loop = false)
    {
        if (source != null && event_clip != null)
        {
            source.loop = loop;
            //source.Play();
            source.PlayOneShot(event_clip);
        }
        else
        {
            print("can't play sound");
        }
    }

    protected virtual void RewindParanLevel()//Met paranormal_level à 0
    {
        if (paranormal_level > 0)
            lock_cam.SetHauntedLevel(-paranormal_level);
        paranormal_level = 0;
    }

    protected virtual void UpParanLevel(int level)
    {
        added_param_level = false;
        paranormal_level = level;
        if (paranormal_level <= 0)
            print("error UpParanLevel");
    }
}
