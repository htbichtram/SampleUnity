using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rBody;
    public float speed;
    private int count;
    public Text countText;
    public Text winText;
    public GameObject prefabObject;
    public GameObject pickUps;
    public Vector3 worldPosition;
    public GameObject ground;
    public LayerMask groundMask;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        count = 0;
        
        SetCountText(); 
        winText.text = "";
    }    
    
    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonUp(0)){                        
            RaycastHit hitPoint = new RaycastHit();                       
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);            
            
            if (Physics.Raycast(ray, out hitPoint, 1000.0f, groundMask))
                Instantiate(prefabObject, new Vector3(hitPoint.point.x, 0.5f, hitPoint.point.z),  Quaternion.identity, pickUps.transform);

            // Plane plane = new Plane(ground.transform.position, 0);
            // float distance;
            // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // if (plane.Raycast(ray, out distance))
            // {
            //     worldPosition = ray.GetPoint(distance);
            //     Instantiate(prefabObject, new Vector3(worldPosition.x, 0.5f, worldPosition.z),  Quaternion.identity, pickUps.transform);
            // }            
        }
    }
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rBody.AddForce(movement * speed);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 11)
            winText.text = "You Win!";
    }

}
