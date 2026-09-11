using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup myUIGroup;
    [SerializeField] private GameObject Canvas;
    [SerializeField] private bool fadeOut;
    [SerializeField] private bool fadeIn;
    [SerializeField] private GameObject MainTheme;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeOut)
        {
            if (myUIGroup.alpha > 0)
            {
                myUIGroup.alpha -= Time.deltaTime;
                if (myUIGroup.alpha <= 0)
                {
                    fadeOut = false;
                    fadeIn = true;
                    Canvas.SetActive(false);
                    MainTheme.SetActive(true);
                    gameObject.SetActive(false);
                    
                }
            }
        }
        if (fadeIn)
        {
            if (myUIGroup.alpha < 1)
            {
                myUIGroup.alpha += Time.deltaTime;
                if (myUIGroup.alpha >= 1)
                {
                    
                    fadeIn = false;
                    StartCoroutine(Wait());
                }
            }
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(4f);
        fadeOut = true;
        
    }
}
