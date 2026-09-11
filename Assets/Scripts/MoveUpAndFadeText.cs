using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveUpAndFadeText : MonoBehaviour
{
    [SerializeField] private CanvasGroup uiObject;
    public float startX;
    public float startY;

    //Performed at start of program.
    void Start()
    {
        startX = this.gameObject.transform.position.x;
        startY = this.gameObject.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void fadeUpandOut()
    {
        if (this.gameObject.transform.position.y <= startY + 250)
        {
            this.gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 1);
            uiObject.alpha -= Time.deltaTime;
            if (uiObject.alpha == 0)
            {
                gameObject.SetActive(false);
            }
        }
        
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);
    } }
