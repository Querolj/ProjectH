using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointsGestion : MonoBehaviour {
    public GameObject ghost;
    public float speed = 0.5f;
    public int initial_waypoint = 0;

    private Transform ghost_t;
    private Rigidbody2D ghost_body;
    private Waypoint[] waypoints;
    private Vector3 current_direction;
    private int current_direction_index = -1;

    private int old_direction = -1;
    private bool waypoint_reached = false;
    private int current_patrol_index = 0;
    private float patrol_time;
	void Start () {
        if (ghost != null)
        {
            ghost_t = ghost.GetComponent<Transform>();
            ghost_body = ghost.GetComponent<Rigidbody2D>();
            waypoints = new Waypoint[this.transform.childCount];
            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = GameObject.Find(this.name + "/W" + i.ToString()).GetComponent<Waypoint>();
            }
            SelectDestination();
        }
        else
            print("ERROR : Pas de ghost dans waypointsgestion");
        
    }
	
	void Update () {
        if(ghost != null)
        {
            MoveToDestination(current_direction);
            if (patrol_time > 0)//|| waypoints[initial_waypoint].waypoints_linked.Length > 0 || waypoints[current_direction_index].waypoints_linked.Length > 0
            {
                patrol_time -= Time.deltaTime;
            }
            else if (waypoint_reached && waypoints[initial_waypoint].waypoints_linked.Length > 0)
            {
                SelectDestination();
                waypoint_reached = false;
            }
        }
        
    }

    private void SelectDestination()
    {
        if(initial_waypoint != -1)
        {
            int lenght = waypoints[initial_waypoint].waypoints_linked.Length;
            if (waypoints[initial_waypoint].waypoints_linked.Length > 0)
            {
                int chosen = Random.Range(0, lenght);
                current_direction_index = waypoints[initial_waypoint].waypoints_linked[chosen];
                initial_waypoint = -1;
            }
            else
                current_direction_index = initial_waypoint;
            current_direction = waypoints[current_direction_index].GetPosition();
            
        }
        else
        {
            int lenght = waypoints[current_direction_index].waypoints_linked.Length;
            
            if (waypoints[initial_waypoint].waypoints_linked.Length > 0)
            {
                int chosen = Random.Range(0, lenght);
                current_direction_index = waypoints[initial_waypoint].waypoints_linked[chosen];
            }
            else
                current_direction_index = initial_waypoint;
            current_direction = waypoints[current_direction_index].GetPosition();
        }
    }

    private void MoveToDestination(Vector3 waypoint_pos)
    {
        //e_body.MovePosition(player_t.position);
        float x_velo = ghost_t.position.x - waypoint_pos.x;
        float y_velo = ghost_t.position.y - waypoint_pos.y;
        float total = Mathf.Abs(x_velo) + Mathf.Abs(y_velo);
        x_velo = (x_velo / total) * speed;
        y_velo = (y_velo / total) * speed;
        ghost_body.velocity = new Vector2(-x_velo, -y_velo);
    }
    //--------------------------------------------------------------------------------------------------------------------------------------------
    public void WaypointReached(int index)//Indique si le waypoint vers lequel on se dirigeait a été atteint
    {
        if(patrol_time <= 0)
        {
            if (current_direction_index == index)
            {
                waypoint_reached = true;
                if (waypoints[current_direction_index].GetPatrolPoints() != null)
                {
                    patrol_time = waypoints[current_direction_index].patrolling_time;
                    current_patrol_index = 0;
                    current_direction = waypoints[current_direction_index].GetPatrolPoints()[current_patrol_index];
                }
            }
        }
    }

    public void PatrolPointReached(int index)
    {
        if((patrol_time > 0 || waypoints[initial_waypoint].waypoints_linked.Length == 0) && current_patrol_index == index)
        {
            if (current_patrol_index < waypoints[current_direction_index].GetPatrolPoints().Length - 1)
            {
                current_patrol_index++;
            }
            else
            {
                current_patrol_index = 0;
            }
            current_direction = waypoints[current_direction_index].GetPatrolPoints()[current_patrol_index];
        }
    }
    public bool IsPatrolling()
    {
        if (patrol_time > 0)
            return true;
        return false;
    }
}
