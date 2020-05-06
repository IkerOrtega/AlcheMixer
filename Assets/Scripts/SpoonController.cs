using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpoonController : MonoBehaviour
{
    #region Variables
    private GameController _gc;

    private Vector2 direction;
    private int sectionSelected = 0;
    private Vector2 oldMousePos;


    #endregion
    #region Unity Functions

    private void Awake()
    {
        _gc = GameObject.FindWithTag("GameController").GetComponent<GameController>();

    }
    // Start is called before the first frame update
    void Start()
    {
        oldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // Update is called once per frame
    void Update()
    {

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        //If input from keyboard or joystick  
        if (horizontalInput != 0 || verticalInput != 0)
        {
            setSpoonSectionSelected(getAngleDegreesFromInput(horizontalInput, verticalInput));
        }
        // else input from mouse
        else
        {
            // Mouse returns postion in pixels, needs to be translated to World units.
            Vector2 newMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Point to mouse only when it moves
            if (!newMousePos.Equals(oldMousePos))
            {
                oldMousePos = newMousePos;
                Vector2 spoonPos = transform.position;
                //Point spoon top to mouse by creating vector from spoon center to mouse.
                 transform.up = newMousePos - spoonPos;
                setSpoonSectionSelected(transform.rotation.eulerAngles.z);
            }
        }
    }

    private void FixedUpdate()
    {


    }

    private void LateUpdate()
    {
        for (int i = 0; i < _gc.ingList.Count; i++)
        {
            _gc.ingList[i].GetComponent<SpriteRenderer>().color = (i == sectionSelected ? Color.yellow : Color.black);
        }

    }
    #endregion
    #region Utilities

    private void setSpoonSectionSelected(float inputAngle)
    {
        // Calculate section we're in
        sectionSelected = Mathf.RoundToInt(inputAngle / _gc.ANGLE_STEP);
        //Snap to nearest ANGLE_STEP
        float snappedAngle = _gc.ANGLE_STEP * sectionSelected;
        // Set rotation of Spoon
        transform.rotation = Quaternion.AngleAxis(snappedAngle, Vector3.forward);
        //Debug.Log("Real Angle: " + inputAngle + "\nSnapped Angle: " + snappedAngle + "\nsectionSelected: " + sectionSelected);

    }

    /// <summary>
    /// Returns the angle of the battle circle in Degrees for the joystick/keyboard Input x,y.   	
    /// </summary>
    /// <param name="x">Input horizontal</param>
    /// <param name="y">Input vertical</param>
    private float getAngleDegreesFromInput(float x, float y)
    {
        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg - Constants.ANGLE_CORRECTION;
        if (angle < 0f)
        {
            angle = Constants.MAX_ANGLE + angle;
        }

        return angle;
    }

   
    #endregion
}
