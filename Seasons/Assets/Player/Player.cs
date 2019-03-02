using System.Collections;
using UnityEngine;

public enum StanceType
{
    neutral, winter, spring, summer, fall
}

public class Player : MonoBehaviour
{
    [Header("Jump Stats")]
    [SerializeField] float maxJumpHeight = 4;
    [SerializeField] float minJumpHeight = .25f;
    [SerializeField] float timeToJumpApex = .4f;

    float grappleDistance = 4;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed = 6;
    float gravity;
    float velocityXSmoothing;
    float minJumpVelocity;
    public float maxJumpVelocity;

    public Vector3 velocity;

    float stanceSwitchCooldownTimer;
    float stanceSwitchCooldown = .5f;
    bool canGrapple = true;

    public PlayerController controller;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] GameObject icePillarPrefab;
    [SerializeField] GameObject fireConePrefab;
    [SerializeField] GameObject windZonePrefab;
    [SerializeField] LineRenderer vine;
    public GameObject vineGrapplePointObject;
    [SerializeField] ParticleSystem smokeParticles;
    [SerializeField] GameObject leavesParticles;

    [SerializeField] StanceType stance;
    int maxWindZones = 1;
    float vineTimer;

    public static Player singleton;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        controller = GetComponent<PlayerController>();
        gravity = -(2 * maxJumpHeight) / (timeToJumpApex * timeToJumpApex);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        vine.enabled = false;
        vine.useWorldSpace = true;
    }


    private void Update()
    {
        if (PauseMenu.gameIsPaused)
            return;
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
        controller.Move(velocity * Time.deltaTime, input);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
        	//GetComponent<SpriteRenderer>.sprite = PlayerJump;
            velocity.y = maxJumpVelocity;
        }

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
            if (fireConePrefab.activeInHierarchy)
                fireConePrefab.SetActive(false);
            smokeParticles.emissionRate = 0;
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
                    //play sprite animation
                    var icePillar = Instantiate(icePillarPrefab, targetPosition, Quaternion.identity);
                    icePillar.GetComponent<IcePillar>().Init(sprite.flipX);
                }
                break;
            case StanceType.spring:
                if (Input.GetButtonDown("Fire1") && canGrapple)
                {
                    StartCoroutine("Swing");                  
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    if (vineTimer > 0)
                        return;
                    canGrapple = true;
                    velocity = Vector3.zero;
                    vine.enabled = false;
                    vineTimer = 0;
                    vineGrapplePointObject = null;
                }
                break;
            case StanceType.summer:
                if(Input.GetButtonDown("Fire1"))
                {
                    //play loop animation here
                    if (!fireConePrefab.activeInHierarchy)
                    {
                        fireConePrefab.SetActive(true);
                        smokeParticles.emissionRate = 15;
                    }
                }
                else if(Input.GetButtonUp("Fire1"))
                {
                    if (fireConePrefab.activeInHierarchy)
                    {
                        fireConePrefab.SetActive(false);
                        smokeParticles.emissionRate = 0;
                    }
                }

                if(fireConePrefab.activeInHierarchy)
                {
                    //or somewhere heres
                    Vector2 targetPosition = Vector3.zero;
                    float directionX = (sprite.flipX) ? -1f : 1f;
                    targetPosition.x += directionX;
                    fireConePrefab.transform.localPosition = targetPosition;
                    fireConePrefab.GetComponent<FireCone>().sprite.flipX = sprite.flipX;
                    smokeParticles.transform.position = fireConePrefab.transform.position;
                }
                break;
            case StanceType.fall:
                if (Input.GetButtonDown("Fire1") && (WindColumn.windColumnCount < maxWindZones))
                {
                    Vector2 targetPosition = transform.position;
                    Instantiate(windZonePrefab, targetPosition, Quaternion.identity);
                    Vector3 targetParticlePos = targetPosition;
                    targetParticlePos.y += .05f;
                    targetParticlePos.z += .369f;
                    GameObject particles = Instantiate(leavesParticles, targetParticlePos, Quaternion.identity);
                    Destroy(particles, 3f);
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
                sprite.color = Color.cyan;
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

    IEnumerator Swing()
    {
        Physics2D.SyncTransforms();
        RaycastHit2D vineGrapplePoint = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, controller.collisionMask);
        if (vineGrapplePoint.collider != null)
        {
            float distance = Vector3.Distance(vineGrapplePoint.collider.transform.position, transform.position);
            vineGrapplePointObject = vineGrapplePoint.collider.gameObject;
            if (distance < grappleDistance)
            {
                vine.SetPosition(0, transform.position);
                vine.SetPosition(1, vineGrapplePointObject.transform.position);
                vine.enabled = true;
                controller.freeze = true;
                //Play animation or sprite swap here
                Vector3 hit3d = vineGrapplePointObject.transform.position;
                Vector3 dir = hit3d - transform.position;
                if (dir.normalized.x < 0)
                    sprite.flipX = true;
                else
                    sprite.flipX = false;
                yield return new WaitForSeconds(.1f);
            }

            controller.freeze = false;
            while (vineGrapplePointObject != null)
            {
                distance = Vector3.Distance(vineGrapplePointObject.transform.position, transform.position);

                if (distance < .1f)
                {
                    canGrapple = false;
                    velocity = Vector3.zero;
                    Debug.Log("Break grapple");
                    vineGrapplePointObject = null;
                    vine.enabled = false;
                }
                else if (distance < grappleDistance)
                {
                    vine.SetPosition(0, transform.position);
                    vine.SetPosition(1, vineGrapplePointObject.transform.position);
                    vine.enabled = true;

                    Vector3 hit3d = vineGrapplePointObject.transform.position;
                    Vector3 dir = hit3d - transform.position;
                    velocity = dir.normalized * 15f;
                }
                if (vineGrapplePointObject != null && ((controller.collisions.above || controller.collisions.right || controller.collisions.left) || distance > grappleDistance))
                {
                    vineGrapplePointObject = null;
                    vine.enabled = false;
                    canGrapple = false;
                    velocity = Vector3.zero;
                }
                yield return null;
            }
        }
        else
        {
            vineTimer = .3f;
            while (vineTimer > 0)
            {
                vine.enabled = true;
                vine.SetPosition(0, transform.position);
                if (sprite.flipX)
                    vine.SetPosition(1, transform.position - Vector3.right * 3);
                else
                    vine.SetPosition(1, transform.position + Vector3.right * 3);
                vineTimer -= Time.deltaTime;
                yield return null;
            }
            if (vine.enabled)
                vine.enabled = false;
        }   
    }
}
