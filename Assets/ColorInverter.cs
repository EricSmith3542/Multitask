using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ColorInverter : MonoBehaviour
{
    private Renderer renderer;

    [SerializeField]
    private Light lightWithColorToInvert;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float h,s,v;
        Color.RGBToHSV(lightWithColorToInvert.color, out h, out s, out v);

        Color newColor = new Color(1.0f - lightWithColorToInvert.color.r, 1.0f - lightWithColorToInvert.color.g, 1.0f - lightWithColorToInvert.color.b);
        float newH, newS, newV;
        Color.RGBToHSV(newColor, out newH, out newS, out newV);

        newColor = Color.HSVToRGB(newH, newS, v);

        renderer.material.color = newColor;
    }
}
