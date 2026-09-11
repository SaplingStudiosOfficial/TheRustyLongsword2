using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTimeline : MonoBehaviour
{
    [SerializeField] public PlayerController player;

    // Standard booleans for editor values
    public bool T1MDone;
    public bool T2MDone;
    public bool ItachiEndingOne;
    public bool ItachiEndingTwo;
    public bool AnkokuEndingOne;
    public bool AnkokuEndingTwo;
    public bool WinstonEndingOne;
    public bool WinstonEndingTwo;
    public bool JesseEndingOne;
    public bool JesseEndingTwo;

    // Booleans to track if a value has been set in the editor
    [SerializeField] private bool isT1MDoneSet;
    [SerializeField] private bool isT2MDoneSet;
    [SerializeField] private bool isItachiEndingOneSet;
    [SerializeField] private bool isItachiEndingTwoSet;
    [SerializeField] private bool isAnkokuEndingOneSet;
    [SerializeField] private bool isAnkokuEndingTwoSet;
    [SerializeField] private bool isWinstonEndingOneSet;
    [SerializeField] private bool isWinstonEndingTwoSet;
    [SerializeField] private bool isJesseEndingOneSet;
    [SerializeField] private bool isJesseEndingTwoSet;

    // Start is called before the first frame update
    void Start()
    {
        // Only set the player's properties if the corresponding value has been set in the editor
        if (isT1MDoneSet) player.T1MDone = T1MDone;
        if (isT2MDoneSet) player.T2MDone = T2MDone;
        if (isItachiEndingOneSet) player.ItachiEndingOne = ItachiEndingOne;
        if (isItachiEndingTwoSet) player.ItachiEndingTwo = ItachiEndingTwo;
        if (isAnkokuEndingOneSet) player.AnkokuEndingOne = AnkokuEndingOne;
        if (isAnkokuEndingTwoSet) player.AnkokuEndingTwo = AnkokuEndingTwo;
        if (isWinstonEndingOneSet) player.WinstonEndingOne = WinstonEndingOne;
        if (isWinstonEndingTwoSet) player.WinstonEndingTwo = WinstonEndingTwo;
        if (isJesseEndingOneSet) player.JesseEndingOne = JesseEndingOne;
        if (isJesseEndingTwoSet) player.JesseEndingTwo = JesseEndingTwo;

        player.SaveData();
    }
}