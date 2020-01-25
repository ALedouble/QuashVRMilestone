using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiLevel : MonoBehaviour
{
    public static MultiLevel Instance;

    [HideInInspector]
    public int levelIndex; 

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
