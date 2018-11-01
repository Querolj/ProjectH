using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningDisplay : MonoBehaviour {
    public float buzzing_time_cd;
    [Range(0, 0.005f)]
    public float buzzing_strength = 0.5f;
    [Range(0, 10)]
    public int buzzing_time_spacing = 5;

    private Text msg;
    private RectTransform location;
    private RectTransform rt;
    private bool buzzing_place = false;
    private float buzzing_time;
    private int buzz_count = 0;
    private bool warning_type = true;//warning_type = true, red message, else green message
    void Start () {
        //buzzing_time = buzzing_time_cd;
        location = GameObject.Find("Player/WarningLoc").GetComponent<RectTransform>();
        rt = this.GetComponent<RectTransform>();
        rt.position = location.position;
        msg = this.GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        rt.position = location.position;
        if(buzzing_time > 0)
        {
            buzzing_time -= Time.deltaTime;
            Buzzing();
            if(buzzing_time <= 0)
                msg.text = "";
        }
    }

    private void Buzzing()
    {
        Vector3 v = rt.position;
        if (buzzing_place)
            v.y += buzzing_strength;
        else
            v.y -= buzzing_strength;
        buzz_count++;
        if(buzz_count == buzzing_time_spacing)
        {
            buzzing_place = !buzzing_place;
            buzz_count = 0;
        }
        
        rt.position = v;
    }
    //------------------------------------------------------------------------------------------
    public void StartBuzz(string w, bool av = true)//av = true, red message, else green message
    {
        if(av)
            msg.color = new Color32(243, 82,82,255);
        else
            msg.color = new Color32(82, 243, 82, 255);
        buzzing_time = buzzing_time_cd;
        msg.text = w;
    }
}
