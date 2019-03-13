using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIMask : MonoBehaviour
{
    public Image Image;
    public Transform fall;
    public Transform summer;
    public Transform spring;
    public Transform winter;
    // Start is called before the first frame update
    void Start()
    {
        Image = GetComponent<Image>();
        Image.rectTransform.localScale = new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        Color stance = Player.singleton.sprite.color;
        if(stance == Color.cyan)
        {

            Image.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            Image.rectTransform.position = winter.position;

        }else if(stance == Color.green)
        {
            Image.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            Image.rectTransform.position = spring.position;

        }
        else if(stance == Color.red)
        {
            Image.rectTransform.localScale = new Vector3(1f, 1f, 1f);

            Image.rectTransform.position = summer.position;

        }
        else if(stance == Color.yellow)
        {
            Image.rectTransform.localScale = new Vector3(1f, 1f, 1f);

            Image.rectTransform.position = fall.position;

        }
        else
        {
            Image.rectTransform.localScale = new Vector3(0f, 0f, 0f);

        }
    }


}
