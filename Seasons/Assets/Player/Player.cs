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

    float grappleDistance = 7;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed = 6;
    float gravity;
    float velocityXSmoothing;
    float minJumpVelocity;
    public float maxJumpVelocity;

    public Vector3 velocity;

    float stanceSwitchCooldownTimer;
    float stanceSwitchCooldown = .1f;
    public bool canGrapple = true;

    public PlayerController controller;
    public SpriteRenderer sprite;
    [SerializeField] GameObject icePillarPrefab;
    [SerializeField] GameObject fireConePrefab;
    [SerializeField] GameObject windZonePrefab;
    [SerializeField] LineRenderer vine;
    public GameObject vineGrapplePointObject;
    [SerializeField] ParticleSystem smokeParticles;
    [SerializeField] GameObject leavesParticles;
    [SerializeField] GameObject vineParticles;
    ParticleSystem vineGlow;

    [SerializeField] StanceType stance;
    int maxWindZones = 1;
    float vineTimer;

    [SerializeField] AudioClip fireSFX;
    [SerializeField] AudioClip vineSFX;
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip stanceSFX;

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
        vineGlow = vineParticles.GetComponent<ParticleSystem>();
        vineParticles.SetActive(false);
        vine.useWorldSpace = true;
    }


    private void Update()
    {
        if (PauseMenu.gameIsPaused)
            return;
        HandlePlayerMovement();
        LevelManager.singleton.HighlightGrappleObjects(transform.position, grappleDistance);
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
        if (controller.collisions.below)
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
        controller.anim.SetFloat("horizontal", Mathf.Abs(velocity.x));
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            AudioManager.singleton.PlaySoundEffect(jumpSFX, .6f);
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
        if (Input.GetKeyDown(KeyCode.Q))
            stance = StanceType.winter;
        else if (Input.GetKeyDown(KeyCode.W))
            stance = StanceType.spring;
        else if (Input.GetKeyDown(KeyCode.E))
            stance = StanceType.summer;
        else if (Input.GetKeyDown(KeyCode.R))
            stance = StanceType.fall;
        if(stance != prevStance)
        {
            if(prevStance == StanceType.summer)
                AudioManager.singleton.TurnOffLoop();
            AudioManager.singleton.PlaySoundEffect(stanceSFX, .1f);
            if (fireConePrefab.activeInHierarchy)
                fireConePrefab.SetActive(false);
            controller.anim.SetBool("isCastingLoop", false);
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
                    controller.anim.Play("Cast");
                    Vector2 targetPosition = transform.position;
                    float directionX = (sprite.flipX) ? -1f : 1f;
                    targetPosition.x += directionX;
                    targetPosition.y += .2f;
                    //play sprite animation
                    var icePillar = Instantiate(icePillarPrefab, targetPosition, Quaternion.identity);
                    icePillar.GetComponent<IcePillar>().Init(sprite.flipX);
                }
                break;
            case StanceType.spring:
                if (Input.GetButtonDown("Fire1") && canGrapple)
                {
                    controller.anim.Play("Cast");
                    StartCoroutine("Swing");                  
                }
                break;
            case StanceType.summer:
                if(Input.GetButtonDown("Fire1"))
                {
                    controller.anim.SetBool("isCastingLoop", true);
                    if (!fireConePrefab.activeInHierarchy)
                    {
                        AudioManager.singleton.PlayLoopSoundEffect(fireSFX);
                        fireConePrefab.SetActive(true);
                        smokeParticles.emissionRate = 15;
                    }
                }
                else if(Input.GetButtonUp("Fire1"))
                {
                    if (fireConePrefab.activeInHierarchy)
                    {
                        controller.anim.SetBool("isCastingLoop", false);
                        AudioManager.singleton.TurnOffLoop();
                        fireConePrefab.SetActive(false);
                        smokeParticles.emissionRate = 0;
                    }
                }

                if(fireConePrefab.activeInHierarchy)
                {
                    //or somewhere heres
                    Vector2 targetPosition = Vector3.zero;
                    float directionX = (sprite.flipX) ? -1.3f : 1.3f;
                    targetPosition.x += directionX;
                    fireConePrefab.transform.localPosition = targetPosition;
                    fireConePrefab.GetComponent<FireCone>().sprite.flipX = sprite.flipX;
                    smokeParticles.transform.position = fireConePrefab.transform.position;
                }
                break;
            case StanceType.fall:
                if (Input.GetButtonDown("Fire1") && (WindColumn.windColumnCount < maxWindZones))
                {
                    controller.anim.Play("Cast");
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
        GrappleObject vineGrapplePoint = LevelManager.singleton.NextGrappleObject(transform.position, grappleDistance);
        if(vineGrapplePoint != null)
        {
            canGrapple = false;
            float distance = Vector3.Distance(vineGrapplePoint.transform.position, transform.position);
            vineGrapplePointObject = vineGrapplePoint.gameObject;
            AudioManager.singleton.PlaySoundEffect(vineSFX, .4f);
            if (distance < grappleDistance)
            {
                vine.SetPosition(0, transform.position);
                vine.SetPosition(1, vineGrapplePointObject.transform.position);
                vine.enabled = true;

                Vector3 midPoint = (vineGrapplePointObject.transform.position + transform.position) / 2f;
                vineParticles.transform.position = midPoint;

                var particleShape = vineGlow.shape;
                particleShape.scale = new Vector3(distance, particleShape.scale.y, particleShape.scale.z);

                vineParticles.transform.LookAt(transform.position);
                Vector3 targetRot = vineParticles.transform.eulerAngles;
                targetRot.z = (vineGrapplePointObject.transform.position.x - transform.position.x >= 0) ? targetRot.x : -targetRot.x;
                targetRot.x = targetRot.y = 0;
                vineParticles.transform.eulerAngles = targetRot;

                vineParticles.SetActive(true);
                controller.freeze = true;

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
                controller.collisions.below = false;
                distance = Vector3.Distance(vineGrapplePointObject.transform.position, transform.position);

                if (distance < 1f)
                {
                    canGrapple = true;
                    //velocity = Vector3.zero;
                    vineGrapplePoint.canGrapple = false;
                    vineGrapplePoint.timer = .75f;
                    Debug.Log("Break grapple");
                    vineGrapplePointObject = null;
                    vine.enabled = false;
                    vineParticles.SetActive(false);
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
                if (vineGrapplePointObject != null && ((controller.collisions.above || controller.collisions.right || controller.collisions.left) || distance > grappleDistance || (controller.collisions.below && distance < 3)))
                {
                    vineGrapplePointObject = null;
                    vine.enabled = false;
                    canGrapple = true;
                    vineGrapplePoint.canGrapple = false;
                    vineGrapplePoint.timer = .75f;
                    vineParticles.SetActive(false);
                    //velocity = Vector3.zero;
                }
                yield return null;
            }
        }
        else
        {
            vineTimer = .3f;
            AudioManager.singleton.PlaySoundEffect(vineSFX, .4f);
            while (vineTimer > 0)
            {
                canGrapple = false;
                vine.enabled = true;
                vine.SetPosition(0, transform.position);
                Vector3 targetPos;
                if (sprite.flipX)
                    targetPos = transform.position - Vector3.right * 3;
                else
                    targetPos = transform.position + Vector3.right * 3;
                vine.SetPosition(1, targetPos);
                float distance = Vector3.Distance(targetPos, transform.position);
                Vector3 midPoint = (targetPos + transform.position) / 2f;
                vineParticles.transform.position = midPoint;
                var particleShape = vineGlow.shape;
                particleShape.scale = new Vector3(distance, particleShape.scale.y, particleShape.scale.z);

                vineParticles.transform.LookAt(transform.position);
                Vector3 targetRot = vineParticles.transform.eulerAngles;
                targetRot.z = (targetPos.x - transform.position.x >= 0) ? targetRot.x : -targetRot.x;
                targetRot.x = targetRot.y = 0;
                vineParticles.transform.eulerAngles = targetRot;

                vineParticles.SetActive(true);
                vineTimer -= Time.deltaTime;
                yield return null;
            }
            if (vine.enabled)
            {
                canGrapple = true;
                vineParticles.SetActive(false);
                vine.enabled = false;
            }
        }   
    }

    public void ResetAbilities()
    {
        if (fireConePrefab.activeInHierarchy)
        {
            controller.anim.SetBool("isCastingLoop", false);
            fireConePrefab.SetActive(false);
            smokeParticles.emissionRate = 0;
        }
        vine.enabled = false;
        canGrapple = true;
        vineParticles.SetActive(false);
    }
}
