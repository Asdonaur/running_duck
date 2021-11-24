using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager scr;
    public AudioSource audSrcSE, audSrcBGM;

    AudioClip bgmMusic0, bgmMusic1;

    public string strLanguage = "eng";
    public string[] strIntro, strQuotes, strEnding;

    public float flVolumeMusic;

    public int[] iQuotesProhibited = { 0, 0 };
    int iQProhibitedPosition = 0;
    public int iIntroQ = 0, iIntroQMax = 0;
    public bool blMusicPlaying = false;

    public bool MouseButtonDown()
    {
        return Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (scr)
        {
            Destroy(gameObject);
        }
        else
        {
            scr = this;
            DontDestroyOnLoad(gameObject);

            audSrcSE = GameObject.Find("audSrc_SE").GetComponent<AudioSource>();
            audSrcBGM = GameObject.Find("audSrc_BGM").GetComponent<AudioSource>();

            flVolumeMusic = audSrcBGM.volume;

            if (!(PlayerPrefs.HasKey("rec")))
            {
                BorrarDatos();
            }
            LoadQuotes();

            bgmMusic0 = Resources.Load<AudioClip>("Audio/bgm/bgm_ATJB_INTRO");
            bgmMusic1 = Resources.Load<AudioClip>("Audio/bgm/bgm_ATJB");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    void BorrarDatos()
    {
        PlayerPrefs.SetInt("rec", 0);
    }

    #region Load Objects (UNUSED)
    public void InstantiateParticles(string cual, Vector3 position)
    {
        string path = string.Format("Prefabs/Particles/part_{0}", cual);
        GameObject particulas = Instantiate( Resources.Load<GameObject>(path) );
        particulas.transform.position = position;
    }
    #endregion

    #region Language
    public void LoadQuotes()
    {
        string path = string.Format("Texts/{0}/intro", strLanguage);
        string path1 = string.Format("Texts/{0}/puns", strLanguage);
        string path2 = string.Format("Texts/{0}/ending", strLanguage);

        TextAsset text = Resources.Load(path) as TextAsset;
        TextAsset text1 = Resources.Load(path1) as TextAsset;
        TextAsset text2 = Resources.Load(path2) as TextAsset;

        string guion = text.text;
        string guion1 = text1.text;
        string guion2 = text2.text;

        strIntro = guion.Split('\n');
        strQuotes = guion1.Split('\n');
        strEnding = guion2.Split('\n');

        iIntroQMax = strIntro.Length - 1;

        List<int> list = new List<int>();
        for (int i = 0; i < Mathf.FloorToInt(guion1.Length * 0.80f); i++)
        {
            list.Add(0);
        }
        iQuotesProhibited = list.ToArray();
    }

    public void vUpdateProhibited(int newProhibited)
    {
        iQuotesProhibited[iQProhibitedPosition] = newProhibited;
        iQProhibitedPosition += 1;
        if (iQProhibitedPosition > iQuotesProhibited.Length - 1)
            iQProhibitedPosition = 0;
    }
    #endregion

    #region Sounds y music
    public void PlaySE(AudioClip sonido)
    {
        audSrcSE.PlayOneShot(sonido);
    }

    public void PlayBGM(AudioClip musica)
    {
        audSrcBGM.Stop();
        audSrcBGM.clip = musica;
        audSrcBGM.Play();
    }

    public void StopBGM(bool repentino = true)
    {
        if (repentino)
        {
            audSrcBGM.Stop();
            blMusicPlaying = false;
        }
        else
        {
            StartCoroutine(ienStopBGM());
        }
    }

    IEnumerator ienStopBGM()
    {
        float volumenAntes = audSrcBGM.volume;
        float factor = 0.1f;

        while (audSrcBGM.volume > 0)
        {
            audSrcBGM.volume -= factor;
            yield return new WaitForSeconds(0.05f);
        }
        audSrcBGM.Stop();
        audSrcBGM.volume = volumenAntes;
        blMusicPlaying = false;
    }

    public void vLevelMusic()
    {
        StartCoroutine(ienLevelMusic());
    }

    IEnumerator ienLevelMusic()
    {
        if (!blMusicPlaying)
        {
            blMusicPlaying = true;
            PlayBGM(bgmMusic0);
            yield return new WaitForSecondsRealtime(7.9f);
            PlayBGM(bgmMusic1);
        }
    }

    #endregion

    #region Scenes
    public void LoadScene(string escena = "a")
    {
        string esc = "e";
        if (escena == "a")
        {
            esc = SceneManager.GetActiveScene().name;
        }
        else
        {
            esc = escena;
        }
        SceneManager.LoadScene(esc);
    }
    #endregion
}
