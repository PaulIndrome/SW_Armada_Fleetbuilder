using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class UIAnimations : MonoBehaviour{


    public void FlashImageRed(Image image){
        image.StartCoroutine(FlashImageColor(image));
    }

    public IEnumerator FlashImageColor(Image image, Color color1, Color color2, int repeats = 4, float duration = 1f){
        Color startColor = image.color;

        image.color = color1;

        float blinkTime = duration / (repeats * 2);
        float blinkSpeed = 1 / blinkTime;

        for(float t = 0; t < duration; t += Time.deltaTime){
            image.color = Color.Lerp(color1, color2, 1 - Mathf.PingPong(t * blinkSpeed, 1));
            yield return null;
        }

        image.color = startColor;
        yield return null;
    }

    public IEnumerator FlashImageColor(Image image, Color color2, int repeats = 4, float duration = 1f){
        Color startColor = image.color;

        float blinkTime = duration / (repeats * 2);
        float blinkSpeed = 1 / blinkTime;

        for(float t = 0; t < duration; t += Time.deltaTime){
            image.color = Color.Lerp(startColor, color2, 1 - Mathf.PingPong(t * blinkSpeed, 1));
            yield return null;
        }

        image.color = startColor;
        yield return null;
    }

    public IEnumerator FlashImageColor(Image image, int repeats = 4, float duration = 1f){
        Color startColor = image.color;

        float blinkTime = duration / (repeats * 2);
        float blinkSpeed = 1 / blinkTime;

        for(float t = 0; t < duration; t += Time.deltaTime){
            image.color = Color.Lerp(Color.black, Color.red, 1 - Mathf.PingPong(t * blinkSpeed, 1));
            yield return null;
        }

        image.color = startColor;
        yield return null;
    }
}