using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LogHolder : MonoBehaviour
{
    [Header("Variables")]
    public float throwPower = 50;
    public float holdPower = 15;
    public float rangerHeight = 1;
    [Space, Header("Pickup system references")]
    public Transform logOverlapCheck;
    public LayerMask logLayer;
    [Space, Header("Animation references")]
    public Sprite holdingSprite;
    public Sprite normalSprite;

    int itemCounter = 0;
    List<Holdable> objectsHeld;
    float currentWeightCarried = 0;
    float throwPowerLittle;
    Animator animator;
    SpriteRenderer sp;

    void Start()
    {
        throwPowerLittle = throwPower / 3;
        objectsHeld = new List<Holdable>();
        sp = transform.Find("Ranger body").GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Theses 3 lines only checks if the player is over and object holdable.
        Vector2 topLeft = new Vector2(logOverlapCheck.position.x - logOverlapCheck.localScale.x / 2, logOverlapCheck.position.y + logOverlapCheck.localScale.y / 2);
        Vector2 bottomRight = new Vector2(logOverlapCheck.position.x + logOverlapCheck.localScale.x / 2, logOverlapCheck.position.y - logOverlapCheck.localScale.y / 2);
        bool isOverLog = Physics2D.OverlapArea(topLeft, bottomRight, logLayer);

        if(Input.GetKeyDown(KeyCode.K) && isOverLog && currentWeightCarried < holdPower) {
            PickupItem();
        }
        if(Input.GetKeyDown(KeyCode.L)) {
            if(Input.GetKey(KeyCode.LeftShift)) {
                ThrowItem(true);
            } else {
                ThrowItem();
            }
        }

        // This loop is to update each objects position, so they will follow the player as he moves.
        foreach (Holdable h in objectsHeld) {
            Vector2 newPos = h.transform.position;
            newPos.x = transform.position.x;
            newPos.y = h.currentYPos + transform.position.y;
            h.transform.position = newPos;
        }
        
    }

    void PickupItem() {

        // On récupère tout les "Holdable" de la scène, on les stock dans une liste. Puis on retire tout les objets déjà porté par le joueur.
        List<Holdable> holdablesOnScene = FindObjectsOfType<Holdable>().ToList();
        IEnumerable<Holdable> itemsEnumerable = holdablesOnScene.Except(objectsHeld);
        List<Holdable> itemsPickable = new List<Holdable>();
        foreach (Holdable h in itemsEnumerable) {
            itemsPickable.Add(h);
        }
        Holdable closestItem = GetClosestHoldable(itemsPickable);


        // This loop is for calculating the height at which the next object will be.
        // I simply loop over every objects carried, and add their height variable to nextItemHeight.
        float nextItemHeight = rangerHeight;
        foreach (Holdable h in objectsHeld) {
            nextItemHeight += h.height;
        }
        closestItem.currentYPos = nextItemHeight;


        //I update his rigidbody component to stop him from moving..
        Rigidbody2D itemRb = closestItem.GetComponent<Rigidbody2D>();
        itemRb.freezeRotation = true;
        itemRb.gravityScale = 0;
        itemRb.velocity = Vector2.zero;
        itemRb.inertia = 0f;
        closestItem.GetComponent<BoxCollider2D>().enabled = false;

        // If the object is a fuel tank, I rotate it by -90 degrees to make it look prettier.
        FuelTank ft;
        if (closestItem.TryGetComponent(out ft))
            closestItem.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
        else
            closestItem.transform.rotation = Quaternion.Euler(Vector3.zero);

        closestItem.transform.parent = transform;


        // I increase the counter and add the item to the currently carried objects list.
        itemCounter++;
        objectsHeld.Add(closestItem);


        // Update visuals...
        if (itemCounter > 0) {
            sp.sprite = holdingSprite;
            animator.enabled = false;
        }

        currentWeightCarried += closestItem.weight;

    }

    void ThrowItem(bool isLittleThrow = false) {
        // I decrease the item counter and i get the object to throw
        itemCounter--;
        itemCounter = Mathf.Clamp(itemCounter, 0, 10000);
        if (objectsHeld.Count <= 0) return; 
        Holdable itemToThrow = objectsHeld[objectsHeld.Count-1];
        objectsHeld.RemoveAt(objectsHeld.Count-1);

        //On reset son parent, et on réactive son collider et la gravité.
        itemToThrow.transform.parent = null;
        itemToThrow.GetComponent<Rigidbody2D>().freezeRotation = false;
        itemToThrow.GetComponent<Rigidbody2D>().gravityScale = 1;
        itemToThrow.GetComponent<BoxCollider2D>().enabled = true;

        //I calculate the throw force and apply it to the object
        Vector2 throwForce;
        if (isLittleThrow) {
            if (sp.flipX)
                throwForce = new Vector2(-throwPowerLittle, 30 / 100 * throwPower);
            else
                throwForce = new Vector2(throwPowerLittle, 30 / 100 * throwPower);
        } else {
            if (sp.flipX)
                throwForce = new Vector2(-throwPower, 30 / 100 * throwPower);
            else
                throwForce = new Vector2(throwPower, 30 / 100 * throwPower);
        }
        itemToThrow.GetComponent<Rigidbody2D>().AddForce(throwForce);

        //If the player doesn't hold any objects anymore, I clear the list to be sure and I update the visuals.
        if(itemCounter <= 0) {
            objectsHeld.Clear();
            sp.sprite = normalSprite;
            animator.enabled = true;
        }

        currentWeightCarried -= itemToThrow.weight;

    }

    // This function is supposed to return the closest object from the player, but it is a bit messy. WIP
    Holdable GetClosestHoldable(List<Holdable> holdable) {
        Holdable bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.GetChild(0).position;

        foreach (Holdable potentialTarget in holdable) {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }

}
