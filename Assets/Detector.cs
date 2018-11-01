using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour {
    public InventoryGestion inventory;

    private PlayerController player;
    private PlayerAnimation player_anime;
    private MovingElem elem_to_move;
    private Transform elem_t;
    private bool action = false;
    private bool obj_insight = false;
    private bool is_moving_obj = false;

    //Inventory Object
    private InventoryObject target_obj;
    private bool start_taking_obj = false;
    private WarningDisplay warning;
	void Start () {
        player = this.GetComponentInParent<PlayerController>();
        player_anime = this.GetComponentInParent<PlayerAnimation>();
        warning = GameObject.Find("CanvasWarning/Warning").GetComponent<WarningDisplay>();
        target_obj = null;
    }
	
	void Update () {
        action = Input.GetButtonDown("Jump");

        PushingObjectModule();
        TakeObject();
    }

    private void TakeObject()
    {
        if (!player.IsDoingAction() && action && target_obj != null)
        {
            player.StartAction();
            start_taking_obj = true;
        }
        if(start_taking_obj && !player.IsDoingAction())
        {
            start_taking_obj = false;
            AddObjectToInventory(target_obj);
        }
    }

    private void PushingObjectModule()
    {
        float direction_y = Input.GetAxis("Vertical");
        
        //print(CheckCenterPoint());
        if (!player.IsDoingAction() && (action || (is_moving_obj && Input.GetButton("Jump"))) && obj_insight && CheckCenterPoint())
        {
            if (!is_moving_obj)
            {
                if (player.GetDirection())
                {
                    player.SetPositionX(elem_to_move.GetMovePointL().x);
                }
                else
                {
                    player.SetPositionX(elem_to_move.GetMovePointR().x);
                }
                player.StartPushingObject(elem_to_move.gameObject, elem_to_move.speed_mult, elem_to_move.GetBody());
            }
            is_moving_obj = true;

        }
        else if (is_moving_obj)
        {
            is_moving_obj = false;
            player.StopPushingObject();
            //RemoveFromPlayer();
        }
    }

    private bool CheckCenterPoint()
    {
        float sense = player.GetCenterPoint().x - elem_t.position.x;
        if ((sense > 0 && !player.GetDirection()) ||
            (sense < 0 && player.GetDirection()))
        {
            return true;
        }
        return false;
    }

    private void RemoveFromPlayer()
    {
        if(elem_to_move != null)
        {
            elem_to_move.gameObject.transform.parent = elem_to_move.GetStartingParent();
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "inventory_object" && target_obj == null)
        {
            target_obj = collision.gameObject.GetComponent<InventoryObject>();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "movable")
        {
            obj_insight = true;
            elem_to_move = collision.gameObject.GetComponent<MovingPoint>().GetMovingElem();
            elem_t = elem_to_move.GetTransform();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "movable" && !is_moving_obj )
        {
            obj_insight = false;
        }
        if (collision.tag == "inventory_object" && !start_taking_obj)
        {
            target_obj = null;
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    public bool IsMovingObj
    {
        get { return is_moving_obj; }
        //set { is_moving_obj = value; }
    }

    public void PlaceObject(float added_speed)
    {
        print(added_speed);
        Vector3 v = elem_t.position;
        v.x += added_speed;
        elem_t.position = v;
    }

    public void AddObjectToInventory(InventoryObject obj)
    {
        if (inventory.AddObject(obj))
            GameObject.Destroy(obj.gameObject);
        else
            warning.StartBuzz("Inventory Full");
    }
}
