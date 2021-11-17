using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager scr;
    GameObject[] optGroups;

    MenuGroup menuAct;
    MenuOption[] options;

    string strMenuAct = "";
    bool blTransition = false;
    
    // Start is called before the first frame update
    void Start()
    {
        scr = this;
        optGroups = GameObject.FindGameObjectsWithTag("OptGroup");
        vChangeGroup("start");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void vChangeGroup(string GROUP_NAME)
    {
        blTransition = true;
        foreach (var item in optGroups)
        {
            item.SetActive(false);
        }
        strMenuAct = GROUP_NAME;

        string strFunc = (GROUP_NAME == "play") ? "vPlay" : "vShowGroup";
        Invoke(strFunc, 0.5f);
    }

    void vPlay()
    {
        SceneManager.LoadScene("SampleScene");
    }

    void vShowGroup()
    {
        foreach (var item in optGroups)
        {
            if (item.name == strMenuAct)
            {
                item.SetActive(true);
                menuAct = item.GetComponent<MenuGroup>();
            }
            else
            {
                item.SetActive(false);
            }
        }
        blTransition = false;
    }

    public void vButtonAction(string action)
    {
        StartCoroutine(ienButtonAct(action));
    }

    IEnumerator ienButtonAct(string action)
    {
        yield return new WaitForSeconds(0.5f);
        switch (action)
        {
            case "fullscreen":
                Screen.fullScreen = !Screen.fullScreen;
                break;

            case "quit":
                Application.Quit();
                break;

            default:
                vChangeGroup(action);
                break;
        }
    }
}
