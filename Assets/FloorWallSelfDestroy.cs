using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorWallSelfDestroy : MonoBehaviour
{
    private float startX;
    private Transform childPlane;
    // Start is called before the first frame update
    void Start()
    {
        startX = transform.position.x;
        childPlane = transform.GetChild(0).GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(startX - transform.position.x > childPlane.localScale.x * 20)
        {
            Destroy(gameObject);
        }
    }
}
