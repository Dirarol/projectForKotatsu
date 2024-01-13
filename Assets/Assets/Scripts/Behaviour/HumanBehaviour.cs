using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBehaviour : MonoBehaviour
{
    public Transform target;
    public float speed;
    private float usedSpeed;
    private bool canGo;

    void Start()
    {}
    
    void Update()
    {        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            canGo = !canGo;
        }

        if (canGo) 
        {
            Vector3 direction = new(target.position.x, transform.position.y, target.position.z);
            transform.LookAt(direction);
        }  
    }

    void FixedUpdate()
    {
        if (!canGo) {
            // если не была нажата кнопка "Y" то, соответственно, человек не идет
            // код далее - снижение скорости, если она и так не на нуле
            // это нужно, чтобы у нас колясочник был не на реактивной тяге и моментально разгонялся
            if (usedSpeed > 0) usedSpeed -= speed * 0.03f;
            return; 
        } else if (usedSpeed < speed) usedSpeed += speed * 0.03f; // постепенный набор скорости

        Vector3 a = transform.position;
        Vector3 b = new(target.position.x, transform.position.y, target.position.z); // соответственно позиция самого дрона (кроме Y, он остается таким же, дабы наш колясочник не улетел)

        transform.position = Vector3.MoveTowards(a, b, usedSpeed);
    }
}
