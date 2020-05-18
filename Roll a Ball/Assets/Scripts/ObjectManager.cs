using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static int MAX_OBJECT_NUM = 50;
    public List<GameObject> objectList;

    public ObjectManager() {
        objectList = new List<GameObject>();
    }

    public void AddNewObject(GameObject _object) {
        if(objectList.Count < MAX_OBJECT_NUM) {
            _object.SetActive(false);            
            objectList.Add(_object);
        }        
    }

    public void ActiveObject(Vector3 position) {        
        for(int i = 0; i < objectList.Count; i++) {
            if (!objectList[i].activeSelf) {
                objectList[i].SetActive(true);
                objectList[i].transform.position = position;
                break;
            }
        }
    }

    public void StoreObject(GameObject _object) {
        _object.SetActive(false);
        _object.transform.position = Vector3.zero;
    }
}
