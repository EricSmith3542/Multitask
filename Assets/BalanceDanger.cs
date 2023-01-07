using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Light))]
public class BalanceDanger : MonoBehaviour
{
    [SerializeField]
    private GameObject ball;
    [SerializeField]
    private GameObject board;
    private Light spotLight;
    private HDAdditionalLightData hdSpotLight;
    
    public float lightTempDangerMin = 20000;
    public float lightTempDangerMax = 1500;

    public float lightIntensityMin = 125;
    public float lightIntensityMax = 1100;

    private float dangerMax;

    // Start is called before the first frame update
    void Start()
    {
        spotLight = GetComponent<Light>();
        hdSpotLight = GetComponent<HDAdditionalLightData>();
        dangerMax = board.transform.localScale.x / 2;
    }

    // Update is called once per frame
    void Update()
    {
        float distFromCenter = Mathf.Min(Vector3.Distance(Vector3.zero, ball.transform.localPosition), dangerMax);
        spotLight.colorTemperature = Utils.MapFloat(distFromCenter, 0, dangerMax, lightTempDangerMin, lightTempDangerMax);
        hdSpotLight.intensity = Utils.MapFloat(distFromCenter, 0, dangerMax, lightIntensityMin, lightIntensityMax);
    }
}
