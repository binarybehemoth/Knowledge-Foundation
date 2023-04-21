using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Block : MonoBehaviour{
    // Start is called before the first frame update

    public int i;
    public float force = 250f;
    public bool push = false;

    private int level;
    private Rigidbody rb;
    private Vector3 f;
    void Start()
    {
        level = Mathf.FloorToInt(i / 3f);
        rb = GetComponent<Rigidbody>();
        if (level % 2 == 0){
            f = (i % 3 == 0) ? (new Vector3(-force, 0f, 0f))
                : (i % 3 == 1) ? (new Vector3(force, 0f, 0f))
                : (new Vector3(-force, 0f, 0f));
        } else {
            f = (i % 3 == 0) ? (new Vector3(0f, 0f, force))
                : (i % 3 == 1) ? (new Vector3(0, 0f, -force))
                : (new Vector3(0f, 0f, force));
        }
    }

    // Update is called once per frame
    void FixedUpdate(){
        if (push)
        {
            transform.position += f / 1000f;
            float x = transform.localPosition.x;
            float z = transform.localPosition.z;
            if (x<-5f || x> 5f || z<-5f || z>5f) push = false;
        }
    }

    public void displayDetails(int s){
        Topic t = (s == 1) ? GameObject.Find("Game Controller").GetComponent<GameController>().t6[i] :
                  (s == 2) ? GameObject.Find("Game Controller").GetComponent<GameController>().t7[i] :
                             GameObject.Find("Game Controller").GetComponent<GameController>().t8[i];
        GameObject.Find("Details").GetComponent<TextMeshProUGUI>().text = "<color=red>" + t.grade + "</color>: " +
                                                                          "<color=yellow>" + t.domain + "</color>\n" +
                                                                          "<color=green>" + t.cluster + "</color>\n" +
                                                                          "<color=white>" + t.standarddescription + "</color>";
 
    }
}
