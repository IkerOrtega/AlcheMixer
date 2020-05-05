using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int initial_sections = 8;

    /// <summary>
    /// Distance at which the ingredients will spawn from the spoon.
    /// </summary>
    public float ingRadius = 0.5f;
    public GameObject spoon;
    public GameObject ingredientPrefab;

    /// <summary>
    /// Angle between the center of a section and its neighbors. It's calculated by 360 / NUM_SECTIONS.
    /// </summary>
    [HideInInspector]
    public float ANGLE_STEP = 45;

    /// <summary>
    /// Number of sections of the battle circle. The circle will be divided in NUM_SECTIONS sections of the same width/angle.
    /// </summary>
    [HideInInspector]
    public int NUM_SECTIONS = 8;

    [HideInInspector]
    public List<GameObject> ingList;


    private void Awake()
    {
        UpdateNumSections(initial_sections);
    }
    // Start is called before the first frame update
    void Start()
    {
        ingList = new List<GameObject>();
        generateIngredients();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnGUI()
    {
        if (ingList != null)
        {
            for (int i = 0; i < ingList.Count; i++)
            {
                Vector2 pos = Camera.main.WorldToScreenPoint(ingList[i].transform.position);
                Rect rectangulo = new Rect(pos.x, Screen.height - pos.y, 50f, 50f);
               
                GUI.Label(rectangulo, "" + i);
            }
        }
    }

    /// <summary>
    /// Updates the number of sections in the battle circle.
    /// Also updates the angle step.   	
    /// </summary>
    /// <param name="sections">Number of sections on the circle</param>
    private void UpdateNumSections(int sections)
    {

        if (NUM_SECTIONS != sections)
        {
            NUM_SECTIONS = sections;
            ANGLE_STEP = Constants.MAX_ANGLE / NUM_SECTIONS;
        }
    }

    /// <summary>
    /// Generates an ingredient in each section around the spoon at specified distance. 	
    /// </summary>
    /// 
    private void generateIngredients()
    {
        if (ingredientPrefab != null && spoon != null)
        {
            for (int i = 0; i < NUM_SECTIONS; i++)
            {
                Quaternion rotation = Quaternion.AngleAxis(ANGLE_STEP * i, Vector3.forward);
                Vector3 direction = rotation * Vector2.up;
                Vector3 position = spoon.transform.position + (direction * ingRadius);
                ingList.Add(Instantiate(ingredientPrefab, position, rotation));
            }

        }
    }


}
