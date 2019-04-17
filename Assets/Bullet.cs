using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject Shooter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet" || other.gameObject.tag == "Money" || other.gameObject.tag == "Gun") return;

        var behaviour = other.GetComponent<Behaviours>();

        if (behaviour != null)
        {
            behaviour.Health -= 10;

            // If Cop is killed by player raise wanted level
            var npc = other.GetComponent<NPC>();
            if (npc != null && behaviour.Health <= 0 && npc.personality == Personality.Cop &&
                Shooter != null && Shooter == GameManager.S.player)
                    GameManager.S.ChasingLevel++;

            if(!behaviour.IsNpc)
                GameManager.S.GetDamageEffect();
        }

        Destroy(gameObject);
    }

}