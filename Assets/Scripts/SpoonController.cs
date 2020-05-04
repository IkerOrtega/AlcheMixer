using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpoonController : MonoBehaviour
{
    public GameObject ingredientPrefab;
    public float ingRadius = 0.5f;

    private GameController _gc;

    private Vector2 direction;
    private List<GameObject> ingList;
    private int sectionSelected = 0;





    private void Awake()
    {
        _gc = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        ingList = new List<GameObject>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (ingredientPrefab != null)
        {
            for (int i = 0; i < _gc.NUM_SECTIONS; i++)
            {
                Quaternion rotation = Quaternion.AngleAxis(_gc.ANGLE_STEP * i, Vector3.forward);
                Vector3 direction = rotation * Vector2.right;
                Vector3 position = transform.position + (direction * ingRadius);
                ingList.Add(Instantiate(ingredientPrefab, position, rotation));
            }
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        // TODO: Not snapping correctly to diagonals.
        // TODO Get input from mouse.
        if (horizontalInput != 0 || verticalInput != 0)
        {
            float tempRotation = Mathf.Atan2(verticalInput, horizontalInput) * Mathf.Rad2Deg;
            if(tempRotation < 0f) {
                tempRotation = Constants.MAX_ANGLE + tempRotation;
            }
            //Snap to nearest ANGLE_STEP
            
            sectionSelected = Mathf.RoundToInt(tempRotation / _gc.ANGLE_STEP);

            // Debug.Log("Direction: (" + horizontalInput + ", " + verticalInput + ")");
            
            transform.rotation = Quaternion.AngleAxis(_gc.ANGLE_STEP * sectionSelected, Vector3.forward);
            Debug.Log("Section Selected: " + sectionSelected);
            Debug.Log("Rotation: " + _gc.ANGLE_STEP * sectionSelected);
        }

       

    }

    private void FixedUpdate()
    {


    }

    private void LateUpdate()
    {
        for (int i = 0; i < ingList.Count; i++)
        {
            ingList[i].GetComponent<SpriteRenderer>().color = (i == sectionSelected ? Color.yellow : Color.white);
        }

    }
}
