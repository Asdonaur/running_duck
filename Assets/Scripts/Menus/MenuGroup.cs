using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGroup : MonoBehaviour
{
    MenuOption[] options;
    GameObject cursor;

    AudioClip seMove, seSelect;

    int numOptActual = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        seMove = Resources.Load<AudioClip>("Audio/se/se_MoveMenu");
        seSelect = Resources.Load<AudioClip>("Audio/se/se_Select");
    }

    private void OnEnable()
    {
        options = GetComponentsInChildren<MenuOption>();
        cursor = GetComponentInChildren<CapsuleCollider>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Up"))
        {
            vMoveCursor(false);
        }
        else if (Input.GetButtonDown("Down"))
        {
            vMoveCursor(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Nothing happens, but this "else if" is to assure the button
            // detected in the next "else if" is not Escape, since it's the
            // Fullscreen button
        }
        else if (Input.anyKeyDown)
        {
            MenuManager.scr.vButtonAction(options[numOptActual].action);
            options[numOptActual].vBlink();
            GameManager.scr.PlaySE(seSelect);
        }
    }

    private void FixedUpdate()
    {
        Vector3 cursorObj = new Vector3(cursor.transform.position.x, options[numOptActual].transform.position.y);
        cursor.transform.position = Vector3.Lerp(cursor.transform.position, cursorObj, 0.5f);
    }

    public void vMoveCursor(bool down)
    {
        if (options.Length <= 1)
        {
            return;
        }

        GameManager.scr.PlaySE(seMove);

        int factor = (down) ? 1 : -1,
            result = numOptActual + factor,
            maxim = options.Length - 1;

        if (result < 0)
        {
            result = maxim;
        }

        if (result > maxim)
        {
            result = 0;
        }

        numOptActual = result;
    }
}
