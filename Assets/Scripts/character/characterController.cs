﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class characterController : MonoBehaviour
{
    CharacterController controller;

    public float speed = 10.0f;
    float sprintFactor = 2;
	private float jumpSpeed = 15;
    public float gravity = 20.0f;
   
    private Vector3 moveDirection = Vector3.zero;
    
    private Rigidbody rigidBody;
	public bool onGround = true;
	public const int jumpCount = 3;
	private int currentJump = 0;
	private float jumpTimer;
	private bool hasPressedSpace;
    float verticalVelocity;
    bool isSprinting;

    public static float health = 100f;
    
	
    public bool lockCursor = true;

    [SerializeField]
    int tempScore;

    public Animator anim;
	
    public GameObject deathMenuUI;
    public GameObject hintMenu;
    public GameObject deathCamera;
    
    //THIS IS STUFF FOR WEAPONS, ANIMATIONS AND GAME OVER MENUES
    //
    //Gun gun;
    //AnimatorStateInfo info;
    //public DeathMenu deathMenu;
    //



    void Start()
    {
        //FINDS CHARACTER COMPONENTS
        controller = GetComponent<CharacterController>();
		//anim = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;    
		rigidBody = GetComponent<Rigidbody>();
        isSprinting = false;
        tempScore = 0;
        PlayerPrefs.SetInt("collected", tempScore);
    }

    void Update()
    {
        if (lockCursor) { Cursor.lockState = CursorLockMode.Locked; }
        else { Cursor.lockState = CursorLockMode.None; }


        //YEAH THAT LOOKS LIKE AN UPDATE

        //--------------------------------------------INPUT CHECKS-----------------------------------------------
        //CHECKS IF ABLE TO JUMP, IF ABLE, DO JUMP
        if (controller.isGrounded)
        {
            onGround = true;
            currentJump = 0;
            hasPressedSpace = false;
            moveDirection.y = 0;
        }
        else
        {
            onGround = false;
        }

        if (Input.GetKey("space") && (onGround || jumpCount > currentJump))
        {
            hasPressedSpace = true;
            moveDirection.y = jumpSpeed;
            onGround = false;
            currentJump++;
        }

        isSprinting = Input.GetKey(KeyCode.LeftShift) ? true : false;
        

        //GO UP 
        moveDirection.y -= gravity * Time.deltaTime;

        //MOVE BITCH
        controller.Move(moveDirection * Time.deltaTime);
        float translation = isSprinting ? Input.GetAxis("Vertical") * speed * sprintFactor : Input.GetAxis("Vertical") * speed;
        float strafe = isSprinting ? Input.GetAxis("Horizontal") * speed * sprintFactor : Input.GetAxis("Horizontal") * speed;
        translation *= Time.deltaTime;
        strafe *= Time.deltaTime;
        transform.Translate(strafe, 0, translation);

    }

    //----------------------------------CHECKS FOR COLLECTABLES-------------------------------------------
	void OnTriggerEnter(Collider collider)
    {
	
		string tagOfTheOtherObject = collider.gameObject.tag;
        if (collider.gameObject.tag == "collectible")
        {
            StartCoroutine(pickAnimationTrigger());
            tempScore++;
            PlayerPrefs.SetInt("collected", tempScore);
            print("Score added! Current Score: " + tempScore);
            if(tempScore > 8)
            {
                GameManager.gameManager.win();
            }

            Destroy(collider.gameObject);
        }
	}

    IEnumerator pickAnimationTrigger()
    {
        anim.SetBool("Pick", true);
        yield return new WaitForSecondsRealtime(0.1f);
        anim.SetBool("Pick", false);
    }


    //-----------------------------TAKE DAMAGE FUNCTION---------------------------------
    public void TakeDamage (float amount)
	{
		
		health -= amount;
		GameObject.Find("LIFECOUNT").GetComponent<TextMeshProUGUI>().text = "" + health + "";

		if (health <= 0)
		{
			Die();
		}
	}

    //-------------------------------RIP-----------------------------------------
	void Die()
	{
        deathCamera.SetActive(true);
        deathMenuUI.SetActive(true);
        Destroy(GameObject.Find("Player"));
        Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

        //GAME OVER STUFF
        //
		//SceneManager.LoadScene("Menu");
		//deathMenu.EndMenu(health);
		//Destroy(Player.GetComponent<camMouseLook>());
		//Destroy(GameObject.Find("Capsule"));
		//
	}
}
		
	


