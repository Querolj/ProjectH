using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed;
    public float jumpVelocity = 10;
    public bool disable_jump = true;

    private float current_speed;
    private float direction;
    private float direction_y;
    private Transform center_point;
    private Rigidbody2D body;
    private Transform t;
    private PlayerAnimation anime;
    private CapsuleCollider2D caps;
    private Collider2D[] all_colliders;
    private SpriteRenderer rend;
    private bool directed = false;
    private bool can_jump = false;
    private bool on_staircase_lr = false;
    private bool on_staircase_rl = false;
    private bool block_move = false;
    private bool look_up = false;
    //LightTransform
    private Transform light_t;
    private Vector3 light_normal;
    private Quaternion light_normal_rot;
    private Vector3 light_up;
    private Quaternion light_up_rot;
    private LockController lock_cam;
    //Mount
    private bool is_mounting = false;
    private bool can_mount = false;
    private float mount_pointy;
    //Mounr Full Elem (x-axis locked)
    private float mount_point_left;
    private float mount_point_right;
    private Vector3 elem_position;
    private bool full_elem_mount = false;
    //GroundCheck
    private GroundChecker ground_checker;
    //Mod Camless
    private bool cam_on = true;
    private bool square = false;
    //Moving obj
    private Detector detector;
    private float pos_diff = 0;
    private float old_pos_x = 0;
    private bool switch_direction = true; //player can change direction?
    private bool old_directed;
    private bool is_pushing = false;
    private Rigidbody2D body_pushed;
    //Inventory
    private GameObject inventory;
    private bool open_inv = false;
    //Cam Gestion
    private FollowPlayer cam;
    private float t_speed = -1;
    //Warning
    private WarningDisplay warning;
    //Anti slope
    private float angle;
    void Start() {
        t = this.GetComponent<Transform>();
        current_speed = speed;
        ground_checker = this.GetComponentInChildren<GroundChecker>();
        light_t = GameObject.Find(this.name + "/Light").GetComponent<Transform>();
        center_point = GameObject.Find(this.name + "/CenterPoint").GetComponent<Transform>();
        light_normal = light_t.localPosition;
        light_normal_rot = light_t.localRotation;
        Transform up = GameObject.Find(this.name + "/LightUp").GetComponent<Transform>();
        light_up = up.localPosition;
        light_up_rot = up.localRotation;
        body = this.GetComponent<Rigidbody2D>();
        anime = this.GetComponent<PlayerAnimation>();
        lock_cam = this.GetComponentInChildren<LockController>();
        caps = this.GetComponent<CapsuleCollider2D>();
        detector = this.GetComponentInChildren<Detector>();
        inventory = GameObject.Find(this.name + "/Inventory");
        cam = GameObject.Find("Main Camera").GetComponent<FollowPlayer>();
        warning = GameObject.Find("CanvasWarning/Warning").GetComponent<WarningDisplay>();
        all_colliders = this.GetComponentsInChildren<Collider2D>();
        rend = this.GetComponent<SpriteRenderer>();
        inventory.SetActive(false);
        pos_diff = t.position.x;
    }

    void Update() {
        if (!can_jump)
            can_jump = Input.GetButtonDown("Jump");
        if (!square)
            square = Input.GetButtonDown("Square");
        if(Input.GetButtonDown("Triangle"))
        {
            InventoryAction();
        }
        DetectSlope();
        pos_diff = Mathf.Abs(t.position.x - old_pos_x);
        old_pos_x = t.position.x;
    }
    void FixedUpdate()
    {
        direction = Input.GetAxis("Horizontal");
        direction_y = Input.GetAxis("Vertical");

        if (direction_y > 0)
        {
            look_up = true;
            light_t.localPosition = light_up;
            light_t.localRotation = light_up_rot;
        }
        else
        {
            look_up = false;
            light_t.localPosition = light_normal;
            light_t.localRotation = light_normal_rot;
        }
        
        if(!IsDoingAction())
        {
            if (can_jump)
                Jump();
            else if (square)
            {
                CamDown();
            }
        }

        if(!block_move)
        {
            if(Mathf.Abs(direction) > 0.3 && !is_mounting)
                Move(direction);
            else if(Mathf.Abs(direction_y) > 0.3 && (OnStairCaseLR || OnStairCaseRL) && !is_mounting)
            {
                Move(direction_y);
            }
            else if(Mathf.Abs(direction_y) > 0.3 && can_mount && !is_mounting)
            {
                StartMounting(t_speed);
            }
        }
        
        //Anti Glissade sur escalier
        if ((OnStairCaseLR || OnStairCaseRL) && direction == 0 && direction_y == 0)
            Freeze(true);
        else
            Freeze(false);
    }

    private void DetectSlope() //Check angle beneath the player
    {
        RaycastHit2D[] hits = new RaycastHit2D[2];
        int h = Physics2D.RaycastNonAlloc(center_point.position, -Vector2.up, hits, 1,layerMask: LayerMask.NameToLayer("ground")); //cast downwards
        if (h >= 1)
        {
            Debug.Log("ola : " + hits[0].normal);

            angle = Mathf.Abs(Mathf.Atan2(hits[0].normal.x, hits[0].normal.y) * Mathf.Rad2Deg); //get angle
            Debug.Log(angle);

            if (angle > 30)
            {
                print(angle);
                //DoSomething(); //change your animation
            }
        }
    }

    private void InventoryAction()
    {
        if (!inventory.activeInHierarchy)
        {
            inventory.SetActive(true);
            block_move = true;
            cam.LockCam = true;
        }
        else
        {
            block_move = false;
            cam.LockCam = false;
            inventory.SetActive(false);
        }
    }

    private void StartMounting(float t_speed = -1)//t_speed = transition speed for camera when mounting an object
    {
        if(!full_elem_mount)
        {
            Vector3 v = t.position;
            v.y = mount_pointy;
            //camera transition
            if (speed <= 0)
                cam.ActivateCameraTransition(v, false, false);
            else
                cam.ActivateCameraTransition(v, false, false, t_speed);

            is_mounting = true;
            anime.StartMount();
            body.velocity = new Vector2(0, 0);
            t.position = v;
            body.isKinematic = true;
        }
        else
        {
            Vector3 v = t.position;
            v.y = mount_pointy;
            
            //Look at where is the player relatif to the element to mount
            float dist = GetCenterPoint().x - elem_position.x;
            if(dist > 0)//player to the right of the element
            {
                v.x = mount_point_right;
            }else//left
            {
                v.x = mount_point_left;
            }

            //camera transition
            if (speed <= 0)
                cam.ActivateCameraTransition(v, false, false);
            else
                cam.ActivateCameraTransition(v, false, false, t_speed);

            is_mounting = true;
            anime.StartMount();
            body.velocity = new Vector2(0, 0);
            t.position = v;
            body.isKinematic = true;

        }
        
    }
    public void ReadyMount(float y, float t_speed = -1)//t_speed = transition speed for camera when mounting an object
    {
        this.t_speed = t_speed;
        full_elem_mount = false;
        can_mount = true;
        mount_pointy = y;
    }

    public void ReadyMount(float v, float left, float right, Vector3 elem_position, float t_speed = -1)//t_speed = transition speed for camera when mounting an object
    {
        print("rdy");

        this.t_speed = t_speed;
        full_elem_mount = true;
        mount_point_left = left;
        mount_point_right = right;
        this.elem_position = elem_position;
        can_mount = true;
        mount_pointy = v;
    }

    public void StopMounting()
    {
        print("stop mount");
        is_mounting = false;
        block_move = false;
        can_mount = false;
        body.isKinematic = false;
        body.velocity = new Vector2(0, 0);
    }
    private void CamDown()
    {
        square = false;
        block_move = true;
        cam_on = !cam_on;
        anime.AnimeCamStart();
    }


    private void Move(float hinput)
    {
        Vector2 moveVel = body.velocity;
        if (OnStairCaseLR)
        {
            if (hinput > 0)
            {
                directed = true;
                moveVel.x = (current_speed * hinput)/2;
                moveVel.y = (current_speed * hinput) / 2;
            }
            else if (hinput < 0)
            {
                directed = false;
                moveVel.x = (current_speed * hinput) / 2;
                moveVel.y = ((current_speed * hinput) / 2);
            }
        }
        else if(OnStairCaseRL)
        {
            if (hinput > 0)
            {
                directed = true;
                moveVel.x = (current_speed * hinput) / 2;
                moveVel.y = -((current_speed * hinput) / 2);
            }
            else if (hinput < 0)
            {
                directed = false;
                moveVel.x = (current_speed * hinput) / 2;
                moveVel.y = (current_speed * hinput) / 2;
            }
        }
        else if(switch_direction)
        {
            if (hinput > 0)
            {
                directed = true;
                moveVel.x = current_speed * hinput;
            }
            else if (hinput < 0)
            {
                directed = false;
                moveVel.x = current_speed * hinput;
            }
        }
        else
        {
            if(!old_directed && hinput < 0)
                moveVel.x = current_speed * hinput;
            else if(old_directed && hinput > 0)
                moveVel.x = current_speed * hinput;

        }

        if(switch_direction)
            old_directed = directed;
        if (is_pushing)
            body_pushed.velocity = moveVel;
        body.velocity = moveVel;
    }
    private void Jump()
    {
        if(!disable_jump)
        {
            body.velocity = new Vector2(0f, 0f);
            body.AddForce(new Vector2(0f, jumpVelocity));
            can_jump = false;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------
    public bool GetDirection()
    {
        return directed;
    }
    public void SetPosition(Vector3 v)
    {
        t.position = v;
    }
    public void SetPositionX(float x)
    {
        Vector3 v = t.position;
        v.x = x;
        t.position = v;
    }
    public Collider2D[] GetAllColliders()
    {
        if(all_colliders == null)
            all_colliders = this.GetComponentsInChildren<Collider2D>();
        return all_colliders;
    }
    public bool IsMoving()
    {
        if (Mathf.Abs(body.velocity.x) > 0)
            return true;
        return false;
    }
    public Vector3 GetPosition()
    {
        return t.position;
    }
    public Vector3 GetCenterPoint()
    {
        return center_point.position;
    }
    public Quaternion GetRotation()
    {
        return t.localRotation;
    }
    public void SetRotation(Quaternion q)
    {
        t.localRotation = q;
    }
    public void SetOrderRenderer(int o)
    {
        rend.sortingOrder = o;
    }
    public void Freeze(bool b)
    {
        if (b)
            body.constraints = RigidbodyConstraints2D.FreezeAll;
        else
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public bool OnStairCaseLR
    {
        get { return on_staircase_lr; }
        set { on_staircase_lr = value; }
    }
    public bool OnStairCaseRL
    {
        get { return on_staircase_rl; }
        set { on_staircase_rl = value; }
    }
    public bool BlockMove
    {
        get { return block_move; }
        set { block_move = value; }
    }
    public bool LookUp
    {
        get { return look_up; }
        set { look_up = value; }
    }
    public bool CanMount
    {
        get { return can_mount; }
        set { can_mount = value; }
    }
    public bool SwitchDirection
    {
        get { return switch_direction; }
        set { switch_direction = value; }
    }
    public void StartAction()
    {
        anime.StartAction();
        block_move = true;
    }
    public LockController GetLock()
    {
        return lock_cam;
    }


    public bool IsDoingAction()
    {
        return anime.GetStartAction();
    }
    
    public CapsuleCollider2D GetColl()
    {
        return caps;
    }
    public bool GetCamOn()
    {
        return cam_on;
    }
    public void StartPushingObject(GameObject obj, float speed_mult, Rigidbody2D obj_body)
    {
        current_speed *= speed_mult;
        body.velocity = new Vector2(0,0);
        switch_direction = false;
        is_pushing = true;
        body_pushed = obj_body;
        anime.StartPushing();
        //AddChildObject(obj);
    }
    public void StopPushingObject()
    {
        current_speed = speed;
        anime.StopPushing();
        switch_direction = true;
        is_pushing = false;
    }

    public void AddChildObject(GameObject obj)
    {
        obj.transform.parent = this.transform;
    }
    public void Warning(string msg, bool b = true)
    {
        warning.StartBuzz(msg,b);
    }

    public bool UseKey(int code)//true if key used
    {
        return inventory.GetComponent<InventoryGestion>().UseKey(code);
    }
    public bool IsOnGround()
    {
        return ground_checker.OnGround;
    }
}

