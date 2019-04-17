using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WantedBar : MonoBehaviour
{
    public GameObject[] stars;

    void Start()
    {
        foreach (var star in stars)
        {
            star.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var star in stars)
        {
            star.SetActive(false);
        }

        if (GameManager.S.ChasingLevel > 0)
        {
            if (GameManager.S.ChasingLevel > 5) GameManager.S.ChasingLevel = 5;

            for (int i = 0; i <= (GameManager.S.ChasingLevel - 1); i++)
            {
                stars[i].SetActive(true);
            }
        }
    }
}
