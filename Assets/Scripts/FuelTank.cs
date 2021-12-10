using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * I plan on making the fuel tank more complex later. For now this script is just used to detect if the currently held item is a fuel tank.
 */

public class FuelTank : MonoBehaviour
{
    public float capacity;

    private void Start() {
        capacity = Random.Range(0.1f, 1);
    }

}
