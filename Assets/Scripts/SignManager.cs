using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignManager : MonoBehaviour
{
    [SerializeField] GameObject signMessage;
    [SerializeField] GameObject readIcon;
    public bool canRead = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(canRead && Input.GetKeyDown(KeyCode.F))
        {
            signMessage.SetActive(true);
            readIcon.SetActive(false);
            
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            canRead = true;
            readIcon.SetActive(true);
            if (signMessage.activeSelf == true)
            {
                readIcon.SetActive(false);
            }
        }

    }
    void OnTriggerExit(Collider collider)
    {
        canRead = false;
        readIcon.SetActive(false);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
    }
}
