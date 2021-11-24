using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    Collider collid;
    SpriteRenderer sprRen;

    public float flSpeed = 2;
    [SerializeField] bool bird = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (LevelManager.scr.state == GameState.lost)
            Destroy(gameObject);

        collid = GetComponent<Collider>();
        sprRen = GetComponentInChildren<SpriteRenderer>();

        yield return null;
        if (bird)
        {
            int rnd = Random.Range(0, 3);
            switch (rnd)
            {
                case 0:
                    transform.Translate(new Vector3(0, 0.5f));
                    break;

                case 1:
                    transform.Translate(new Vector3(0, 1f));
                    break;

                case 2:
                    transform.Translate(new Vector3(0, 1.5f));
                    break;
            }
        }

        yield return new WaitForSeconds(0.25f);
        float factor = 25 / flSpeed;
        if (LevelManager.scr.state == GameState.won)
        {
            collid.enabled = false;
        }
        yield return new WaitForSeconds(factor);
        if (LevelManager.scr.state == GameState.playing)
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // THIS CODE MOVES THE OBSTACLE FORWARD

        transform.Translate(new Vector3(-flSpeed, 0) * Time.deltaTime);

        // THIS CODE IS TO ENSURE THE PLAYER DOESN'T LOSE UNFAIRLY BY JUMPING A BIT TO LATE,
        // BY UNENABLING THE OBSTACLE'S COLLIDER IF IT JUMPS ENOGHLY NEAR

        if (Input.GetButtonDown("Up"))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            PlayerMovement playerScript = player.GetComponent<PlayerMovement>();
            float dist = Vector3.Distance(player.transform.position, transform.position),
                required = 0.9f, range = 0.25f;

            if ((dist > required - range) && (dist < required + range) && (playerScript.blJump) && (!bird))
            {
                collid.enabled = false;
            }
        }
    }

    public void vSortOrder(int num)
    {
        if (!sprRen)
            sprRen = GetComponentInChildren<SpriteRenderer>();

        sprRen.sortingOrder += num;
    }
}
