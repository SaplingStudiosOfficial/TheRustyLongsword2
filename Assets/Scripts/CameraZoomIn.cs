using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraZoomIn : MonoBehaviour
{
    [SerializeField] GameObject cam;
    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z + Time.deltaTime *3);
        StartCoroutine(WaitThenLoadScene());
    }
    IEnumerator WaitThenLoadScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("3DEnviroment");
    }
}
