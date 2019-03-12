using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using UnityEngine.UI;

public class GroundInteraction : MonoBehaviour
{
    public static Player singleton;

    Tilemap tilemap;
    public GameObject tile_go;
    public GameObject element_obj;
    public string element_type = "Snowflakes Made: ";
    public int total_elements = 35;
    public float progression = 0f;
    private List<float> positions = new List<float>();
    ContactPoint2D[] contacts = new ContactPoint2D[2];
    private float x_pos_min, x_pos_max;

    public Slider progress_bar;
    public Text progress_text;




    private void Start()
    {
        if (tile_go)
        {
            tilemap = tile_go.GetComponent<Tilemap>();
        }
        x_pos_min = this.gameObject.transform.position.x;
        x_pos_max = this.gameObject.transform.position.x;
        progress_bar.value = Calculate_progress();
        

    }

    private void SetName()
    {
        if (element_obj)
        {
            switch (element_obj.name)
            {
                case "Snow_Obj":
                    element_type = "Snowflakes Made: ";
                    break;
                case "Sprout_Obj":
                    element_type = "Sprouts Grown: ";
                    break;
                case "Fall_Obj":
                    element_type = "Leaves Dropped: ";
                    break;
                case "Sun_Obj":
                    element_type = "Suns Made: ";
                    break;
                default:
                    element_type = "Elements Made: ";
                    break;
            }
        }
    }

    public float Calculate_progress()
    {
        return progression / total_elements;
    }

    private void Update()
    {
        progress_bar.value = Calculate_progress();
        
        progress_text.text = element_type + progression + " /" + total_elements;
        if(progression > total_elements)
        {
            progress_text.text = "Objective Reached!";
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.Mouse0) && Player.singleton.controller.collisions.below)
        {
            Vector3 current_pos = this.gameObject.transform.position;
  
            if (current_pos.x < x_pos_min - 2)
            {
                
                x_pos_min = current_pos.x;
                positions.Add(current_pos.y);
                current_pos.y -= .1f;
                Instantiate(element_obj, current_pos, Quaternion.identity);
                SetName();
                progression += 1;

            }else if (current_pos.x > x_pos_max + 2)
            {
                x_pos_max = current_pos.x;
                positions.Add(current_pos.y);
                current_pos.y -= .1f;
                Instantiate(element_obj, current_pos, Quaternion.identity);
                SetName();
                progression += 1;

            }
            
        }
    }

}

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 hitPosition = Vector3.zero;
        Debug.Log("hit");
        if (collision.gameObject.CompareTag("GrassTile"))
        {
            foreach(ContactPoint2D box in collision.contacts)
            {
                hitPosition.x = box.point.x - 0.01f * box.normal.x;
                hitPosition.y = box.point.y - 0.01f * box.normal.y;
                tilemap.SetTile(tilemap.WorldToCell(hitPosition), null);
            }
        }
    }*/

