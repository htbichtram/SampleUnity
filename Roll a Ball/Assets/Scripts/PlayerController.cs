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
    public List<GameObject> selectedPickUps;   
    public ObjectManager objectManager = new ObjectManager(); 

    public List<GameObject> dyingPickups;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        count = 0;
        
        SetCountText(); 
        winText.text = "";
        selectedPickUps = new List<GameObject>();
        dyingPickups = new List<GameObject>();
        
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
                // if (!selectedPickUps.Contains(pickUpHitPoint.collider))
                    // selectedPickUps.Add(pickUpHitPoint.collider);
            // clicked on ground
            } else if (Physics.Raycast(ray, out groundHitPoint, 1000.0f, groundMask)) {
                Debug.Log("clicked on ground");
                GameObject pickupObject = prefabObject.Spawn(new Vector3(groundHitPoint.point.x, 0.5f, groundHitPoint.point.z));                
                // objectManager.ActiveObject(new Vector3(groundHitPoint.point.x, 0.5f, groundHitPoint.point.z));
                pickupObject.GetComponent<Animator>().SetInteger("chooseColor",  (int)Random.Range(0.0f, 2.0f));
                selectedPickUps.Add(pickupObject);
            }    
        } 

        for (int i = 0; i < dyingPickups.Count; i++) {
            if (dyingPickups[i].GetComponent<Animator>().GetCurrentAnimatorStateInfo(2).IsName("Pickup Die Anim")) {  
                dyingPickups[i].Kill();
                dyingPickups.RemoveAt(i);
            }
        }           
    }
    void FixedUpdate()
    {
        if (selectedPickUps.Count != 0 && selectedPickUps[0] != null) {
            GetComponent<Animator>().SetBool("isMoving", true);
            Vector3 movement = new Vector3(selectedPickUps[0].transform.position.x, 0.0f, selectedPickUps[0].transform.position.z);  
            float step =  speed * Time.deltaTime;
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, movement, step);                       
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Pick Up"))
        {            
            selectedPickUps.Remove(other.gameObject);
            GetComponent<Animator>().SetBool("isMoving", false);
            count++;
            // objectManager.StoreObject(other.gameObject);
            other.gameObject.GetComponent<Animator>().SetTrigger("isDying");
            // other.gameObject.Kill();
            dyingPickups.Add(other.gameObject);
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
