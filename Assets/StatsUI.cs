using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    public GameObject Player;
    private Behaviours playerValues;

    public Text lifeIndicator;
    public Text bulletsIndicator;
    public Text moneyIndicator;

    // Start is called before the first frame update
    void Start()
    {
        playerValues = Player.GetComponent<Behaviours>();
    }

    // Update is called once per frame
    void Update()
    {
        lifeIndicator.text = playerValues.Health.ToString();
        bulletsIndicator.text = playerValues.GunBullets.ToString();
        moneyIndicator.text = playerValues.Money.ToString();
    }
}
