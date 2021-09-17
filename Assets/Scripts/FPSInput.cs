using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSInput : MonoBehaviour
{
    public float maxSpeed = 7.0f;
    public float moveSpeed = 6.0f;
    public float rotationSpeed = 100.0f;
    public float rotationSpeedMouse = 4.0f;
    public float acceleration = .25f;
    public float timeLimit = 1.5f;

    public AudioSource audioSource;
    public AudioClip thrustClip;

    private float time = 0.0f;
    private Camera _camera;
    private ScreenBorderRules screenBorderRules;

    private bool stopMovementsFlag = false;

    private float playClipTime = 0.0f;
    private float thrustClipTime = 0.288f;

    private GameObject sceneController;
    private string sceneControllerObjName = "SceneController";

    private float deltaX = 0.0f, deltaY = 0.0f;
    private bool isKeyboardControl = true;
    private bool isButtonDown = false;
    private float mouseButtonHackTime = 0.0f;
    private float buttonPressTime = 0.8f;

    void Start()
    {
        _camera = Camera.main;
        screenBorderRules = GetComponent<ScreenBorderRules>();

        sceneController = GameObject.Find(sceneControllerObjName);
    }

    void stopMovements()
    {
        StartCoroutine(stopMovementsDelay());
    }

    IEnumerator stopMovementsDelay()
    {
        stopMovementsFlag = true;
        time = 0.0f;

        yield return new WaitForSeconds(0.1f);

        stopMovementsFlag = false;
    }

    void Update()
    {
        if (!stopMovementsFlag)
        {
            deltaY = Input.GetAxis("Vertical") * moveSpeed;

            if (isKeyboardControl)
            {
                deltaX = Input.GetAxis("Horizontal") * rotationSpeed;

                if (deltaX != 0.0f)
                {
                    transform.Rotate(0, 0, -deltaX * Time.deltaTime);
                }
            }
            else
            {
                if (Input.GetAxis("Vertical") == 0.0f)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        isButtonDown = true;
                    }
                }

                if (isButtonDown)
                {
                    if (mouseButtonHackTime < buttonPressTime)
                    {
                        mouseButtonHackTime += Time.deltaTime;
                        deltaY = 1.0f * moveSpeed;
                    }
                    else
                    {
                        isButtonDown = false;
                        mouseButtonHackTime = 0.0f;
                        deltaY = 0.0f;
                    }
                }

                float angle = getMouseAngle();
                Quaternion direction = Quaternion.Euler(new Vector3(0, 0, angle));
                transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotationSpeedMouse * Time.deltaTime);
            }

            if (deltaY > 0.0f)
            {
                deltaY += acceleration * Mathf.Pow(time, 2);

                if (deltaY > maxSpeed)
                {
                    deltaY = maxSpeed;
                }

                transform.Translate(0, deltaY * Time.deltaTime, 0);

                if (time < timeLimit)
                {
                    time += Time.deltaTime;
                }

                if (playClipTime == 0.0f)
                {
                    audioSource.PlayOneShot(thrustClip);
                    playClipTime += Time.deltaTime;
                }
                else if (playClipTime < thrustClipTime)
                {
                    playClipTime += Time.deltaTime;
                }
                else 
                {
                    playClipTime = 0.0f;
                }
            }
            else
            {
                if (time > 0.0f)
                {
                    deltaY = acceleration * Mathf.Pow(time, 2);

                    if (deltaY > maxSpeed)
                    {
                        deltaY = maxSpeed;
                    }

                    transform.Translate(0, deltaY * Time.deltaTime, 0);
                    time -= Time.deltaTime;
                }
                else
                {
                    time = 0.0f;
                }
            }
        }

        transform.position = screenBorderRules.screenBordersRoutine(transform);
    }

    private float getMouseAngle()
    { 
        Vector3 mousePos = Input.mousePosition;
        Vector3 objectPos = _camera.WorldToScreenPoint(transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float angle = (Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg) - 90.0f;

        return angle;
    }

    public void setUpdateEnabled(bool value)
    {
        stopMovementsFlag = value;
        isKeyboardControl = sceneController.GetComponent<SceneController>().isKeyboardControl();
    }
}
