using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soloGraph : MonoBehaviour
{
    public GameObject graph;
    private bool openG;

    // Start is called before the first frame update
    void Start()
    {
        openG = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openGraph()
    {
        if (openG == true)
        {
            openG = false;
        }
        else
        {
            openG = true;
        }
        graph.SetActive(openG);
        Debug.Log("paraPanel" + openG);
    }
}
