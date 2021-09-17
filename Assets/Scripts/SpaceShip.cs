using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    public int bulletsPerSecond = 3;
    public float fireTimeLimit = 1.0f;

    public AudioSource audioSource;
    public AudioClip shootClip;
    public AudioClip explodeClip;
    public GameObject sceneController;

    private FPSInput inputScript;
    private float dontDestroyTime = 3.0f;
    private float flashDelay = .5f;
    private int bulletCount = 0;
    private float fireTime = 0.0f;
    private bool fireTimeStart = false;
    private bool gamePause = false;
    private bool isKeyboardControl = true;

    private string bulletGameObjectName = "bullet(Clone)";
    private string spaceShipBulletTag = "spaceShipBullet";

    void Start()
    {
        inputScript = GetComponent<FPSInput>();
    }

    void Update()
    {
        if (!gamePause)
        {
            if ((isKeyboardControl && Input.GetButtonDown("Fire1") && !Input.GetMouseButton(0))
                || (!isKeyboardControl && Input.GetButtonDown("Fire1")))
            {
                if (bulletCount < bulletsPerSecond)
                {
                    shoot();
                    bulletCount++;
                    fireTimeStart = true;
                }
            }

            if (fireTimeStart)
            {
                if (fireTime < fireTimeLimit)
                {
                    fireTime += Time.deltaTime;
                }
                else
                {
                    bulletCount = 0;
                    fireTime = 0.0f;
                    fireTimeStart = false;
                }
            }
        }
    }

    public void setUpdateEnabled(bool value)
    {
        gamePause = value;
        isKeyboardControl = sceneController.GetComponent<SceneController>().isKeyboardControl();
    }

    private void shoot()
    {
        GameObject bullet = ObjectPool.SharedInstance.GetPooledObject("bullet");

        if (bullet != null)
        {
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            bullet.SetActive(true);

            bullet.SendMessage("shoot", SendMessageOptions.DontRequireReceiver);

            audioSource.PlayOneShot(shootClip);
        }
    }

    void explodeSpaceShip()
    {
        inputScript.SendMessage("stopMovements", SendMessageOptions.DontRequireReceiver);

        sceneController.GetComponent<SceneController>().decreaseLives();

        transform.eulerAngles = Vector3.zero;
        transform.position = Vector3.zero;

        StartCoroutine(spawnState());

        audioSource.PlayOneShot(explodeClip);
    }

    IEnumerator spawnState()
    {
        GetComponent<BoxCollider2D>().enabled = false;

        StartCoroutine(flashProcess());

        yield return new WaitForSeconds(dontDestroyTime);

        GetComponent<BoxCollider2D>().enabled = true;
    }

    IEnumerator flashProcess()
    {
        float _time = 0.0f;

        while (_time < dontDestroyTime)
        {
            yield return new WaitForSeconds(flashDelay / 2.0f);

            GetComponent<SpriteRenderer>().enabled = false;
            _time += flashDelay;

            yield return new WaitForSeconds(flashDelay / 2.0f);

            GetComponent<SpriteRenderer>().enabled = true;
        }

        yield return new WaitForSeconds(flashDelay);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == bulletGameObjectName && other.tag != spaceShipBulletTag)
        {
            explodeSpaceShip();
        }
    }
}
