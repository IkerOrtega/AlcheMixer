using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int initial_sections = 8;

// TODO: parametrize ingredients by choosing layer at which they appear instead of distance.
    /// <summary>
    /// Distance at which the ingredients will spawn from the spoon.
    /// </summary>
    public float ingRadius = 0.5f;
    public float maxLineDistance = 1f;
    public float minLineDistance = 0.2f;


    public List<float> circleRadiusList = new List<float>(){0.5f,1.5f};

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
    public List<GameObject> ingList = new List<GameObject>();

    private List<Circle> layerSeparationCircleList = new List<Circle>();

    private List<LineRenderer> sectionSeparationLineList = new List<LineRenderer>();

    private class Circle
    {
        public float radius;
        public List<LineRenderer> lines;

        public Circle(float rad = 0f)
        {
            this.radius = rad;
            this.lines = new List<LineRenderer>();
        }
    }


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

    private void initializeScene()
    {
        generateIngredients();
        DrawSectionSeparationLines();
        DrawLayerSeparationCircles();
    }
    /// <summary>
    /// Updates the number of sections in the battle circle.
    /// Also updates the angle step.  
    /// Also redraws the parts of the scene that depend on the number of sections.	
    /// </summary>
    /// <param name="sections">Number of sections on the circle</param>
    private void UpdateNumSections(int sections)
    {

        if (NUM_SECTIONS != sections)
        {
            NUM_SECTIONS = sections;
            ANGLE_STEP = Constants.MAX_ANGLE / NUM_SECTIONS;
        }
        initializeScene();
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

    private LineRenderer DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.03f;
        lr.endWidth = 0.03f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        //GameObject.Destroy(myLine, duration);
        return lr;
    }


    private void DrawSectionSeparationLines()
    {
        if (spoon != null)
        {
            DeleteSectionSeparationLines();
            for (int i = 0; i < NUM_SECTIONS; i++)
            {
                float angle = ((ANGLE_STEP * i) + ANGLE_STEP / 2f) % Constants.MAX_ANGLE;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                Vector3 direction = rotation * Vector2.up;

                //Vertical lines
                Vector3 farPoint = spoon.transform.position + (direction * maxLineDistance);
                Vector3 closePoint = spoon.transform.position + (direction * minLineDistance);
                sectionSeparationLineList.Add(DrawLine(closePoint, farPoint, Color.black));
            }
        }

    }

    private void DeleteSectionSeparationLines()
    {
        sectionSeparationLineList.ForEach(lr => Destroy(lr.gameObject));
        sectionSeparationLineList.Clear();
    }

    private void DeleteLayerSeparationCircles()
    {
        layerSeparationCircleList.ForEach(cr => cr.lines.ForEach(ln => Destroy(ln.gameObject)));
        layerSeparationCircleList.Clear();
    }

    private void DrawLayerSeparationCircles()
    {
        if (circleRadiusList != null)
        {
            DeleteLayerSeparationCircles();
            //Draw circles from inner to outer
            circleRadiusList.Sort((a, b) => a.CompareTo(b));
            foreach (var rad in circleRadiusList)
            {
                Vector3 actualPoint, firstPoint = Vector2.zero, lastPoint = Vector2.zero;
                Circle circle = new Circle(rad);

                for (int j = 0; j < NUM_SECTIONS; j++)
                {

                    float angle = ((ANGLE_STEP * j) + ANGLE_STEP / 2f) % Constants.MAX_ANGLE;
                    Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    Vector3 direction = rotation * Vector2.up;

                    if (j == 0)
                    {
                        lastPoint = spoon.transform.position + (direction * rad);
                        firstPoint = lastPoint;
                    }
                    else
                    {
                        actualPoint = spoon.transform.position + (direction * rad);
                        circle.lines.Add(DrawLine(lastPoint, actualPoint, Color.black));
                        lastPoint = actualPoint;
                        // If last point, connect to first point
                        if (j == NUM_SECTIONS - 1)
                        {
                            circle.lines.Add(DrawLine(actualPoint, firstPoint, Color.black));
                        }
                    }
                }

                layerSeparationCircleList.Add(circle);

            }
        }
    }



}
