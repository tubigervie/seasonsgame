using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
public class SceneTransition : MonoBehaviour
{
    
    public Image Image;
    public Text Text;
    public float fadeSpeed = 0.8f;
    public enum FadeDirection
    {
        In, //Alpha = 1
        Out // Alpha = 0
    }
    

    void OnEnable()
    {
        StartCoroutine(Fade(FadeDirection.Out));
    }
   

    private IEnumerator Fade(FadeDirection fadeDirection)
    {
        float alpha = (fadeDirection == FadeDirection.Out) ? 1 : 0;
        float fadeEndValue = (fadeDirection == FadeDirection.Out) ? 0 : 1;
        if (fadeDirection == FadeDirection.Out)
        {
            while (alpha >= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }
            Image.enabled = false;
            Text.enabled = false;
        }
        else
        {
            Image.enabled = true;
            Text.enabled = true;
            while (alpha <= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }
        }
    }
    

    public IEnumerator FadeAndLoadScene(FadeDirection fadeDirection, string sceneToLoad)
    {
        yield return Fade(fadeDirection);
        SceneManager.LoadScene(sceneToLoad);
    }
    private void SetColorImage(ref float alpha, FadeDirection fadeDirection)
    {
        Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, alpha);
        Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, alpha);
        alpha += Time.deltaTime * (1.0f / fadeSpeed) * ((fadeDirection == FadeDirection.Out) ? -1 : 1);
    }
   

}