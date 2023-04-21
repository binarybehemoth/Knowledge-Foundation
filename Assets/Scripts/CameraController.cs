using UnityEngine;

public class CameraController : MonoBehaviour{

    public Transform target;
    public float distance = 2.0f;
    public float xSpeed = 1f;
    public float ySpeed = 1f;
    public float yMinLimit = -90f;
    public float yMaxLimit = 90f;
    public float distanceMin = 10f;
    public float distanceMax = 10f;
    public float smoothTime = 2f;
    float rotationYAxis = 0.0f;
    float rotationXAxis = 0.0f;
    float velocityX = 0.0f;
    float velocityY = 0.0f;

    void Awake(){
        // Flush the camera
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    void Start(){
        Vector3 angles = transform.eulerAngles;
        rotationYAxis = angles.y;
        rotationXAxis = angles.x;
        if (GetComponent<Rigidbody>()){
            GetComponent<Rigidbody>().freezeRotation = true;
        }
    }
    
    private int fLimit = 100;
    void FixedUpdate()  {   // stop the camera inertia after mouse up
        fLimit--;
    }

    void LateUpdate(){
        if (Stack.cameraState == 1) return;
        else if (Stack.cameraState == 2){
            Vector3 angles = transform.eulerAngles;
            rotationYAxis = angles.y;
            rotationXAxis = angles.x;
        }
        if (target){
            if (Input.GetMouseButton(0)){
                velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.02f;
                velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
                fLimit = 100;
            }
            if (fLimit > 0)
            {
                rotationYAxis += velocityX;
                rotationXAxis -= velocityY;
                rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
                Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
                Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
                Quaternion rotation = toRotation;
                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
                RaycastHit hit;
                if (Physics.Linecast(target.position, transform.position, out hit)) distance -= hit.distance;
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * negDistance + target.position;
                transform.rotation = rotation;
                transform.position = new Vector3(position.x, 13f, position.z);
                velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
                velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
            }
        }
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) angle += 360F;
        if (angle > 360F) angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

}
