using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeDistanceEditor : MonoBehaviour
{
    public float distance;
    [SerializeField] private Terrain terrain;

    // Start is called before the first frame update
    void Start()
    {
        terrain.treeDistance = distance;
    }
}
