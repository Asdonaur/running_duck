using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum GameState
{
    start, playing, lost, won
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager scr;

    [SerializeField] GameObject objMenuGameOver;
    [SerializeField] TextMeshPro tmpScore, tmpGameOver;
    [SerializeField] Animator animBckg;

    AudioClip seLose;

    string path = "Prefabs/Level/ob_Obstacle";

    [SerializeField] float speedDefault = 5,
        speedActual = 0;

    public GameState state = GameState.playing;

    [SerializeField] int numScore = 0;
    int numRecord = 0;

    bool blCanRestart = false;

    GameObject randomObstacle(int rnd)
    {
        int random = Random.Range(0, rnd);
        return Resources.Load<GameObject>(path + random);
    }

    string IntToScore(int input, int howMany = 4)
    {
        if (input == 0)
        {
            string result = "";
            for (int i = 0; i < howMany; i++)
            {
                result += "0";
            }
            return result;
        }
        else
        {
            int zeros = howMany;
            float big = Mathf.Pow(10, zeros);
            string result = "";

            for (int i = 0; i < howMany; i++)
            {
                zeros -= 1;
                big = Mathf.Pow(10, zeros);
                if (input < big)
                {
                    result += "0";
                }
            }
            result += input;
            return result;
        }
    }
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        scr = this;
        objMenuGameOver.SetActive(false);

        seLose = Resources.Load<AudioClip>("Audio/se/se_Lose");

        yield return null;
        if (GameManager.scr.iIntroQ == GameManager.scr.iIntroQMax)
        {
            GameManager.scr.vLevelMusic();
        }
        numRecord = PlayerPrefs.GetInt("rec", 0);

        StartCoroutine(ienSpawnObstacles());
        StartCoroutine(ienPoints());
        GameManager.scr.audSrcBGM.volume = GameManager.scr.flVolumeMusic;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case GameState.start:
                break;
            case GameState.playing:
                break;
            case GameState.lost:
                if (blCanRestart)
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        // Nothing...
                    }
                    else if (Input.anyKeyDown)
                    {
                        vRestart();
                    }
                }
                break;
            default:
                break;
        }
    }

    public void vLose()
    {
        if (state == GameState.playing)
        {
            vCambiarVelocidad(0);
            state = GameState.lost;
            objMenuGameOver.SetActive(true);

            if (GameManager.scr.iIntroQ > GameManager.scr.iIntroQMax)
            {
                int which = 0;
                bool okay = true;
                int tries = 0;
                do
                {
                    tries++;
                    okay = true;
                    which = Random.Range(0, GameManager.scr.strQuotes.Length);
                    foreach (var item in GameManager.scr.iQuotesProhibited)
                    {
                        if (item == which)
                        {
                            okay = false;
                        }
                    }
                } while ((!okay) && (tries < 10));

                tmpGameOver.text = GameManager.scr.strQuotes[which];
                GameManager.scr.vUpdateProhibited(which);
            }
            else
            {
                int which = GameManager.scr.iIntroQ;
                tmpGameOver.text = GameManager.scr.strIntro[which];
            }

            if (numScore > numRecord)
            {
                PlayerPrefs.SetInt("rec", numScore);
            }

            GameManager.scr.iIntroQ += 1;
            GameManager.scr.PlaySE(seLose);
            StartCoroutine(ienShake());
            GameManager.scr.audSrcBGM.volume = (GameManager.scr.flVolumeMusic / 10) * 6;
            Invoke("vLoseInvoke", 0.2f);
        }
    }

    void vLoseInvoke()
    {
        blCanRestart = true;
    }

    void vRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void vCambiarVelocidad(float vel)
    {
        foreach (var item in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            Obstacle obsScript = item.GetComponent<Obstacle>();
            obsScript.flSpeed = vel;
        }
        animBckg.speed = vel;
    }

    IEnumerator ienSpawnObstacles()
    {
        int numSpawnedObs = 0;
        float speedFactor = 0, speedMax = 20;
        int rnd = 1, rndMax = 4;

        speedActual = speedDefault + speedFactor;
        animBckg.speed = speedActual;

        bool Multiplo()
        {
            return (numSpawnedObs % 6 == 0) && (numSpawnedObs != 0);
        }

        while (state == GameState.playing)
        {
            if (speedFactor < speedMax)
            {
                speedFactor += 0.125f;
                vCambiarVelocidad(speedDefault + speedFactor);
            }

            if (Multiplo())
            {
                if (rnd < rndMax)
                {
                    rnd += 1;
                }
            }

            GameObject obstacle = randomObstacle(rnd);

            if (state == GameState.playing)
            {
                int veces = 1 + Mathf.FloorToInt(Random.Range(0, numSpawnedObs / 6));
                if (veces > 5)
                    veces = 5;

                for (int i = 0; i < veces; i++)
                {
                    bool spawn = ((GameManager.scr.iIntroQ < GameManager.scr.iIntroQMax) ? true : (numSpawnedObs > 0));

                    if (spawn)
                    {
                        GameObject obj = Instantiate(obstacle);
                        Obstacle script = obj.GetComponent<Obstacle>();
                        script.flSpeed = speedDefault + speedFactor;
                        speedActual = script.flSpeed;
                        script.vSortOrder(i);
                    }
                    numSpawnedObs += 1;
                    yield return new WaitForSeconds(0.05f);
                }
            }
            int numFactor = (numSpawnedObs > 100) ? 100 : numSpawnedObs;
            float add = Mathf.Abs(Random.Range(1.05f, 1.95f) - (numFactor * 0.02f));
            add = (add < 0) ? 0 : add;
            float wait = 0.5f + add;

            yield return new WaitForSeconds(wait);
        }
    }

    IEnumerator ienPoints()
    {
        float timelapse = 0.02f;

        while ((state == GameState.playing) && (numScore < 9999))
        {
            numScore += 2;
            tmpScore.text = "HI: " + IntToScore(numRecord) + " / SCORE: " + IntToScore(numScore);

            switch (numScore)
            {
                case 3500:
                    GameManager.scr.vLevelMusic();
                    break;

                default:
                    if (numScore >= 5000)
                    {
                        int num1 = numScore - 5000;
                        float inten = num1 / 1000;
                    }
                    break;
            }

            yield return new WaitForSeconds(timelapse);
        }

        if (numScore >= 9999)
        {
            StartCoroutine(ienFinish());
        }
    }

    IEnumerator ienShake(float intensity = 0.4f, float time = 0.15f)
    {
        Camera mainCam = Camera.main;
        Vector3 v3Pos = mainCam.transform.position;

        float randomPos()
        {
            return Random.Range(-intensity, intensity);
        }

        float timer = 0;
        while (timer < time)
        {
            timer += Time.deltaTime;
            mainCam.transform.position = v3Pos + new Vector3(randomPos(), randomPos());
            yield return null;
        }

        mainCam.transform.position = v3Pos;
    }

    IEnumerator ienFinish()
    {
        state = GameState.won;
        PlayerPrefs.SetInt("rec", 0);

        float f = 0, fm = 10;

        while (f < fm)
        {
            float timelapse = Random.Range(0f, 0.2f);

            // SCREEN SHAKE
            StartCoroutine(ienShake(0.35f, timelapse * 0.9f));

            // CHANGE THE POINTS COUNTER RANDOMLY
            int points = Random.Range(0, 9999),
                points2 = Random.Range(0, 9999);
            string str1 = IntToScore(points),
                str2 = IntToScore(points2);

            tmpScore.text = "ER: " + str1 + " ERROR: " + str2;

            // MUSIC PITCH CHANGING RANDOMLY
            float rndPitch = Random.Range(-0.5f, 0.5f);
            GameManager.scr.audSrcBGM.pitch = 1 + rndPitch;
            GameManager.scr.audSrcSE.pitch = 1 + rndPitch;

            // GENERATE OBSTACLES ERRATICALLY
            GameObject obstacle = randomObstacle(4);
            GameObject obj = Instantiate(obstacle);
            Obstacle script = obj.GetComponent<Obstacle>();
            script.flSpeed = speedDefault * Random.Range(0.5f, 3f);

            // ADD TO THE TIMER AND REPEAT
            f += timelapse;
            yield return new WaitForSeconds(timelapse);
        }
        GameManager.scr.audSrcBGM.pitch = 1;
        GameManager.scr.audSrcSE.pitch = 1;
        GameManager.scr.StopBGM();
        GameManager.scr.LoadScene("ending");
    }
}
