using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{
    public UnityEngine.UI.Text displayBullet;
    public int totalBullet;

    private Stack<GameObject> bulletPool;
    private GameObject aBullet;
    private bool playerActive;

    public float speed;
    public float tilt;
    public Boundary boundary;

    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;

    private float nextFire;
    private Vector3 origin, currentPosition;
    private Touch touch;

    private void Start()
    {
        bulletPool = new Stack<GameObject>();

        for (int i = 0; i < totalBullet; i++)
        {
            aBullet = Instantiate(shot);
            aBullet.gameObject.SetActive(false);
            bulletPool.Push(aBullet);
        }
        displayBullet.text = "Bullet : " + totalBullet + "/" + totalBullet;
    }

    private void OnBecameVisible()
    {
        playerActive = false;
        origin = Vector3.zero;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            currentPosition = Camera.main.ScreenToWorldPoint(touch.position);
            currentPosition.y = 0.0f;
            currentPosition.z += 1.0f;
            if (!playerActive)
            {
                double res = Math.Sqrt(Math.Pow((currentPosition.x - origin.x), 2) + Math.Pow((currentPosition.z - origin.z), 2));
                if (res < 1)
                {
                    origin = currentPosition;
                    playerActive = true;
                }
                else playerActive = false; //displayBullet.text = "res " + res;
            }

            if (playerActive)
            {
                if (currentPosition.z <= boundary.zMax)
                    transform.position = currentPosition;
                else if (boundary.zMax - origin.z < 2)
                    transform.position = new Vector3(currentPosition.x, 0.0f, boundary.zMax);
                else
                {
                    float a = currentPosition.z - origin.z; // origin is in (x1, y1)
                    float b = origin.x - currentPosition.x;
                    float c = -a * origin.x - b * origin.z;
                    float x = -(b * boundary.zMax + c) / a;
                    transform.position = new Vector3(x, 0.0f, boundary.zMax);
                }
            }
        }
        else
        {
            if (playerActive) origin = gameObject.transform.position;
            playerActive = false;// displayBullet.text = transform.position.x + ", " + transform.position.y + ", " + transform.position.z;
        }

        if ((Input.GetButton("Fire1_mod") || playerActive) && Time.time > nextFire)
        {
            displayBullet.text = Input.GetButton("Fire1") + " and " + playerActive + "  " + transform.position;
            nextFire = Time.time + fireRate;
            Fire();
            GetComponent<AudioSource>().Play();
        }
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        GetComponent<Rigidbody>().velocity = movement * speed;

        GetComponent<Rigidbody>().position = new Vector3
        (
            Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
        );

        GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
    }

    void Fire()
    {
        if (bulletPool.Count > 0)
        {
            aBullet = bulletPool.Pop();
            aBullet.transform.position = shotSpawn.position;
            aBullet.SetActive(true);
            displayBullet.text = "Bullet : " + bulletPool.Count + '/' + totalBullet;
        }
    }
    public void ReloadBullet(GameObject bullet)
    {
        bullet.gameObject.SetActive(false);
        bulletPool.Push(bullet);
        displayBullet.text = "Bullet : " + bulletPool.Count + '/' + totalBullet;
    }
}