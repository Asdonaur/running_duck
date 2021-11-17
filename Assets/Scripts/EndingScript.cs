using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndingScript : MonoBehaviour
{
    [SerializeField] GameObject objDialogo, objCredits;
    [SerializeField] TextMeshPro tmpQuote;

    AudioClip seJump;

    string[] strCutseneQuotes;
    bool free = true;
    int numQuoteMax;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        GameManager.scr.PlaySE(Resources.Load<AudioClip>("Audio/se/se_TvOff"));
        seJump = Resources.Load<AudioClip>("Audio/se/se_Jump");
        yield return new WaitForSeconds(0.1f);
        strCutseneQuotes = GameManager.scr.strEnding;
        numQuoteMax = strCutseneQuotes.Length - 1;
        vHideText();
        yield return new WaitForSeconds(3f);
        StartCoroutine(ienCutsene());
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.anyKeyDown) && (!free))
        {
            free = true;
        }
    }

    void vSay(string text)
    {
        GameManager.scr.PlaySE(seJump);
        objDialogo.SetActive(true);
        tmpQuote.text = text;
    }

    void vCredits()
    {
        GameManager.scr.PlaySE(seJump);
        objCredits.SetActive(true);
    }

    void vHideText()
    {
        tmpQuote.text = "";
        objDialogo.SetActive(false);
        objCredits.SetActive(false);
    }

    IEnumerator ienCutsene()
    {
        for (int i = 0; i <= numQuoteMax; i++)
        {
            free = false;
            switch (strCutseneQuotes[i].Substring(0, 4))
            {
                case "/cre":
                    vCredits();
                    break;

                case "/end":
                    Application.Quit();
                    break;

                default:
                    vSay(strCutseneQuotes[i]);
                    break;
            }

            while (!free)
            {
                yield return null;
            }
            vHideText();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
