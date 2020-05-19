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
    public LayerMask pickUpMask;
    public List<Collider> selectedPickUps;   
    public ObjectManager objectManager = new ObjectManager(); 

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        count = 0;
        
        SetCountText(); 
        winText.text = "";
        selectedPickUps = new List<Collider>();
        
        // for (int i = 0; i < ObjectManager.MAX_OBJECT_NUM; i++) {
        //     objectManager.AddNewObject(Instantiate(prefabObject, Vector3.zero,  Quaternion.identity, pickUps.transform));
        // }

        GetComponent<Animator>().SetBool("isMoving", false);
    }
    
    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonUp(0)){                        
            RaycastHit groundHitPoint = new RaycastHit();
            RaycastHit pickUpHitPoint = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);            
            
            // clicked on a pick-up
            if (Physics.Raycast(ray, out pickUpHitPoint, 1000.0f, pickUpMask)) {
                Debug.Log("clicked on a pick-up");
                // pick-up does not exits in selected list
                if (!selectedPickUps.Contains(pickUpHitPoint.collider))
                    selectedPickUps.Add(pickUpHitPoint.collider);
            // clicked on ground
            } else if (Physics.Raycast(ray, out groundHitPoint, 1000.0f, groundMask)) {
                Debug.Log("clicked on ground");
                prefabObject.Spawn(new Vector3(groundHitPoint.point.x, 0.5f, groundHitPoint.point.z));
                // objectManager.ActiveObject(new Vector3(groundHitPoint.point.x, 0.5f, groundHitPoint.point.z));
            }

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
        // float moveHorizontal = Input.GetAxis("Horizontal");
        // float moveVertical = Input.GetAxis("Vertical");

        if (selectedPickUps.Count != 0 && selectedPickUps[0] != null) {                                    
            float moveHorizontal = selectedPickUps[0].transform.position.x - rBody.transform.position.x;
            float moveVertical = selectedPickUps[0].transform.position.z - rBody.transform.position.z;

            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            rBody.AddForce(movement * speed); 
            GetComponent<Animator>().SetBool("isMoving", true);           
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Pick Up"))
        {            
            selectedPickUps.Remove(other);
            rBody.velocity = Vector3.zero;
            rBody.angularVelocity = Vector3.zero;
            GetComponent<Animator>().SetBool("isMoving", false);
            count++;
            // objectManager.StoreObject(other.gameObject);
            other.gameObject.Kill();
            SetCountText();

        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        // if (count >= 11)
        //     winText.text = "You Win!";
    }

}
