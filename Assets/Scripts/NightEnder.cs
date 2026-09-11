using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using static System.Net.Mime.MediaTypeNames;

public class NightEnder : MonoBehaviour
{
    [SerializeField] private int counts;
    [SerializeField] private GameObject Canvas;
    [SerializeField] private CanvasGroup myUIGroup;
    [SerializeField] private PlayerController Player;
    public int counter;
    public bool loadWorld;

    // Start is called before the first frame update
    void Start()
    {
        if (loadWorld)
        {
            StartCoroutine(WaitThemLoadWorld());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (myUIGroup.alpha <= 1)
        {
            myUIGroup.alpha += Time.deltaTime;
        }
    }
    IEnumerator WaitThemLoadWorld()
    {
        Player.invitedCharacters.Clear();
        Debug.Log("Characters Clear.");
        if (Player.worldMusic == 0)
        {
            Player.gameObject.transform.position = new Vector3(-157f, 55f, -205f);
        }else if( Player.worldMusic == 1)
        {
            Player.gameObject.transform.position = new Vector3(-161.8782f, 71.02307f, -28.9041f);
        }
        else if (Player.worldMusic == 2)
        {
            Player.gameObject.transform.position = new Vector3(30.79005f, 130f, 188.3f);
        }
        Debug.Log("Character Position Reset.");
        Player.SaveData();
        Debug.Log("Character Data Saved.");
        yield return new WaitForSeconds(2f);
        Debug.Log("Loading");
        SceneManager.LoadScene("3DEnviroment");
    }
}
