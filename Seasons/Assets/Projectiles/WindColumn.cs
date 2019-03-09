﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindColumn : MonoBehaviour
{
    public static int windColumnCount;
    [SerializeField] float durationTimer;
    static List<WindColumn> windPool = new List<WindColumn>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.velocity.y = .75f * player.maxJumpVelocity;
            player.controller.Move(player.velocity * Time.deltaTime, player.controller.playerInput);
        }
    }

    private void Start()
    {
        durationTimer = .2f;
        windPool.Add(this);
    }

    public static void DecrementWindCount()
    {
        windColumnCount = 0;
    }

    private void Update()
    {
        durationTimer -= Time.deltaTime;
        if (durationTimer < 0)
        {
            windPool.Remove(this);
            Destroy(this.gameObject);
        }
    }
}
