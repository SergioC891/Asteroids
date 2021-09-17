using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 8.0f;
    public string asteroidGameObjectName = "asteroid(Clone)";
    public string ufoGameObjectName = "UFO";
    public string sceneControllerName = "SceneController";
    public int ufoScore = 200;

    private bool fireFlag = false;
    private ScreenBorderRules screenBorderRules;
    private string ufoBulletTag = "ufoBullet";
    private string spaceShipBulletTag = "spaceShipBullet";

    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (fireFlag)
        {
            transform.Translate(0, bulletSpeed * Time.deltaTime, 0);
        }

        bulletScreenBordersRoutine();
    }

    public void shoot()
    {
        this.gameObject.tag = spaceShipBulletTag;
        GetComponent<SpriteRenderer>().color = Color.green;
        fireFlag = true;
    }

    public void ufoShoot()
    {
        this.gameObject.tag = ufoBulletTag;
        GetComponent<SpriteRenderer>().color = Color.red;
        fireFlag = true;
    }

    void bulletScreenBordersRoutine()
    {
        Vector3 pos = _camera.WorldToScreenPoint(transform.position);

        if (pos.x < 0.0f || pos.x > Screen.width || pos.y < 0.0f || pos.y > Screen.height)
        {
            this.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (
            (other.name == asteroidGameObjectName 
            || other.name == ufoGameObjectName) 
            && this.gameObject.tag == spaceShipBulletTag
           )
        {
            if (other.name == ufoGameObjectName)
            {
                GameObject sceneController = GameObject.Find(sceneControllerName);
                sceneController.GetComponent<SceneController>().increaseScore(ufoScore);
            }

            this.gameObject.SetActive(false);
        }
    }
}
