using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    [SerializeField] CanvasGroup myUICanvas;
    [SerializeField] GameObject Canvas;
    public bool fadeIn;
    public bool fadeOut;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait());
        fadeIn = true;
        fadeOut = false;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeIn) {
            FadeCanvasIn();
        }
        if (fadeOut)
        {
            FadeCanvasOut();
        }
        if (Input.GetKeyDown(KeyCode.A) && myUICanvas.alpha == 1)
        {
                fadeOut = true;
        }
        if (Input.GetKeyDown(KeyCode.S) && myUICanvas.alpha == 1)
        {
            fadeOut = true;
        }
        if (Input.GetKeyDown(KeyCode.D) && myUICanvas.alpha == 1)
        {
            fadeOut = true;
        }
        if (Input.GetKeyDown(KeyCode.W) && myUICanvas.alpha == 1)
        {
            fadeOut = true;
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
    }

    public void FadeCanvasIn()
    {
    if (myUICanvas.alpha < 1)
        {
            myUICanvas.alpha += Time.deltaTime;
            if (myUICanvas.alpha == 1)
            {
                fadeIn = false;
            }
        }
    }
    public void FadeCanvasOut()
    {
    if (myUICanvas.alpha > 0)
        {
            myUICanvas.alpha -= Time.deltaTime;
            if (myUICanvas.alpha == 0)
            {
                fadeOut = false;
                gameObject.SetActive(false);
            }
        }
    }
}
