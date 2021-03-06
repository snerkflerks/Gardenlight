
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour {
	public float water = 100;
	public PlayerController player;
	public Transform plant;
	public Transform BeanSproutPlant;
	//public GameObject plant;
	public Transform plantPassed;
	private Vector3 currentLocation;
    public GameObject inventoryCanvas;

	public GameObject[] sproutSeeds;
	public GameObject[] tallSeets;

	public Animator anim;

	public bool plantTimed;
	public bool waterTimed;
	public bool sunTimed;

	public float timer;

	public float jumpForce;
	public float moveSpeed;

	public float playerHeight = 1; //this should be changed based on height of player avatar
	public int waterLevel = 10; //this is an arbitrary minimum water level to water plants; change as needed
	public Slider waterBar;
	public Text waterText;
	public float plantDistance = 4;
	public float seedDistance = 3;
	private bool plantContact;

	public int plantSelected = 1;
	//1 is beanstalk
	//2 is mushroom plant

	// Use this for initialization
	void Start () 
	{
		player = this.GetComponent<PlayerController>();
		plantTimed = false;
		waterTimed = false;
		sunTimed = false;
		jumpForce = player.jumpForce;
		moveSpeed = player.runSpeed;
		anim = GetComponent<Animator> ();

		waterText.text = "Water level: " + water.ToString();

		plantContact = false;
        //plantDistance = playerHeight/2;
        //inventoryCanvas = GameObject.Find("InventoryCanvas"); I don't think this line is necessary since I declared it at the beginning of the class and it looks like we can drag it in, but the forums say to have this, so idk
    }

    // Update is called once per frame
    void Update () 
	{
		checkPassed ();

		if (!player.isMoving && (plantTimed || waterTimed))
		{
			if (plantTimed)
				StartCoroutine (planting ());

		}

		else
		{
			if (!player.isMoving && Input.GetKeyDown(KeyCode.P)  && player.OnGround () == true) //press P to plant seed
			{
				startPlant();
				currentLocation = this.transform.position;
                Inventory inventory = inventoryCanvas.GetComponent<Inventory>();
                if (plantSelected == 1)
                {
                    inventory.seed1.Decrement();
                }
                else
                {
                    inventory.seed2.Decrement();
                }
			}

			else if (!player.isMoving && Input.GetKeyDown(KeyCode.O) && plantPassed != null && plantContact) //press O to water plant
			{
				if (water >= waterLevel) //if water levels are high enough
				{
					startWater();
					startSun();
					currentLocation = this.transform.position;
				}

				else
				{
					//display message "Water Levels aren't High Enough" above player head
					//can add another argument for GUIStyle (background, color, etc.) if needed


					//GUI.Label(new Rect(this.transform.position.x, this.transform.position.y + playerHeight / 2, 100, 20), "Your water levels are too low!");
                    print("could not water");
                }
			}
			else if (Input.GetKeyDown(KeyCode.Y))
			{
				plantSelected++;
				if (plantSelected >= 2) 
				{
					plantSelected = 0;
				}
				Debug.Log ("Plant " + plantSelected + " is selected");
			}
		}

		waterText.text = "Water level: " + water.ToString();
		waterBar.value = water / 100f;

	}

	IEnumerator planting()
	{
		while(plantTimed)
		{
			//Debug.Log(timer);
			if (timer > 0) timer -= Time.deltaTime;
			else
			{
				plantSeed();
			}
		}
		yield return new WaitForSeconds(0);  //does nothing but yield a return value
	}
		

	IEnumerator watering()
	{
		yield return new WaitForSeconds(timer);
		waterPlant();
		sunPower();




		//yield return new WaitForSecondsRealtime(timer);

	}

	void startPlant()
	{
		player.canMove = false;
		plantTimed = true;
		player.runSpeed = 0;
		player.jumpForce = 0;
		timer = 1; //this freezes for 1 second
	}

	void startWater()
	{
		anim.SetTrigger ("Water");
		player.canMove = false;
		waterTimed = true;
		player.runSpeed = 0;
		player.jumpForce = 0;
		timer = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
		StartCoroutine (watering ());
		

	}

	void startSun()
	{
		player.canMove = false;
		sunTimed = true;
		player.runSpeed = 0;
		player.jumpForce = 0;
		timer = 10;
	}

	void plantSeed()
	{
		GameObject seed;
		Vector3 current = this.transform.position;
		//the following instantiates a seed prefab at your feet slightly offset
		if (player.facingRight) 
		{ //player is facing right
			if (plantSelected == 1)
				Instantiate (plant, new Vector3 (this.transform.position.x + seedDistance, this.transform.position.y - playerHeight / 2 - 0.3f), transform.rotation);
			else 
				Instantiate (BeanSproutPlant, new Vector3 (this.transform.position.x + seedDistance, this.transform.position.y - playerHeight / 2 - 0.3f), transform.rotation);
		} 
		else 
		{//player is facing left
			if (plantSelected == 1)
				Instantiate (plant, new Vector3 (this.transform.position.x - seedDistance, this.transform.position.y - playerHeight / 2 - 0.3f), transform.rotation);
			else
				Instantiate (BeanSproutPlant, new Vector3 (this.transform.position.x - seedDistance, this.transform.position.y - playerHeight / 2 - 0.3f), transform.rotation);
		}
		//please add animation trigger stuff here

		plantTimed = false;
		player.runSpeed = moveSpeed;
		player.jumpForce = jumpForce;
		player.canMove = true;
		this.transform.position = currentLocation;
	}

	void waterPlant()
	{
		plantPassed.GetComponent<SpawnPlant>().water();
		plantPassed.GetComponent<SpawnPlant> ().spawnPlant ();
		water -= 5; //lose 5 waters for each time you water a plant
		waterTimed = false;
		player.runSpeed = moveSpeed;
		player.jumpForce = jumpForce;
		player.canMove = true;
		this.transform.position = currentLocation;
	}

	void sunPower()
	{
		//insert sun animations here
		//plantPassed.GetComponent<SpawnPlant>().sun();
		sunTimed = false;
		player.runSpeed = moveSpeed;
		player.jumpForce = jumpForce;
		player.canMove = true;
		this.transform.position = currentLocation;
	}

	void OnTriggerEnter2D(Collider2D other){
		plantPassed = other.transform;
        print("plant is passed");
		plantContact = true;
	}

	void OnTriggerExit2D(Collider2D other){
        plantPassed = null;
		plantContact = false;
    }

	public void incrementWater(float value)
	{
		water = Mathf.Min (100, water + value);
	}

	void checkPassed() {
		float temp = plantDistance;
		 
		if (plantPassed != null) {
			float plantx = plantPassed.transform.position.x;
			if (player.facingRight &&  plantx < this.transform.position.x) {
				plantContact = false;
			} else if (!player.facingRight && plantx > this.transform.position.x) {
				plantContact = false;
			} else {
				plantContact = true;
			}


		}
//		if(plantPassed != null && (Mathf.Abs(plantPassed.transform.position.x - this.transform.position.x) > plantDistance //plant is too far away
//			|| plantPassed.transform.position.x < this.transform.position.x - 0.5)){ //plant is behind player
//			//print("plant is now null");
//			//print(Mathf.Abs(plantPassed.transform.position.x - this.transform.position.x));
//			//print(plantDistance);
//			//if (Mathf.Abs(plantPassed.transform.position.x - this.transform.position.x) > plantDistance) print("first condition");
//			//if(plantPassed.transform.position.x < this.transform.position.x) print("second condition");
//			plantPassed = null;
//		}
	}

}
