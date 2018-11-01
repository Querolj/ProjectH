using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Light2D;
public class Structure : MonoBehaviour {
    public EdgeCollider2D coll_to_check;
    public GameObject side_objs;
    [Range(0.001f,1)]
    public float renderer_transition_speed;
    public DoorControl door;
    public bool door_sense;//false = left exterior, right interior of the structure
    //Outside object ignored ALL the time
    public GameObject[] objects_to_ignore;//Ignore their colliders when player is inside the structure
    private List<Collider2D> objects_to_ignore_col; //Contains colliders of objects_to_ignore
    public SpriteRenderer[] rends_to_ignore; //Same with renderer


    private List<Collider2D> struct_colliders;
    private Collider2D[] player_colliders;
    private SpriteRenderer[] side_renderers;
    private bool is_activated; //if struct_colliders are activated (meaning they collide with the player)
    private PlayerController player;
    private float current_alpha = 1;
    //Light stuff
    private List<GameObject> light_sources; //light source inside the structure
    private List<GameObject> light_obstacles; //light obstacles inside the structure
    private Collider2D door_collider;

    //Outside object dynamique activity, ignored only when in front of the outside wall
    private List<Collider2D> objects_to_ignore_dyn;//Not used for now
    private List<SpriteRenderer> rends_to_ignore_dyn;

    private StructExtDetector ext_detector;
    private StructIntDetector int_detector;
    void Start () {
        light_sources = new List<GameObject>();
        light_obstacles = new List<GameObject>();
        struct_colliders = new List<Collider2D>();
        objects_to_ignore_col = new List<Collider2D>();
        objects_to_ignore_dyn = new List<Collider2D>();
        rends_to_ignore_dyn = new List<SpriteRenderer>();

        ext_detector = this.GetComponentInChildren<StructExtDetector>();
        int_detector = this.GetComponentInChildren<StructIntDetector>();

        if (side_objs != null)
            side_renderers = side_objs.GetComponentsInChildren<SpriteRenderer>();
        else
            print("side_objs null");
        foreach (Collider2D struct_col in this.GetComponentsInChildren<Collider2D>())
            struct_colliders.Add(struct_col);
        struct_colliders.Remove(this.GetComponent<BoxCollider2D>());
        struct_colliders.Remove(ext_detector.gameObject.GetComponent<Collider2D>());
        struct_colliders.Remove(int_detector.gameObject.GetComponent<Collider2D>());


        player = GameObject.Find("Player").GetComponent<PlayerController>();
        player_colliders = new Collider2D[5];
        player_colliders = player.GetAllColliders();
        if (player_colliders == null)
            print("No player collider");
        if (coll_to_check.enabled)
            is_activated = true;
        else
            is_activated = false;

        //seeking all light_source
        foreach(Transform obj in this.transform)
        {
            foreach(Transform obj_child in obj)
            {
                if (obj_child.gameObject.name == "Light")
                    light_sources.Add(obj_child.gameObject);
                if (obj_child.gameObject.GetComponent<LightObstacleGenerator>() != null)
                    light_obstacles.Add(obj_child.gameObject);
            }
            if (obj.gameObject.name == "Light")
                light_sources.Add(obj.gameObject);
            if (obj.gameObject.GetComponent<LightObstacleGenerator>() != null)
                light_obstacles.Add(obj.gameObject);
        }
        door_collider = door.gameObject.GetComponent<Collider2D>();

        //The structure needs to ignore all objects_to_ignore_col
        if (objects_to_ignore.Length > 0)
        {
            foreach (Collider2D struct_col in struct_colliders)
            {
                foreach (GameObject obj in objects_to_ignore)
                {
                    foreach(Collider2D ignore_col in obj.GetComponentsInChildren<Collider2D>())
                    {
                        Physics2D.IgnoreCollision(ignore_col, struct_col, true);
                        objects_to_ignore_col.Add(ignore_col);
                    }
                }
            }
        }

    }

    void Update () {
		if((!coll_to_check.enabled && is_activated ) || (door!=null && CheckDoorState()))//Hide interior of the structure
        {
            ActivateExtObjRendererDyn(true);
            ActivateExtObjRenderer(true);
            ActiveColliders(true);
            ActivateLight(false);
            player.SetOrderRenderer(Settings.player_order_ext);
        }
        else if(coll_to_check.enabled && !is_activated && door.state)//Activate and show interior of the structure
        {
            ActivateExtObjRendererDyn(false);
            ActivateExtObjRenderer(false);
            ActiveColliders(false);
            ActivateLight(true);
            player.SetOrderRenderer(Settings.player_order_in);
        }

        Transition();
        
	}

    private bool CheckDoorState()//True if player is outside of the structure and door is closed
    {
        float dist = player.GetCenterPoint().x - door.gameObject.transform.position.x;
        return (dist < 0 && !door_sense && !door.state) ||
            (dist > 0 && door_sense && !door.state);
    }

    private void ActivateExtObjRendererDyn(bool b)//Hide or not exterior sprite from detector
    {
        foreach(SpriteRenderer rend_ext in int_detector.GetRenderers())
        {
            print("oy");
            rend_ext.enabled = b;
        }
    }

    private void ActivateExtObjRenderer(bool b) //Hide or not exterior sprite (from objects_to_ignore)
    {
        foreach (SpriteRenderer rend_ext in rends_to_ignore)
        {
            rend_ext.enabled = b;
        }
    }

    private void ActiveColliders(bool b)//Ignore player colliders (false = interior colliders OR true = exterior colliders)
    {
        foreach(Collider2D player_col in player_colliders)//Ignore or not interior collider
        {
            foreach (Collider2D struct_col in struct_colliders)
            {
                Physics2D.IgnoreCollision(player_col, struct_col,b);
            }
            foreach (Collider2D ext_col in objects_to_ignore_col)
            {
                Physics2D.IgnoreCollision(player_col, ext_col, !b);
            }
            if (b && door != null)//Don't desactivate door collision
            {
                Physics2D.IgnoreCollision(player_col, door_collider, false);
            }
        }
        is_activated = !b;
    }

    private void Transition()
    {
        bool do_loop = false;
        if (!is_activated && current_alpha != 1)
        {
            current_alpha += renderer_transition_speed;
            if (current_alpha > 1)
                current_alpha = 1;
            do_loop = true;
        }
        else if(is_activated && current_alpha != 0)
        {
            current_alpha -= renderer_transition_speed;
            if (current_alpha < 0)
                current_alpha = 0;
            do_loop = true;
        }
        if(do_loop)
        {
            foreach (SpriteRenderer rend in side_renderers)
            {
                rend.color = new Color(1, 1, 1, current_alpha);
            }
        }
        
    }
    private void ActivateLight(bool b)
    {
        foreach(GameObject light in light_sources)
        {
            light.SetActive(b);
        }
        foreach(GameObject obstacle in light_obstacles)
        {
            obstacle.SetActive(b);
        }
    }


    //------------------------------------------------------------------------------------------------------------------
    public void CheckRenderer(SpriteRenderer ext_renderer)
    {
        if(ext_detector.CheckMatch(ext_renderer))//if ext_renderer is in exterior zone
        {
            ext_renderer.sortingOrder = Settings.min_order_in - 1;//renderer is put behind the wall of the house
        }
        else//if fully behind the wall, disable the renderer
        {
            ext_renderer.enabled = false;
        }
    }
}
