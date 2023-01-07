using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Light))]
public class BalanceDanger : MiniGame
{
    [SerializeField]
    private GameObject ball;
    [SerializeField]
    private GameObject board;
    private float dangerMax;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        dangerMax = board.transform.localScale.x / 2;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        DetectDanger();
    }

    public override Vector2 DetectDanger()
    {
        float distFromCenter = Mathf.Min(Vector3.Distance(Vector3.zero, ball.transform.localPosition), dangerMax);
        float newTemp = Utils.MapFloat(distFromCenter, 0, dangerMax, lightTempDangerMin, lightTempDangerMax);
        float newIntensity = Utils.MapFloat(distFromCenter, 0, dangerMax, lightIntensityMin, lightIntensityMax);
        return new Vector2(newTemp, newIntensity);
    }
}
