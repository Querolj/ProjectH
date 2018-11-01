using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
    public Transform player_t;
    public Vector3 offset;
    public float limite_right;
    public float limite_left;
    public float limit_up;
    public float limit_down;
    public float offset_x_fading = 0.01f;
    public float transition_speed = 0.025f;

    private float offset_x_add = 0;
    private PlayerController player;

    //Transition
    private Vector3 transition_point;
    private Vector3 vector_transition;
    private bool transition = false;
    private bool old_direction = false;
    private bool lock_cam = false;
    private bool x_cam = true;
    private bool y_cam = false;
    void Start()
    {
        offset_x_add = -offset.x;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        SetPosition();
        old_direction = player.GetDirection();
    }

    void Update()
    {
        if (transition)
            Transition();
        else if(!lock_cam)
            SetPosition();
        old_direction = player.GetDirection();
    }

    bool LimiteRL()
    {
        return limite_right > player_t.position.x && limite_left < player_t.position.x;
    }
    bool LimiteUD()
    {
        return limit_up > player_t.position.y && limit_down < player_t.position.y;
    }

    private void Transition()
    {
        if (((vector_transition.x > 0 && transform.position.x > transition_point.x )||
           (vector_transition.x < 0 && transform.position.x < transition_point.x)) && !x_cam)
        {
            if(player.GetDirection())
                offset_x_add = offset.x;
            else
                offset_x_add = -offset.x;
            transition = false;
        }

        if (((vector_transition.y > 0 && transform.position.y > transition_point.y) ||
           (vector_transition.y < 0 && transform.position.y < transition_point.y)) && !y_cam)
        {
            transition = false;
        }

        transform.position = new Vector3(transform.position.x + vector_transition.x , transform.position.y +vector_transition.y, offset.z);
    }

    public void ActivateCameraTransition(Vector3 transition_point, bool x_cam = true, bool y_cam = true, float speed = -1)
    {
        Vector3 v = transition_point;
        if(player.GetDirection())
            v.x += offset.x;
        else
            v.x -= offset.x;
        v.y += offset.y;
        transition_point = v;
        this.transition_point = transition_point;
        this.x_cam = x_cam;
        this.y_cam = y_cam;
        float x,y;
        if (x_cam)
            x = 0;
        else
            x = transition_point.x - transform.position.x;
        if (y_cam)
            y = 0;
        else
            y = transition_point.y - transform.position.y;
        float total = Mathf.Abs(x) + Mathf.Abs(y);
        x = x / total;
        y = y / total;
        if(speed <= 0)
            vector_transition = new Vector3(x , y) * transition_speed;
        else
            vector_transition = new Vector3(x, y) * speed;
        transition = true;
    }

    public void SetPosition()
    {
        if (LimiteRL() && LimiteUD())
        {
            if (offset_x_add < offset.x && player.GetDirection())
            {
                offset_x_add += offset_x_fading;
            }
            else if(offset_x_add > -offset.x && !player.GetDirection())
            {
                offset_x_add -= offset_x_fading;
            }
                
            if (player.GetDirection())
            {
                transform.position = new Vector3(player_t.position.x + offset_x_add, player_t.position.y + offset.y, offset.z);
            }
            else
            {
                transform.position = new Vector3(player_t.position.x + offset_x_add, player_t.position.y + offset.y, offset.z);
            }
            return;
        }

        /*float x = player_t.position.x + offset.x;
        float y = player_t.position.y + offset.y;

        if (!(limit_down < player_t.position.y))
            y = limit_down;
        else if(!(limit_up > player_t.position.y))
            y = limit_up;
        if (!(limite_right > player_t.position.x))
            x = limite_right;
        else if (!(limite_left < player_t.position.x))
            x = limite_left;
        
        transform.position = new Vector3(x, y, offset.z);*/
    }
    public bool LockCam
    {
        get { return lock_cam; }
        set { lock_cam = value; }
    }

}
