/*****************************************************************************
* INSTANT JOYSTICK
* DESC: The instant joystick appears whereever the player places their finger
* or mouse cursor. The idea is that a joystick can be anywhere on the screen.
* Author: Jonathan L Clark
* Date: 1/24/2021
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantJoystick : MonoBehaviour
{
    public float joystickRange = 100.0f;
    public RectTransform leftJoystickImage;
    public RectTransform rightJoystickImage;

    private Vector3 leftJoystickPosition = new Vector3(0.0f, 0.0f);
    private Vector3 rightJoystickPosition = new Vector3(0.0f, 0.0f);
    private bool lastLeftTouchState = false;
    private bool lastRightTouchState = false;
    public float vertical_left { get; private set; } = 0.0f;
    public float horizontal_left { get; private set; } = 0.0f;

    public float vertical_right { get; private set; } = 0.0f;
    public float horizontal_right { get; private set; } = 0.0f;

    private float screenSectionSize = Screen.width / 10; // Split the screen into 1/10ths 
    private float heightSectionSize = Screen.height / 10;


    // Start is called before the first frame update
    void Start()
    {
        leftJoystickImage.gameObject.SetActive(false);
        rightJoystickImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// Ensure that our joystick position is within the desired range
    /// then apply a scale to the joystick position from 0-1
    /// </summary>
    /// <param name="inputDelta"></param>
    /// <returns></returns>
    private float ScaleAxis(float inputDelta)
    {
        if (inputDelta < -joystickRange)
        {
            inputDelta = -joystickRange;
        }
        else if (inputDelta > joystickRange)
        {
            inputDelta = joystickRange;
        }
        return inputDelta / joystickRange;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
#if MOBILE_INPUT
        bool leftJoystickActive = false;
        bool rightJoystickActive = false;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Vector3 touchPosition = Input.GetTouch(i).position;
            // Left joystick
            if (touchPosition.x <= screenSectionSize * 5 && touchPosition.y <= heightSectionSize * 8)
            {
                leftJoystickActive = true;
                // Last joystick not present, create a joystick at this position
                if (!lastLeftTouchState)
                {
                    leftJoystickPosition = touchPosition;
                }
                else
                {
                    Vector3 deltaPosition = touchPosition - leftJoystickPosition;
                    float horizontal = ScaleAxis(deltaPosition.x);
                    float verticle = ScaleAxis(deltaPosition.y);
                    vertical_left = verticle;
                    horizontal_left = horizontal;
                }
                touchPosition.x = Mathf.Clamp(touchPosition.x, leftJoystickPosition.x - joystickRange, leftJoystickPosition.x + joystickRange);
                touchPosition.y = Mathf.Clamp(touchPosition.y, leftJoystickPosition.y - joystickRange, leftJoystickPosition.y + joystickRange);
                leftJoystickImage.position = touchPosition;
                leftJoystickImage.gameObject.SetActive(true);

            }
            // Right joystick
            else if (touchPosition.x > screenSectionSize * 5 && touchPosition.y <= heightSectionSize * 8)
            {
                rightJoystickActive = true;
                if (!lastRightTouchState)
                {
                    rightJoystickPosition = touchPosition;
                }
                else
                {
                    Vector3 deltaPosition = touchPosition - rightJoystickPosition;
                    float horizontal = ScaleAxis(deltaPosition.x);
                    float verticle = ScaleAxis(deltaPosition.y);
                    vertical_right = verticle;
                    horizontal_right = horizontal;
                }
                touchPosition.x = Mathf.Clamp(touchPosition.x, rightJoystickPosition.x - joystickRange, rightJoystickPosition.x + joystickRange);
                touchPosition.y = Mathf.Clamp(touchPosition.y, rightJoystickPosition.y - joystickRange, rightJoystickPosition.y + joystickRange);
                rightJoystickImage.position = touchPosition;
                rightJoystickImage.gameObject.SetActive(true);
            }
        }
        if (!leftJoystickActive && lastLeftTouchState)
        {
            vertical_left = 0.0f;
            horizontal_left = 0.0f;
            leftJoystickImage.gameObject.SetActive(false);
        }
        if (!rightJoystickActive && lastRightTouchState)
        {
            vertical_right = 0.0f;
            horizontal_right = 0.0f;
            rightJoystickImage.gameObject.SetActive(false);
        }
        lastLeftTouchState = leftJoystickActive;
        lastRightTouchState = rightJoystickActive;

#else
        bool touchState = Input.GetMouseButton(0);

        if (touchState && !lastLeftTouchState)
        {
            leftJoystickPosition = Input.mousePosition;
        }
        else if (touchState)
        {
            Vector3 deltaPosition = Input.mousePosition - leftJoystickPosition;
            float horizontal = ScaleAxis(deltaPosition.x);
            float verticle = ScaleAxis(deltaPosition.y);
            if (isRightSide)
            {
                vertical_right = verticle;
                horizontal_right = horizontal;
            }
            else
            {
                vertical_left = verticle;
                horizontal_left = horizontal;
            }
        }
        else
        {
            vertical_left = 0.0f;
            horizontal_left = 0.0f;
        }
        lastLeftTouchState = touchState;
#endif
    }

}
