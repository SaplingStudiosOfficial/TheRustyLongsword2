using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nextPageInfo : MonoBehaviour
{


    [SerializeField] GameObject NextPageData;
    [SerializeField] GameObject PageText;
    void Start()
    {
        PageText.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Renderer>().enabled = false;
            PageText.SetActive(false);
            StartCoroutine(WaitWhilePageFlips());
        }

    }
    IEnumerator WaitWhilePageFlips()
    {
        
        yield return new WaitForSeconds(.6f);
        NextPageData.SetActive(true);
        gameObject.SetActive(false);
    }

}
