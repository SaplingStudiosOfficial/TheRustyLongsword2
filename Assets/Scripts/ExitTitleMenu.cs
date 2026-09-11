using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitTitleMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup TitleMenu;
    [SerializeField] private GameObject Menu;
    private bool fadeOut = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeOut)
        {
            if (TitleMenu.alpha > 0)
            {
                TitleMenu.alpha -= Time.deltaTime;
                if (TitleMenu.alpha <= 0)
                {
                    fadeOut = false;
                    Menu.SetActive(false);
                }
            }

        }   
    }

    public void Exit()
    {
        fadeOut = true;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
    }
}
