using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Behaviours behaviour;
    private EmulatedInput virtualInput;

    void Awake()
    {
        behaviour = GetComponent<Behaviours>();
        virtualInput = GetComponent<EmulatedInput>();
    }

    void Update()
    {
        if (SimpleInput.GetButton("Fire2") || virtualInput.Fire2)
        {
            virtualInput.Fire2 = false;

            if (behaviour.IsRiding)
            {
                behaviour.Vehicle.GetComponent<Vehicle>().StopRidding();
            }
            else
            {
                if (!CheckProximityVehicle())
                    behaviour.Punch();
            }
        }

        if (SimpleInput.GetButton("Fire1") || virtualInput.Fire1)
        {
            if (behaviour.IsRiding) return;
            virtualInput.Fire1 = false;
            behaviour.Shoot();
        }

        var horizontal = SimpleInput.GetAxis("Horizontal");

        if (horizontal == 0)
        {
            behaviour.StopHorizontal();
        }
        else if (horizontal > 0)
        {
            behaviour.MoveRight();

        }
        else
        {
            behaviour.MoveLeft();
        }

        var vertical = SimpleInput.GetAxis("Vertical");

        if (vertical == 0)
        {
            behaviour.StopVertical();
        }
        else if (vertical > 0)
        {
            behaviour.MoveDown();
        }
        else
        {
            behaviour.MoveUp();
        }

        if (SimpleInput.GetButton("Jump"))
        {
            GameManager.S.PauseUnpause();
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.S.PauseUnpause();
            }
        }
    }

    bool CheckProximityVehicle()
    {
        if (behaviour.IsRiding) return false;

        var factor = 1f;
        var factorDist = transform.right.x < 0 ? -0.4f : 0.4f;
        var factorArea = 0.2f;

        var startingPos = new Vector3(transform.position.x + factorDist, transform.position.y - factorArea, transform.position.z);
        var stoppingPoint = new Vector3(startingPos.x + (transform.right.x * factorDist), startingPos.y + (factorArea * 2), startingPos.z);

        var stopPoint = (stoppingPoint + transform.right * factor);
        //Debug.DrawLine(startingPos, stopPoint);

        var hit = Physics2D.OverlapArea(startingPos, stopPoint);

        if (hit != null)
        {
            if (hit.transform.tag == "Car")
            {
                var rb = hit.gameObject.GetComponent<Rigidbody2D>();
                if (Mathf.Abs(rb.velocity.x) < 1f)
                {
                    var vehicle = hit.gameObject.GetComponent<Vehicle>();

                    if (vehicle == null || vehicle.IsDestroyed) return false;

                    vehicle.Ride(gameObject);
                    virtualInput.Fire2 = false;
                    return true;
                }
            }
        }

        return false;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Punch")
        {
            behaviour.Health -= 10;
            GameManager.S.GetDamageEffect();
        }
    }
}