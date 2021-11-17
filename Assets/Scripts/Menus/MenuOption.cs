using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuOption : MonoBehaviour
{
    TextMeshPro tmp;
    public string action;

    private void OnEnable()
    {
        tmp = GetComponent<TextMeshPro>();
    }

    public void vBlink()
    {
        StartCoroutine(ienBlink());
    }

    IEnumerator ienBlink()
    {
        Color defColor = tmp.color,
            transColor = new Color(0, 0, 0, 0);

        for (int i = 0; i < 6; i++)
        {
            tmp.color = (tmp.color == defColor) ? transColor : defColor;
            yield return new WaitForSecondsRealtime(0.05f);
        }
        tmp.color = defColor;
    }
}
