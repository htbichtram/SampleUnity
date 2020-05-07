using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    // Start is called before the first frame update
    private MeshRenderer cubeRender;
    private Transform transform;
    private Color[] colorList = {Color.red, Color.green, Color.blue};
    private int currentColor = 0;

    void Start()
    {
        cubeRender = this.GetComponent<MeshRenderer>();
        cubeRender.material.SetColor("_Color", colorList[currentColor]);

        transform = this.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) {
            currentColor = (currentColor + 1) % colorList.Length;
            cubeRender.material.SetColor("_Color", colorList[currentColor]);
            
            transform.Rotate(Vector3.left, 100f);
        }
    }
}
