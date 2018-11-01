using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterDetector : MonoBehaviour {
    private PlayerController player;
    private MountElem mount_elem;
    void Start () {
        player = this.GetComponentInParent<PlayerController>();
        mount_elem = null;
    }
	
	void Update () {
		if(!player.IsOnGround())
        {
            if(!(mount_elem != null && mount_elem.IsMoutableFromMount()))
                player.CanMount = false;
        }
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "mountable" && !player.CanMount)
        {
            mount_elem = collision.gameObject.GetComponent<MountElem>();
            if(mount_elem.IsMoutableFromMount() || player.IsOnGround())
            {
                if (!mount_elem.full_mount_elem)
                {
                    Vector3 mount_point = mount_elem.GetMountPoint();
                    player.ReadyMount(mount_point.y, mount_elem.t_speed);
                }
                else
                {
                    Vector3 mount_point_left = mount_elem.GetMountPointLeft();
                    Vector3 mount_point_right = mount_elem.GetMountPointRight();
                    player.ReadyMount(mount_point_left.y, mount_point_left.x, mount_point_right.x, mount_elem.GetPosition(), mount_elem.t_speed);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "mountable")
        {
            player.CanMount = false;
        }
    }
}
