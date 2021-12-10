using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * The fire script contains an enum called FireState, that will determine the current state of the fire. This is to choose which sprite is currently used.
 * 
 */

public class Fire : MonoBehaviour {
    public enum FireState { Charcoal, SomeEmbers, ALittleFire, AFire, AGoodFire }
    [Header("Variables")]
    public float fireMaxTime;
    [Header("Graphics references")]
    public Sprite[] fireSprites;
    public ParticleSystem smokeEffect;
    public ParticleSystem burnObjectEffect;
    public Slider fireSlider;
    public Color gazBurningSliderColor;
    public Color baseSliderColor;

    ParticleSystem.EmissionModule smokeEffectEmission;

    FireState _currentFireState;
    public FireState CurrentFireState {
        get { return _currentFireState; }
        set {
            if (_currentFireState == value) return;
            if (isGazBurning) return;
            _currentFireState = value;
            UpdateVisuals();
        }
    }

    float fireTimer;
    float emberTimer = 5;
    bool isGazBurning;

    private void Awake() {
        smokeEffectEmission = smokeEffect.emission;
        smokeEffectEmission.enabled = true;
    }

    void Start() {
        CurrentFireState = FireState.AFire;
        fireTimer = 19;
        isGazBurning = false;
        fireSlider.maxValue = fireMaxTime;
    }

    void Update() {
        if (!isGazBurning) {
            if (RainManager.Instance.isRaining) {
                fireTimer -= Time.deltaTime * 1.5f;
                fireTimer = Mathf.Clamp(fireTimer, 0, fireMaxTime);
            }
            else {
                fireTimer -= Time.deltaTime;
                fireTimer = Mathf.Clamp(fireTimer, 0, fireMaxTime);
            }
        }

        if (fireTimer < 10)
            CurrentFireState = FireState.ALittleFire;
        if (fireTimer >= 10 && fireTimer < 20)
            CurrentFireState = FireState.AFire;
        if (fireTimer >= 20 && fireTimer <= 30)
            CurrentFireState = FireState.AGoodFire;
        
        if(fireTimer == 0) {
            CurrentFireState = FireState.SomeEmbers;
            emberTimer -= Time.deltaTime;
            emberTimer = Mathf.Clamp(emberTimer, 0, 5);

            if(emberTimer == 0) {
                CurrentFireState = FireState.Charcoal;

                GameManager.Instance.GameIsLost();

            }
        }

        fireSlider.value = fireTimer;
        if (isGazBurning)
            fireSlider.transform.Find("Fill").GetComponent<Image>().color = gazBurningSliderColor;
        else
            fireSlider.transform.Find("Fill").GetComponent<Image>().color = baseSliderColor;

    }

    void BurnSomething(Holdable something) {
        
        if(CurrentFireState == FireState.Charcoal)
            return;

        if (!(CurrentFireState == FireState.SomeEmbers)) {

            FuelTank fuelTank;
            if(something.TryGetComponent(out fuelTank)) {

                if (isGazBurning) return;

                isGazBurning = true;
                Invoke("GazBurned", fuelTank.capacity*10);
                UpdateVisuals();
                smokeEffectEmission.rateOverTime = 10;
            }

            fireTimer += something.burnTime - GameManager.Instance.difficulty/(10+GameManager.Instance.difficulty/2);
            Destroy(something.gameObject);

            fireTimer = Mathf.Clamp(fireTimer, 0, fireMaxTime);

        } else {
            fireTimer = 10;
            emberTimer = 5 - GameManager.Instance.difficulty/4;
            CurrentFireState = FireState.ALittleFire;
            Destroy(something.gameObject);
        }

        burnObjectEffect.Play();

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        print("Trigger enter");
        print(collision.gameObject.layer);
        if(collision.gameObject.layer == 9 || collision.gameObject.layer == 11) {
            BurnSomething(collision.GetComponent<Holdable>());
        }
    }

    void UpdateVisuals() {
        SpriteRenderer sp = GetComponent<SpriteRenderer>();
        sp.sprite = fireSprites[(int)CurrentFireState];

        if((int)CurrentFireState <= 1)
            GetComponent<Animator>().enabled = false;
        else
            GetComponent<Animator>().enabled = true;
        

        smokeEffectEmission.rateOverTime =  2*(int)CurrentFireState;

    }

    void GazBurned() {
        isGazBurning = false;
        UpdateVisuals();
    }

}
