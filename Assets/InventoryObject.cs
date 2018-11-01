using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryObject : MonoBehaviour {
    public string obj_name;
    [TextArea]
    public string description;
    public Sprite sprite_pic;
    public int key_code = -1;

    private Image picture;
    void Start () {
        if(picture != null && sprite_pic != null)
            picture.sprite = sprite_pic;
        else
            picture = null;
    }

    public Image GetImage()
    {
        return picture;
    }
    public int IsKey()//-1 if not a key, else key_code
    {
        return key_code;
    }
}
