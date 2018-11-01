using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPoint : MonoBehaviour {

    private MovingElem moving_elem;
    void Start () {
        moving_elem = this.GetComponentInParent<MovingElem>();
        if (moving_elem == null)
            print("ERROR pas de movingElem pour MovingPoint");
    }
	public MovingElem GetMovingElem()
    {
        return moving_elem;
    }
}
