using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeInfinite : MonoBehaviour
{
    private const float distanceBeforeWarp = 15f;
    private Vector3 startPosition;
    public float speed = .1f;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(transform.localPosition.x) > distanceBeforeWarp)
        {
            transform.position = new Vector3(startPosition.x, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), speed * Time.deltaTime);
        }
    }
}
