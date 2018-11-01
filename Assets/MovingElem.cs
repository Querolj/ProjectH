using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingElem : MonoBehaviour {
    public Sprite[] movingSprites;
    public float speed_mult = 0.5f;
    public int fps_moving;

    private Transform move_pointL;
    private Transform move_pointR;
    private Transform starting_parent;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D body;
    private float old_x;
    void Start () {
		if(this.transform.parent != null)
        {
            starting_parent = this.transform.parent.transform;
            move_pointL = GameObject.Find(this.transform.parent.name + "/" + this.name + "/MovePointL").GetComponent<Transform>();
            move_pointR = GameObject.Find(this.transform.parent.name + "/" + this.name + "/MovePointR").GetComponent<Transform>();
        }
        else
        {
            starting_parent = null;
            move_pointL = GameObject.Find(this.name + "/MovePointL").GetComponent<Transform>();
            move_pointR = GameObject.Find(this.name + "/MovePointR").GetComponent<Transform>();
        }
        old_x = this.transform.position.x;
        body = this.GetComponent<Rigidbody2D>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
	
	void Update () {
        if(old_x > 0)
        {
            int index = (int)(Time.timeSinceLevelLoad * fps_moving);
            index = index % movingSprites.Length;
            spriteRenderer.sprite = movingSprites[index];
        }
        else if(old_x < 0)
        {
            int index = (int)(Time.timeSinceLevelLoad * fps_moving);
            index = index % movingSprites.Length;
            index = Mathf.Abs(index - (movingSprites.Length-1));
            spriteRenderer.sprite = movingSprites[index];
        }

        old_x = body.velocity.x;
    }

    public Vector3 GetMovePointL()
    {
        return move_pointL.position;
    }
    public Vector3 GetMovePointR()
    {
        return move_pointR.position;
    }
    public Transform GetTransform()
    {
        return this.transform;
    }
    public void SetTransform(Vector3 v)
    {
        this.transform.position = v;
    }
    public Transform GetStartingParent()
    {
        return starting_parent;
    }
    public Rigidbody2D GetBody()
    {
        return body;
    }
}
