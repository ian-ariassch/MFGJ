using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerElementController : MonoBehaviour
{
    public GameController gc;
    public float elementSwitchInterval = 5f;
    public GameObject airBall, waterBall;
    public int healthPoints, maxHealthPoints;
    public List<AnimationClip> animations;
    public float airCooldown = 5f;

    public float invulnerabilityTime = 1f;
    float airCooldownTimer;

    public float waterCooldown = 5f;
    float waterCooldownTimer;

    float invulnerabilityTimer = 0;

    int earthShield = 2;
    float currentTime;
    string[] elements = new string[3] { "Water", "Air", "Earth" };
    string currentElement;

    Animator anim;
    SpriteRenderer playerSprite;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= Time.deltaTime;
        airCooldownTimer -= Time.deltaTime;
        waterCooldownTimer -= Time.deltaTime;
        invulnerabilityTimer -= Time.deltaTime;
        if(currentTime < 0) 
        {
            currentElement = PickRandomElement();
            switch (currentElement)
            {
                case "Water":
                    anim.Play("waterAnimation");
                    waterCooldownTimer = 0;
                    break;
                case "Air":
                    anim.Play("windAnimation");
                    airCooldownTimer = 0;
                    break;
                case "Earth":
                    anim.Play("earthFull");
                    earthShield = 2;
                    break;
            }

            currentTime = elementSwitchInterval;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            switch (currentElement) 
            {
                case "Air":
                    if(airCooldownTimer < 0)
                    {
                        GameObject spawnedAirBall = Instantiate(airBall, transform.position, Quaternion.identity);
                        Rigidbody2D airballRb = spawnedAirBall.GetComponent<Rigidbody2D>();
                        airballRb.mass = 100000;
                        airballRb.gravityScale = 0;
                        airballRb.AddForce(transform.up * airballRb.mass * 1000);
                        anim.Play("windIdle");
                        airCooldownTimer = airCooldown;
                    }
                    break;
                case "Water":
                    if(waterCooldownTimer < 0)
                    {
                        GameObject spawnedWaterBall = Instantiate(waterBall, transform.position, Quaternion.identity);
                        Rigidbody2D waterBallRb = spawnedWaterBall.GetComponent<Rigidbody2D>();
                        waterBallRb.AddForce(transform.up * waterBallRb.mass * 500);
                        anim.Play("waterIdle");
                        waterCooldownTimer = waterCooldown;
                    }
                    break;
            }
        }

        if(invulnerabilityTimer < 0)
        {
            Color tempColor = playerSprite.color;
            tempColor.a = 1;
            playerSprite.color = tempColor;
        }

        if(healthPoints <= 0)
        {
            Death();
        }
    }

    string PickRandomElement() 
    {
        int randomIndex = Random.Range(0, 3);
        return elements[randomIndex];
    }

    public void TakeDamage()
    {
        if(invulnerabilityTimer < 0)
        {
            if(currentElement == "Earth")
            {
                if(earthShield == 1)
                {
                    anim.Play("earthIdle");
                    earthShield--;
                }
                else if(earthShield == 2)
                {
                    anim.Play("earthBroken");
                    earthShield--;
                }
                else
                {
                    healthPoints--;
                }
            }
            else
            {
                healthPoints--;
            }
            Color tempColor = playerSprite.color;
            tempColor.a = 0.5f;
            playerSprite.color = tempColor; 
            invulnerabilityTimer = invulnerabilityTime;
        }
    }

    public void Heal()
    {
        if(healthPoints < maxHealthPoints)
        healthPoints++;
    }

    void Death()
    {
        Destroy(gameObject.transform.root.gameObject);
        gc.StartCoroutine("GameOver");
    }
}
