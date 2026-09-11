using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private float gotopoint;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void followPlayer()
    {
        if((player.transform.position.x - transform.position.x) >= 9 || (player.transform.position.x - transform.position.x) <= -9)
        {
            gotopoint = player.transform.position.x;
            transform.position = new Vector3(gotopoint, transform.position.y, transform.position.z);
        }
    }
}
