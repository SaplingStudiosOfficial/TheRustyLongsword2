using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    public float speed;
    public bool rotateLeft;
    public bool rotateUp;

    // Update is called once per frame
    void Update()
    {
        if (rotateLeft) {
            transform.Rotate(Vector3.left * speed * Time.deltaTime);
        }
        if (rotateUp)
        {
            transform.Rotate(Vector3.up * speed * Time.deltaTime);
        }
    }
}
