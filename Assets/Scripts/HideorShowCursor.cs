using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideorShowCursor : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void hideCursor()
    {
        Cursor.visible = false;
    }
    public void showCursor()
    {
        Cursor.visible = true;
    }
}
