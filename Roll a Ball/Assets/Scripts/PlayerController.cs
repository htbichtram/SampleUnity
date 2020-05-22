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
    public GameObject fireParPrefab;
    public GameObject exploreParPrefab;
    public Vector3 worldPosition;
    public GameObject ground;
    public LayerMask groundMask;
    public LayerMask pickUpMask;
    private List<GameObject> selectedPickUps;
    public GameObject dyingPar;
    public GameObject revivalPar;
    public float revivalParTime = 4.0f;
    [HideInInspector]
    public float revivalParStartTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        count = 0;
        
        SetCountText(); 
        winText.text = "";
        selectedPickUps = new List<GameObject>();

        GetComponent<Animator>().SetBool("isMoving", false);
        dyingPar.SetActive(false);
        revivalPar.SetActive(false);
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
                int pickUpColor = 0;//(int)Random.Range(0.0f, 2.9f);
                pickupObject.GetComponent<Animator>().SetInteger("chooseColor",  pickUpColor);
                
                selectedPickUps.Add(pickupObject);
                if (pickUpColor == 0)
                    pickupObject.GetComponent<PickUp>().firePre = fireParPrefab.Spawn(new Vector3(groundHitPoint.point.x, 1.5f, groundHitPoint.point.z));
            }    
        }

        if (revivalParStartTime != 0 && (Time.time - revivalParStartTime) >= revivalParTime) {
            revivalPar.SetActive(false);
            revivalParStartTime = 0;
        }
    }
    void FixedUpdate()
    {
        if (GetComponent<Animator>().GetBool("isDying"))
            return;
        if (selectedPickUps.Count != 0 && selectedPickUps[0] != null) {
            Vector3 targetPostition = new Vector3( selectedPickUps[0].transform.position.x, 
                                        transform.position.y, 
                                        selectedPickUps[0].transform.position.z ) ;
            transform.LookAt( targetPostition ) ;

            GetComponent<Animator>().SetBool("isMoving", true);  

            Vector3 movement = new Vector3(selectedPickUps[0].transform.position.x, 0.0f, selectedPickUps[0].transform.position.z);  
            float step =  speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, movement, step);                       
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Pick Up"))
        {            
            selectedPickUps.Remove(other.gameObject);

            count++;
            SetCountText();

            GetComponent<Animator>().SetBool("isMoving", false);
            GetComponent<Animator>().SetBool("isDying", true);  
            dyingPar.SetActive(true);

            other.gameObject.GetComponent<Animator>().SetTrigger("isDying");
            other.gameObject.GetComponent<PickUp>().explorePre = exploreParPrefab.Spawn(other.gameObject.transform.position);            
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
    }

}
