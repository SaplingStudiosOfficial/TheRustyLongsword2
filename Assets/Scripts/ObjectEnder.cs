using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEnder : MonoBehaviour
{
    public GameObject Collectable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void disableCollectable()
    {
       
        Collectable.SetActive(false);
    }
}
