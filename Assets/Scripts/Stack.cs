using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stack : MonoBehaviour
{
    public static int sIndex;
    public int stackIndex;
    private Ray ray;
    private RaycastHit hit;
    public static int cameraState = 0;
    private Vector3 cPos, fPos;
    private Transform target;
    public bool tested = false;

    void Update()
    {
        cPos = Camera.main.transform.position;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)){
            if (hit.collider.tag != "Block") return;
            if (Input.GetMouseButtonDown(0)){
                //Camera.main.GetComponent<CameraController>().target = hit.transform.parent;
                switch (hit.transform.parent.GetComponent<Stack>().stackIndex)
                {
                    case 1:
                        fPos = new Vector3(-20f, 13f, -10f);
                        break;
                    case 2:
                        fPos = new Vector3(0f, 13f, -10f);
                        break;
                    case 3:
                        fPos = new Vector3(20f, 13f, -10f);
                        break;
                }
                target = hit.transform.parent;
                cameraState = 1;
            } else if (Input.GetMouseButtonDown(1)){
                int s = hit.transform.parent.GetComponent<Stack>().stackIndex;
                if (s != stackIndex) return;
                if (!tested){
                    tested = true;
                    GameObject.Find("Game Controller").GetComponent<GameController>().PullGlassBlocks(s);
                } else {
                    tested = false;
                    GameObject.Find("Game Controller").GetComponent<GameController>().BuildStack(s);
                }
            } else {
                hit.collider.transform.gameObject.GetComponent<Block>().displayDetails(hit.transform.parent.GetComponent<Stack>().stackIndex);
            }
        }
    }

    void LateUpdate(){
        switch (cameraState){
            case 1:
                if (((cPos - fPos).magnitude <= 0.5f))
                {
                    cameraState = 2;
                    return;
                }
                if ((cPos - fPos).magnitude > 0.5f)
                    Camera.main.transform.position = new Vector3(cPos.x + (fPos.x - cPos.x) * Time.deltaTime,
                                                                 cPos.y + (fPos.y - cPos.y) * Time.deltaTime,
                                                                 cPos.z + (fPos.z - cPos.z) * Time.deltaTime);

                Quaternion lookOnLook = Quaternion.LookRotation(target.position - cPos);
                Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookOnLook, Time.deltaTime);
        break;
            case 2:
                Camera.main.GetComponent<CameraController>().target = target;
                cameraState = 0;
                break;
        }
    }
}
