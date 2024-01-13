using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    Rigidbody rb;
    public Transform FL, FR, RL, RR;
    public Vector3 CenterOfMass;
    public float liftSensitivity;
    public float tiltSensitivity;
    public float yawSensitivity;
    public bool CombineYaw = true;
    public float stabilizationTorque;
    public float turbulanceAmount, turbulanceFrequency, extraLiftAmountHor, extraLiftAmountVer;
    float phase = 0;
    float[] x = { 0, 0, 0, 0 };
    float counterx, speedx, stabilizeX, stabilizeZ;
    public Material RotorBladeMaterial;
    public Transform[] DispRotor;
    public Texture[] BlurAmount;

    public float rpm = 0;
    bool motorswitch = false;
    float coolDownReset = 0;
    public Camera primaryCamera,secondaryCamera;
    public Light rightLight,leftLight;
    public Material BlinkerMat;
    bool camswitched;
    int blinkermode=0;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.centerOfMass = CenterOfMass;
        DisplayRotor();
        if (motorswitch == true)
        {
            Lift();
            Vertical();
            Horizontal();
            Yaw();
            TiltCheck();
            Reset();
            Hover();
            TurbulanceFunction();
            Turbulance();
        }
    }
    void Update()
    {
        CamSwitch();
        LightSwitch();
        Blinkers();
    }
    public void Lift()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.AddForceAtPosition(transform.up * (liftSensitivity + rb.mass * 9.81f / 4), FL.transform.position);
            rb.AddForceAtPosition(transform.up * (liftSensitivity + rb.mass * 9.81f / 4), FR.transform.position);
            rb.AddForceAtPosition(transform.up * (liftSensitivity + rb.mass * 9.81f / 4), RL.transform.position);
            rb.AddForceAtPosition(transform.up * (liftSensitivity + rb.mass * 9.81f / 4), RR.transform.position);
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            rb.AddForceAtPosition(new Vector3(0, 1, 0) * -(liftSensitivity + rb.mass * 9.81f / 4), FL.transform.position);
            rb.AddForceAtPosition(new Vector3(0, 1, 0) * -(liftSensitivity + rb.mass * 9.81f / 4), FR.transform.position);
            rb.AddForceAtPosition(new Vector3(0, 1, 0) * -(liftSensitivity + rb.mass * 9.81f / 4), RL.transform.position);
            rb.AddForceAtPosition(new Vector3(0, 1, 0) * -(liftSensitivity + rb.mass * 9.81f / 4), RR.transform.position);
        }
    }

    public void Vertical()
    {
        rb.AddForceAtPosition(transform.up * -Input.GetAxis("Vertical") * tiltSensitivity, FL.transform.position);
        rb.AddForceAtPosition(transform.up * -Input.GetAxis("Vertical") * tiltSensitivity, FR.transform.position);
        rb.AddForceAtPosition(transform.up * Input.GetAxis("Vertical") * tiltSensitivity, RL.transform.position);
        rb.AddForceAtPosition(transform.up * Input.GetAxis("Vertical") * tiltSensitivity, RR.transform.position);
    }

    public void Horizontal()
    {
        rb.AddForceAtPosition(transform.up * -Input.GetAxis("Horizontal") * tiltSensitivity, FL.transform.position);
        rb.AddForceAtPosition(transform.up * -Input.GetAxis("Horizontal") * tiltSensitivity, RL.transform.position);
        rb.AddForceAtPosition(transform.up * Input.GetAxis("Horizontal") * tiltSensitivity, FR.transform.position);
        rb.AddForceAtPosition(transform.up * Input.GetAxis("Horizontal") * tiltSensitivity, RR.transform.position);

    }
    public void Yaw()
    {
        if (CombineYaw == true)
        {
            if (Input.GetAxis("Horizontal") > 0.99f || Input.GetKey(KeyCode.E))
            {
                rb.AddForceAtPosition(transform.right * yawSensitivity, FL.transform.position);
                rb.AddForceAtPosition(transform.right * yawSensitivity, FR.transform.position);
                rb.AddForceAtPosition(transform.right * -yawSensitivity, RL.transform.position);
                rb.AddForceAtPosition(transform.right * -yawSensitivity, RR.transform.position);

            }

            if (Input.GetAxis("Horizontal") < -0.99f || Input.GetKey(KeyCode.Q))
            {
                rb.AddForceAtPosition(transform.right * -yawSensitivity, FL.transform.position);
                rb.AddForceAtPosition(transform.right * -yawSensitivity, FR.transform.position);
                rb.AddForceAtPosition(transform.right * yawSensitivity, RL.transform.position);
                rb.AddForceAtPosition(transform.right * yawSensitivity, RR.transform.position);
            }

        }
        else if (CombineYaw == false)
        {
            if (Input.GetKey(KeyCode.E))
            {
                rb.AddForceAtPosition(transform.right * yawSensitivity, FL.transform.position);
                rb.AddForceAtPosition(transform.right * yawSensitivity, FR.transform.position);
                rb.AddForceAtPosition(transform.right * -yawSensitivity, RL.transform.position);
                rb.AddForceAtPosition(transform.right * -yawSensitivity, RR.transform.position);

            }

            if (Input.GetKey(KeyCode.Q))
            {
                rb.AddForceAtPosition(transform.right * -yawSensitivity, FL.transform.position);
                rb.AddForceAtPosition(transform.right * -yawSensitivity, FR.transform.position);
                rb.AddForceAtPosition(transform.right * yawSensitivity, RL.transform.position);
                rb.AddForceAtPosition(transform.right * yawSensitivity, RR.transform.position);
            }
        }

    }

    public void TiltCheck()
    {
        var turnAnglex = transform.eulerAngles.x;
        if (transform.eulerAngles.x > 180)
            turnAnglex = transform.eulerAngles.x - 360;

        var turnAnglez = transform.eulerAngles.z;
        if (transform.eulerAngles.z > 180)
            turnAnglez = transform.eulerAngles.z - 360;

        if (turnAnglex < 90 || turnAnglex > -90 || turnAnglez < 90 || turnAnglez > -90)
        {



            counterx = Mathf.Abs(turnAnglex) / 100;
            speedx = turnAnglex * turnAnglex / 500;

            if (turnAnglex < 0)
                speedx = -speedx;

            if (turnAnglex > 30)
            {
                rb.AddForceAtPosition(transform.up * tiltSensitivity * counterx, FL.transform.position);
                rb.AddForceAtPosition(transform.up * tiltSensitivity * counterx, FR.transform.position);
                rb.AddForceAtPosition(transform.up * -tiltSensitivity * counterx, RL.transform.position);
                rb.AddForceAtPosition(transform.up * -tiltSensitivity * counterx, RR.transform.position);
            }

            rb.AddForceAtPosition(transform.forward * stabilizationTorque * speedx, FL.transform.position);
            rb.AddForceAtPosition(transform.forward * stabilizationTorque * speedx, FR.transform.position);
            rb.AddForceAtPosition(transform.forward * stabilizationTorque * speedx, RL.transform.position);
            rb.AddForceAtPosition(transform.forward * stabilizationTorque * speedx, RR.transform.position);

            ExtraLift(Mathf.Abs(speedx));

            if (turnAnglex < -30)
            {
                rb.AddForceAtPosition(transform.up * -tiltSensitivity * counterx, FL.transform.position);
                rb.AddForceAtPosition(transform.up * -tiltSensitivity * counterx, FR.transform.position);
                rb.AddForceAtPosition(transform.up * tiltSensitivity * counterx, RL.transform.position);
                rb.AddForceAtPosition(transform.up * tiltSensitivity * counterx, RR.transform.position);
            }


            if (turnAnglez > 35)
            {
                rb.AddForceAtPosition(transform.up * -tiltSensitivity, FL.transform.position);
                rb.AddForceAtPosition(transform.up * -tiltSensitivity, RL.transform.position);
                rb.AddForceAtPosition(transform.up * tiltSensitivity, FR.transform.position);
                rb.AddForceAtPosition(transform.up * tiltSensitivity, RR.transform.position);
            }

            if (turnAnglez > 30)
            {
                rb.AddForceAtPosition(-transform.right * stabilizationTorque, FL.transform.position);
                rb.AddForceAtPosition(-transform.right * stabilizationTorque, FR.transform.position);
                rb.AddForceAtPosition(-transform.right * stabilizationTorque, RL.transform.position);
                rb.AddForceAtPosition(-transform.right * stabilizationTorque, RR.transform.position);
                ExtraLift();

            }

            if (turnAnglez < -35)
            {
                rb.AddForceAtPosition(transform.up * tiltSensitivity, FL.transform.position);
                rb.AddForceAtPosition(transform.up * tiltSensitivity, RL.transform.position);
                rb.AddForceAtPosition(transform.up * -tiltSensitivity, FR.transform.position);
                rb.AddForceAtPosition(transform.up * -tiltSensitivity, RR.transform.position);
            }

            if (turnAnglez < -30)
            {
                rb.AddForceAtPosition(transform.right * stabilizationTorque, FL.transform.position);
                rb.AddForceAtPosition(transform.right * stabilizationTorque, FR.transform.position);
                rb.AddForceAtPosition(transform.right * stabilizationTorque, RL.transform.position);
                rb.AddForceAtPosition(transform.right * stabilizationTorque, RR.transform.position);
                ExtraLift();

            }

            //Stabilize
            stabilizeX = turnAnglex / 40;

            rb.AddForceAtPosition(transform.up * tiltSensitivity * stabilizeX, FL.transform.position);
            rb.AddForceAtPosition(transform.up * tiltSensitivity * stabilizeX, FR.transform.position);
            rb.AddForceAtPosition(transform.up * -tiltSensitivity * stabilizeX, RL.transform.position);
            rb.AddForceAtPosition(transform.up * -tiltSensitivity * stabilizeX, RR.transform.position);



            stabilizeZ = turnAnglez / 40;

            rb.AddForceAtPosition(transform.up * -tiltSensitivity * stabilizeZ, FL.transform.position);
            rb.AddForceAtPosition(transform.up * -tiltSensitivity * stabilizeZ, RL.transform.position);
            rb.AddForceAtPosition(transform.up * tiltSensitivity * stabilizeZ, FR.transform.position);
            rb.AddForceAtPosition(transform.up * tiltSensitivity * stabilizeZ, RR.transform.position);

        }
    }

    public void Reset()
    {
        if (rb.velocity.magnitude < 1 && Vector3.Dot(transform.up, Vector3.down) > 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            transform.rotation = new Quaternion(0, 0, 0, 0);
            
        }
        if(Input.GetKey(KeyCode.R)&&Mathf.RoundToInt(coolDownReset)==0)
        {
            coolDownReset = 1;
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        if(coolDownReset>0)
        coolDownReset-=Time.fixedDeltaTime;

    }
    public void Hover()
    {
        rb.AddForceAtPosition(transform.up * ((rb.mass * 9.81f / 4)), FL.transform.position);
        rb.AddForceAtPosition(transform.up * ((rb.mass * 9.81f / 4)), FR.transform.position);
        rb.AddForceAtPosition(transform.up * ((rb.mass * 9.81f / 4)), RL.transform.position);
        rb.AddForceAtPosition(transform.up * ((rb.mass * 9.81f / 4)), RR.transform.position);
    }
    public float[] TurbulanceFunction()
    {
        phase += Time.deltaTime * turbulanceFrequency;
        x[0] = Mathf.PerlinNoise(phase, phase);
        x[1] = Mathf.PerlinNoise(phase + 0.15f, phase + 0.15f);
        x[2] = Mathf.PerlinNoise(phase + 0.2f, phase + 0.2f);
        x[3] = Mathf.PerlinNoise(phase + 0.35f, phase + 0.35f);

        return x;
    }
    public void Turbulance()
    {
        rb.AddForceAtPosition(transform.up * turbulanceAmount * (0.5f - TurbulanceFunction()[0]), FL.transform.position);
        rb.AddForceAtPosition(transform.up * turbulanceAmount * (0.5f - TurbulanceFunction()[2]), FR.transform.position);
        rb.AddForceAtPosition(transform.up * turbulanceAmount * (0.5f - TurbulanceFunction()[3]), RL.transform.position);
        rb.AddForceAtPosition(transform.up * turbulanceAmount * (0.5f - TurbulanceFunction()[1]), RR.transform.position);
    }
    public void ExtraLift()
    {
        rb.AddForceAtPosition(new Vector3(0, 1, 0) * extraLiftAmountHor, FL.transform.position);
        rb.AddForceAtPosition(new Vector3(0, 1, 0) * extraLiftAmountHor, FR.transform.position);
        rb.AddForceAtPosition(new Vector3(0, 1, 0) * extraLiftAmountHor, RL.transform.position);
        rb.AddForceAtPosition(new Vector3(0, 1, 0) * extraLiftAmountHor, RR.transform.position);
    }
    public void ExtraLift(float passedTurnAngleX)
    {
        rb.AddForceAtPosition(new Vector3(0, 1, 0) * passedTurnAngleX * extraLiftAmountVer, FL.transform.position);
        rb.AddForceAtPosition(new Vector3(0, 1, 0) * passedTurnAngleX * extraLiftAmountVer, FR.transform.position);
        rb.AddForceAtPosition(new Vector3(0, 1, 0) * passedTurnAngleX * extraLiftAmountVer, RL.transform.position);
        rb.AddForceAtPosition(new Vector3(0, 1, 0) * passedTurnAngleX * extraLiftAmountVer, RR.transform.position);
    }
    public void DisplayRotor()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            rpm += Time.deltaTime * 500;
        if (rpm > 2000)
        {
            motorswitch = true;
            rpm = 2000;
        }

        DispRotor[0].transform.RotateAround(DispRotor[0].transform.position, transform.up, Time.deltaTime * rpm * (1 + TurbulanceFunction()[0]));
        DispRotor[1].transform.RotateAround(DispRotor[1].transform.position, transform.up, Time.deltaTime * rpm * (1 + TurbulanceFunction()[1]));
        DispRotor[2].transform.RotateAround(DispRotor[2].transform.position, transform.up, Time.deltaTime * rpm * (1 + TurbulanceFunction()[2]));
        DispRotor[3].transform.RotateAround(DispRotor[3].transform.position, transform.up, Time.deltaTime * rpm * (1 + TurbulanceFunction()[3]));

        if (rpm < 1200)
            RotorBladeMaterial.SetTexture("_MainTex", BlurAmount[0]);
        else if (rpm > 1200 && rpm < 1500)
            RotorBladeMaterial.SetTexture("_MainTex", BlurAmount[1]);
        else if (rpm > 1500 && rpm < 1800)
            RotorBladeMaterial.SetTexture("_MainTex", BlurAmount[2]);
        else if (rpm > 1500 && rpm < 2000)
            RotorBladeMaterial.SetTexture("_MainTex", BlurAmount[3]);
        else
            RotorBladeMaterial.SetTexture("_MainTex", BlurAmount[4]);
    }
    public void CamSwitch()
    {
        if(Input.GetKeyUp(KeyCode.C))
            camswitched = !camswitched;
        secondaryCamera.targetDisplay = camswitched?0:1;
    }
    public void LightSwitch()
    {
        if(Input.GetKeyUp(KeyCode.L))
        {
            rightLight.enabled = !rightLight.enabled;
            leftLight.enabled = !leftLight.enabled;
        }    
    }
    public void Blinkers()
    {
        if(Input.GetKeyUp(KeyCode.B))
        {
            blinkermode++;
        }
        if(blinkermode%3==0)
        {
            BlinkerMat.SetColor("_EmissionColor", new Color (Mathf.PingPong(Time.time,0.735f),Mathf.PingPong(Time.time*1/0.735f,1),0,0));

        }
        else if(blinkermode%3==1)
        {
            BlinkerMat.SetColor("_EmissionColor", Color.black);

        }
        else if(blinkermode%3==2)
        {
            BlinkerMat.SetColor("_EmissionColor", new Color(0.735f,1,0));
        }
    }
}
