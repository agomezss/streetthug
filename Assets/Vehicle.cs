using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public GameObject driver;
    public float speedMultiplicator;
    public float maxVehicleSpeed = 1000f;
    public float health = 100;
    public bool IsDestroyed = false;
    public SpriteRenderer _renderer;
    public float RidedTime;
    public Sprite DestroyedSprite;
    public GameObject hitPerson;
    public float distPad = 1.5f;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        maxVehicleSpeed = maxVehicleSpeed * GameManager.S.globalVehicleSpeedMultiplicator;
    }

    void FixedUpdate()
    {
        CheckBondaries();

        if (health <= 0 && !IsDestroyed)
        {
            var rb = GetComponent<Rigidbody2D>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;

            _renderer.sprite = DestroyedSprite;
            _renderer.sortingLayerName = "BG";

            var bc = GetComponents<BoxCollider2D>();

            foreach (var collider in bc)
            {
                collider.enabled = false;
            }

            var behaviour = driver.GetComponent<Behaviours>();

            if (behaviour != null)
            {
                behaviour.Health = 0;
            }

            driver = null;
            transform.tag = "Corpse";
            IsDestroyed = true;

            var audioSource = GetComponent<AudioSource>();
            audioSource.clip = GameManager.S.sfxExplosion;
            audioSource.volume = 1f;
            audioSource.loop = false;
            audioSource.Play();

            GameManager.S.SpawnCar(transform.position.y);
        }
    }

    void CheckBondaries()
    {
        if (transform.position.x < -8)
            transform.position = new Vector3(186f, transform.position.y, transform.position.z);
        else if (transform.position.x > 186)
            transform.position = new Vector3(-7.5f, transform.position.y, transform.position.z);
    }

    public void Ride(GameObject newDriver)
    {
        if (IsDestroyed || (RidedTime != 0f && Time.time - RidedTime < 2f) || driver == newDriver) return;
        RidedTime = Time.time;

        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0f;

        newDriver.GetComponent<BoxCollider2D>().enabled = false;

        var mainRender = newDriver.GetComponent<SpriteRenderer>();
        mainRender.enabled = false;

        if (driver != null)
        {
            var mainRenderDriver = driver.GetComponent<SpriteRenderer>();
            mainRenderDriver.enabled = true;

            driver.transform.position = new Vector3(newDriver.transform.position.x, newDriver.transform.position.y - 1f, newDriver.transform.position.z);
            driver.GetComponent<BoxCollider2D>().enabled = true;

            var driverBehaviour = driver.GetComponent<Behaviours>();
            driverBehaviour.IsRiding = false;
            driverBehaviour.Vehicle = null;
        }

        newDriver.transform.position = transform.position;

        var behaviour = newDriver.GetComponent<Behaviours>();
        behaviour.IsRiding = true;
        behaviour.Vehicle = gameObject;

        driver = newDriver;
    }

    public void StopRidding()
    {
        if (Time.time - RidedTime < 2f) return;

        var renderers = driver.GetComponentsInChildren<SpriteRenderer>();

        foreach (var r in renderers)
        {
            r.enabled = true;
        }

        driver.GetComponent<BoxCollider2D>().enabled = true;
        driver.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

        var behaviour = driver.GetComponent<Behaviours>();
        behaviour.StopHorizontal();
        behaviour.StopVertical();
        behaviour.IsRiding = false;
        behaviour.Vehicle = null;

        driver.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        driver.GetComponent<Rigidbody2D>().angularVelocity = 0f;

        RidedTime = Time.time;
        driver = null;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject == driver) return;

        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Char")
        {
            var rb = GetComponent<Rigidbody2D>();
            if (Mathf.Abs(rb.velocity.x) < 1f) return;

            hitPerson = other.gameObject;

            var otherBehaviour = other.gameObject.GetComponent<Behaviours>();
            otherBehaviour.GetHitByCar();
        }
        else if (other.gameObject.tag == "Car")
        {
            var player = driver.GetComponent<Player>();
            if(player == null) return;

            var audioSource = GetComponent<AudioSource>();

            if (audioSource.clip != GameManager.S.sfxCarCrash)
            {
                audioSource.clip = GameManager.S.sfxCarCrash;
                audioSource.volume = 1f;
                audioSource.loop = false;
                audioSource.Play();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            if (IsDestroyed) return;
            health -= 10;
        }
    }
}