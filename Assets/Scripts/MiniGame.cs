using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

public abstract class MiniGame : MonoBehaviour
{
    [Header("Danger Indicator Controls")]
    [SerializeField] private Light spotLight;
    [SerializeField] public float lightTempDangerMin = 6900;
    [SerializeField] public float lightTempDangerMax = 1500;
    [SerializeField] public float lightIntensityMin = 400;
    [SerializeField] public float lightIntensityMax = 1100;
    [SerializeField] public float noDangerDistance = 2f;

    private HDAdditionalLightData hdSpotLight;
    private FullGameLogic gameController;

    // Start is called before the first frame update
    public virtual void Start()
    {
        hdSpotLight = spotLight.GetComponent<HDAdditionalLightData>();
        if(SceneManager.GetActiveScene().name == "Full Game Loop")
        {
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<FullGameLogic>();
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //Change the spotlight Temp and Intensity based on the (temp, intensity) vector returned from the DetectDanger method
        Vector2 tempAndIntensity = DetectDanger();
        spotLight.colorTemperature = Mathf.Lerp(spotLight.colorTemperature, tempAndIntensity.x, Time.deltaTime * 5f);
        hdSpotLight.intensity = Mathf.Lerp(hdSpotLight.intensity, tempAndIntensity.y, Time.deltaTime * 5f);
    }

    public HDAdditionalLightData getHDLight()
    {
        return hdSpotLight;
    }

    public abstract Vector2 DetectDanger();

    public void EndGame()
    {
        //add some screen shatter/sound effects here or some indication of which game failed
        gameController.FailToRestartScreen();
    }
}
