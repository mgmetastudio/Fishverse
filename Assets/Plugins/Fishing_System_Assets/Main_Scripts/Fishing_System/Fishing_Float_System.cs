using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishing_Float_System : MonoBehaviour
{
    public GameObject Fishing_Action_System;
    public Transform Fishing_Orbit_Camera;
    public Transform Float_Camera;
    public MeshRenderer[] Meshes;
    public Animator AnimController;
    [Header("UI")]
    public GameObject Place_Float_3D_Canvas;
    [Header("All Fishes")]
    public GameObject[] Fishes;
    public bool canTellFishBite;
    public Collider Trigger_Collider;
    public Transform Point_To_Bite;
    public GameObject Current_Fish;
    [Header("Effects")]
    public GameObject Splash_Effect;
    public GameObject Circle_Effect;
    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip[] Splash_Big_Sound;
    public AudioClip[] Splash_Small_Sound;
    private AudioClip soundClip;
    [Header("Fish Behavior")]
    public bool Floater_Follow_Fish_Position;
    public bool isSwimmingStrong;
    public bool canPerformStrongSwim = true;
    public int maxStrongSwimValue = 5;
    public int current_Strong_Swim_Value = 0;
    [Header("Fishing Float Height")]
    public float Fishing_Float_Height = 4.263f;
    [Header("Player")]
    public Transform Player;
    public float Float_Speed_To_Player = 5f;
    [Header("Backpack")]
    public Backpack_System Backpack;

    public void Start()
    {
        AnimController.Play("Float_Idle_Animation");

        canTellFishBite = false;

        Disable_Mesh();

        Find_Fishing_Action_System();

        //Search all fishes and put them into the array
        Fishes = GameObject.FindGameObjectsWithTag("Fish");

        Circle_Effect.SetActive(false);
    }

    public void Find_Fishing_Action_System()
    {
        //Searching the Find_Fishing_Action_System...
        if (GameObject.FindGameObjectWithTag("Fishing_Action_System") != null)
        {
            //We have found the Find_Fishing_Action_System assign him to the Find_Fishing_Action_System variable
            if (Fishing_Action_System == null)
                Fishing_Action_System = GameObject.FindGameObjectWithTag("Fishing_Action_System");
        }
        else
        {
            //The Fishing_Action_System is not there
        }
    }

    public void Enable_Fishing_Orbit_Camera()
    {
        Fishing_Orbit_Camera.gameObject.SetActive(true);
    }

    public void Disable_Fishing_Orbit_Camera()
    {
        Fishing_Orbit_Camera.gameObject.SetActive(false);
    }

    public void Enable_Float_Camera()
    {
        Float_Camera.gameObject.SetActive(true);
    }

    public void Disable_Float_Camera()
    {
        Float_Camera.gameObject.SetActive(false);
    }

    public void Enable_Mesh()
    {
        foreach(MeshRenderer MeshR in Meshes)
        {
            MeshR.enabled = true;
            Circle_Effect.SetActive(true);
            Splash_Effect.SetActive(false);
        } 
    }

    public void Disable_Mesh()
    {
        foreach (MeshRenderer MeshR in Meshes)
        {
            MeshR.enabled = false;
            Circle_Effect.SetActive(false);
            Splash_Effect.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider Fish)
    {
        if(Fish.tag == "Fish" & canTellFishBite == true & Fishing_Action_System.GetComponent<Fishing_Action_System>().isPullUpEmptyFloater == false)
        {   //if the fish which has entered the trigger is an non Predatory Fish and the player is fishing with an non Predatory Fish bait, will the fish bite.
            if (Fish.GetComponent<AIFishControl>().isPredatoryFish == false & Backpack.hasPredatoryFishBite == false & Backpack.isUsingAdditionalBait == true)
            {
                foreach (GameObject AllFishes in Fishes)
                {
                    AnimController.Play("Float_Down_Animation");

                    Play_Splash_Sound_Big();
                    Splash_Effect.SetActive(true);
                    Find_Fishing_Action_System();
                    AllFishes.GetComponent<AIFishControl>().canBite = false;
                    Current_Fish = Fish.gameObject;
                    Fishing_Action_System.GetComponent<Fishing_Action_System>().Player_Fishing_Force = Fish.GetComponent<AIFishControl>().Player_Fishing_Force;
                    Fishing_Action_System.GetComponent<Fishing_Action_System>().Current_Fish_Name = Fish.GetComponent<AIFishControl>().Fish_Name;
                    Fishing_Action_System.GetComponent<Fishing_Action_System>().Fish_Name.text = Fish.GetComponent<AIFishControl>().Fish_Name;
                    Fishing_Action_System.GetComponent<Fishing_Action_System>().Fish_Size.text = Fish.GetComponent<AIFishControl>().Fish_Size;
                    maxStrongSwimValue = Fish.GetComponent<AIFishControl>().maxStrongSwimValue;
                    Fish.GetComponent<AIFishControl>().canBite = true;
                    Fish.GetComponent<AIFishControl>().Bite = true;
                    Fish.GetComponent<AIFishControl>().Point_To_Bite = Point_To_Bite;
                    Fishing_Action_System.GetComponent<Fishing_Action_System>().Set_Start_Bite_Amount(Fish.GetComponent<AIFishControl>().Bite_Volume);
                    Fishing_Action_System.GetComponent<Fishing_Action_System>().Start_Fish_Bite();
                    Trigger_Collider.enabled = false;
                    canTellFishBite = false;
                    //If the current fish has the isSwimmingAwayAfterBite boolean enabled
                    if (Fish.GetComponent<AIFishControl>().isSwimmingAfterBite == true)
                    {
                        //this.transform.position = Fish.transform.position;
                        Fish.GetComponent<AIFishControl>().isSwimmingWithFloater = true;
                        StartCoroutine(Swimming_With_Floater());
                    }
                    //Remove current Additional bait, because the fish has eat it.
                    if(Backpack.isUsingCheese == true)
                    {
                        int Current_Cheese_Amount;
                        //Sets the amount of cheese items.
                        Current_Cheese_Amount = PlayerPrefs.GetInt(Backpack.Cheese_Amount_Save_Code);
                        //Subtract 1 item count
                        Current_Cheese_Amount -= 1;
                        PlayerPrefs.SetInt(Backpack.Cheese_Amount_Save_Code, Current_Cheese_Amount);
                        if (Current_Cheese_Amount < 1)
                        {
                            //if the item amount is lower then 1, delete it
                            PlayerPrefs.DeleteKey(Backpack.Cheese_Save_Code);
                            PlayerPrefs.DeleteKey(Backpack.Cheese_Using_Save_Code);
                            Current_Cheese_Amount = 0;
                            PlayerPrefs.SetInt(Backpack.Cheese_Amount_Save_Code, 0);
                            Backpack.isUsingAdditionalBait = false;
                            Backpack.Deselect_Cheese();
                        }
                    }
                    if (Backpack.isUsingWorm == true)
                    {
                        int Current_Worm_Amount;
                        //Sets the amount of worm items.
                        Current_Worm_Amount = PlayerPrefs.GetInt(Backpack.Worm_Amount_Save_Code);
                        //Subtract 1 item count
                        Current_Worm_Amount -= 1;
                        PlayerPrefs.SetInt(Backpack.Worm_Amount_Save_Code, Current_Worm_Amount);
                        if (Current_Worm_Amount < 1)
                        {
                            //if the item amount is lower then 1, delete it
                            PlayerPrefs.DeleteKey(Backpack.Worm_Save_Code);
                            PlayerPrefs.DeleteKey(Backpack.Worm_Using_Save_Code);
                            Current_Worm_Amount = 0;
                            PlayerPrefs.SetInt(Backpack.Worm_Amount_Save_Code, 0);
                            Backpack.isUsingAdditionalBait = false;
                            Backpack.Deselect_Worm();
                        }
                    }
                }
            }
            //if the fish which has entered the trigger is an Predatory Fish and the player is fishing with an only Predatory Fish bait, will the fish bite.
            if (Fish.GetComponent<AIFishControl>().isPredatoryFish == true & Backpack.hasPredatoryFishBite == true & Fishing_Action_System.GetComponent<Fishing_Action_System>().isSpinning == true)
            {
                foreach (GameObject AllFishes in Fishes)
                {
                    AnimController.Play("Float_Down_Animation");

                    Play_Splash_Sound_Big();
                    Splash_Effect.SetActive(true);
                    Find_Fishing_Action_System();
                    AllFishes.GetComponent<AIFishControl>().canBite = false;
                    Current_Fish = Fish.gameObject;
                    Fishing_Action_System.GetComponent<Fishing_Action_System>().Player_Fishing_Force = Fish.GetComponent<AIFishControl>().Player_Fishing_Force;
                    Fishing_Action_System.GetComponent<Fishing_Action_System>().Current_Fish_Name = Fish.GetComponent<AIFishControl>().Fish_Name;
                    Fishing_Action_System.GetComponent<Fishing_Action_System>().Fish_Name.text = Fish.GetComponent<AIFishControl>().Fish_Name;
                    Fishing_Action_System.GetComponent<Fishing_Action_System>().Fish_Size.text = Fish.GetComponent<AIFishControl>().Fish_Size;
                    maxStrongSwimValue = Fish.GetComponent<AIFishControl>().maxStrongSwimValue;
                    Fish.GetComponent<AIFishControl>().canBite = true;
                    Fish.GetComponent<AIFishControl>().Bite = true;
                    Fish.GetComponent<AIFishControl>().Point_To_Bite = Point_To_Bite;
                    Fishing_Action_System.GetComponent<Fishing_Action_System>().Set_Start_Bite_Amount(Fish.GetComponent<AIFishControl>().Bite_Volume);
                    Fishing_Action_System.GetComponent<Fishing_Action_System>().Start_Fish_Bite();
                    Trigger_Collider.enabled = false;
                    canTellFishBite = false;
                    //If the current fish has the isSwimmingAwayAfterBite boolean enabled
                    if (Fish.GetComponent<AIFishControl>().isSwimmingAfterBite == true)
                    {
                        //this.transform.position = Fish.transform.position;
                        Fish.GetComponent<AIFishControl>().isSwimmingWithFloater = true;
                        StartCoroutine(Swimming_With_Floater());
                    }
                }
            }
        }
    }
    IEnumerator Swimming_With_Floater()
    {
        yield return new WaitForSeconds(2);
        Floater_Follow_Fish_Position = true;
    }

    public void Reset_All_Fishes()
    {
        foreach (GameObject AllFishes in Fishes)
        {
            Find_Fishing_Action_System();
            AllFishes.GetComponent<AIFishControl>().isFishAttachedToFloater = false;
            AllFishes.GetComponent<AIFishControl>().Set_Fish_Speed_To_Normal();
            AllFishes.GetComponent<AIFishControl>().canBite = true;
            AllFishes.GetComponent<AIFishControl>().Bite = false;
            AllFishes.GetComponent<AIFishControl>().Point_To_Bite = null;
        }
    }

    public void Clear_Fish_List()
    {
        //Clear current fish array and search all fishes and put them into the array
        Fishes = null;
        Fishes = GameObject.FindGameObjectsWithTag("Fish");
    }

    void Update()
    {
        if(transform.position.y > Fishing_Float_Height)
        {
            transform.position = new Vector3 (transform.position.x, Fishing_Float_Height, transform.position.z);
        }

        //Spinning
        if (Backpack.isUsingFloater == false & Fishing_Action_System.GetComponent<Fishing_Action_System>().isSpinning == true)
        {
            Vector3 New_Current_Floater_Position = new Vector3(Player.transform.position.x - 0.2f, Fishing_Float_Height, Player.transform.position.z);

            this.transform.position = Vector3.MoveTowards(this.transform.position, New_Current_Floater_Position, Time.deltaTime * Float_Speed_To_Player);

            //this.transform.LookAt(Player.transform);
        }

        //Pull up empty floater
        if (Backpack.isUsingFloater == true & Fishing_Action_System.GetComponent<Fishing_Action_System>().isPullUpEmptyFloater == true & Fishing_Action_System.GetComponent<Fishing_Action_System>().hasFishBite == false)
        {
            Vector3 New_Current_Floater_Position = new Vector3(Player.transform.position.x - 0.2f, Fishing_Float_Height, Player.transform.position.z);

            this.transform.position = Vector3.MoveTowards(this.transform.position, New_Current_Floater_Position, Time.deltaTime * Float_Speed_To_Player);

            //this.transform.LookAt(Player.transform);
        }

        if (Floater_Follow_Fish_Position == true)
        {
            if (isSwimmingStrong == false)
            {
                //Floater follow current fish position
                if (Fishing_Action_System.GetComponent<Fishing_Action_System>().isMouseKeyDown == false)
                {
                    Current_Fish.GetComponent<AIFishControl>().isPlayerPullUpFish = false;
                    Vector3 New_Current_Fish_Position = new Vector3(Current_Fish.transform.position.x - 0.2f, Fishing_Float_Height, Current_Fish.transform.position.z);

                    this.transform.position = Vector3.MoveTowards(this.transform.position, New_Current_Fish_Position, Time.deltaTime * 5f);

                    //this.transform.position = new Vector3 (Current_Fish.transform.position.x - 0.2f, 4.263f, Current_Fish.transform.position.z);
                    Current_Fish.GetComponent<AIFishControl>().Search_New_Destination();
                }
                else
                {
                    Current_Fish.GetComponent<AIFishControl>().isPlayerPullUpFish = true;
                    /*//Fish follows the floater.
                    Vector3 New_Current_Fish_Position = new Vector3(this.transform.position.x - 0.2f, Current_Fish.transform.position.y, this.transform.position.z);

                    Current_Fish.transform.position = Vector3.MoveTowards(Current_Fish.transform.position, New_Current_Fish_Position, Time.deltaTime * 5f);*/

                    //Floater moves slow to players position, while the player scroll up the fishing line.
                    Vector3 New_Current_Floater_Position = new Vector3(Player.transform.position.x - 0.2f, Fishing_Float_Height, Player.transform.position.z);

                    this.transform.position = Vector3.MoveTowards(this.transform.position, New_Current_Floater_Position, Time.deltaTime * Float_Speed_To_Player);
                    Current_Fish.GetComponent<AIFishControl>().Search_New_Destination();

                    if (canPerformStrongSwim == true & current_Strong_Swim_Value < maxStrongSwimValue)
                    {
                        int Strong_Swim = Random.Range(0, 10);
                        switch (Strong_Swim)
                        {
                            case 0:
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                            case 5:
                                break;
                            case 6:
                                break;
                            case 7:
                                Perform_Strong_Swimming();
                                break;
                            case 8:
                                break;
                            case 9:
                                break;
                            case 10:
                                break;
                            default: break;
                        }
                    }
                }
            }
            else
            {
                Current_Fish.GetComponent<AIFishControl>().isPlayerPullUpFish = false;
                Vector3 New_Current_Fish_Position = new Vector3(Current_Fish.transform.position.x - 0.2f, Fishing_Float_Height, Current_Fish.transform.position.z);

                this.transform.position = Vector3.MoveTowards(this.transform.position, New_Current_Fish_Position, Time.deltaTime * 5f);

                //this.transform.position = new Vector3 (Current_Fish.transform.position.x - 0.2f, 4.263f, Current_Fish.transform.position.z);
                Current_Fish.GetComponent<AIFishControl>().Search_New_Destination();
            }
        }
    }

    public void Play_Splash_Sound_Big()
    {
        //Play Sound
        int index = Random.Range(0, Splash_Big_Sound.Length);
        soundClip = Splash_Big_Sound[index];
        audioSource.clip = soundClip;
        audioSource.Play();
    }

    public void Play_Splash_Sound_Small()
    {
        //Play Sound
        int index = Random.Range(0, Splash_Small_Sound.Length);
        soundClip = Splash_Small_Sound[index];
        audioSource.clip = soundClip;
        audioSource.Play();
    }

    public void Perform_Strong_Swimming()
    {
        current_Strong_Swim_Value += 1;
        isSwimmingStrong = true;
        Current_Fish.GetComponent<AIFishControl>().isPlayerPullUpFish = false;
        Current_Fish.GetComponent<AIFishControl>().Search_New_Destination();
        StartCoroutine(Disable_Strong_Swimming());
    }

    IEnumerator Disable_Strong_Swimming()
    {
        yield return new WaitForSeconds(5);

        canPerformStrongSwim = false;
        isSwimmingStrong = false;

        StartCoroutine(Fish_Can_Perform_Strong_Swim());
    }

    IEnumerator Fish_Can_Perform_Strong_Swim()
    {
        yield return new WaitForSeconds(5);

        canPerformStrongSwim = true;
    }
}
