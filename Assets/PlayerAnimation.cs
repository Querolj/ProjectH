using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    public Sprite[] spritesWalking;
    public Sprite[] spritesWalkingCamDown;
    public Sprite[] spritesWalkingLookUp;
    public Sprite[] spritesStanding;
    public Sprite[] spritesAction;
    public Sprite[] spritesCamDown;
    public Sprite[] spritesMount;
    public Sprite[] spritesPushing;
    public Sprite spriteLookUp;

    public int fps_walking;
    public int fps_standing;
    public int fps_pushing;
    public float timecd_action=0.4f;
    public float timecd_mount = 0.3f;
    private PlayerController player;
    private SpriteRenderer spriteRenderer;
    private int count_action = 0;
    private int count_mount = 0;
    private float time_action;
    private float time_mount;
    private bool start_action = false;
    private bool start_mount = false;
    private bool anime_cam = false;
    private bool pushing = false;
    void Start () {
        player = this.GetComponent<PlayerController>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
	
	void Update () {
        FlipRenderer();

        if(start_action)
        {
            if(time_action > 0)
            {
                time_action -= Time.deltaTime;
                if(time_action <= 0)
                {
                    count_action++;
                    time_action = timecd_action;
                }
            }
            //if (count_action == spritesAction.Length - 1)
            //    start_action = false;
            if (count_action < spritesAction.Length)
                spriteRenderer.sprite = spritesAction[count_action];
            else
            {
                player.BlockMove = false;
                start_action = false;
                count_action = 0;
            }
        }
        else if(start_mount)
        {
            if (time_mount > 0)
            {
                time_mount -= Time.deltaTime;
                if (time_mount <= 0)
                {
                    count_mount++;
                    time_mount = timecd_mount;
                }
            }

            if (count_mount < spritesMount.Length)
                spriteRenderer.sprite = spritesMount[count_mount];
            else
            {
                player.StopMounting();
                start_mount = false;
                count_mount = 0;
            }
        }
        else if(pushing)
        {
            int index_pushing = (int)(Time.timeSinceLevelLoad * fps_pushing);
            if (player.IsMoving())
            {
                index_pushing = index_pushing % spritesPushing.Length;
                spriteRenderer.sprite = spritesPushing[index_pushing];
            }
                
        }
        else if(anime_cam)
        {
            if (time_action > 0)
            {
                time_action -= Time.deltaTime;
                if (time_action <= 0)
                {
                    count_action++;
                    time_action = timecd_action;
                }
            }
            if (count_action < spritesCamDown.Length)
            {
                if(player.GetCamOn())
                {
                    spriteRenderer.sprite = spritesCamDown[count_action];
                }
                else
                {
                    spriteRenderer.sprite = spritesCamDown[spritesCamDown.Length - count_action - 1];
                }
            }
            else
            {
                player.BlockMove = false;
                count_action = 0;
                anime_cam = false;
            }
        }
        else if (player.IsMoving())
        {
            int index_walking = (int)(Time.timeSinceLevelLoad * fps_walking);
            if (player.LookUp)
            {
                index_walking = index_walking % spritesWalkingLookUp.Length;
                spriteRenderer.sprite = spritesWalkingLookUp[index_walking];
            }
            else
            {
                index_walking = index_walking % spritesWalking.Length;
                spriteRenderer.sprite = spritesWalking[index_walking];
            }
        }
        else
        {
            if(player.LookUp)
            {
                spriteRenderer.sprite = spriteLookUp;
            }
            else
            {
                int index_standing = (int)(Time.timeSinceLevelLoad * fps_standing);
                index_standing = index_standing % spritesStanding.Length;
                spriteRenderer.sprite = spritesStanding[index_standing];
            }
            
        }
	}

    private void FlipRenderer()
    {
        Quaternion q = player.GetRotation();
        if (player.GetDirection())
        {
            q.y = 0;
        }
        else
        {
            q.y = 180;
        }
        player.SetRotation(q);
    }


    //---------------------------------------------------------------------------------------------
    public void StartAction()
    {
        start_action = true;
        time_action = timecd_action;
    }
    public void StartMount()
    {
        start_mount = true;
        time_mount = timecd_mount;
    }
    public bool GetStartAction()
    {
        return start_action;
    }
    public void AnimeCamStart()
    {
        anime_cam = true;
        time_action = timecd_action;
    }
    public void StartPushing()
    {
        pushing = true;
        spriteRenderer.sprite = spritesPushing[0];
    }
    public void StopPushing()
    {
        pushing = false;
    }

}
