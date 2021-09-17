using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ufo : MonoBehaviour
{
    public float UfoVelocity = 2.0f;
    public int UfoApearedMinTime = 20;
    public int UfoApearedMaxTime = 40;
    public int UfoFireMinTime = 2;
    public int UfoFireMaxTime = 5;

    public AudioSource audioSource;
    public AudioClip shootClip;
    public AudioClip explodeClip;

    private int appeareTime = 0;
    private bool UfoAppeare = false;
    private int flyDirection = 1;

    private string spaceShipGameObjectName = "SpaceShip";
    private string bulletGameObjectName = "bullet(Clone)";

    private ScreenBorderRules screenBorderRules;
    private Camera _camera;
    private float fireTime = 0.0f;
    private int fireDelay = 2;
    private float finalPosition = 0.0f;

    private string ufoBulletTag = "ufoBullet";
    private bool gamePause = false;

    void Start()
    {
        generateAppeareTime();
        enableDestroy(false);
        _camera = Camera.main;
    }

    void Update()
    {
        if (UfoAppeare && !gamePause)
        {
            transform.Translate(flyDirection * UfoVelocity * Time.deltaTime, 0.0f, 0.0f);

            if (fireTime < fireDelay)
            {
                fireTime += Time.deltaTime;
            }
            else
            {
                fireTime = 0.0f;
                generateFireDelay();
                shoot();
            }

            if ((transform.position.x >= finalPosition && flyDirection == 1)
                || (transform.position.x <= finalPosition && flyDirection == -1)
               )
            {
                UfoAppeare = false;
                fireTime = 0.0f;
                generateAppeareTime();
                enableDestroy(false);
            }
        }
    }

    public void setUpdateEnabled(bool value)
    {
        gamePause = value;
    }

    void shoot()
    {
        GameObject bullet = ObjectPool.SharedInstance.GetPooledObject("bullet");

        if (bullet != null)
        {
            GameObject spaceShip = GameObject.Find(spaceShipGameObjectName);

            bullet.transform.position = transform.position;

            Vector3 dir = spaceShip.transform.position - transform.position;            
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            bullet.SetActive(true);

            bullet.SendMessage("ufoShoot", SendMessageOptions.DontRequireReceiver);

            audioSource.PlayOneShot(shootClip);
        }
    }

    void generateAppeareTime()
    {
        appeareTime = Random.Range(UfoApearedMinTime, UfoApearedMaxTime);
        generateFireDelay();

        StartCoroutine(waitingUfo(appeareTime));
    }

    void generateFireDelay()
    {
        fireDelay = Random.Range(UfoFireMinTime, UfoFireMaxTime);
    }

    IEnumerator waitingUfo(int time)
    {
        yield return new WaitForSeconds(time);

        enableDestroy(true);

        int leftOrRightBorder = Random.Range(1, 3);
        float screenPercentageHeight = Screen.height / 5;

        if (leftOrRightBorder == 1)
        {
            flyDirection = 1;
            transform.position = _camera.ScreenToWorldPoint(new Vector3(0.0f, Random.Range(screenPercentageHeight, (Screen.height - screenPercentageHeight)), 10.0f));
            Vector3 finalPos = _camera.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, 0.0f));
            finalPosition = finalPos.x;
        }
        else
        {
            flyDirection = -1;
            transform.position = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Random.Range(screenPercentageHeight, (Screen.height - screenPercentageHeight)), 10.0f));
            Vector3 finalPos = _camera.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
            finalPosition = finalPos.x;
        }

        UfoAppeare = true;
    }

    public void explodeUFO()
    {
        enableDestroy(false);

        UfoAppeare = false;
        generateAppeareTime();

        audioSource.PlayOneShot(explodeClip);
    }

    void enableDestroy(bool value)
    {
        GetComponent<SpriteRenderer>().enabled = value;
        GetComponent<BoxCollider2D>().enabled = value;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == spaceShipGameObjectName)
        {
            GameObject spaceShip = GameObject.Find(spaceShipGameObjectName);

            if (spaceShip != null)
            {
                spaceShip.SendMessage("explodeSpaceShip", SendMessageOptions.DontRequireReceiver);
                explodeUFO();
            }
        }

        if (other.name == bulletGameObjectName && other.tag != ufoBulletTag)
        {
            explodeUFO();
        }
    }
}
