using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * I guess you know what a character controller is.
 * If you have any questions tho, ask me: xentiiiie@gmail.com
 * 
 * For the movements, I originally used transform.position = newPosition, but this is not a good way for the game to handle collisions and all, so i switched to rigidbody2d movement.
 */

public class CharacterController : MonoBehaviour
{
    [Header("Movement variables")]
    public float speed = 0.5f;
    public float runningSpeed = 1f;
    public float runningTime = 3f;
    public float jumpForce = 1;
    [Space, Header("Jump system references")]
    public LayerMask groundLayers;
    [Space, Header("Animation references")]
    public Sprite bodyRunning;
    public Sprite bodyNormal;
    [Space, Header("UI elements")]
    public Slider staminaBarSlider;
    public Canvas staminaBarCanvas;
    public Color staminaBarUsable;
    public Color staminaBarDepleted;
    public GameObject imageLeft;
    public GameObject imageRight;

    float horizontalSpeed;
    Vector2 velocity;
    Transform t_body;
    Transform t_legs;
    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    float runningTimer = 3;
    bool isRunningTimerDepleted = false;

    void Start()
    {
        runningTimer = runningTime;
        t_body = transform.Find("Ranger body");
        t_legs = transform.Find("Ranger legs");
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {

        if(Input.GetKey(KeyCode.LeftControl) && runningTimer > 0 && !isRunningTimerDepleted) {
            horizontalSpeed = runningSpeed;
            runningTimer -= Time.deltaTime;
        } else {
            horizontalSpeed = speed;

            if(runningTimer < 0)
                isRunningTimerDepleted = true;
            if (runningTimer < runningTime)
                runningTimer += Time.deltaTime / 2;
            else
                isRunningTimerDepleted = false;
        }

        staminaBarSlider.value = runningTimer;
        if(isRunningTimerDepleted)
            staminaBarSlider.transform.Find("Fill").GetComponent<Image>().color = staminaBarDepleted;
        else
            staminaBarSlider.transform.Find("Fill").GetComponent<Image>().color = staminaBarUsable;
        

        //Changement de la direction des sprites
        if (Input.GetKey(KeyCode.A)) {
            t_body.GetComponent<SpriteRenderer>().flipX = true;
            t_legs.GetComponent<SpriteRenderer>().flipX = true;
            t_legs.GetComponent<Animator>().SetBool("isRunning", true);
        } else
        if(Input.GetKey(KeyCode.D)) {
            t_body.GetComponent<SpriteRenderer>().flipX = false;
            t_legs.GetComponent<SpriteRenderer>().flipX = false;
            t_legs.GetComponent<Animator>().SetBool("isRunning", true);
        } else
            t_legs.GetComponent<Animator>().SetBool("isRunning", false);

        //Saut
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) {
            print("jump");
            rb.velocity = Vector2.up * jumpForce;
        }

        //Mouvement
        if (Input.GetKey(KeyCode.A))
            rb.velocity = new Vector2(-horizontalSpeed, rb.velocity.y);
        else {
            if (Input.GetKey(KeyCode.D))
                rb.velocity = new Vector2(+horizontalSpeed, rb.velocity.y);
            else
                rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if(runningTimer >= 3)
            staminaBarCanvas.gameObject.SetActive(false);
        else
            staminaBarCanvas.gameObject.SetActive(true);
        
        

        if(transform.position.x < -6.4f)
            imageRight.gameObject.SetActive(true);
        else
            imageRight.gameObject.SetActive(false);
        

        if (transform.position.x > 7.4f)
            imageLeft.gameObject.SetActive(true);
        else
            imageLeft.gameObject.SetActive(false);
        

    }

    bool IsGrounded() {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, .1f, groundLayers);
        return raycastHit2D.collider != null;
    }

}
