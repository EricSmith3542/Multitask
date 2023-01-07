using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDownObstacle : MonoBehaviour
{
    private float speed = 0f;
    private float deleteDistance = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x - deleteDistance, transform.position.y, transform.position.z), speed * Time.deltaTime);
        if(deleteDistance != 0 && Mathf.Abs(transform.localPosition.x) >= Mathf.Abs(deleteDistance))
        {
            Destroy(gameObject);
        }
    }

    public void setSpeed(float s)
    {
        speed = s;
    }

    public void setDeleteDistance(float dist)
    {
        deleteDistance = dist;
    }

}
