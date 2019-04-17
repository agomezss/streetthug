using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviours : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform target;
    public GameObject bulletPrefab;

    public GameObject moneyPrefab;
    public GameObject gunPrefab;

    public float BaseHorizontalVelocity = 2.5f;
    public float BaseVerticalVelocity = 1.5f;

    public float HorizontalVehicleVelocity = 0f;
    public float VerticalVehicleVelocity = 0f;
    public float vehicleSpeedMultiplier = 1000f;
    public float vehicleBaseHorizontalSpeed = 1500f;
    public float vehicleBaseVerticalSpeed = 1000f;

    public float MaxSpeed = 100f;
    public float HorizontalVelocity = 0f;
    public float VerticalVelocity = 0f;

    public bool ApproachingSomething = false;
    public GameObject ApproachedObject;

    [Range(0, 500)]

    public int Health = 100;
    [Range(0, 1000000)]
    public int Money = 0;
    [Range(0, 100)]
    public int GunBullets = 0;

    public float BulletMaxSpeed = 100f;
    public float lastShootedTime;

    public bool IsPunching = false;
    public float lastPunhcedTime;

    public bool IsRunning = false;
    public float RunningSpeedMultiplicator = 1.25f;

    public bool IsChasing = false;
    public Vector2 ChasingTarget;

    public bool IsRiding = false;
    public GameObject Vehicle;

    public bool IsNpc = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckAlive();
        Move();
    }

    void CheckAlive()
    {
        if (Health <= 0)
        {
            if (!IsNpc)
            {
                GameOver();
            }
            else
            {
                // Spawn money
                if (Money > 0)
                    Instantiate(moneyPrefab, transform.position, Quaternion.identity);

                // Spawn gun
                if (GunBullets > 0)
                    Instantiate(gunPrefab, transform.position, Quaternion.identity);

                GameManager.S.SpawnNPC();
                Destroy(gameObject);
            }
        }
    }

    public void GameOver()
    {
        Destroy(gameObject);
        GameManager.S.GameOver();
    }

    void Move()
    {
        if (IsChasing)
        {
            Pursue(null);
        }

        if (IsRiding)
        {
            transform.position = Vehicle.transform.position;

            if (IsNpc && ApproachingSomething) return;

            var vehiclerb = Vehicle.GetComponent<Rigidbody2D>();
            var vehicleS = Vehicle.GetComponent<Vehicle>();
            var multipliers = vehicleS.speedMultiplicator * vehicleSpeedMultiplier;

            vehiclerb.AddForce(new Vector2(HorizontalVehicleVelocity * Time.deltaTime * multipliers, VerticalVehicleVelocity * Time.deltaTime * multipliers));

            // Limit Speed
            if (vehiclerb.velocity.magnitude > vehicleS.maxVehicleSpeed)
            {
                vehiclerb.velocity = Vector2.ClampMagnitude(vehiclerb.velocity, vehicleS.maxVehicleSpeed);
            }

            if (!IsNpc)
            {
                var audioSource = Vehicle.GetComponent<AudioSource>();

                if (!audioSource.isPlaying && Mathf.Abs(HorizontalVehicleVelocity) > 1f)
                {
                    audioSource.clip = GameManager.S.sfxCarEngine;
                    audioSource.volume = 0.5f;
                    audioSource.Play();
                }
            }
        }
        else
        {
            transform.position = new Vector3(transform.position.x + HorizontalVelocity * Time.deltaTime, transform.position.y + VerticalVelocity * Time.deltaTime, transform.position.z);
        }

    }

    public void MoveRight()
    {
        HorizontalVelocity = BaseHorizontalVelocity * (IsRunning ? RunningSpeedMultiplicator : 1f);

        if (IsRiding)
        {
            HorizontalVehicleVelocity = vehicleBaseHorizontalSpeed;
            Vehicle.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void MoveLeft()
    {
        HorizontalVelocity = BaseHorizontalVelocity * -1f * (IsRunning ? RunningSpeedMultiplicator : 1f);

        if (IsRiding)
        {
            HorizontalVehicleVelocity = vehicleBaseHorizontalSpeed * -1;
            Vehicle.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    public void MoveUp()
    {
        VerticalVelocity = BaseVerticalVelocity * -1f;

        if (IsRiding)
        {
            VerticalVehicleVelocity = vehicleBaseVerticalSpeed * -1;
        }
    }

    public void MoveDown()
    {
        VerticalVelocity = BaseVerticalVelocity;

        if (IsRiding)
        {
            VerticalVehicleVelocity = vehicleBaseVerticalSpeed;
        }
    }

    public void StopHorizontal()
    {
        HorizontalVelocity = 0;
        HorizontalVehicleVelocity = 0;
    }

    public void StopVertical()
    {
        VerticalVelocity = 0;
        VerticalVehicleVelocity = 0;
    }

    public void Punch()
    {
        if (Time.time - lastPunhcedTime > 0.5f)
        {
            lastPunhcedTime = Time.time;
            var punch = transform.Find("Punch").gameObject;
            punch.SetActive(true);

            var audioSource = GetComponent<AudioSource>();
            audioSource.clip = GameManager.S.sfxPunch;
            audioSource.Play();

            Invoke("HidePunch", 0.5f);
        }
    }

    private void HidePunch()
    {
        transform.Find("Punch").gameObject.SetActive(false);
    }

    public void Shoot()
    {
        if (!IsNpc && GunBullets <= 0) return;

        if (Time.time - lastShootedTime > 0.3f)
        {
            GunBullets--;
            lastShootedTime = Time.time;
            var gun = transform.Find("Gun").gameObject;
            gun.SetActive(true);
            Invoke("HideGun", 0.5f);

            var bulletInstance = Instantiate(bulletPrefab, gun.transform.position, Quaternion.identity);
            bulletInstance.GetComponent<Bullet>().Shooter = gameObject;
            bulletInstance.GetComponent<Rigidbody2D>().AddForce(transform.right * (IsNpc ? 10 : 2) * BulletMaxSpeed);

            Physics2D.IgnoreCollision(bulletInstance.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            var audioSource = GetComponent<AudioSource>();
            audioSource.clip = GameManager.S.sfxShoot;
            audioSource.Play();

            Destroy(bulletInstance, 2);
        }
    }

    private void HideGun()
    {
        transform.Find("Gun").gameObject.SetActive(false);
    }

    public void Pursue(Transform targetPursue)
    {
        IsChasing = true;
        target = targetPursue ?? target;

        if (target == null)
        {
            IsChasing = false;
            return;
        }

        if (!(Mathf.Abs(target.position.x - transform.position.x) > 0.35f))
        {
            HorizontalVelocity = 0f;
        }
        else
        {
            if (target.position.x > transform.position.x)
            {
                MoveRight();
            }
            else
            {
                MoveLeft();
            }
        }


        if (!(Mathf.Abs(target.position.y - transform.position.y) > 0.1f))
        {
            VerticalVelocity = 0f;
        }
        else
        {
            if (target.position.y > transform.position.y)
            {
                MoveDown();
            }
            else
            {
                MoveUp();
            }
        }

    }

    public void Run(Transform target)
    {
        IsRunning = true;

        var audioSource = GetComponent<AudioSource>();
        audioSource.clip = GameManager.S.sfxScream;
        audioSource.Play();

        if (target.position.x > transform.position.x)
        {
            MoveLeft();
        }
        else
        {
            MoveRight();
        }
    }

    public void GetHitByCar()
    {
        Health -= 20;

        var audioSource = GetComponent<AudioSource>();
        audioSource.clip = GameManager.S.sfxScream;
        audioSource.Play();

        if (!IsNpc)
        {
            GameManager.S.GetDamageEffect();
        }

        if (Health <= 0) return;

        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<SpriteRenderer>().sortingLayerName = "BG";
        Invoke("RecoverFromCarHit", 1.5f);
    }

    private void RecoverFromCarHit()
    {
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<Rigidbody2D>().isKinematic = false;
        GetComponent<SpriteRenderer>().sortingLayerName = "Player";
    }
}
