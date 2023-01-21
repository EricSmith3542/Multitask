using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class MouseGameBomb : MonoBehaviour
{

    public float lifeTimeSeconds = 5f;
    public float maxShakeAmount = 2f;

    private Light spotLight;
    private HDAdditionalLightData hdSpotLight;
    private float lightTempDangerMin;
    private float lightTempDangerMax;
    private float lightIntensityMin;
    private float lightIntensityMax;
    private bool isLightController = false;


    private float currentAmount = 0f;
    private float timeElapsed = 0f;
    private Vector3 anchor;

    // Start is called before the first frame update
    void Start()
    {
        anchor = transform.position;
        StartCoroutine(FailCounter(lifeTimeSeconds));
        hdSpotLight = spotLight.GetComponent<HDAdditionalLightData>();
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        float xForFunction = Utils.MapFloat(timeElapsed, 0, 5, 5, 0);
        currentAmount = Mathf.Pow(.2f, xForFunction) * maxShakeAmount;

        Vector2 shake = Random.insideUnitCircle * (Time.deltaTime * currentAmount);
        transform.position = new Vector3(anchor.x + shake.x, anchor.y + shake.y, 0);

        if(isLightController && lightIntensityMin != 0)
        {
            spotLight.colorTemperature = Utils.MapFloat(timeElapsed, 0, lifeTimeSeconds, lightTempDangerMin, lightTempDangerMax);
            hdSpotLight.intensity = Utils.MapFloat(timeElapsed, 0, lifeTimeSeconds, lightIntensityMin, lightIntensityMax);
        }
    }

    IEnumerator FailCounter(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Debug.Log("MOUSE GAME FAIL");
        GameObject.FindGameObjectWithTag("GameController").GetComponent<FullGameLogic>().FailToRestartScreen();
    }

    public void attachLight(Light light, float tempMin, float tempMax, float intensityMin, float intensityMax)
    {
        spotLight = light;
        lightTempDangerMax = tempMax;
        lightTempDangerMin = tempMin;
        lightIntensityMax = intensityMax;
        lightIntensityMin = intensityMin;
    }

    public void SetAsLightController()
    {
        isLightController = true;
    }

    private void OnDestroy()
    {
        if (isLightController && transform.parent.childCount > 1)
        {
            transform.parent.GetChild(transform.GetSiblingIndex() + 1).GetComponent<MouseGameBomb>().SetAsLightController();
        }
    }
}
