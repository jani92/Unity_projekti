using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : PlatformerCharacter
{
    public float moveAcceleration = 80f;
    public float moveSpeed = 15f;
    public float runSpeed = 20f;
    public float jumpVelocity = 20f;
    public float jumpDownwardAcceleration = 150f;

    public ParticleSystem dustEmitter;

    public AudioSource sfxAudio;
    public AudioSource wallHangAudio;
    public AudioSource anchorReelAudio;
    public AudioSource rushingWaterAudio;
    public AudioClip[] footsteps;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip wallHitSound;
    public AudioClip wallJumpSound;
    public AudioClip anchorShotSound;
    public AudioClip stompSound;
    public AudioClip deathSound;

    Transform character;
    Transform cam;
    AnchorAbility anchorAbility;
    Vector2 startPos;
    Animator anim;
    float jumpState;
    bool wallJump;
    bool inWallContact;

    float playerProfileAngle = 30f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        character = transform.GetChild(0);
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        anchorAbility = GetComponent<AnchorAbility>();
        anim = GetComponentInChildren<Animator>();
        startPos = new Vector2(transform.position.x, transform.position.y);
        ResetToStartPosition();

        Cursor.visible = false;
    }

    public void ResetToStartPosition()
    {
        jumpState = 0f;
        dead = false;
        jumped = false;
        holdingJump = false;
        groundedTimer = 0f;
        //transform.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.white;
        Teleport(startPos);
        //cam.position = CalculateCameraPos();
    }

    bool dead = false;
    float deathFreezeTimer = 0f;
    protected override void Die()
    {
        if (dead)
            return;
        dead = true;
        deathFreezeTimer = 0.5f;
        //transform.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
        //anim.enabled = false;
        anim.Play("Death");
        Time.timeScale = 0.001f;
        ResetAudio();
        PlayDeathSound();
    }

    protected override void Finish()
    {
        if (dead)
            return;
        dead = true;
        deathFreezeTimer = 0.5f;
        Time.timeScale = 0.001f;
    }

    //protected override void Stomp(Transform enemy)
    //{
    //    if (anchorAbility.AnchoredObject() != null && anchorAbility.AnchoredObject() == enemy)
    //        anchorAbility.ReleaseAnchor();
    //    Destroy(enemy.gameObject);
    //    velocity = new Vector2(velocity.x, jumpVelocity * 0.75f);
    //}

    float input;
    bool runInput;
    Vector2 pointDir = new Vector2(1f, 0f);
    protected override void Update()
    {
        //kamera
        UpdateCamera(Time.unscaledDeltaTime * 10f);

        if (dead)
        {
            deathFreezeTimer = Mathf.Max(0f, deathFreezeTimer - Time.unscaledDeltaTime);
            anim.Update(Time.unscaledDeltaTime);
            if (deathFreezeTimer == 0f)
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            return;
        }

        //kontrollit
        input = Input.GetAxisRaw("Horizontal");
        input = (Mathf.Abs(input) >= 0.01f ? input = Mathf.Sign(input) : 0f);
        runInput = false;// Input.GetButton("Run");

        if (groundedTimer == 0f && (wallHitLeft || wallHitRight))
            pointDir = new Vector2(wallHitLeft ? 1f : -1f, 0f);
        else if (input != 0f)
            pointDir = new Vector2(input, 0f);

        if (grounded)
        {
            if ((input == 0f || (input > 0f) != (velocity.x > 0f)) && !anchorAbility.Attached())
                velocity.x = 0f;

            if (groundedTimer == 0f)
            {
                PlayLandSound();
                EmitDust(30, 2f);
            }
            groundedTimer = nonGroundedJumpTime;
        }
        groundedTimer = Mathf.Max(0f, groundedTimer - Time.deltaTime);

        if (!jumped && !holdingJump && (groundedTimer > 0f || wallHitLeft || wallHitRight/* || anchorAbility.Attached()*/) && Input.GetButtonDown("Jump"))
        {
            jumped = true;
            holdingJump = true;
            //if (anchorAbility.Attached())
            //    velocity = new Vector2(anchorAbility.AnchorDir().x * 8.5f, jumpVelocity);
            if (groundedTimer > 0f)
            {
                velocity = new Vector2(velocity.x, jumpVelocity);
                PlayJumpSound();
                EmitDust(20, 2f);
            }
            else if (wallHitLeft)
            {
                velocity = new Vector2(1f, 1f) * jumpVelocity;
                PlayWallJumpSound();
                EmitDust(20, 4f);
            }
            else if (wallHitRight)
            {
                velocity = new Vector2(-1f, 1f) * jumpVelocity;
                PlayWallJumpSound();
                EmitDust(20, 4f);
            }
            groundedTimer = 0f;
            if (anchorAbility.Attached())
                anchorAbility.ReleaseAnchor();
        }
        else if (Input.GetButtonUp("Jump"))
        {
            holdingJump = false;
        }

        if (groundedTimer == 0f && !holdingJump && Input.GetButtonDown("Jump") && !anchorAbility.Attached())
        {
            anchorAbility.ShootAutoAimAnchor(position, 10f);
            if (anchorAbility.Attached())
            {
                velocity = Vector2.zero;
                jumped = false;

                if (anchorAbility.AnchoredObjectType() == AnchorAbility.ObjectType.enemy)
                {
                    foreach (Collider coll in anchorAbility.AnchoredObject().GetComponentsInChildren<Collider>())
                        coll.enabled = false;

                    //LeftRight enemy = anchorAbility.AnchoredObject().GetComponent<LeftRight>();
                    //if (enemy != null)
                    //{
                    //    enemy.GetComponent<Collider>().enabled = false;
                    //    enemy.enabled = false;
                    //    enemy.transform.GetChild(0).gameObject.SetActive(false);
                    //}
                } else
                {
                    MovablePlatform platform = anchorAbility.AnchoredObject().GetComponent<MovablePlatform>();
                    platform.Move(new Vector2(-anchorAbility.AnchorDir().x > 0f ? 2f : -2f, 0f), 0.5f);
                }
                PlayAnchorShotSound();
            }
        }
        if (anchorAbility.Attached())
        {
            velocity = Vector2.zero;
            if (anchorAbility.AnchoredObjectType() == AnchorAbility.ObjectType.enemy)
            {
                anchorAbility.Reel();
                position = anchorAbility.shooterReelPos;
                anchorAbility.AnchoredObject().position = new Vector3(anchorAbility.targetReelPos.x, anchorAbility.targetReelPos.y, 0f);
                if (Vector2.Distance(anchorAbility.shooterReelPos, anchorAbility.targetReelPos) < 1f)
                {
                    Destroy(anchorAbility.AnchoredObject().gameObject);
                    anchorAbility.ReleaseAnchor();
                    velocity = new Vector2(0f, jumpVelocity * 1f);
                    PlayStompSound();
                    EmitDust(30, 3f);
                }
            }
            else if (anchorAbility.ShotTimer() > 0.6f)
            {
                velocity = anchorAbility.AnchorDir() * jumpVelocity * 0.5f + Vector2.up * jumpVelocity * 0.5f;
                anchorAbility.ReleaseAnchor();
            }
        }

        //objektin sijainnin päivitys
        base.Update();

        //hahmon rotaatio
        character.rotation = Quaternion.Euler(0f, 90f + (pointDir.x > 0f ? 0f + playerProfileAngle : 180f - playerProfileAngle), 0f);

        //animaatio
        UpdateAnim();

        //äänet
        UpdateSFX();
    }


    float groundedTimer = 0f;
    float nonGroundedJumpTime = 0.1f;
    bool jumped = false;
    bool holdingJump = false;

    protected override void FixedUpdate()
    {
        if (dead)
            return;

        float speed = (!runInput ? moveSpeed : runSpeed);

        //pelaajaan kohdistuvat voimat
        velocity += Vector2.right * input * Time.fixedDeltaTime *
            (input > 0f ? 
            Mathf.Lerp(0f, moveAcceleration, Mathf.Max(0f, speed - Mathf.Max(0f, velocity.x))) * (wallHitRight ? 0f : 1f) * (wallHitLeft ? 0.5f : 1f) :
            Mathf.Lerp(0f, moveAcceleration, Mathf.Max(0f, speed - Mathf.Max(0f, -velocity.x))) * (wallHitLeft ? 0f : 1f) * (wallHitRight ? 0.5f : 1f)
            );

        if (jumped)
        {
            if (!holdingJump)
                velocity -= Vector2.up * jumpDownwardAcceleration * Time.fixedDeltaTime;
            if (velocity.y <= 0f)
                jumped = false;
        }

        //if (anchorAbility.Attached())
        //{
        //    Vector2 pullDir = Vector3.Normalize(position - new Vector2(anchorAbility.AnchoredObject().position.x, anchorAbility.AnchoredObject().position.y));
        //    anchorAbility.AnchoredObject().AddForce(pullDir * Time.fixedDeltaTime * anchorPullForce);
        //    //velocity += Vector2.Lerp(Vector2.zero, -pullDir * Time.fixedDeltaTime * anchoringAcceleration, 1f - Mathf.Clamp(Vector2.Dot(velocity, -pullDir * anchoringSpeed), 0f, 1f));
        //    velocity += -pullDir * Time.fixedDeltaTime * playerAnchorAcceleration;
        //    ApplyDrag(Time.fixedDeltaTime * anchorMoveDampen, Time.fixedDeltaTime * anchorMoveDampen * 10f);
        //    anchorAbility.AnchoredObject().drag = anchorMoveDampen;
        //}

        //fysiikat
        base.FixedUpdate();
    }

    void UpdateCamera(float speed)
    {
        cam.position = Vector3.Lerp(cam.position, CalculateCameraPos(), speed);
    }
    Vector3 CalculateCameraPos()
    {
        return new Vector3(position.x, position.y, cam.position.z);
    }

    void UpdateAnim()
    {
        if (groundedTimer == 0f && (wallHitLeft || wallHitRight))
        {
            if (!inWallContact)
                PlayWallHitSound();
            inWallContact = true;
            wallJump = true;
        } else
        {
            inWallContact = false;
        }

        if (wallJump)
        {
            if (wallHitLeft || wallHitRight)
                jumpState = 0f;
            else
                jumpState = Mathf.Clamp(jumpState + Time.deltaTime * 4f, 0f, 1f);
            if (jumpState == 1f || groundedTimer > 0f)
            {
                wallJump = false;
                jumpState = 0.5f;
            }
        }
        else
        {
            if (jumped || (groundedTimer == 0f && jumpState > 0f))
                jumpState = Mathf.Clamp(jumpState + Time.deltaTime * (jumped ? 1f : 2f), 0.15f, 0.5f);
            else if (groundedTimer == 0f)
                jumpState = 0.5f;
            else
                jumpState = 0f;
        }

        anim.SetBool("move", input != 0f);
        anim.SetFloat("speed", Mathf.Abs(velocity.x) / 5f);
        anim.SetFloat("jumpState", jumpState);
        anim.SetBool("wallJump", wallJump);
    }

    public void EmitDust(int particles, float speed)
    {
        ParticleSystem.MainModule main = dustEmitter.main;
        main.startSpeedMultiplier = speed * 0.5f;
        //main.startLifetimeMultiplier = (1f + 1f / speed) / 2f;
        dustEmitter.Emit(particles);
    }

    float stepTimer = 0f;
    float stepInterval = 0.15f;
    int preStep = 0;
    float wallHangParticleEmitTimer = 0f;
    float rushingWaterFade = 0f;
    void UpdateSFX()
    {
        if (input != 0f && jumpState == 0f && wallJump == false)
            stepTimer += Time.deltaTime;
        if (stepTimer >= stepInterval) {
            stepTimer = 0f;
            preStep = (int)Mathf.Repeat(preStep + Random.Range(1, footsteps.Length), footsteps.Length);
            sfxAudio.PlayOneShot(footsteps[preStep], 0.3f);
            EmitDust(15, 1f);
        }

        wallHangParticleEmitTimer += (inWallContact ? Time.deltaTime : 0f);
        if (wallHangParticleEmitTimer > 0.05f)
        {
            wallHangParticleEmitTimer = 0f;
            EmitDust(2, 1f);
        }

        float wallHangVolume = Mathf.Min(Mathf.Abs(velocity.y) * 0.1f, 1f) * 0.15f;
        wallHangAudio.volume = Mathf.MoveTowards(wallHangAudio.volume, inWallContact ? wallHangVolume : 0f, Time.deltaTime * 2f);
        anchorReelAudio.volume = Mathf.MoveTowards(anchorReelAudio.volume, anchorAbility.Attached() ? 0.1f : 0f, Time.deltaTime * 2f);
        bool reelingSelf = (anchorAbility.Attached() && anchorAbility.AnchoredObjectType() == AnchorAbility.ObjectType.enemy && anchorAbility.ShotTimer() > 0.1f);
        float rushingWaterAmount = (reelingSelf ? 1f : Mathf.Min(1f, velocity.magnitude / 15f));
        rushingWaterFade = Mathf.MoveTowards(rushingWaterFade, rushingWaterAmount > 0f ? 1f : 0f, Time.deltaTime * 0.5f);
        rushingWaterAudio.volume = rushingWaterFade * rushingWaterAmount * 0.2f;
        rushingWaterAudio.pitch = Mathf.Lerp(0.6f, 1f, rushingWaterAmount);
    }
    void ResetAudio()
    {
        wallHangAudio.volume = 0f;
        anchorReelAudio.volume = 0f;
        rushingWaterAudio.volume = 0f;
    }
    void PlayJumpSound() { sfxAudio.PlayOneShot(jumpSound, 0.5f); }
    void PlayWallHitSound() { sfxAudio.PlayOneShot(wallHitSound, 0.5f); }
    void PlayWallJumpSound() { sfxAudio.PlayOneShot(wallJumpSound, 0.5f); }
    void PlayLandSound() { sfxAudio.PlayOneShot(landSound, 0.5f); }
    void PlayAnchorShotSound() { sfxAudio.PlayOneShot(anchorShotSound, 0.5f); }
    void PlayStompSound() { sfxAudio.PlayOneShot(stompSound, 0.5f); }
    void PlayDeathSound() { sfxAudio.PlayOneShot(deathSound, 0.25f); }
}
