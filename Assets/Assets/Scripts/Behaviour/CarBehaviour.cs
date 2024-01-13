using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
    public Transform car;
    public Transform finishingPoint;
    public Transform drone;
    public CarHandler handler;
    public float speed;
    private float usedSpeed;

    void Start()
    {}

    void FixedUpdate()
    {
        if (drone.position.x >= 22 && drone.position.x <= 59 && car.position.z < drone.position.z) { // проверяем на дороге ли дрон, и вообще находится ли машина до участка дороги, на котором находится дрон
            // TODO чекаем позицию дрона

            if (usedSpeed > 0) usedSpeed -= speed * 0.03f; // постепенно уменьшаем скорость, дабы не было моментальной остановки
        } else if (usedSpeed < speed) usedSpeed += speed * 0.01f; // постепенно повышаем скорость, опять же дабы не было моментального набора скорости

        Vector3 a = transform.position;
        Vector3 b = finishingPoint.position;

        transform.position = Vector3.MoveTowards(a, b, usedSpeed); // ну это просто движение самой машины
        if (Vector3.Distance(a, b) < 2f) {
            handler.sendANewCar();
            Destroy(this);
        }
    }
}
