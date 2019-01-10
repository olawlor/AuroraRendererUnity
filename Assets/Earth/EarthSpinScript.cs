using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthSpinScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float spinrate = -0.5f; // degrees per second
        float spin = spinrate * Time.deltaTime;
        transform.Rotate(Vector3.up * spin);
    }
}
