using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Cinemachine;
using System;

public class PlayerController : MonoBehaviour
{
    // =========================================================
    // RUN CAMERA
    // =========================================================

    [Header("Run Camera")]

    [SerializeField]
    private float runCameraZoomAmount = 0.75f;

    [SerializeField]
    private float runCameraLowerAmount = 0.2f;

    [SerializeField]
    private float runCameraTransitionSpeed = 5f;

    [SerializeField]
    private float runCameraShakeAmount = 0.03f;

    [SerializeField]
    private float runCameraShakeSpeed = 10f;

    // Where the player appears horizontally in the frame.
    // 0 = left, 0.5 = center, 1 = right.
    [SerializeField]
    [Range(0f, 1f)]
    private float runCameraScreenX = 0.35f;

    // Physically rotates the camera around the player while running.
    //
    // This gives the camera a slightly angled chase view,
    // allowing you to see some of the side of the character.
    //
    // Try values around 5 - 10 degrees first.
    [SerializeField]
    [Range(-30f, 30f)]
    private float runCameraAngleOffset = 8f;


    // Original FreeLook orbit values.
    private float[] normalOrbitRadii = new float[3];
    private float[] normalOrbitHeights = new float[3];

    // Original horizontal framing.
    private float normalScreenX = 0.5f;

    // How far we are currently transitioned into the run camera.
    // 0 = normal camera
    // 1 = full run camera
    private float runCameraBlend = 0f;

    // The actual camera angle offset currently being applied.
    // Movement uses this to compensate for the angled camera.
    private float currentRunCameraAngleOffset = 0f;


    //Character States
    public string HensonState;
    public string MayorKState;
    public string FarmerKl3pState;
    public string JefferyState;
    public string SmithState;
    public string DuckState;
    public string MrSmithState;
    public string ItachiState;
    public string AnkokuState;
    public string SukiState;
    public string WinstonState;
    public string JesseState;
    public string JessyState;
    public string SherriffState;


    //Physicals Information
    public Transform cam;

    [SerializeField]
    private float speed = 6f;

    [SerializeField]
    private float gravity = -9.81f;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private Animator anim2;

    public bool isPlaying;

    [SerializeField]
    GameObject EnterBarMenu;

    [SerializeField]
    GameObject FreeLookCam;

    private CinemachineFreeLook freeLook;

    [SerializeField]
    GameObject VistaCam;

    [SerializeField]
    private GameObject TreasureCam;

    [SerializeField]
    GameObject PauseMenu;

    public CharacterController controller;

    [SerializeField]
    public GameObject Rana;

    public float turnSmoothing = 0.1f;

    float turnSmoothVelocity;


    //UI Info
    [SerializeField]
    GameObject FlyText;

    [SerializeField]
    GameObject PineText;


    //Timeline Saves
    public bool T1HDone;
    public bool T2HDone;
    public bool T1MDone;
    public bool T2MDone;
    public bool T3Done;
    public bool ItachiEndingOne;
    public bool ItachiEndingTwo;
    public bool AnkokuEndingOne;
    public bool AnkokuEndingTwo;
    public bool WinstonEndingOne;
    public bool WinstonEndingTwo;
    public bool JesseEndingOne;
    public bool JesseEndingTwo;


    //Player Save Information
    public bool DuckIn;
    public int walkSpeed;
    public int runSpeed;
    private float ySpeed;
    public float jumpSpeed;
    private bool isSliding;
    private Vector3 slopeSlideVelocity;
    private bool walkSoundPlaying;
    private bool runSoundPlaying;
    public int VillageOneCollectable;
    public int VillageTwoCollectable;
    public int VillageThreeCollectable;
    public int[] VillageOneCollectableInfo;
    public int[] VillageTwoCollectableInfo;
    public int[] VillageThreeCollectableInfo;
    public int[] UniqueCoins;
    public int worldMusic;
    public GameObject teleportParticlesPrefab;
    public List<string> invitedCharacters;
    public AudioClip WalkNoise;
    public AudioClip TeleNoise;
    public AudioClip SlowWalkNoise;

    [SerializeField]
    private AudioClip TreasureSound;

    private AudioSource SoundPlayer;

    public AudioSource SoundPlayerTwo;

    public int amtRum = 1000;
    public int amtFly;
    public int amtPine;
    public int amtSea;
    public bool tutorialDone;
    public bool enterBarTutorialDone;
    public bool mixerTutorialDone;
    public bool isPaused;
    public bool hasSeenCutscene;
    public int night;
    public float VolumeValue;
    public float mouseSensitivity;


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        UniqueCoins = new int[11]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        VillageOneCollectableInfo = new int[50]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        VillageTwoCollectableInfo = new int[50]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        VillageThreeCollectableInfo = new int[50]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        isPlaying = false;

        SoundPlayer =
            this.GetComponent<AudioSource>();

        night = 1;

        freeLook =
            FreeLookCam.GetComponent<CinemachineFreeLook>();


        // =====================================================
        // REMEMBER NORMAL CAMERA SETTINGS
        // =====================================================

        for (int i = 0; i < 3; i++)
        {
            normalOrbitRadii[i] =
                freeLook.m_Orbits[i].m_Radius;

            normalOrbitHeights[i] =
                freeLook.m_Orbits[i].m_Height;
        }


        // Remember the normal Screen X value.
        CinemachineComposer middleComposer =
            freeLook.GetRig(1)
                .GetCinemachineComponent<CinemachineComposer>();

        if (middleComposer != null)
        {
            normalScreenX =
                middleComposer.m_ScreenX;
        }


        amtFly = 0;
        amtPine = 0;
        amtSea = 0;
        night = 0;
        isPaused = false;
        mixerTutorialDone = false;
        tutorialDone = false;
        worldMusic = 0;

        LoadData();
        FinishIntro();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        movement();

        UpdateRunCamera();

        if (Input.GetKeyDown(KeyCode.Escape)
            && !isPaused
            && isPlaying)
        {
            pauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape)
            && isPaused
            && !isPlaying)
        {
            unpauseGame();
        }
    }


    // =========================================================
    // RUN CAMERA
    // =========================================================

    private void UpdateRunCamera()
    {
        if (freeLook == null)
        {
            return;
        }


        float horizontal =
            Input.GetAxis("Horizontal");

        float vertical =
            Input.GetAxis("Vertical");

        Vector2 movementInput =
            new Vector2(
                horizontal,
                vertical
            );


        bool isRunning =
            isPlaying
            && movementInput.magnitude >= 0.1f
            && Input.GetKey(KeyCode.LeftShift);


        // =====================================================
        // SMOOTH RUN TRANSITION
        // =====================================================

        float targetBlend =
            isRunning ? 1f : 0f;

        runCameraBlend =
            Mathf.MoveTowards(
                runCameraBlend,
                targetBlend,
                runCameraTransitionSpeed
                * Time.deltaTime
            );


        // =====================================================
        // CAMERA ANGLE
        // =====================================================

        // Work out how much of our angled chase view
        // should currently be active.
        currentRunCameraAngleOffset =
            Mathf.Lerp(
                0f,
                runCameraAngleOffset,
                runCameraBlend
            );


        // =====================================================
        // RUN SHAKE
        // =====================================================

        float shake = 0f;

        if (isRunning)
        {
            shake =
                Mathf.Sin(
                    Time.time
                    * runCameraShakeSpeed
                )
                * runCameraShakeAmount
                * runCameraBlend;
        }


        // =====================================================
        // MODIFY FREELOOK ORBITS
        // =====================================================

        for (int i = 0; i < 3; i++)
        {
            // Move camera closer.
            float targetRadius =
                normalOrbitRadii[i]
                - runCameraZoomAmount;

            float radius =
                Mathf.Lerp(
                    normalOrbitRadii[i],
                    targetRadius,
                    runCameraBlend
                );


            // Move camera lower.
            float targetHeight =
                normalOrbitHeights[i]
                - runCameraLowerAmount;

            float height =
                Mathf.Lerp(
                    normalOrbitHeights[i],
                    targetHeight,
                    runCameraBlend
                );


            // Add subtle running bob.
            height += shake;


            freeLook.m_Orbits[i].m_Radius =
                Mathf.Max(
                    0.1f,
                    radius
                );

            freeLook.m_Orbits[i].m_Height =
                height;
        }


        // =====================================================
        // SCREEN FRAMING
        // =====================================================

        float screenX =
            Mathf.Lerp(
                normalScreenX,
                runCameraScreenX,
                runCameraBlend
            );


        for (int i = 0; i < 3; i++)
        {
            CinemachineComposer composer =
                freeLook.GetRig(i)
                    .GetCinemachineComponent<CinemachineComposer>();

            if (composer != null)
            {
                composer.m_ScreenX =
                    screenX;
            }
        }


        // =====================================================
        // PHYSICAL CAMERA ORBIT
        // =====================================================

        // Cinemachine's X axis represents the camera's
        // horizontal orbit around the Follow/LookAt target.
        //
        // We ADD only the CHANGE in our desired run offset.
        // This is important because it allows normal mouse
        // camera movement to continue working.
        //
        // For example:
        // last frame offset = 4 degrees
        // this frame offset = 4.2 degrees
        // we only rotate Cinemachine another 0.2 degrees.

        float previousOffset =
            Mathf.Lerp(
                0f,
                runCameraAngleOffset,
                Mathf.MoveTowards(
                    runCameraBlend,
                    targetBlend,
                    -runCameraTransitionSpeed
                    * Time.deltaTime
                )
            );

        float offsetDelta =
            Mathf.DeltaAngle(
                previousOffset,
                currentRunCameraAngleOffset
            );

        freeLook.m_XAxis.Value +=
            offsetDelta;
    }


    // =========================================================
    // PAUSE
    // =========================================================

    public void unpauseGame()
    {
        isPaused = false;
        freeLook.enabled = true;
        PauseMenu.SetActive(false);
        Cursor.visible = false;
        isPlaying = true;
    }


    public void pauseGame()
    {
        isPaused = true;
        PauseMenu.SetActive(true);
        freeLook.enabled = false;
        Cursor.visible = true;
        isPlaying = false;
        hasSeenCutscene = true;

        anim.SetInteger("state", 0);
        anim2.SetInteger("state", 0);
    }


    public void StartPlay()
    {
        StartCoroutine(Wait());
        isPlaying = true;
    }


    // =========================================================
    // TRIGGERS
    // =========================================================

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "tutorialMessage")
        {
            tutorialDone = true;
            SaveSystem.SavePlayer(this);
        }

        if (collider.gameObject.tag == "Door")
        {
            freeLook.enabled = false;
            Cursor.visible = true;
            EnterBarMenu.SetActive(true);
            isPlaying = false;

            anim.SetInteger("state", 0);
            anim2.SetInteger("state", 0);
        }

        if (collider.gameObject.tag == "vistaOne")
        {
            freeLook.enabled = false;
            VistaCam.SetActive(true);
        }

        if (collider.gameObject.tag == "flies")
        {
            StartCoroutine(
                PlayTreasureAnimation(
                    collider.gameObject
                )
            );

            GainFly();

            StartCoroutine(
                DeactivateFly()
            );

            FlyText.SetActive(true);
        }

        if (collider.gameObject.tag == "pine")
        {
            StartCoroutine(
                PlayTreasureAnimation(
                    collider.gameObject
                )
            );

            GainPine();

            StartCoroutine(
                DeactivatePine()
            );

            PineText.SetActive(true);
        }

        if (collider.gameObject.tag == "Treasure")
        {
            StartCoroutine(
                PlayTreasureAnimation(
                    collider.gameObject
                )
            );

            GainPine();

            StartCoroutine(
                DeactivatePine()
            );

            PineText.SetActive(true);
        }

        if (collider.gameObject.tag == "MusicChanger")
        {
            if (worldMusic !=
                collider.gameObject
                    .GetComponent<ChangeWorldMusic>()
                    .worldMusic)
            {
                worldMusic =
                    collider.gameObject
                        .GetComponent<ChangeWorldMusic>()
                        .worldMusic;
            }
            else
            {
                worldMusic =
                    collider.gameObject
                        .GetComponent<ChangeWorldMusic>()
                        .originalTheme;
            }
        }

        if (collider.gameObject.tag == "Finder")
        {
            Player_Seer finderScript =
                collider.GetComponent<Player_Seer>();

            if (finderScript != null)
            {
                GameObject startPoint =
                    finderScript.GetStartPoint();

                if (startPoint != null)
                {
                    GameObject teleportThing =
                        Instantiate(
                            teleportParticlesPrefab,
                            transform.position,
                            Quaternion.identity
                        );

                    StartCoroutine(
                        MoveToPosition(
                            startPoint.transform.position,
                            .1f
                        )
                    );

                    StartCoroutine(
                        DestroyAfterDelay(
                            teleportThing,
                            2f
                        )
                    );
                }
                else
                {
                    Debug.LogWarning(
                        "Start_Point is not assigned in the FinderScript"
                    );
                }
            }
            else
            {
                Debug.LogWarning(
                    "FinderScript component not found on the Finder object"
                );
            }
        }
    }


    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "vistaOne")
        {
            if (collider.gameObject.tag == "vistaOne")
            {
                freeLook.enabled = true;
                VistaCam.SetActive(false);
            }
        }
    }


    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);
    }


    // =========================================================
    // MOVEMENT
    // =========================================================

    private void movement()
    {
        float horizontal =
            Input.GetAxis("Horizontal");

        float vertical =
            Input.GetAxis("Vertical");

        Vector3 direction =
            new Vector3(
                horizontal,
                0f,
                vertical
            ).normalized;


        if (controller.isGrounded
            && direction.magnitude < 0.1)
        {
            SoundPlayer.Stop();

            anim.SetInteger("state", 0);
            anim2.SetInteger("state", 0);
        }


        //Makes player fall towards ground at the speed of gravity.
        if (controller.isGrounded == false)
        {
            Vector3 fall =
                new Vector3(
                    0f,
                    gravity * Time.deltaTime,
                    0f
                );

            controller.Move(fall);
        }


        //Checks if the character is currently playing or in menu.
        //Lets movement happen if they're playing.
        if (isPlaying)
        {
            // =================================================
            // RUN
            // =================================================

            if (direction.magnitude >= 0.1
                && Input.GetKey(KeyCode.LeftShift))
            {
                if (controller.isGrounded)
                {
                    if (!runSoundPlaying)
                    {
                        SoundPlayer.Stop();

                        runSoundPlaying = true;

                        SoundPlayer.PlayOneShot(
                            WalkNoise
                        );

                        StartCoroutine(
                            restartRunningSound()
                        );
                    }


                    //Sets Animation to Run
                    anim.SetInteger("state", 1);
                    anim2.SetInteger("state", 2);
                }


                // =================================================
                // IMPORTANT:
                //
                // The actual Cinemachine camera is now rotated
                // around the player while running.
                //
                // cam.eulerAngles.y therefore includes that extra
                // run-camera angle.
                //
                // Subtracting currentRunCameraAngleOffset here
                // means the player's movement continues using the
                // ORIGINAL chase-camera forward direction.
                //
                // Result:
                //
                // Player runs straight.
                // Camera views player from slightly off to one side.
                // =================================================

                float movementCameraAngle =
                    cam.eulerAngles.y
                    - currentRunCameraAngleOffset;


                float targetangle =
                    Mathf.Atan2(
                        direction.x,
                        direction.z
                    )
                    * Mathf.Rad2Deg
                    + movementCameraAngle;


                float angle =
                    Mathf.SmoothDampAngle(
                        transform.eulerAngles.y,
                        targetangle,
                        ref turnSmoothVelocity,
                        turnSmoothing
                    );


                transform.rotation =
                    Quaternion.Euler(
                        0f,
                        angle,
                        0f
                    );


                Vector3 moveDir =
                    Quaternion.Euler(
                        0f,
                        targetangle,
                        0f
                    )
                    * Vector3.forward;


                moveDir =
                    AdjustVelocityToSlope(
                        moveDir
                    );


                controller.Move(
                    moveDir.normalized
                    * runSpeed
                    * Time.deltaTime
                );
            }


            // =================================================
            // WALK
            // =================================================

            else if (direction.magnitude >= 0.1)
            {
                if (controller.isGrounded)
                {
                    if (!SoundPlayer.isPlaying)
                    {
                        SoundPlayer.Stop();

                        SoundPlayer.PlayOneShot(
                            SlowWalkNoise
                        );
                    }


                    //Sets Player Animation to Walk
                    anim.SetInteger("state", 0);
                    anim2.SetInteger("state", 1);
                }


                // Normal walking is unchanged.
                float targetangle =
                    Mathf.Atan2(
                        direction.x,
                        direction.z
                    )
                    * Mathf.Rad2Deg
                    + cam.eulerAngles.y;


                float angle =
                    Mathf.SmoothDampAngle(
                        transform.eulerAngles.y,
                        targetangle,
                        ref turnSmoothVelocity,
                        turnSmoothing
                    );


                transform.rotation =
                    Quaternion.Euler(
                        0f,
                        angle,
                        0f
                    );


                Vector3 moveDir =
                    Quaternion.Euler(
                        0f,
                        targetangle,
                        0f
                    )
                    * Vector3.forward;


                moveDir =
                    AdjustVelocityToSlope(
                        moveDir
                    );


                controller.Move(
                    moveDir.normalized
                    * speed
                    * Time.deltaTime
                );
            }
        }
        else if (direction.magnitude == 0)
        {
            //Sets Animation to Idle when magnitude is 0
            anim.SetInteger("state", 0);
            anim2.SetInteger("state", 0);
        }
    }


    // =========================================================
    // INVENTORY
    // =========================================================

    public void UseRum()
    {
        amtRum -= 1;
    }


    public void UseFly()
    {
        amtFly -= 1;
        SaveData();
    }


    public void UsePine()
    {
        amtPine -= 1;
        SaveData();
    }


    public void UseSea()
    {
        amtSea -= 1;
        SaveData();
    }


    private void GainRum()
    {
        amtRum += 1;
    }


    private void GainFly()
    {
        amtFly += 1;
    }


    private void GainPine()
    {
        amtPine += 1;
    }


    private void GainSea()
    {
        amtSea += 1;
    }


    public void FinishNight()
    {
        night += 1;
        SaveData();
    }


    // =========================================================
    // SAVE / LOAD
    // =========================================================

    public void SaveData()
    {
        SaveSystem.SavePlayer(this);
    }


    //Do I really gotta explain this?
    public void LoadData()
    {
        SaveManagerScript player =
            SaveSystem.LoadData();


        HensonState =
            player.HensonState;

        MayorKState =
            player.MayorKState;

        ItachiState =
            player.ItachiState;

        AnkokuState =
            player.AnkokuState;

        SukiState =
            player.SukiState;

        FarmerKl3pState =
            player.FarmerKl3pState;

        JefferyState =
            player.JefferyState;

        SmithState =
            player.SmithState;

        DuckState =
            player.DuckState;

        MrSmithState =
            player.MrSmithState;

        WinstonState =
            player.WinstonState;

        JesseState =
            player.JesseState;

        JessyState =
            player.JessyState;

        SherriffState =
            player.SherriffState;


        transform.position =
            new Vector3(
                player.posX,
                player.posY + 3f,
                player.posZ
            );


        mixerTutorialDone =
            player.mixerTutorialDone;

        tutorialDone =
            player.tutorialDone;

        amtFly =
            player.amtFlies;

        amtPine =
            player.amtPine;

        hasSeenCutscene =
            player.hasSeenCutscene;

        night =
            player.night;


        invitedCharacters =
            new List<string>(
                player.invitedCharacters
            );


        T1HDone =
            player.T1HDone;

        T2HDone =
            player.T2HDone;

        T1MDone =
            player.T1MDone;

        T2MDone =
            player.T2MDone;

        T3Done =
            player.T3Done;


        ItachiEndingOne =
            player.ItachiEndingOne;

        ItachiEndingTwo =
            player.ItachiEndingTwo;

        AnkokuEndingOne =
            player.AnkokuEndingOne;

        AnkokuEndingTwo =
            player.AnkokuEndingTwo;

        JesseEndingOne =
            player.JesseEndingOne;

        JesseEndingTwo =
            player.JesseEndingTwo;

        WinstonEndingOne =
            player.WinstonEndingOne;

        WinstonEndingTwo =
            player.WinstonEndingTwo;


        DuckIn =
            player.DuckIn;

        worldMusic =
            player.worldMusic;

        enterBarTutorialDone =
            player.enterBarTutorialDone;

        VolumeValue =
            player.VolumeValue;

        mouseSensitivity =
            player.mouseSensitivity;


        for (int i = 0; i < 50; i++)
        {
            VillageOneCollectableInfo[i] =
                player.VillageOneCollectableInfo[i];
        }


        for (int i = 0;
            i < UniqueCoins.Length;
            i++)
        {
            UniqueCoins[i] =
                player.UniqueCoins[i];
        }


        for (int i = 0; i < 50; i++)
        {
            VillageTwoCollectableInfo[i] =
                player.VillageTwoCollectableInfo[i];
        }


        for (int i = 0; i < 50; i++)
        {
            VillageThreeCollectableInfo[i] =
                player.VillageThreeCollectableInfo[i];
        }


        UniqueCoins =
            player.UniqueCoins;

        VillageTwoCollectable =
            player.VillageTwoCollectable;

        VillageThreeCollectable =
            player.VillageThreeCollectable;
    }


    public void clearInventory()
    {
        amtFly = 0;
        amtPine = 0;
        mixerTutorialDone = true;

        SaveData();
    }


    public void FinishMixerTutorial()
    {
        mixerTutorialDone = true;

        SaveData();
    }


    public void FinishIntro()
    {
        hasSeenCutscene = true;

        SaveData();
    }


    public void EndNight()
    {
        SaveData();
    }


    public void addInvitedCharacter(
        string character
    )
    {
        if (!invitedCharacters.Contains(character))
        {
            invitedCharacters.Add(character);
        }

        SaveData();
    }


    public void clearInvites()
    {
        invitedCharacters.Clear();
    }


    public void removeInvited(
        string name
    )
    {
        if (invitedCharacters.Contains(name))
        {
            invitedCharacters.Remove(name);

            SaveData();
        }
    }


    public void HideMouse()
    {
        Cursor.visible = false;
    }


    public void ShowMouse()
    {
        Cursor.visible = true;
    }


    public void ResetTimelines()
    {
        T1HDone = false;
        T2HDone = false;
        T1MDone = false;
        T2MDone = false;

        SaveData();
    }


    public void replaceWorldMusic(
        int input
    )
    {
        worldMusic = input;

        SaveData();
    }


    // =========================================================
    // SLOPE
    // =========================================================

    private Vector3 AdjustVelocityToSlope(
        Vector3 Velocity
    )
    {
        var ray =
            new Ray(
                transform.position,
                Vector3.down
            );


        if (Physics.Raycast(
            ray,
            out RaycastHit Hitinfo,
            0.2f))
        {
            var slopeRotation =
                Quaternion.FromToRotation(
                    Vector3.up,
                    Hitinfo.normal
                );


            var adjustedVelocity =
                slopeRotation
                * Velocity;


            if (adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }


        return Velocity;
    }


    // =========================================================
    // AUDIO
    // =========================================================

    public IEnumerator restartRunningSound()
    {
        yield return
            new WaitForSeconds(1.5f);

        runSoundPlaying = false;
    }


    // =========================================================
    // TREASURE
    // =========================================================

    public IEnumerator PlayTreasureAnimation(
        GameObject Pickup
    )
    {
        Pickup.transform.position =
            new Vector3(
                transform.position.x,
                transform.position.y + 2,
                transform.position.z
            );


        Pickup.transform.localScale =
            new Vector3(
                Pickup.transform.localScale.x / 2,
                Pickup.transform.localScale.y / 2,
                Pickup.transform.localScale.z / 2
            );


        anim.SetInteger("state", 0);
        anim2.SetInteger("state", 0);

        isPlaying = false;
        freeLook.enabled = false;

        TreasureCam.SetActive(true);

        SoundPlayerTwo.PlayOneShot(
            TreasureSound
        );


        yield return
            new WaitForSeconds(3.5f);


        isPlaying = true;
        freeLook.enabled = true;

        Destroy(Pickup);

        TreasureCam.SetActive(false);
    }


    public IEnumerator PlayTreasureAnimation()
    {
        anim.SetInteger("state", 0);
        anim2.SetInteger("state", 0);

        isPlaying = false;
        freeLook.enabled = false;

        TreasureCam.SetActive(true);

        SoundPlayerTwo.PlayOneShot(
            TreasureSound
        );


        yield return
            new WaitForSeconds(3.5f);


        isPlaying = true;
        freeLook.enabled = true;

        TreasureCam.SetActive(false);
    }


    public IEnumerator DeactivateFly()
    {
        yield return
            new WaitForSeconds(3f);

        FlyText.SetActive(false);
    }


    public IEnumerator DeactivatePine()
    {
        yield return
            new WaitForSeconds(3f);

        PineText.SetActive(false);
    }


    // =========================================================
    // DELETE SAVE
    // =========================================================

    public void deleteSave()
    {
        File.Delete(
            Application.persistentDataPath
            + "/newerSaveData.json"
        );
    }


    // =========================================================
    // TELEPORT
    // =========================================================

    IEnumerator MoveToPosition(
        Vector3 targetPosition,
        float duration
    )
    {
        float time = 0;

        Vector3 startPosition =
            transform.position;


        while (time < duration)
        {
            transform.position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    time / duration
                );

            time +=
                Time.deltaTime;

            yield return null;
        }


        transform.position =
            targetPosition;
    }


    IEnumerator DestroyAfterDelay(
        GameObject objectToDestroy,
        float delay
    )
    {
        yield return
            new WaitForSeconds(delay);

        Destroy(objectToDestroy);
    }
}