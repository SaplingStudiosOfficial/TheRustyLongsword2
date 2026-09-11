using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPageInfoBool : MonoBehaviour
{
    [SerializeField] public PlayerController player;
    [SerializeField] public GameObject Timeline1;
    [SerializeField] public GameObject Timeline2;
    [SerializeField] public GameObject Timeline3;
    [SerializeField] public GameObject Timeline4;
    [SerializeField] public GameObject Timeline5;
    [SerializeField] public GameObject JesseEndingOne;
    [SerializeField] public GameObject JesseEndingTwo;
    [SerializeField] public GameObject WinstonEndingOne;
    [SerializeField] public GameObject WinstonEndingTwo;
    [SerializeField] public GameObject ItachiEndingOne;
    [SerializeField] public GameObject ItachiEndingTwo;
    [SerializeField] public GameObject AnkokuEndingOne;
    [SerializeField] public GameObject AnkokuEndingTwo;
    [SerializeField] public GameObject PageText;

    // Start is called before the first frame update
    void Start()
    {
        PageText.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (player.T1MDone)
            {
                player.T1MDone = false;
                GetComponent<Renderer>().enabled = false;
                PageText.SetActive(false);
                StartCoroutine(WaitWhilePageFlips(Timeline1));
            }
            else if (player.T2MDone)
            {
                player.T2MDone = false;
                GetComponent<Renderer>().enabled = false;
                PageText.SetActive(false);
                StartCoroutine(WaitWhilePageFlips(Timeline2));
            }
            else if (player.T1HDone)
            {
                player.T1HDone = false;
                GetComponent<Renderer>().enabled = false;
                PageText.SetActive(false);
                StartCoroutine(WaitWhilePageFlips(Timeline3));
            }
            else if (player.T2HDone)
            {
                player.T2HDone = false;
                GetComponent<Renderer>().enabled = false;
                PageText.SetActive(false);
                StartCoroutine(WaitWhilePageFlips(Timeline4));
            }
            else if (player.JesseEndingOne)
            {
                player.JesseEndingOne = false;
                GetComponent<Renderer>().enabled = false;
                PageText.SetActive(false);
                StartCoroutine(WaitWhilePageFlips(JesseEndingOne));
            }
            else if (player.JesseEndingTwo)
            {
                player.JesseEndingTwo = false;
                GetComponent<Renderer>().enabled = false;
                PageText.SetActive(false);
                StartCoroutine(WaitWhilePageFlips(JesseEndingTwo));
            }
            else if (player.WinstonEndingOne)
            {
                player.WinstonEndingOne = false;
                GetComponent<Renderer>().enabled = false;
                PageText.SetActive(false);
                StartCoroutine(WaitWhilePageFlips(WinstonEndingOne));
            }
            else if (player.WinstonEndingTwo)
            {
                player.WinstonEndingTwo = false;
                GetComponent<Renderer>().enabled = false;
                PageText.SetActive(false);
                StartCoroutine(WaitWhilePageFlips(WinstonEndingTwo));
            }
            else if (player.ItachiEndingOne)
            {
                player.WinstonEndingOne = false;
                GetComponent<Renderer>().enabled = false;
                PageText.SetActive(false);
                StartCoroutine(WaitWhilePageFlips(ItachiEndingOne));
            }
            else if (player.ItachiEndingTwo)
            {
                player.WinstonEndingOne = false;
                GetComponent<Renderer>().enabled = false;
                PageText.SetActive(false);
                StartCoroutine(WaitWhilePageFlips(ItachiEndingTwo));
            }
            else if (player.AnkokuEndingOne)
            {
                player.AnkokuEndingOne = false;
                GetComponent<Renderer>().enabled = false;
                PageText.SetActive(false);
                StartCoroutine(WaitWhilePageFlips(AnkokuEndingOne));
            }
            else if (player.AnkokuEndingTwo)
            {
                player.AnkokuEndingOne = false;
                GetComponent<Renderer>().enabled = false;
                PageText.SetActive(false);
                StartCoroutine(WaitWhilePageFlips(AnkokuEndingTwo));
            }
            if (!player.T1MDone && !player.T2MDone && !player.T1HDone && !player.T2HDone)
            {
                GetComponent<Renderer>().enabled = false;
                PageText.SetActive(false);
                StartCoroutine(WaitWhilePageFlips(Timeline5));
            }
        }
    }
    IEnumerator WaitWhilePageFlips(GameObject timeline)
    {

        yield return new WaitForSeconds(.6f);
        timeline.SetActive(true);
        gameObject.SetActive(false);
    }
}
