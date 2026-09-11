using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDrinkThenHideDrink : MonoBehaviour
{
    public bool show;
    public bool hide;


    void Start()
    {
        //Sets show to true upon startup.
        show = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (show)
        {
            Show();
        }
        if (hide)
        {
            Hide();
        }
    }

    //Moves Object into View.
    public void Show()
    {
        if (gameObject.transform.position.y < Screen.height*.38)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * 2000, transform.position.z);
        }
        else if (gameObject.transform.position.y >= Screen.height * .38)
        {
            show = false;
            StartCoroutine(WaitThenHide());
        }
    }

    //Moves Object away from view.
    public void Hide()
    {
        if (gameObject.transform.position.y > -10000)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * 2000, transform.position.z);
        }
        if (gameObject.transform.position.y <= -10000)
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator WaitThenHide()
    {
        yield return new WaitForSeconds(2f);
        hide = true;
    }
}
