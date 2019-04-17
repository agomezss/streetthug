using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Personality personality;
    public Behaviours behaviour;

    void Awake()
    {
        behaviour = GetComponent<Behaviours>();

        if (personality == Personality.Regular)
        {
            var random = UnityEngine.Random.Range(1, 4);

            if (random == 1) personality = Personality.Regular;
            else if (random == 2) personality = Personality.RegularAngry;
            else personality = Personality.Thug;
        }

        switch (personality)
        {
            case Personality.Cop:
                behaviour.BaseHorizontalVelocity = 2.2f;
                behaviour.BaseVerticalVelocity = 1.3f;
                behaviour.GunBullets = 1000;
                behaviour.Health = 60;
                behaviour.MaxSpeed = 120f;
                behaviour.Money = UnityEngine.Random.Range(0, 1);
                behaviour.RunningSpeedMultiplicator = 1.6f;
                break;
            case Personality.Thug:
                behaviour.BaseHorizontalVelocity = 2.0f;
                behaviour.BaseVerticalVelocity = 1f;
                behaviour.GunBullets = 500;
                behaviour.Health = 50;
                behaviour.MaxSpeed = 120f;
                behaviour.Money = UnityEngine.Random.Range(0, 5);
                behaviour.RunningSpeedMultiplicator = 1.2f;
                break;
            default:
            case Personality.RegularAngry:
            case Personality.Regular:
                behaviour.BaseHorizontalVelocity = 1.2f;
                behaviour.BaseVerticalVelocity = 0.8f;
                behaviour.Money = UnityEngine.Random.Range(0, 3);
                behaviour.GunBullets = 0;
                behaviour.Health = 30;
                behaviour.MaxSpeed = 100f;
                behaviour.RunningSpeedMultiplicator = 1.5f;
                break;
        }
    }

    void NormalMovement()
    {
        if (behaviour.IsChasing || behaviour.IsRunning) return;

        if (behaviour.IsRiding && behaviour.ApproachingSomething)
        {
            var vehicleRb = behaviour.Vehicle.GetComponent<Rigidbody2D>();
            var velocity = vehicleRb.velocity;
            var targetVelocity = vehicleRb.velocity * 0.5f;
            vehicleRb.velocity = Vector2.Lerp(vehicleRb.velocity, targetVelocity, 2f * Time.deltaTime);
            vehicleRb.angularVelocity = 0f;
            return;
        }

        if (transform.position.y > -1.4f)
        {
            behaviour.MoveRight();
        }
        else
        {
            behaviour.MoveLeft();
        }
    }

    void Update()
    {
        NormalMovement();
        CheckBondaries();
        StopIfNearToSomething();
        IfIsCopPursue();

        if (behaviour.IsChasing)
        {
            Attack();
        }
    }

    private void IfIsCopPursue()
    {
        if (!behaviour.IsChasing && !behaviour.IsRiding &&
            personality == Personality.Cop && GameManager.S.ChasingLevel > 0)
        {
            var player = GameManager.S.player;

            if (player != null)
            {
                behaviour.Pursue(player.transform);
            }
            else
            {
                behaviour.IsChasing = false;
            }
        }
    }

    void StopIfNearToSomething()
    {
        if (behaviour.IsRunning || behaviour.IsChasing) return;

        var factor = behaviour.IsRiding ? 12f : 4f;
        var positionMultiplier = transform.position.y < -1.4f ? -1f : 1f;
        var factorDist = behaviour.IsRiding ? behaviour.Vehicle.GetComponent<Vehicle>().distPad * positionMultiplier : 0.4f * positionMultiplier;
        var factorArea = behaviour.IsRiding ? 0.5f : 0.2f;

        var startingPos = new Vector3(transform.position.x + factorDist, transform.position.y - factorArea, transform.position.z);
        var stoppingPoint = new Vector3(startingPos.x + (transform.right.x * factorDist), startingPos.y + (factorArea * 2), startingPos.z);

        var stopPoint = (stoppingPoint + transform.right * factor);

        //Debug.DrawLine(startingPos, stopPoint);

        var hit = Physics2D.OverlapArea(startingPos, stopPoint);

        if (hit != null)
        {
            if ((hit.transform.tag == "Char" || hit.transform.tag == "Car" || hit.transform.tag == "Player" || hit.transform.tag == "Punch") &&
                 hit.transform.gameObject != gameObject && hit.transform.gameObject != behaviour.Vehicle)
            {
                if (behaviour.IsRiding)
                {
                    behaviour.ApproachingSomething = true;
                    behaviour.ApproachedObject = hit.transform.gameObject;
                }
                else
                {
                    behaviour.ApproachingSomething = true;
                    var behaviourTarget = hit.transform.gameObject.GetComponent<Behaviours>();

                    if (behaviourTarget != null)
                    {
                        behaviour.HorizontalVelocity = Mathf.Clamp(behaviour.HorizontalVelocity * 0.3f, 0f, behaviour.HorizontalVelocity);
                    }
                }
            }
            else
            {
                behaviour.ApproachingSomething = false;
                behaviour.ApproachedObject = null;
            }
        }
        else
        {
            behaviour.ApproachingSomething = false;
            behaviour.ApproachedObject = null;
        }
    }


    void Attack()
    {
        if (personality == Personality.RegularAngry)
        {
            Punch();
        }
        else
        {
            Shoot();
        }

    }

    void Punch()
    {
        if (Mathf.Abs(behaviour.target.position.x - transform.position.x) < 0.5f)
        {
            var random = UnityEngine.Random.Range(1, 4000);

            if (random > 3900)
            {
                behaviour.Punch();
            }

            behaviour.Punch();
        }
    }

    void Shoot()
    {
        var random = UnityEngine.Random.Range(1, 10000);

        if (random > 9900)
        {
            var renderer = GetComponent<Renderer>();

            if (renderer.isVisible)
                behaviour.Shoot();
        }
    }

    void CheckBondaries()
    {
        if (transform.position.x < -8)
            transform.position = new Vector3(186f, transform.position.y, transform.position.z);
        else if (transform.position.x > 186)
            transform.position = new Vector3(-7.5f, transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var HasShooted = other.gameObject.tag == "Bullet";
        var HasPunched = other.gameObject.tag == "Punch";

        if (HasShooted || HasPunched)
        {
            behaviour.Health -= HasPunched ? 10 : 0;

            var target = HasShooted ? other.gameObject.GetComponent<Bullet>().Shooter.transform
                                    : other.gameObject.transform;

            if (this.personality == Personality.Regular)
            {
                behaviour.Run(target);
            }
            else
            {
                if (this.personality == Personality.Cop && GameManager.S.ChasingLevel == 0)
                {
                    GameManager.S.ChasingLevel = 1;
                }
                else
                {
                    var player = other.GetComponent<Player>();
                    if (player != null && behaviour.Health <= 0 && personality == Personality.Cop)
                    {
                        GameManager.S.ChasingLevel++;
                        return;
                    }
                }

                var targetPersonality = target.GetComponent<NPC>();
                if (targetPersonality != null && targetPersonality.personality == Personality.Cop) return;

                behaviour.Pursue(target);
            }
        }
    }
}