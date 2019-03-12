using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIMask : MonoBehaviour
{
    public Image Image;
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
            Image.rectTransform.position = new Vector3(26f, Image.rectTransform.position.y, Image.rectTransform.position.z);

        }else if(stance == Color.green)
        {
            Image.rectTransform.localScale = new Vector3(1f, 1f, 1f);

            Image.rectTransform.position = new Vector3(65.5f, Image.rectTransform.position.y, Image.rectTransform.position.z);

        }
        else if(stance == Color.red)
        {
            Image.rectTransform.localScale = new Vector3(1f, 1f, 1f);

            Image.rectTransform.position = new Vector3(105f, Image.rectTransform.position.y, Image.rectTransform.position.z);

        }
        else if(stance == Color.yellow)
        {
            Image.rectTransform.localScale = new Vector3(1f, 1f, 1f);

            Image.rectTransform.position = new Vector3(144f, Image.rectTransform.position.y, Image.rectTransform.position.z);

        }
        else
        {
            Image.rectTransform.localScale = new Vector3(0f, 0f, 0f);

        }
    }


}
