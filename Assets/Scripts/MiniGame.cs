using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public abstract class MiniGame : MonoBehaviour
{
    [Header("Danger Indicator Controls")]
    [SerializeField]
    public Light spotLight;
    private HDAdditionalLightData hdSpotLight;
    public float lightTempDangerMin = 6900;
    public float lightTempDangerMax = 1500;
    public float lightIntensityMin = 400;
    public float lightIntensityMax = 1100;
    public float noDangerDistance = 2f;
    // Start is called before the first frame update
    public virtual void Start()
    {
        hdSpotLight = spotLight.GetComponent<HDAdditionalLightData>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //Change the spotlight Temp and Intensity based on the (temp, intensity) vector returned from the DetectDanger method
        Vector2 tempAndIntensity = DetectDanger();
        spotLight.colorTemperature = tempAndIntensity.x;
        hdSpotLight.intensity = tempAndIntensity.y;
    }

    public HDAdditionalLightData getHDLight()
    {
        return hdSpotLight;
    }

    public abstract Vector2 DetectDanger();
}
