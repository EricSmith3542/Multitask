using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class BalanceDanger : MonoBehaviour
{
    [SerializeField]
    private GameObject ball;
    [SerializeField]
    private GameObject board;
    private Light spotLight;
    
    public float lightTempDangerMin = 20000;
    public float lightTempDangerMax = 1500;

    private float dangerMax;

    // Start is called before the first frame update
    void Start()
    {
        spotLight = GetComponent<Light>();
        dangerMax = board.transform.localScale.x / 2;
    }

    // Update is called once per frame
    void Update()
    {
        float distFromCenter = Mathf.Min(Vector3.Distance(Vector3.zero, ball.transform.localPosition), dangerMax);
        spotLight.colorTemperature = Utils.MapFloat(distFromCenter, 0, dangerMax, lightTempDangerMin, lightTempDangerMax);
    }
}
