﻿using UnityEngine;

public enum StanceType
{
    neutral, winter, spring, summer, fall
}

public class Player : MonoBehaviour
{
    [Header("Jump Stats")]
    public float maxJumpHeight = 4;
    public float minJumpHeight = .25f;
    public float timeToJumpApex = .4f;

    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed = 6;
    float gravity;
    public float maxJumpVelocity;
    float minJumpVelocity;
    public Vector3 velocity;
    float velocityXSmoothing;

    float stanceSwitchCooldownTimer;
    float stanceSwitchCooldown = 3f;

    public PlayerController controller;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] GameObject icePillarPrefab;
    [SerializeField] GameObject fireConePrefab;
    [SerializeField] GameObject windZonePrefab;

    [SerializeField] StanceType stance;
    int maxWindZones = 2;

    private void Start()
    {
        controller = GetComponent<PlayerController>();
        gravity = -(2 * maxJumpHeight) / (timeToJumpApex * timeToJumpApex);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);     
    }


    private void Update()
    {
        HandlePlayerMovement();
        HandleStanceSwitch();
        HandleSpellCasting();
        HandleCooldowns();
    }

    private void HandleCooldowns()
    {
        stanceSwitchCooldownTimer -= Time.deltaTime;
    }

    void HandlePlayerMovement()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            if(WindColumn.windColumnCount > 0)
                WindColumn.DecrementWindCount();
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (input.x > 0)
            sprite.flipX = false;
        else if (input.x < 0)
            sprite.flipX = true;

        HandleJump();

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
            velocity.y = maxJumpVelocity;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (velocity.y > minJumpVelocity)
                velocity.y = minJumpVelocity;
        }
    }

    void HandleStanceSwitch()
    {
        if (stanceSwitchCooldownTimer > 0)
            return;
        StanceType prevStance = stance;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            stance = StanceType.winter;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            stance = StanceType.spring;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            stance = StanceType.summer;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            stance = StanceType.fall;
        if(stance != prevStance)
        {
            UpdateStance(stance);
            stanceSwitchCooldownTimer = stanceSwitchCooldown;
        }
    }

    void HandleSpellCasting()
    {
        switch (stance)
        {
            case StanceType.neutral:
                break;
            case StanceType.winter:
                if (Input.GetButtonDown("Fire1"))
                {
                    Vector2 targetPosition = transform.position;
                    float directionX = (sprite.flipX) ? -1f : 1f;
                    targetPosition.x += directionX;
                    var icePillar = Instantiate(icePillarPrefab, targetPosition, Quaternion.identity);
                    icePillar.GetComponent<IcePillar>().Init(sprite.flipX);
                }
                break;
            case StanceType.spring:
                break;
            case StanceType.summer:
                if(Input.GetButtonDown("Fire1"))
                {                  
                    if (!fireConePrefab.activeInHierarchy)
                        fireConePrefab.SetActive(true);              
                }
                else if(Input.GetButtonUp("Fire1"))
                {
                    if (fireConePrefab.activeInHierarchy)
                        fireConePrefab.SetActive(false);
                }

                if(fireConePrefab.activeInHierarchy)
                {
                    Vector2 targetPosition = Vector3.zero;
                    float directionX = (sprite.flipX) ? -1f : 1f;
                    targetPosition.x += directionX;
                    fireConePrefab.transform.localPosition = targetPosition;
                    fireConePrefab.GetComponent<FireCone>().sprite.flipX = sprite.flipX;
                }
                break;
            case StanceType.fall:
                if (Input.GetButtonDown("Fire1") && (WindColumn.windColumnCount < maxWindZones))
                {
                    Vector2 targetPosition = transform.position;
                    targetPosition.y -= .38f;
                    Instantiate(windZonePrefab, targetPosition, Quaternion.identity);
                    WindColumn.windColumnCount++;
                }
                break;
        }
    }

    void UpdateStance(StanceType stance)
    {
        switch (stance)
        {
            case StanceType.winter:
                sprite.color = Color.blue;
                break;
            case StanceType.spring:
                sprite.color = Color.green;
                break;
            case StanceType.summer:
                sprite.color = Color.red;
                break;
            case StanceType.fall:
                sprite.color = Color.yellow;
                break;
        }
    }
}
