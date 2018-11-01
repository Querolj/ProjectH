using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour {
    public int index = 0;


    private WaypointsGestion way_gestion;
    void Start () {
        way_gestion = this.GetComponentInParent<WaypointsGestion>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "ghost")
        {
            way_gestion.PatrolPointReached(index);
        }
    }
}
