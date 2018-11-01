using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPieceGestion : MonoBehaviour {
    public float fading_rate = 0.05f;

    private SpriteRenderer rend;
    void Start () {
        rend = this.GetComponent<SpriteRenderer>();

    }
	
	void Update () {
		
	}

    private void Fading(bool b)//true = appear
    {
        Color c = rend.color;
        if (b)
        {
            if (c.a + fading_rate >= 1)
            {
                c.a = 1;
            }
            else
                c.a += fading_rate;
        }
        else
        {
            if (c.a - fading_rate <= 0)
            {
                c.a = 0;
            }
            else
                c.a -= fading_rate;
        }
        rend.color = c;
    }
}
