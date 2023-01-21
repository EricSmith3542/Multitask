using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FullGameLogic : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject StartScreenUI;
    [SerializeField] private GameObject RestartScreenUI;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image timerCircle;
    [SerializeField] private float secondsToDepleteProgressBar = 1.5f;

    [Header("Games")]
    [SerializeField] private GameObject miniGameHolder;
    [SerializeField] private List<GameObject> miniGamePrefabs;
    [SerializeField] private AudioSource[] miniGameAudioSources;

    [Header("Game Logic Settings")]
    [SerializeField] private float timeScale = 1f;
    [SerializeField] private float difficultyIncrement = .1f;
    [SerializeField] private float[] timeBetweenGameAdds;
    [SerializeField] private float viewPortSnapFinishRange = .01f;
    [SerializeField] private bool shuffleGames = true;
    [SerializeField] private float instructionFadeInSeconds = 1f, instructionFadeOutSeconds = 1f;
    [SerializeField] private float cameraTransitionSpeed = 1f;
    [SerializeField] private AudioClip transitionSound;
    [SerializeField] private AudioClip failSound;

    [Header("Debugging")]
    [SerializeField] private bool debugNoFail = false;

    private float totalTimeSurvived = 0f;
    private float timeInCurrentRound, timeUntilNextGame;
    private bool gameStarted = false;
    private bool waitForNoInput = false;
    
    //Index of the next game to be instansiated from the miniGamePrefabs list
    private int currentGameIndex = 0;

    //Spacing between games as they are instansiated
    private float yDistanceBetweenGames = 50f;

    //Camera Transition Variables
    private List<Camera> gameCameras;
    private List<Camera> shrinkingCameras;
    private Camera growingCamera;
    private InstructionFade currentGameInstructionFader;
    private Rect finalGrowingRect;
    private Rect finalShrinkingRect;
    private bool growHorizontal;
    private bool movingCameras = false;
    private bool incrementTimer = false;
    private bool failed = false;
    private bool firstGame = true;


    //Jank Jumpgame fix variables
    private const float JUMP_CAM_SHIFT = -3.5f;
    private bool jumpGameCameraShiftNeeded = false;
    private bool jumpGameUnshift = false;
    private int jumpGameCameraIndex = 0;
    private Camera jumpGameCamera = null;

    //Mouse Game Fix Variables
    private bool mouseGameBombClean = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        GameSetup();
    }

    // Update is called once per frame
    void Update()
    {
        if (waitForNoInput)
        {
            waitForNoInput = Input.anyKey;
            if (movingCameras)
            {
                AdjustViewports();
            }
        }
        else
        {
            if (!gameStarted)
            {
                if (Input.GetKey(KeyCode.Escape))
                {
                    Application.Quit();
                }
                else if (Input.anyKey)
                {
                    gameStarted = true;
                    failed = false;
                    StartCoroutine(AddNextGame(0));
                }
            }
            else
            {
                if (movingCameras)
                {
                    AdjustViewports();
                }
                else if (failed)
                {
                    GameSetup();
                    foreach (Transform game in miniGameHolder.transform)
                    {
                        Destroy(game.gameObject);
                    }
                    //Maybe timeScale reset here?
                    gameStarted = false;
                    failed = false;
                }
                else if(incrementTimer)
                {
                    totalTimeSurvived += Time.unscaledDeltaTime;
                    timeInCurrentRound += Time.unscaledDeltaTime;
                    timerText.text = totalTimeSurvived.ToString("0.0");
                    timerCircle.fillAmount = timeInCurrentRound / timeUntilNextGame;
                }
            }
        }
    }

    IEnumerator AddNextGame(float secondsUntilNextGame)
    {
        yield return new WaitForSecondsRealtime(secondsUntilNextGame);

        Time.timeScale = 0;
        incrementTimer = false;
        SoundManager.Instance.StopAllMusic();
        SoundManager.Instance.PlaySound(transitionSound);
        yield return new WaitForSecondsRealtime(transitionSound.length);

        StartCoroutine(DepleteProgressBar());

        //Create new game and add its camera to the list of cameras
        GameObject newGame = Instantiate(miniGamePrefabs[currentGameIndex], new Vector3(0, (currentGameIndex + 1) * yDistanceBetweenGames, 0), Quaternion.identity, miniGameHolder.transform);

        //Add the games camera to the list of cameras
        Camera gameCam = newGame.GetComponentInChildren<Camera>();
        currentGameInstructionFader = newGame.GetComponent<InstructionFade>();
        growingCamera = gameCam;

        
        SoundManager.Instance.PlayMusicWithoutLoop(currentGameIndex);
        TransitionCameras();
    }

    IEnumerator IncrementDifficulty(float secondsUntilDifficultyIncrement)
    {
        yield return new WaitForSecondsRealtime(secondsUntilDifficultyIncrement);
        SoundManager.Instance.PlaySound(transitionSound);

        timeInCurrentRound = 0f;
        timerCircle.fillAmount = 0f;

        Time.timeScale += difficultyIncrement;
        SoundManager.Instance.ChangePitch(Time.timeScale);

        StartCoroutine(IncrementDifficulty(timeUntilNextGame));
    }

    void AdjustViewports()
    {
        float growingX = Mathf.LerpUnclamped(growingCamera.rect.x, finalGrowingRect.x, Time.unscaledDeltaTime * cameraTransitionSpeed);
        float growingY = Mathf.LerpUnclamped(growingCamera.rect.y, finalGrowingRect.y, Time.unscaledDeltaTime * cameraTransitionSpeed);
        float growingWidth = Mathf.LerpUnclamped(growingCamera.rect.width, finalGrowingRect.width, Time.unscaledDeltaTime * cameraTransitionSpeed);
        float growingHeight = Mathf.LerpUnclamped(growingCamera.rect.height, finalGrowingRect.height, Time.unscaledDeltaTime * cameraTransitionSpeed);

        growingCamera.rect = new Rect(growingX, growingY, growingWidth, growingHeight);

        for(int i = 0; i < shrinkingCameras.Count; i++)
        {
            Camera shrinkingCamera = shrinkingCameras[i];

            //This is essentially: "If we failed or are looking at the thrid game added while adding the forth game, slide the camera to the left"
            float shrinkingX = shrinkingCamera.rect.x;
            if (failed || (shrinkingCamera.rect.x != 0f && !(shrinkingCamera.rect.x >= .5f)))
            {
                shrinkingX = Mathf.LerpUnclamped(shrinkingCamera.rect.x, 0f, Time.unscaledDeltaTime * cameraTransitionSpeed);
            }

            float shrinkingY = Mathf.LerpUnclamped(shrinkingCamera.rect.y, finalShrinkingRect.y, Time.unscaledDeltaTime * cameraTransitionSpeed);
            float shrinkingWidth = Mathf.LerpUnclamped(shrinkingCamera.rect.width, finalShrinkingRect.width, Time.unscaledDeltaTime * cameraTransitionSpeed);
            float shrinkingHeight = Mathf.LerpUnclamped(shrinkingCamera.rect.height, finalShrinkingRect.height, Time.unscaledDeltaTime * cameraTransitionSpeed);
            
            shrinkingCamera.rect = new Rect(shrinkingX, (growHorizontal || failed) ? shrinkingCamera.rect.y:shrinkingY, shrinkingWidth, shrinkingHeight);
        }

        //Jank Jump Game Camera fix
        if (jumpGameCameraShiftNeeded && currentGameIndex >= 1)
        {
            if (!jumpGameCamera)
            {
                if (jumpGameCameraIndex == 0)
                {
                    jumpGameCamera = shrinkingCameras[0];
                }
                else
                {
                    jumpGameCamera = growingCamera;
                }
            }
            
            jumpGameCamera.transform.position = new Vector3(Mathf.LerpUnclamped(jumpGameCamera.transform.position.x, JUMP_CAM_SHIFT, Time.unscaledDeltaTime * cameraTransitionSpeed), jumpGameCamera.transform.position.y, jumpGameCamera.transform.position.z);
        }
        else if (jumpGameUnshift)
        {
            jumpGameCamera.transform.position = new Vector3(Mathf.LerpUnclamped(jumpGameCamera.transform.position.x, 0, Time.unscaledDeltaTime * cameraTransitionSpeed), jumpGameCamera.transform.position.y, jumpGameCamera.transform.position.z);
        }
        

        //Check if we are done changing the cameras
        if (isViewportNearTargetSize(growingCamera.rect, finalGrowingRect))
        {
            movingCameras = false;

            StartCoroutine(FinalizeCameraTransition());
        }
    }

    IEnumerator FinalizeCameraTransition()
    {
        //Set the viewports to be exactly the correct size
        growingCamera.rect = new Rect(finalGrowingRect);

        foreach (Camera shrinkingCamera in shrinkingCameras)
        {
            float targetX = shrinkingCamera.rect.x;
            if (targetX != 0f && targetX != .5f)
            {
                targetX = 0f;
            }
            shrinkingCamera.rect = new Rect(targetX, growHorizontal ? shrinkingCamera.rect.y : finalShrinkingRect.y, finalShrinkingRect.width, finalShrinkingRect.height);
        }

        if (!failed)
        {
            if (jumpGameCameraShiftNeeded && currentGameIndex >= 1)
            {
                jumpGameCamera.transform.position = new Vector3(JUMP_CAM_SHIFT, jumpGameCamera.transform.position.y, jumpGameCamera.transform.position.z);
                jumpGameCameraShiftNeeded = false;
                jumpGameUnshift = true;
            }
            else if (jumpGameUnshift)
            {
                jumpGameCamera.transform.position = new Vector3(0f, jumpGameCamera.transform.position.y, jumpGameCamera.transform.position.z);
                jumpGameUnshift = false;
            }

            if (mouseGameBombClean && currentGameIndex >= 1)
            {
                GameObject mouseGame = GameObject.FindGameObjectWithTag("Mouse Game");
                Camera mouseGameCamera = mouseGame.GetComponentInChildren<Camera>();
                Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(mouseGameCamera);

                foreach (Transform t in mouseGame.transform)
                {
                    if (t.tag.Equals("Bomb Holder"))
                    {
                        foreach (Transform bomb in t)
                        {
                            //Remove any bomb that isnt in the camera any more
                            if (!GeometryUtility.TestPlanesAABB(cameraPlanes, bomb.GetComponent<Collider>().bounds))
                            {
                                Destroy(bomb.gameObject);
                            }
                        }
                    }
                }
                mouseGameBombClean = false;
            }
        }
        
        //Add the camera to the list of cameras for the next transition
        gameCameras.Add(growingCamera);

        //Deactive the main menu object when done transitioning only on the first game
        if (currentGameIndex == 0) { 
            (firstGame ? StartScreenUI:RestartScreenUI).SetActive(false);
            firstGame = false;
        }

        //TODO: Add some trigger here to start some indication to the player that the game will be unfreezing soon, maybe change the WaitForSeconds below to a WaitUntil so that the timing is decoupled
        if (!failed)
        {
            StartCoroutine(ShowInstructions(instructionFadeInSeconds, instructionFadeOutSeconds));
            yield return new WaitForSecondsRealtime(instructionFadeInSeconds + instructionFadeOutSeconds);
            SoundManager.Instance.StartAllMusicUpToIndex(currentGameIndex);

            timeUntilNextGame = currentGameIndex < timeBetweenGameAdds.Length ? timeBetweenGameAdds[currentGameIndex] : timeBetweenGameAdds[timeBetweenGameAdds.Length - 1];
            timeInCurrentRound = 0;

            incrementTimer = true;
            Time.timeScale = timeScale;
            SoundManager.Instance.ChangePitch(Time.timeScale);
            currentGameIndex++;
            if (currentGameIndex < miniGamePrefabs.Count)
            {
                //Add the next game after the timeUntilNextGame elapses. The time is either timeBetweenGameAdds[currentGameIndex] or just the last element in the timeBetweenGameAdds array if curreGameIndex is out of bounds
                StartCoroutine(AddNextGame(timeUntilNextGame));
            }
            else
            {
                StartCoroutine(IncrementDifficulty(timeUntilNextGame));
            }
        }


    }

    IEnumerator ShowInstructions(float fadeInSecs, float fadeOutSecs)
    {
        currentGameInstructionFader.StartFadeIn(fadeInSecs);
        yield return new WaitForSecondsRealtime(fadeInSecs);
        currentGameInstructionFader.StartFadeOut(fadeOutSecs);
    }

    void TransitionCameras()
    {
        Rect startRect;
        shrinkingCameras = gameCameras;
        
        //The first game is a special case since it uses the menu instead of other games
        if (currentGameIndex == 0)
        {
            shrinkingCameras = new List<Camera>() { (firstGame ? StartScreenUI : RestartScreenUI).GetComponentInChildren<Camera>() };

            startRect = new Rect(1f, 0f, 0f, 1f);
            finalGrowingRect = new Rect(0f, 0f, 1f, 1f);
            finalShrinkingRect = new Rect(0f, 0f, 0f, 1f);
        }
        else
        {
            //Grab the viewport of the previously added camera
            Rect preExistingCameraViewport = gameCameras[currentGameIndex-1].rect;

            //Determine start and finish viewport sizes depending on the game index
            growHorizontal = currentGameIndex % 2 == 1;
            if (growHorizontal)
            {
                startRect = new Rect(1f, 0f, 0f, preExistingCameraViewport.height);
                finalGrowingRect = new Rect(.5f, preExistingCameraViewport.y, .5f, preExistingCameraViewport.height);
                finalShrinkingRect = new Rect(preExistingCameraViewport.x, preExistingCameraViewport.y, .5f, preExistingCameraViewport.height);
            }
            else
            {
                startRect = new Rect(0f, 0f, 1f, 0f);
                finalGrowingRect = new Rect(.25f, 0f, .5f, preExistingCameraViewport.height / 2);
                finalShrinkingRect = new Rect(preExistingCameraViewport.x, (1 - preExistingCameraViewport.y) / 2, preExistingCameraViewport.width, preExistingCameraViewport.height/2);
            }
            
        }

        //Set the cameras starting ViewPort
        growingCamera.rect = new Rect(startRect);
        movingCameras = true;
    }

    //Method returns true if both the height and width of rect1 are >= rect2's
    bool isViewportNearTargetSize(Rect rect1, Rect rect2)
    {
        return (rect1.width >= rect2.width - viewPortSnapFinishRange) && (rect1.height >= rect2.height - viewPortSnapFinishRange);
    }

    public void FailToRestartScreen()
    {
        if (!debugNoFail)
        {
            StopAllCoroutines();
            SoundManager.Instance.StopAllMusic();
            SoundManager.Instance.ChangePitch(1);
            SoundManager.Instance.PlaySound(failSound);
            
            waitForNoInput = true;
            Time.timeScale = 0;
            timerCircle.fillAmount = 0;
            growingCamera = RestartScreenUI.GetComponentInChildren<Camera>();
            growingCamera.rect = new Rect(1f, 0f, 0f, 1f);
            finalGrowingRect = new Rect(0f, 0f, 1f, 1f);

            shrinkingCameras = gameCameras;
            finalShrinkingRect = new Rect(0f, 0f, 0f, shrinkingCameras[0].rect.height);

            RestartScreenUI.SetActive(true);

            failed = true;
            movingCameras = true;
        }
    }

    private void GameSetup()
    {
        SoundManager.Instance.StartMainMenuMusic();
        //TODO: This Shuffle parameter will be used more later when I add more configuration options to how people want to play the game
        if (shuffleGames)
        {
            Utils.Shuffle(miniGamePrefabs);
        }

        InitializeAudioClips();
        //The jump game is the only game where the camera isnt centered on the controllable piece, so when the viewport ratio isn't 1:1, the camera needs to be shifted to keep the jumper on the screen
        JumpGameCheck();
        MouseGameCheck();

        gameCameras = new List<Camera>();
        shrinkingCameras = new List<Camera>();
        currentGameIndex = 0;
        totalTimeSurvived = 0;
        jumpGameCamera = null;
        jumpGameUnshift = false;
    }

    private void InitializeAudioClips()
    {
        for (int i = 0; i < miniGamePrefabs.Count; i++)
        {
            AudioSource gameAudio = miniGamePrefabs[i].GetComponent<AudioSource>();
            miniGameAudioSources[i].clip = gameAudio.clip;
        }
    }

    void JumpGameCheck()
    {
        //TODO: Assess a better fix to this
        if (miniGamePrefabs[0].tag.Equals("Jump Game") || miniGamePrefabs[1].tag.Equals("Jump Game"))
        {
            jumpGameCameraShiftNeeded = true;
            if (miniGamePrefabs[1].tag.Equals("Jump Game"))
            {
                jumpGameCameraIndex = 1;
            }
            else
            {
                jumpGameCameraIndex = 0;
            }
        }
        else
        {
            jumpGameCameraShiftNeeded = false;
        }
    }

    void MouseGameCheck()
    {
        //TODO: Assess a better fix to this
        if (miniGamePrefabs[0].tag.Equals("Mouse Game") || miniGamePrefabs[1].tag.Equals("Mouse Game"))
        {
            mouseGameBombClean = true;
        }
        else
        {
            mouseGameBombClean = false;
        }
    }

    IEnumerator DepleteProgressBar()
    {
        float t = 0f;
        while (timerCircle.fillAmount > 0)
        {
            t += Time.unscaledDeltaTime;
            timerCircle.fillAmount = Mathf.LerpUnclamped(timerCircle.fillAmount, 0f, t/secondsToDepleteProgressBar);
            yield return new WaitForEndOfFrame();
        }
    }

}
