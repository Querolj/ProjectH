using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryGestion : MonoBehaviour {

    private InventoryObject[] objects;
    
    private Image[] objects_display;
    private Image[] squares_sprites;
    private float direction;
    private float direction_y;
    private float old_x = 0;
    private float old_y = 0;
    private Text title;
    private Text desc;
    private int targeted_square = 0;
    
    void Start () {
        objects = new InventoryObject[9];
        squares_sprites = new Image[9];
        objects_display = new Image[9];
        for (int i=0;i<objects.Length;i++)
        {
            GameObject o = GameObject.Find("Player/Inventory/Objects/InventoryDisplay/Square" + i.ToString() + "/Obj" + i.ToString());
            objects[i] = o.GetComponent<InventoryObject>();
            objects_display[i] = o.GetComponent<Image>();
            squares_sprites[i] = GameObject.Find("Player/Inventory/Objects/InventoryDisplay/Square" + i.ToString()).GetComponent<Image>();
        }
        title = GameObject.Find("Player/Inventory/Objects/DescriptionDisplay/ObjNamePic/Text").GetComponent<Text>();
        desc = GameObject.Find("Player/Inventory/Objects/DescriptionDisplay/DescPic/Text").GetComponent<Text>();

    }
	
	void Update () {
        direction = Input.GetAxis("Horizontal");
        direction_y = Input.GetAxis("Vertical");
        Navigation(direction, direction_y);

        old_x = direction;
        old_y = direction_y;

    }

    private void Navigation(float lr, float ud)
    {
        squares_sprites[targeted_square].color = new Color(1, 1, 1, 1);
        if(old_x == 0)
        {
            if (lr > 0 && targeted_square < 8)
            {
                targeted_square++;
            }
            else if (lr < 0 && targeted_square > 0)
            {
                targeted_square--;
            }
        }
        
        if(old_y == 0)
        {
            if (ud > 0)
            {
                targeted_square =((targeted_square - 3) % 9);
                if (targeted_square < 0)
                    targeted_square += 9;
            }
            else if (ud < 0)
            {
                targeted_square = (targeted_square + 3) % 9;
            }
        }
        
        squares_sprites[targeted_square].color = new Color(1,0.2f,0.2f,1);
        if(objects_display[targeted_square].sprite == null)
        {
            title.text = "";
            desc.text = "";
        }
        else
        {
            title.text = objects[targeted_square].obj_name;
            desc.text = objects[targeted_square].description;
        }
    }

    public bool AddObject(InventoryObject added_object) //Return false if full inventory
    {
        for(int i=0;i<objects.Length;i++)
        {
            if (objects[i].sprite_pic == null)
            {
                objects[i] = added_object;
                objects_display[i].sprite = objects[i].sprite_pic;
                objects_display[i].color = new Color(1, 1, 1, 1);
                return true;
            }
        }
        return false;
    }
    public bool RemoveObjectByName(string obj_name)//Return false if can't remove item
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].sprite_pic != null && objects[i].obj_name == obj_name)
            {
                objects[i] = new InventoryObject();
                objects_display[i].sprite = null;
                objects_display[i].color = new Color(1, 1, 1, 0);
                return true;
            }
        }
        return false;
    }
    public bool HasKey(int code)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].IsKey() == code)
            {
                
                return true;
            }
        }
        return false;
    }
    public bool UseKey(int code)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].IsKey() == code)
            {
                print(i);
                objects[i] = new InventoryObject();
                objects_display[i].sprite = null;
                objects_display[i].color = new Color(1, 1, 1, 0);
                return true;
            }
        }
        return false;
    }
}
