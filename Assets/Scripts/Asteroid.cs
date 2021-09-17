using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float asteroidVelocity = 2.5f;
    public Vector3 bigSizeScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 mediumSizeScale = new Vector3(0.75f, 0.75f, 1.0f);
    public Vector3 smallSizeScale = new Vector3(0.5f, 0.5f, 1.0f);
    public string bulletGameObjectName = "bullet(Clone)";
    public string spaceShipGameObjectName = "SpaceShip";
    public string ufoGameObjectName = "UFO";
    public int minAsteroidVelocity = 1;
    public int maxAsteroidVelocity = 4;
    public string sceneControllerName = "SceneController";
    public int bigAsteroidScore = 20;
    public int mediumAsteroidScore = 50;
    public int smallAsteroidScore = 100;

    private string spaceShipBulletTag = "spaceShipBullet";

    private bool moveAsteroidFlag = false;
    private ScreenBorderRules screenBorderRules;

    private float startAngle = 0.0f;
    private AsteroidSize size = AsteroidSize.Big;

    private Camera _camera;
    private bool gamePause = false;

    enum AsteroidSize 
    { 
        Big,
        Medium,
        Small
    }

    void Start()
    {
        screenBorderRules = GetComponent<ScreenBorderRules>();
    }

    void createNewAsteroid()
    {
        startAngle = Random.Range(-180, 180);
        transform.eulerAngles = new Vector3(0.0f, 0.0f, startAngle);

        setAsteroidSize(AsteroidSize.Big);

        _camera = Camera.main;
        transform.position = _camera.ScreenToWorldPoint(new Vector3(Random.Range(0.0f, Screen.width), Screen.height, 0.0f));

        moveAsteroidFlag = true;
    }

    void Update()
    {
        if (!gamePause)
        {
            if (moveAsteroidFlag)
            {
                transform.Translate(0, asteroidVelocity * Time.deltaTime, 0);
            }

            transform.position = screenBorderRules.screenBordersRoutine(transform);
        }
    }

    public void setUpdateEnabled(bool value)
    {
        gamePause = value;
    }

    public void moveAsteroid()
    {
        moveAsteroidFlag = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == bulletGameObjectName && other.tag == spaceShipBulletTag)
        {
            divideAsteroid();
        }

        if (other.name == spaceShipGameObjectName)
        {
            GameObject spaceShip = GameObject.Find(spaceShipGameObjectName);

            if (spaceShip != null)
            {
                spaceShip.SendMessage("explodeSpaceShip", SendMessageOptions.DontRequireReceiver);
                this.gameObject.SetActive(false);
            }
        }

        if (other.name == ufoGameObjectName)
        {
            GameObject ufo = GameObject.Find(ufoGameObjectName);

            if (ufo != null)
            {
                ufo.SendMessage("explodeUFO", SendMessageOptions.DontRequireReceiver);
                this.gameObject.SetActive(false);
            }
        }
    }

    void divideAsteroid()
    {
        GameObject sceneController = GameObject.Find(sceneControllerName);

        if (size == AsteroidSize.Big)
        {
            setAsteroidSize(AsteroidSize.Medium);
            sceneController.GetComponent<SceneController>().increaseScore(bigAsteroidScore);
        }
        else if (size == AsteroidSize.Medium)
        {
            setAsteroidSize(AsteroidSize.Small);
            sceneController.GetComponent<SceneController>().increaseScore(mediumAsteroidScore);
        }
        else
        {
            setAsteroidSize(AsteroidSize.Big);
            sceneController.GetComponent<SceneController>().increaseScore(smallAsteroidScore);
            this.gameObject.SetActive(false);
        }

        if (this.gameObject.activeSelf)
        {
            transform.eulerAngles = new Vector3(0.0f, 0.0f, startAngle + 45.0f);
            setAsteroidVelocity(Random.Range(minAsteroidVelocity, maxAsteroidVelocity));
            createAsteroid(size, asteroidVelocity);
        }
    }

    void createAsteroid(AsteroidSize size, float velocity)
    {
        GameObject asteroid = ObjectPool.SharedInstance.GetPooledObject("asteroid");

        if (asteroid != null)
        {
            asteroid.transform.position = transform.position;
            asteroid.transform.eulerAngles = new Vector3(0.0f, 0.0f, startAngle - 45.0f);
            asteroid.SetActive(true);

            asteroid.SendMessage("setAsteroidSize", size);
            asteroid.SendMessage("setAsteroidVelocity", velocity);
            asteroid.SendMessage("moveAsteroid", SendMessageOptions.DontRequireReceiver);
        }
    }

    void setAsteroidVelocity(float velocity)
    {
        asteroidVelocity = velocity;
    }

    void setAsteroidSize(AsteroidSize asteroidSize)
    {
        size = asteroidSize;

        if (size == AsteroidSize.Big)
        {
            transform.localScale = bigSizeScale;
        }
        else if (size == AsteroidSize.Medium)
        {
            transform.localScale = mediumSizeScale;
        }
        else if (size == AsteroidSize.Small)
        {
            transform.localScale = smallSizeScale;
        }
    }
}
