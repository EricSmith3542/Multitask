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

    private float secondsToWaitBeforeDanger = 1f;
    private bool showDanger = false;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        dangerMax = board.transform.localScale.x / 2;
        StartCoroutine(WaitForDanger());
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    public override Vector2 DetectDanger()
    {
        if (showDanger)
        {
            float distFromCenter = Mathf.Min(Vector3.Distance(Vector3.zero, ball.transform.localPosition), dangerMax);
            float newTemp = Utils.MapFloat(distFromCenter, 0, dangerMax, lightTempDangerMin, lightTempDangerMax);
            float newIntensity = Utils.MapFloat(distFromCenter, 0, dangerMax, lightIntensityMin, lightIntensityMax);
            return new Vector2(newTemp, newIntensity);
        }
        return new Vector2(lightTempDangerMin, lightIntensityMin);
    }

    IEnumerator WaitForDanger()
    {
        yield return new WaitForSeconds(secondsToWaitBeforeDanger);
        showDanger = true;
    }
}
