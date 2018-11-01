using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockController : MonoBehaviour {
    public float speed_lock = 0.05f;

    private Transform t;
    private float directionH;
    private float directionV;
    private PlayerController player;
    private int haunted_level = 0;
    private Color32 init_color;
    private Color32 red_color;

    private SpriteRenderer rend;
    private bool old_direction = false;
    void Start () {
        t = this.GetComponent<Transform>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        rend = this.GetComponent<SpriteRenderer>();
        init_color = new Color32(90,175,50,240);
        red_color = new Color32(200, 70, 50, 240);
    }
	
	void Update () {
        if (old_direction != player.GetDirection())
        {
            haunted_level = 0;
            rend.color = init_color;
        }
            
        directionH = Input.GetAxis("LockHorizontal");
        directionV = Input.GetAxis("LockVertical");
        MoveLock();
        //AdjustColor();

        old_direction = player.GetDirection();
    }

    private void AdjustColor()
    {
        Color32 c = rend.color; ;
        print(haunted_level);
        if(haunted_level > 0)
        {
            if (c.r < red_color.r)
                c.r += 1;
            if (c.g > red_color.g)
                c.g -= 1;
        }
        else
        {
            if (c.r > init_color.r)
                c.r -= 1;
            if (c.g < init_color.g)
                c.g += 1;
        }
        //c = init_color;
        rend.color = c;

    }

    private void MoveLock()
    {
        Vector3 moveVel = t.localPosition;
        if(directionH > 0)
        {
            if (player.GetDirection())
                moveVel.x += speed_lock * Mathf.Abs(directionH);
            else
                moveVel.x -= speed_lock * Mathf.Abs(directionH);
        }
        else if(directionH < 0)
        {
            if (player.GetDirection())
                moveVel.x -= speed_lock * Mathf.Abs(directionH);
            else
                moveVel.x += speed_lock * Mathf.Abs(directionH);
        }
        if (directionV > 0)
        {
            moveVel.y -= speed_lock * Mathf.Abs(directionV);
        }
        else if (directionV < 0)
        {
            moveVel.y += speed_lock * Mathf.Abs(directionV);
        }
        t.localPosition = moveVel;
    }

    /*public void AddHauntedLevel()
    {
        haunted_level++;
    }
    public void LessHauntedLevel()
    {
        haunted_level--;
    }*/
    public void SetHauntedLevel(int level)
    {
        //print("set : " + level.ToString());
        haunted_level += level;
    }
}
