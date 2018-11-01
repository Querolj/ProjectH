using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {
    public int[] waypoints_linked;//Cad depuis this on peut aller à ces waypoints (mais l'inverse n'est pas nécessairement vrai)
    public int index;
    public float patrolling_time = 30;

    private Transform t;
    private WaypointsGestion way_gestion;
    private Vector3[] partol_points;
	void Start () {
        t = this.GetComponent<Transform>();
        way_gestion = this.GetComponentInParent<WaypointsGestion>();

        partol_points = new Vector3[this.transform.childCount];
        for (int i = 0; i < partol_points.Length; i++)
        {
            partol_points[i] = GameObject.Find(this.name + "/P" + i.ToString()).GetComponent<Transform>().position;
        }
    }

    //------------------------------------------------------------------------------------------------------------
    public Vector3 GetPosition()
    {
        return t.position;
    }
    public Vector3[] GetPatrolPoints()
    {
        if (partol_points.Length == 0)
            return null;
        else
            return partol_points;
    }
    //------------------------------------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "ghost" && !way_gestion.IsPatrolling())
        {
            way_gestion.WaypointReached(index);
        }
    }
}
