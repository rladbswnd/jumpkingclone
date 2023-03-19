using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform player;
    public float offset = 2.5f;
    void Start()
    {
        if (!player)
            player = FindObjectOfType<PlayerController>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.position.y - transform.position.y >= offset)
        {
            // transform.position = new Vector3(transform.position.x, transform.position.y + (offset * 2), transform.position.z);
            transform.Translate(Vector2.up * offset * 2);
        }
        else if(transform.position.y - player.position.y >= offset)
        {
            transform.Translate(Vector2.down * offset * 2);
        }
    }
}
