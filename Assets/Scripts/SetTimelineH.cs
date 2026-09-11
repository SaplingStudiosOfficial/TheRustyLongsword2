using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTimelineH : MonoBehaviour
{
    [SerializeField] public PlayerController player;
    public bool T1HDone;
    public bool T2HDone;

    // Start is called before the first frame update
    void Start()
    {
        player.T1HDone = T1HDone;
        player.T2HDone = T2HDone;
        player.SaveData();
    }
}
