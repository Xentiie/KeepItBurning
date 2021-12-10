using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script that makes any object holdable by the player.
 * The pickup time isn't currently used.
 */

[RequireComponent(typeof(Rigidbody2D))]
public class Holdable : MonoBehaviour, System.IEquatable<Holdable> {

    public Sprite itemSprite;
    public float weight;
    public float height;
    public float pickupTime;
    public float burnTime;
    [Space]
    public float currentYPos;

    public bool Equals(Holdable other) {
        if (other is null)
            return false;

        return gameObject == other.gameObject;
    }

    public override bool Equals(object obj) => Equals(obj as Holdable);
    public override int GetHashCode() => (itemSprite, weight, height, pickupTime, burnTime).GetHashCode();

}
