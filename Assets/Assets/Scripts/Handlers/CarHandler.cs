using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHandler : MonoBehaviour
{
    public List<GameObject> cars = new();
    public Transform startingPoint;
    public Transform finishingPoint;
    public Transform drone;
    private int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        sendANewCar();
    }
    
    public void sendANewCar() { // отправить от туннеля новую машину по списку
        counter++;
        if (counter - 1 >= cars.Count) {
            counter = 0;
        }

        var car = Instantiate(cars[counter], startingPoint.position, transform.rotation);
        var behaviour = car.GetComponent<CarBehaviour>();

        behaviour.car = car.transform;
        behaviour.finishingPoint = finishingPoint;
        behaviour.drone = drone;
        behaviour.handler = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
