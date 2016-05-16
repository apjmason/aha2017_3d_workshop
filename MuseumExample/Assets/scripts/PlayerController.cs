using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public GameObject player;
    public GameObject head;
    private bool hasGyro = false;
    private Vector3 previousLook;
    private Vector3 offset;
    private float curWalkingSpeed;
    private Vector3 translateBy;
    private float velocityNeededToStartWalking = 0.02f;
    private SineFit fitter = new SineFit();
    float maxWalkingSpeed = 1.5f;
    // Use this for initialization
    void Start()
    {
        // Activate the gyroscope
        offset = transform.position - player.transform.position;
        if (SystemInfo.supportsGyroscope)
        {
            hasGyro = true;
            Input.gyro.enabled = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (hasGyro)
        {
            //Quaternion gyroRaw = Input.gyro.attitude;
            //Quaternion gyroLH = new Quaternion(gyroRaw.x, gyroRaw.y, -gyroRaw.z, -gyroRaw.w);
            //Quaternion gyroInUnityCameraCoordinates = Quaternion.Euler(90, 0, 0) * gyroLH;
            //this.transform.rotation = gyroInUnityCameraCoordinates;

            // WALK IN PLACE: MOVE THE CAMERA FORWARD WHEN SIDE-TO-SIDE WALKING IN PLACE MOTION IS DETECTED:
     
            Vector3 lookDirection = head.transform.TransformDirection(new Vector3(0, 0, 1));
            // FIRST: Add recent acceleration input events to the sine fit data stream

            // To avoid inadvertent walking when pickup up or putting down the device (or during other fast movements)
            // only walk if the current look vector is very close to the previous look vector.
            if (Vector3.Dot(lookDirection, previousLook) > 0.98)
            {
                foreach (AccelerationEvent evnt in Input.accelerationEvents)
                {
                    fitter.addSample(evnt.acceleration.x);
                }
            }
            else
            {
                foreach (AccelerationEvent evnt in Input.accelerationEvents)
                {
                    fitter.addSample(0);
                }
            }
            previousLook = 0.1f * lookDirection + 0.9f * previousLook;
            // SECOND: Move forward if the sine curve fit to the acceleration data indicates strong side-to-side movement
            fitter.doFit();
            float curVel = fitter.getVelocity();
            // Cap this... at least with newer iPhones, a velocity below 0.02 seems to just be noise
            curVel = curVel - velocityNeededToStartWalking;
            if (curVel < 0.0f)
            {
                curVel = 0.0f;
            }

            float targetWalkingSpeed = maxWalkingSpeed * curVel;
            curWalkingSpeed = 0.75f * targetWalkingSpeed + 0.25f * curWalkingSpeed;
            Vector3 n = new Vector3(lookDirection.x, 0.0f, lookDirection.z);
            Vector3 moveDirection = Vector3.Normalize(n);

            translateBy = moveDirection * curWalkingSpeed;
        }
        //transform.Translate(translateBy, Space.World);
        player.transform.Translate(translateBy, Space.World);
        transform.position = player.transform.position + offset;
    }
}
