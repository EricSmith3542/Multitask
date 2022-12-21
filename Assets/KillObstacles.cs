using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillObstacles : MonoBehaviour
{
    [SerializeField]
    private string killTag = "JumpObstacle";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == killTag)
        {
            Destroy(other.gameObject);
        }
    }
}
