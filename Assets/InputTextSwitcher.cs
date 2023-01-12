using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
[RequireComponent(typeof(PlayerInput))]
public class InputTextSwitcher : MonoBehaviour
{
    [SerializeField]
    private string actionName;

    // Start is called before the first frame update
    void Start()
    {
        InputBinding binding = GetComponent<PlayerInput>().actions[actionName].bindings[0];
        if (binding.isComposite)
        {
            
        }
        //GetComponent<TMP_Text>().text = .path;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
