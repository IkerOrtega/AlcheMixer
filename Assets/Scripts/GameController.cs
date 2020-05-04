using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int initial_sections = 8;

    [HideInInspector]
    public float ANGLE_STEP = 45;
    [HideInInspector]
    public int NUM_SECTIONS = 8;


    private void Awake()
    {
       UpdateNumSections(initial_sections);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void UpdateNumSections(int sections) {
        if(NUM_SECTIONS != sections) {
            NUM_SECTIONS = sections;
            ANGLE_STEP = Constants.MAX_ANGLE / NUM_SECTIONS;
        }
    }
}
