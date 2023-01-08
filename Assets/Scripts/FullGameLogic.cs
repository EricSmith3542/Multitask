using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullGameLogic : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField]
    private GameObject StartScreenUI;

    [Header("Games")]
    [SerializeField]
    private List<GameObject> miniGamePrefabs;
    private List<Camera> gameCameras;

    [Header("Game Logic Settings")]
    [SerializeField]
    private float timeBetweenGameAdds = 15f;
    [SerializeField]
    private float cameraTransitionSpeed = 1f;
    [SerializeField]
    private float secondsToWaitAfterTransition = 2f;
    [SerializeField]
    private float viewPortSnapFinishRange = .01f;

    private bool gameStarted = false;
    private bool shuffle = true;
    private bool movingCameras = false;

    private int currentGameIndex = 0;
    private float yDistanceBetweenGames = 50f;

    private List<Camera> shrinkingCameras;
    private Camera growingCamera;
    private Rect finalGrowingRect;
    private Rect finalShrinkingRect;
    private bool jumpGameCameraShiftNeeded = false;
    private bool growHorizontal;

    // Start is called before the first frame update
    void Start()
    {
        //TODO: This Shuffle parameter will be used more later when I add more configuration options to how people want to play the game
        if (shuffle)
        {
            Utils.Shuffle(miniGamePrefabs);
        }
        gameCameras = new List<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStarted && Input.anyKey)
        {
            
            gameStarted = true;
            StartCoroutine(AddNextGame(0));
        }

        if (movingCameras)
        {
            AdjustViewports();
        }
    }

    IEnumerator AddNextGame(float secondsUntilNextGame)
    {
        yield return new WaitForSeconds(secondsUntilNextGame);

        Time.timeScale = 0;
        //Create new game and add its camera to the list of cameras
        GameObject newGame = Instantiate(miniGamePrefabs[currentGameIndex], new Vector3(0, (currentGameIndex + 1) * yDistanceBetweenGames, 0), Quaternion.identity, transform);

        //The jump game is the only game where the camera isnt centered on the controllable piece, so when the viewport ratio isn't 1:1, the camera needs to be shifted to keep the jumper on the screen
        //TODO: Assess a better fix to this
        if(newGame.tag.Equals("Jump Game"))
        {
            jumpGameCameraShiftNeeded = true;
        }

        //Add the games camera to the list of cameras
        Camera gameCam = newGame.GetComponentInChildren<Camera>();
        growingCamera = gameCam;
        
        TransitionCameras();
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
            //float shrinkingX = Mathf.LerpUnclamped(shrinkingCamera.rect.x, finalShrinkingRect.x, Time.unscaledDeltaTime * cameraTransitionSpeed);
            float shrinkingY = Mathf.LerpUnclamped(shrinkingCamera.rect.y, finalShrinkingRect.y, Time.unscaledDeltaTime * cameraTransitionSpeed);
            float shrinkingWidth = Mathf.LerpUnclamped(shrinkingCamera.rect.width, finalShrinkingRect.width, Time.unscaledDeltaTime * cameraTransitionSpeed);
            float shrinkingHeight = Mathf.LerpUnclamped(shrinkingCamera.rect.height, finalShrinkingRect.height, Time.unscaledDeltaTime * cameraTransitionSpeed);

            shrinkingCamera.rect = new Rect(shrinkingCamera.rect.x, growHorizontal ? shrinkingCamera.rect.y:shrinkingY, shrinkingWidth, shrinkingHeight);
        }
        

        //Check if we are done changing the cameras
        if (isViewportNearTargetSize(growingCamera.rect, finalGrowingRect))
        {
            movingCameras = false;

            //Set the viewports to be exactly the correct size
            growingCamera.rect = new Rect(finalGrowingRect);

            foreach(Camera shrinkingCamera in shrinkingCameras)
            {
                shrinkingCamera.rect = new Rect(shrinkingCamera.rect.x, growHorizontal ? shrinkingCamera.rect.y:finalShrinkingRect.y, finalShrinkingRect.width, finalShrinkingRect.height);
            }
            

            //Add the camera to the list of cameras for the next transition
            gameCameras.Add(growingCamera);

            //TODO: Add some trigger here to start some indication to the player that the game will be unfreezing soon, maybe change the WaitForSeconds below to a WaitUntil so that the timing is decoupled


            //Deactive the main menu object when done transitioning only on the first game
            if (currentGameIndex == 0) { StartScreenUI.SetActive(false); }

            ////Wait and unfreeze game
            //yield return new WaitForSecondsRealtime(secondsToWaitAfterTransition);
            Time.timeScale = 1;
            currentGameIndex++;
            if(currentGameIndex < miniGamePrefabs.Count)
            {
                StartCoroutine(AddNextGame(timeBetweenGameAdds));
            }
        }
    }

    void TransitionCameras()
    {
        Rect startRect;
        shrinkingCameras = gameCameras;
        
        //The first game is a special case since it uses the menu instead of other games
        if (currentGameIndex == 0)
        {
            shrinkingCameras = new List<Camera>() { StartScreenUI.GetComponentInChildren<Camera>() };

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
                finalGrowingRect = new Rect(.5f, preExistingCameraViewport.y, preExistingCameraViewport.width / 2, preExistingCameraViewport.height);
                finalShrinkingRect = new Rect(preExistingCameraViewport.x, preExistingCameraViewport.y, preExistingCameraViewport.width / 2, preExistingCameraViewport.height);
            }
            else
            {
                startRect = new Rect(0f, 0f, 1f, 0f);
                finalGrowingRect = new Rect(0f, 0f, 1f, preExistingCameraViewport.height / 2);
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
}