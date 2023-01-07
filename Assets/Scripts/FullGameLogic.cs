using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullGameLogic : MonoBehaviour
{
    private const float VIEWPORT_BUFFER = .1f;
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

    private bool gameStarted = false;
    private bool shuffle = false;
    private bool movingCameras = false;

    private int currentGameIndex = 0;
    private float yDistanceBetweenGames = 50f;

    private Camera shrinkingCamera;
    private Camera growingCamera;
    private Rect finalGrowingRect;
    private Rect finalShrinkingRect;

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
            AddNextGame(timeBetweenGameAdds);
        }

        //TODO: FIX THIS STUFF for some reason it never stops changing the camera size
        if (movingCameras)
        {
            float growingX = Mathf.Lerp(growingCamera.rect.x, finalGrowingRect.x, Time.unscaledDeltaTime * cameraTransitionSpeed);
            float growingY = Mathf.Lerp(growingCamera.rect.y, finalGrowingRect.y, Time.unscaledDeltaTime * cameraTransitionSpeed);
            float growingWidth = Mathf.Lerp(growingCamera.rect.width, finalGrowingRect.width + VIEWPORT_BUFFER, Time.unscaledDeltaTime * cameraTransitionSpeed);
            float growingHeight = Mathf.Lerp(growingCamera.rect.height, finalGrowingRect.height + VIEWPORT_BUFFER, Time.unscaledDeltaTime * cameraTransitionSpeed);

            float shrinkingX = Mathf.Lerp(shrinkingCamera.rect.x, finalShrinkingRect.x, Time.unscaledDeltaTime * cameraTransitionSpeed);
            float shrinkingY = Mathf.Lerp(shrinkingCamera.rect.y, finalShrinkingRect.y, Time.unscaledDeltaTime * cameraTransitionSpeed);
            float shrinkingWidth = Mathf.Lerp(shrinkingCamera.rect.width, finalShrinkingRect.width - VIEWPORT_BUFFER, Time.unscaledDeltaTime * cameraTransitionSpeed);
            float shrinkingHeight = Mathf.Lerp(shrinkingCamera.rect.height, finalShrinkingRect.height - VIEWPORT_BUFFER, Time.unscaledDeltaTime * cameraTransitionSpeed);

            growingCamera.rect = new Rect(growingX, growingY, growingWidth, growingHeight);
            shrinkingCamera.rect = new Rect(shrinkingX, shrinkingY, shrinkingWidth, shrinkingHeight);
            if(isRectLarger(growingCamera.rect, finalGrowingRect) && isRectLarger(finalShrinkingRect, shrinkingCamera.rect))
            {
                 movingCameras = false;

                //Set the viewports to be exactly the correct size
                growingCamera.rect = new Rect(finalGrowingRect);
                shrinkingCamera.rect = new Rect(finalShrinkingRect);

                //TODO: Add some trigger here to start some indication to the player that the game will be unfreezing soon, maybe change the WaitForSeconds below to a WaitUntil so that the timing is decoupled


                //Deactive the main menu object when done transitioning only on the first game
                if (currentGameIndex == 0) { StartScreenUI.SetActive(false); }

                ////Wait and unfreeze game
                //yield return new WaitForSecondsRealtime(secondsToWaitAfterTransition);
                Time.timeScale = 1;
                currentGameIndex++;
            }
        }
    }

    void AddNextGame(float secondsUntilNextGame)
    {
        Time.timeScale = 0;
        //Create new game and add its camera to the list of cameras
        GameObject newGame = Instantiate(miniGamePrefabs[currentGameIndex], new Vector3(0, currentGameIndex * yDistanceBetweenGames, 0), Quaternion.identity, transform);
        Camera gameCam = newGame.GetComponentInChildren<Camera>();
        gameCameras.Add(gameCam);

        TransitionCameras();
    }

    void TransitionCameras()
    {
        //The first game is a special case since it uses the menu instead of other games
        if (currentGameIndex == 0)
        {
            shrinkingCamera = StartScreenUI.GetComponentInChildren<Camera>();
            growingCamera = gameCameras[currentGameIndex];

            Rect startRect = new Rect(0f, 0f, 0f, 1f);
            finalGrowingRect = new Rect(0f, 0f, 1f, 1f);
            finalShrinkingRect = new Rect(0f, 0f, 0f, 1f);

            //Set the cameras starting ViewPort
            growingCamera.rect = new Rect(startRect);
            movingCameras = true;
        }
    }

    //Method returns true if both the height and width of rect1 are >= rect2's
    bool isRectLarger(Rect rect1, Rect rect2)
    {
        return (rect1.width >= rect2.width) && (rect1.height >= rect2.height);
    }
}
