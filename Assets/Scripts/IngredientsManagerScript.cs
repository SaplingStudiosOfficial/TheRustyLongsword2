using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientsManagerScript : MonoBehaviour
{
    [SerializeField]
    public int amtRum = 1000;
    public int amtFly = 0;
    public int amtPine = 0;
    public int amtSea = 0;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UseRum()
    {
        amtRum -= 1;
    }

    private void UseFly()
    {
        amtFly -= 1;
    }

    private void UsePine()
    {
        amtPine -= 1;
    }
    private void UseSea()
    {
        amtSea -= 1;
    }
}
