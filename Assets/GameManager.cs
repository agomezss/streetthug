using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager S;
    public int totalNPCPerRow = 5;
    public int totalCarPerRow = 2;
    public GameObject player;
    public GameObject[] cars = new GameObject[5];
    public GameObject[] person = new GameObject[4];
    public GameObject objParent;
    public float globalVehicleSpeedMultiplicator = 1f;

    [Range(0f, 5f)]
    public int ChasingLevel;

    public GameObject GameOverText;

    public GameObject PauseText;
    public bool IsPaused = false;
    public float LastPausedTime;

    public UnityEngine.Camera cam;

    public float lastTimeDamageEffect;


    public AudioClip sfxBackground;
    public AudioClip sfxLife;
    public AudioClip sfxMoney;
    public AudioClip sfxShoot;
    public AudioClip sfxPunch;
    public AudioClip sfxScream;
    public AudioClip sfxExplosion;
    public AudioClip sfxGameOver;
    public AudioClip sfxCarEngine;
    public AudioClip sfxCarCrash;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitialSpawn();
        RestoreDamageEffect();

        PauseText.SetActive(false);
        GameOverText.SetActive(false);
        ChasingLevel = 0;

        var audioSource = GetComponent<AudioSource>();
        audioSource.clip = GameManager.S.sfxBackground;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void GameOver()
    {
        var audioSource = GetComponent<AudioSource>();
        audioSource.clip = GameManager.S.sfxGameOver;
        audioSource.loop = false;
        audioSource.Play();

        GameOverText.SetActive(true);
        Invoke("Reload", 5f);
    }

    public void Reload()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void PauseUnpause()
    {
        if (IsPaused) UnPause();
        else Pause();
    }

    private void Pause()
    {
        if (Time.realtimeSinceStartup - LastPausedTime < 0.5f) return;
        LastPausedTime = Time.realtimeSinceStartup;
        PauseText.SetActive(true);
        IsPaused = true;
        Time.timeScale = 0f;
    }

    public void UnPause()
    {
        if (Time.realtimeSinceStartup - LastPausedTime < 0.5f) return;
        LastPausedTime = Time.realtimeSinceStartup;
        PauseText.SetActive(false);
        IsPaused = false;
        Time.timeScale = 1f;
    }

    public void GetDamageEffect()
    {
        if (Time.unscaledTime - lastTimeDamageEffect < 0.5f) return;
        lastTimeDamageEffect = Time.unscaledTime;

        cam.backgroundColor = Color.red;
        cam.cullingMask = 0;

        Invoke("RestoreDamageEffect", 0.05f);

    }

    private void RestoreDamageEffect()
    {
        cam.cullingMask = -1;
        cam.backgroundColor = Color.gray;
    }

    void InitialSpawn()
    {
        // Walking NPCs
        for (int i = 0; i < totalNPCPerRow; i++)
        {
            SpawnNPCUp(i);
        }

        for (int j = 0; j < totalNPCPerRow; j++)
        {
            SpawnNPCDown(j);
        }

        // Cars
        for (int k = 0; k < totalCarPerRow; k++)
        {
            SpawnCarUp(k);
        }

        for (int l = 0; l < totalCarPerRow; l++)
        {
            SpawnCarDown(l);
        }
    }

    GameObject SpawnNPC(float distancex, float distancey)
    {
        var random = Random.Range(0, person.Length - 1);
        var choosed = person[random];
        var transformP = new Vector3(distancex, distancey, 0f);

        var obj = Instantiate(choosed, transformP, Quaternion.identity);
        obj.transform.parent = objParent.transform;
        return obj;
    }

    public void SpawnNPC()
    {
        var random = Random.Range(1, 2);
        var xRange = Random.Range(0, 180);
        if (random == 1) SpawnNPCUp(xRange);
        else SpawnNPCDown(xRange);
    }

    void SpawnNPCUp(int distance)
    {
        var random = Random.Range(0, person.Length - 1);
        var choosed = person[random];
        var random2 = Random.Range(1, 2);

        var transformP = new Vector3(distance * 20 * random2, 1.5f, 0f);


        var obj = Instantiate(choosed, transformP, Quaternion.identity);
        obj.transform.parent = objParent.transform;
    }

    void SpawnNPCDown(int distance)
    {
        var random = Random.Range(0, person.Length - 1);
        var choosed = person[random];
        var random2 = Random.Range(1, 2);

        var transformP = new Vector3(distance * 20 * random2, -4.8f, 0f);

        var obj = Instantiate(choosed, transformP, Quaternion.identity);
        obj.transform.parent = objParent.transform;
    }

    public void SpawnCar(float yPosition)
    {
        var random = Random.Range(0, cars.Length - 1);
        var choosed = cars[random];
        var xPosition = -3.0f;

        var transformP = new Vector3(xPosition, yPosition, 0f);

        //Debug.Log($"Car Respwaned At: x:{xPosition}, y:{yPosition}");

        var npc = SpawnNPC(xPosition, -1f * yPosition - 2f);

        var car = Instantiate(choosed, transformP, Quaternion.identity);
        car.GetComponent<Vehicle>().Ride(npc);
        car.transform.parent = objParent.transform;
    }

    void SpawnCarUp(int distance)
    {
        var random = Random.Range(0, cars.Length - 1);
        var choosed = cars[random];
        var random2 = Random.Range(1, 2);
        var position = distance * 25 * random2;

        var transformP = new Vector3(position, -0.4f, 0f);

        var npc = SpawnNPC(position, -0.4f - 1f);

        var car = Instantiate(choosed, transformP, Quaternion.identity);
        car.GetComponent<Vehicle>().Ride(npc);
        car.transform.parent = objParent.transform;
    }

    void SpawnCarDown(int distance)
    {
        var random = Random.Range(0, cars.Length - 1);
        var choosed = cars[random];
        var random2 = Random.Range(1, 2);
        var position = distance * 25 * random2;
        var transformP = new Vector3(position, -3.0f, 0f);

        var npc = SpawnNPC(position, -3.2f - 1f);

        var car = Instantiate(choosed, transformP, Quaternion.identity);
        car.GetComponent<Vehicle>().Ride(npc);
        car.transform.parent = objParent.transform;
    }

    // Used by UI Button
    public void QuitGame()
    {
        Application.Quit();
    }
}