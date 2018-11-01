using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountElem : MonoBehaviour {
    //Transition speed for camera
    public float t_speed = -1;
    public bool full_mount_elem = false;//For element with boxcollider that collider. Need two child mount_point_left, mount_point_r where the player should pop when mounting in the x-axis 
    public bool accept_mount_from_mountable = false;//if we can mount the element from another mountable element 

    private PlayerController player;
    private PlayerAnimation player_anime;
    private Transform mount_point;
    private Transform mount_point_left;
    private Transform mount_point_right;


    void Start () {
        GameObject p = GameObject.Find("Player");
        player = p.GetComponent<PlayerController>();
        player_anime = p.GetComponent<PlayerAnimation>();
        
        if(full_mount_elem)
        {
            if (this.transform.parent != null)
            {
                mount_point_left = GameObject.Find(this.transform.parent.name + "/" + this.name + "/MountPointLeft").GetComponent<Transform>();
                mount_point_right = GameObject.Find(this.transform.parent.name + "/" + this.name + "/MountPointRight").GetComponent<Transform>();
            }
            else
            {
                mount_point_left = GameObject.Find(this.name + "/MountPointLeft").GetComponent<Transform>();
                mount_point_right = GameObject.Find(this.name + "/MountPointRight").GetComponent<Transform>();
            }
            if (mount_point_left == null || mount_point_right == null)
                print("ERROR, full_mount_elem activated, need two child : MountPointLeft and MountPointRight.");
        }
        else
        {
            if (this.transform.parent != null)
                mount_point = GameObject.Find(this.transform.parent.name + "/" + this.name + "/MountPoint").GetComponent<Transform>();
            else
                mount_point = GameObject.Find(this.name + "/MountPoint").GetComponent<Transform>();
            if (mount_point == null )
                print("ERROR, full_mount_elem desactivated, child MountPoint needed");
        }
    }
	

    public Vector3 GetMountPoint()
    {
        return mount_point.position;
    }
    public Vector3 GetMountPointLeft()
    {
        return mount_point_left.position;
    }
    public Vector3 GetMountPointRight()
    {
        return mount_point_right.position;
    }
    public Vector3 GetPosition()
    {
        return this.transform.position;
    }
    public bool IsMoutableFromMount()
    {
        return accept_mount_from_mountable;
    }
}
