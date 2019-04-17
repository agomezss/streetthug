using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            var audioSource = other.gameObject.GetComponent<AudioSource>();
            var behaviour = other.gameObject.GetComponent<Behaviours>();

            if (gameObject.tag == "Money")
            {
                behaviour.Money += 50;
                audioSource.clip = GameManager.S.sfxMoney;
                audioSource.Play();
            }
            else if (gameObject.tag == "Gun")
            {
                behaviour.GunBullets += 20;
                audioSource.clip = GameManager.S.sfxLife;
                audioSource.Play();
            }
            else if (gameObject.tag == "Medicine")
            {
                behaviour.Health = 100;
                audioSource.clip =  GameManager.S.sfxLife;
                audioSource.Play();
            }

            Destroy(gameObject);
        }
    }
}