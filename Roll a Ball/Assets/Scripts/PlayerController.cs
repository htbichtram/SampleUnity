using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rBody;    
    public float speed;
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

    public GameObject menuCanvas;
    public GameObject menuTitle;
    public GameObject optionsButton;
    public GameObject playButton;
    public GameObject backButton;
    public GameObject pauseButton;
    public GameObject gamePlayCanvas;
    public GameObject hp;
    public GameObject hpText;
    public GameObject timeText;
    public bool isGamePausing;
    public float playerHP;
    public float playerHPDownPerHit = 0.2f;
    private float startPlayTime;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();

        selectedPickUps = new List<GameObject>();

        GetComponent<Animator>().SetBool("isMoving", false);
        dyingPar.SetActive(false);
        revivalPar.SetActive(false);

        PauseGame();
        playButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "PLAY";
        menuTitle.GetComponent<Text>().text = "MAIN MENU";
        playerHP = 1.0f;
        startPlayTime = Time.time;
    }
    
    // Update is called once per frame
    void Update() {
        if (isGamePausing) return;
        GetComponent<Animator>().enabled = true;

        if (revivalParStartTime != 0 && (Time.time - revivalParStartTime) >= revivalParTime) {
            revivalPar.SetActive(false);
            revivalParStartTime = 0;

            playerHP = 1.0f;
        }

        if (GetComponent<Animator>().GetBool("isDying") || revivalParStartTime != 0) return;

        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject()){                        
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
                int pickUpColor = (int)Random.Range(0.0f, 2.9f);
                pickupObject.GetComponent<Animator>().SetInteger("chooseColor",  pickUpColor);
                
                selectedPickUps.Add(pickupObject);
                if (pickUpColor == 0)
                    pickupObject.GetComponent<PickUp>().firePre = fireParPrefab.Spawn(new Vector3(groundHitPoint.point.x, 1.5f, groundHitPoint.point.z));
            }    
        }       

        updateHealthAndPlayTime();
    }
    void FixedUpdate()
    {
        if (isGamePausing) return;

        if (GetComponent<Animator>().GetBool("isDying") || revivalParStartTime != 0) return;

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
        if (isGamePausing) return;

        if (other.gameObject.CompareTag("Pick Up"))
        {            
            playerHP -= playerHPDownPerHit;

            selectedPickUps.Remove(other.gameObject);

            GetComponent<Animator>().SetBool("isMoving", false);
            if (playerHP <= 0.0f) {
                GetComponent<Animator>().SetBool("isDying", true);  
                dyingPar.SetActive(true);
            }

            other.gameObject.GetComponent<Animator>().SetTrigger("isDying");
            other.gameObject.GetComponent<PickUp>().explorePre = exploreParPrefab.Spawn(other.gameObject.transform.position);            
        }
    }

    public void PauseGame(){
        isGamePausing = true;
        playButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "RESUME";
        optionsButton.SetActive(true);
        backButton.SetActive(false);
        pauseButton.SetActive(false);
        menuTitle.GetComponent<Text>().text = "INGAME MENU";
        menuCanvas.SetActive(true);

        GetComponent<Animator>().enabled = false;
    }

    public void ResumeGame() {
        isGamePausing = false;
        menuCanvas.SetActive(false);
        pauseButton.SetActive(true);        
    }

    public void updateHealthAndPlayTime(){
        hp.GetComponent<Image>().fillAmount = playerHP;
        hpText.GetComponent<Text>().text = (int)(playerHP * 10.0f) + " / 10";
        
        int playSecond = (int)(Time.time - startPlayTime);
        int playMinute = playSecond % 60;
        playMinute = playMinute / 60;
        timeText.GetComponent<Text>().text = TimeToText(playMinute) + ":" + TimeToText(playSecond);
    }

    public string TimeToText(int time) {
        return (time > 9 ? "" + time : "0" + time);
    }

    public void PressedPlay() {
        if (isGamePausing){
            ResumeGame();
        } else {
            PauseGame();
        }
    }

    public void PressedOptions() {
        optionsButton.SetActive(false);
        backButton.SetActive(true);
        menuTitle.GetComponent<Text>().text = "OPTION MENU";
    }

    public void PressedExit() {

    }

    public void PressedBack(){
        optionsButton.SetActive(true);
        backButton.SetActive(false);
        menuTitle.GetComponent<Text>().text = "INGAME MENU";
    }

    public void PressedPause(){
        PauseGame();
    }
}
