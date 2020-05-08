using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;


public class GameController : MonoBehaviour
{
    #region Public Variables
    /// <summary>
    ///Layer in which the ingredients will spawn.
    /// </summary>
    public int ingredientLayer = 1;


    public int sections = 8;
    public int layers = 3;
    public float separationBetweenLayers = 0.3f;
    public float initialLayerDistance = 0.5f;
    public float layerSeparationGrowth = 0.1f;


    public GameObject spoon;
    public GameObject ingredientPrefab;

    #endregion
    #region Public Hidden Variables
    /// <summary>
    /// Angle between the center of a section and its neighbors. It's calculated by 360 / NUM_SECTIONS.
    /// </summary>
    [HideInInspector]
    public float ANGLE_STEP = 45;

    #endregion
    #region Private Variables

    private Cell[,] battleMatrix;
    private List<LineRenderer> lineList = new List<LineRenderer>();
    private List<GameObject> elementList = new List<GameObject>();

    #endregion
    #region Private Classes
    private class Cell
    {
        public int element;

        public Vector2 center;
        public int[] lines = new int[4];
        public Vector2[] points = new Vector2[4];
        public Cell(){}

    }


    #endregion
    #region Unity Methods
    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        BuildBattleMatrix(sections, layers,ingredientLayer);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            BuildBattleMatrix(sections, layers, ingredientLayer);
        }
    }

    #endregion
    
    #region Main Methods

    /// <summary>
    /// Deletes all lines and prefabs of the Matrix and makes the matrix null.
    /// </summary>
    private void DeleteBattleMatrix()
    {
        lineList.ForEach(line => Destroy(line.gameObject));
        lineList.Clear();
        elementList.ForEach(elem => Destroy(elem));
        elementList.Clear();
        battleMatrix = null;

    }

    /// <summary>
    /// Builds the battle matrix with the passed number of sections and layers. Also instantiates elements in the passed layer. 
    /// </summary>
    /// <param name="sections"></param>
    /// <param name="layers"></param>
    private void BuildBattleMatrix(int sections, int layers, int elementLayer)
    {
        DeleteBattleMatrix();

        ANGLE_STEP = Constants.ANGLES.MAX_ANGLE / sections;
        battleMatrix = new Cell[sections, layers];

        for (int sec = 0; sec < sections; sec++)
        {
            for (int lay = 0; lay < layers; lay++)
            {

                Cell cell = new Cell();

                // ---------------------------- Calculate 4 corner points and center ------------------------------------

                Vector2 bottomLeft = new Vector2();
                Vector2 topLeft = new Vector2();
                Vector2 topRight = new Vector2();
                Vector2 bottomRight = new Vector2();

                Vector2 center = new Vector2();


                Vector3 dirRight = getDirection(sec - 1);
                Vector3 dirLeft = getDirection(sec);

                float distBottom = getDistance(lay);
                float distTop = getDistance(lay + 1);

                bottomLeft = spoon.transform.position +  dirLeft * distBottom;
                topLeft = spoon.transform.position + dirLeft * distTop;
                topRight = spoon.transform.position + dirRight * distTop;
                bottomRight = spoon.transform.position + dirRight * distBottom;


                cell.points[Constants.POINTS.BOTTOMLEFT] = bottomLeft;
                cell.points[Constants.POINTS.TOPLEFT] = topLeft;
                cell.points[Constants.POINTS.TOPRIGHT] = topRight;
                cell.points[Constants.POINTS.BOTTOMRIGHT] = bottomRight;


                center = getCenter(cell.points);

                cell.center = center;

                // ---------------------------- Draw cell lines -------------------------------------------------------------------

                int floor = -1;
                int leftWall = -1;
                int ceiling = -1;
                int rightWall = -1;


                if (sec == 0)
                {
                    if(lay == 0)
                    {
                        lineList.Add(DrawLine(bottomRight, bottomLeft, Color.black));
                        floor = lineList.Count - 1;
                    }
                    else
                    {
                        //Floor of layer 1 is ceiling of layer 0;
                        floor = battleMatrix[sec, lay - 1].lines[Constants.LINES.CEILING];          
                    }

                    lineList.Add(DrawLine(topRight, bottomRight, Color.black));
                    rightWall = lineList.Count - 1;
                }
                else
                {
                    if (lay == 0)
                    {
                        lineList.Add(DrawLine(bottomRight, bottomLeft, Color.black));
                        floor = lineList.Count - 1;
                    }
                    else
                    {
                        //Floor of layer 1 is ceiling of layer 0;
                        floor = battleMatrix[sec, lay - 1].lines[Constants.LINES.CEILING];
                    }

                    

                    // Right wall of section 1 is left wall of section 0.
                    rightWall = battleMatrix[sec - 1, lay].lines[Constants.LINES.LEFTWALL];

                }

                lineList.Add(DrawLine(bottomLeft, topLeft, Color.black));
                leftWall = lineList.Count - 1;
                lineList.Add(DrawLine(topLeft, topRight, Color.black));
                ceiling = lineList.Count - 1;

                cell.lines[Constants.LINES.FLOOR] = floor;
                cell.lines[Constants.LINES.LEFTWALL] = leftWall;
                cell.lines[Constants.LINES.CEILING] = ceiling;
                cell.lines[Constants.LINES.RIGHTWALL] = rightWall;

                // ---------------------------- Draw inner element --------------------------------------------------------------------
                if (elementLayer == lay)
                {
                    GameObject element = Instantiate(ingredientPrefab, cell.center, Quaternion.identity);
                    elementList.Add(element);
                    cell.element = elementList.Count - 1;
                }

                battleMatrix[sec, lay] = cell;
            }
        }
    }

    #endregion
    #region Utilities

    /// <summary>
    /// Creates a GameObject with a LineRenderer component from start to end.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    private LineRenderer DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject("Line");
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
        return lr;
    }
    /// <summary>
    /// Returns a Vector of the direction of the section separator on the left of section.
    /// </summary>
    /// <param name="section"></param>
    /// <returns></returns>
    private Vector3 getDirection(int section)
    {
        Quaternion rotation = Quaternion.AngleAxis(ANGLE_STEP * (section + 0.5f), Vector3.forward);
        return rotation * Vector2.up;
    }

    /// <summary>
    /// Returns the distance from (0,0) to the layer separator below layer.
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    private float getDistance(int layer)
    {
        return initialLayerDistance + (separationBetweenLayers * (layer - 0.5f));
    }


    /// <summary>
    /// Returns the centroid of points.
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    private Vector2 getCenter(Vector2[] points)
    {
        Vector2 center = Vector2.zero;

        for (int i = 0; i < points.Length; i++)
        {

            center += points[i];
        }
        center /= points.Length;

        return center;

    }
    #endregion





}
