using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fishing_Action_System : MonoBehaviour
{
    [Header("Floater")]
    public bool isPullUpEmptyFloater = false;
    [Header("Spinner")]
    public float minSpinnerDistance = 3;
    public GameObject No_Bite_Selected_Warning;
    public bool isSpinning = false;
    [Header("Find Water")]
    public bool isRotating = false;
    public bool isDisablingRoatation = false;
    public float Player_Rotation_Speed = 1f;
    public GameObject Rotate_To_Water_Button;
    public Transform Center_Of_Water_Position;
    [Header("Fishing Holder")]
    public GameObject Fishing_Holder;
    [Header("Fishing Float Movement")]
    public bool shouldMoveWithMouse = true;
    public float Float_Z_Position = 1f;
    [Header("Aim Graphic")]
    public GameObject Aim_Graphic;
    public bool isMouseOnAimGraphic = false;
    public bool isFishInTrigger = false;
    public GameObject Catched_Trigger;
    [Header("Keys")]
    public bool isMouseKeyDown = false;
    public bool isFishingUpAnimationPlaying = false;
    [Header("Fishing Line")]
    public bool isDrawingFishingLine;
    public GameObject Fishing_Line;
    public GameObject Spawned_Fishing_Line;
    public Transform Fishing_Rod_Line_Point;
    [Header("Backpack")]
    public Backpack_System Backpack;
    [Header("Fishing Areas")]
    public GameObject Fishing_Area;
    public GameObject Fish_Swim_Area;
    [Header("Global Settings")]
    public Fishing_System Fishing_Main_Script;
    public GameObject Fishing_Start_System_Camera;
    public Transform Main_Camera;
    public Transform Player;
    public GameObject Progress_Menu;
    public GameObject Start_Fishing_System_Canvas;
    public GameObject Leave_Fishing_Button;
    public GameObject Start_Fishing_Button;
    [Header("UI Settings")]
    public GameObject Start_Warning_Window;
    public Toggle Do_Not_Show_Again_Toogle;
    public GameObject Fishing_System_Canvas;
    public GameObject While_Fishing_Canvas;
    [Header("Fishing")]
    public bool canFishing;
    public bool isInFishSwimArea;
    public bool FloatIsInWater;
    public Transform Fishing_Camera_To_Player;
    [Header("Fish Bite")]
    public float Player_Fishing_Force;
    public string Current_Fish_Name;
    public bool hasFishBite;
    public Image Fish_Bite_Bar;
    public bool isPlayingUpAnimation;
    public GameObject Pull_Up_Fishing_Line_No_Bite;
    public GameObject Pull_Up_Fishing_Line_Bite;
    public GameObject Pull_In_Fishing_Line;
    public GameObject Cancel_Fishing_Button;
    [Header("Fish Stats")]
    public Text Fish_Name;
    public Text Fish_Size;
    public GameObject Keep_Release_Fish_Canvas;
    [Header("Fishing Camera Views")]
    public GameObject Change_To_Float_Camera_Button;
    public GameObject Change_To_Player_Camera_Button;
    public GameObject Hold_Fish_Camera;
    [Header("Spawn Fishing Float")]
    public GameObject Fishing_Float_Prefab;
    public bool isMovingFloat;
    public bool canPlaceFloat;
    public float Fishing_Float_High;
    public Transform Float_Place_Camera;
    public GameObject Float_Move_Ground;
    [Header("Fish Prefabs")]
    public GameObject Koi_Carp_Prefab;
    public GameObject Bream_Prefab;
    public GameObject Carp_Prefab;
    public GameObject Pike_Prefab;
    public GameObject Shiner_Prefab;
    public GameObject Trout_Prefab;
    public GameObject Cod_Prefab;
    public GameObject Barracuda_Prefab;
    public GameObject Black_Spotted_Grunt_Prefab;
    public GameObject Butterfly_Fish_Prefab;
    public GameObject Clownfish_Prefab;
    public GameObject Discus_Prefab;
    public GameObject Emperor_Prefab;
    public GameObject Foureye_Prefab;
    public GameObject Mackerel_Prefab;
    public GameObject Mandarin_Prefab;
    public GameObject Moorish_Idol_Prefab;
    public GameObject Nimbochromis_Prefab;
    public GameObject Orangespine_Unicron_Fish_Prefab;
    public GameObject Frontosa_Prefab;
    public GameObject Rainbow_Cichlid_Prefab;
    public GameObject Salmon_Prefab;
    public GameObject Siganus_Javus_Prefab;
    public GameObject Siganus_Prefab;
    public GameObject Trewavas_Cichlid_Prefab;
    public GameObject Great_White_Shark_Prefab;
    //Marlin is a big fish
    public GameObject Blue_Marlin_Prefab;
    public GameObject Blue_Marlin_Catched;
    public GameObject Fish_Spawned;
    [Header("Fish Player Objects")]
    public GameObject Koi_Carp_Object;
    public GameObject Bream_Object;
    public GameObject Carp_Object;
    public GameObject Pike_Object;
    public GameObject Shiner_Object;
    public GameObject Trout_Object;
    public GameObject Cod_Object;
    public GameObject Barracuda_Object;
    public GameObject Black_Spotted_Grunt_Object;
    public GameObject Butterfly_Fish_Object;
    public GameObject Clownfish_Object;
    public GameObject Discus_Object;
    public GameObject Emperor_Object;
    public GameObject Foureye_Object;
    public GameObject Mackerel_Object;
    public GameObject Mandarin_Object;
    public GameObject Moorish_Idol_Object;
    public GameObject Nimbochromis_Object;
    public GameObject Orangespine_Unicron_Fish_Object;
    public GameObject Frontosa_Object;
    public GameObject Rainbow_Cichlid_Object;
    public GameObject Salmon_Object;
    public GameObject Siganus_Javus_Object;
    public GameObject Siganus_Object;
    public GameObject Trewavas_Cichlid_Object;
    public GameObject Great_White_Shark_Object;
    public GameObject Blue_Marlin_Object;
    [Header("Circle Settings")]
    public GameObject Fish_Here_Button;
    public GameObject Cant_Fish_Here_Button;
    public bool pointerDown;
    private float pointerDownTimer;
    public float requiredHoldTime;
    [SerializeField]
    private Image fillImage;

    //Info message on start open
    public void Open_Start_Warning_Window()
    {
        Start_Warning_Window.SetActive(true);
        Start_Warning_Window.GetComponent<Animator>().Play("Spawn_Frien_Window_In");
    }
    public void Close_Start_Warning_Window()
    {
        if (Do_Not_Show_Again_Toogle.isOn == true)
        {
            PlayerPrefs.SetInt("Fishing_Near_Start_Message_Saved", 1);
        }
        Start_Warning_Window.GetComponent<Animator>().Play("Spawn_Frien_Window_Out");
        Cursor.lockState = CursorLockMode.Locked; //lock cursor
        Cursor.visible = false; //disable visible mouse
        StartCoroutine(Disable_Start_Warning_Menu_After_Seconds());
    }
    IEnumerator Disable_Start_Warning_Menu_After_Seconds()
    {
        yield return new WaitForSeconds(1);
        Start_Warning_Window.SetActive(false);
        Start_Warning_Window.GetComponent<Animator>().Rebind();
    }

    public void Back_To_Start_Fishing_System()
    {
        if (Main_Camera == null)
            Main_Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        Cursor.lockState = CursorLockMode.None; //unlock cursor
        Cursor.visible = true; //make mouse visible
        Player.GetComponent<Animator>().enabled = true;

        Player.GetComponent<Animator>().Play("Walk_W_Fishing_Rod");

        Fishing_Main_Script.Check_Catched_Fishes();
        //Enable Start_Fishing_System
        Leave_Fishing_Button.SetActive(false);
        Start_Fishing_Button.SetActive(true);
        Player.GetComponent<PauseMenuToggleCP>().DisablePauseMenuUI();
        Player.GetComponent<PauseMenuToggleCP>().enabled = false;
        Main_Camera.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>().enabled = false;
        //Disable player control
        Player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().canControl = false;
        this.gameObject.SetActive(false);
        Start_Fishing_System_Canvas.SetActive(true);
        Fishing_Start_System_Camera.SetActive(true);
    }

    public void Start_Fishing()
    {
        Leave_Fishing_Button.SetActive(true);
        Start_Fishing_Button.SetActive(false);
        Fishing_Start_System_Camera.SetActive(false);
        if (Main_Camera == null)
            Main_Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        if (Progress_Menu == null)
            Progress_Menu = GameObject.FindGameObjectWithTag("Level_System");
        Cursor.lockState = CursorLockMode.Locked; //lock cursor
        Cursor.visible = false; //disable visible mouse

        Player.GetComponent<Animator>().Play("Walk_W_Fishing_Rod");

        //Check info message
        if (PlayerPrefs.HasKey("Fishing_Near_Start_Message_Saved"))
        {
            //The player do not wan't to see the start info again, so don't open it.
        }
        else
        {
            Open_Start_Warning_Window();
            Cursor.lockState = CursorLockMode.None; //unlock cursor
            Cursor.visible = true; //make mouse visible
        }

        //Searching the Fishing_Area...
        if (GameObject.FindGameObjectWithTag("Fishing_Area") != null)
        {
            //We have found the Find_Fishing_Action_System assign him to the Fishing_Area variable
            if (Fishing_Area == null)
                Fishing_Area = GameObject.FindGameObjectWithTag("Fishing_Area");

            //Let the area find this script
            Fishing_Area.GetComponent<Fishing_Area>().Find_Fishing_Action_System();
        }
        else
        {
            //The Fishing_Area is not there
        }

        //Searching the Fish_Swim_Area...
        if (GameObject.FindGameObjectWithTag("Fish_Swim_Area") != null)
        {
            //We have found the Fish_Swim_Area assign him to the Fishing_Area variable
            if (Fish_Swim_Area == null)
                Fish_Swim_Area = GameObject.FindGameObjectWithTag("Fish_Swim_Area");

            //Let the area find this script
            Fish_Swim_Area.GetComponent<Fish_Swim_Area>().Find_Fishing_Action_System();
        }
        else
        {
            //The Fish_Swim_Area is not there
        }
    }

    /*void OnTriggerStay(Collider Fish_Area)
    {
        if (Fish_Area.tag == "Fishing_Area" & Fish_Area.tag != "Fish_Swim_Area" & isInFishSwimArea == false & canFishing == true)
        {
            canFishing = true;
        }

        if (Fish_Area.tag == "Fish_Swim_Area")
        {
            canFishing = false;
            isInFishSwimArea = true;
        }
    }

    void OnTriggerExit(Collider Fish_Area)
    {
        if(Fish_Area.tag != "Fish_Swim_Area")
        {
            isInFishSwimArea = false;
            canFishing = true;
        }

        if (Fish_Area.tag != "Fishing_Area")
        {
            canFishing = false;
        }
    }*/

    private void Reset()
    {
        pointerDown = false;
        pointerDownTimer = 0;
        fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
    }

    public void Update()
    {

        if (isRotating == true)
        {
            //Rotates the player to the water.
            Vector3 lTargetDir = Center_Of_Water_Position.position - transform.position;
            lTargetDir.y = 0.0f;
            Player.rotation = Quaternion.RotateTowards(Player.rotation, Quaternion.LookRotation(lTargetDir), Time.time * Player_Rotation_Speed);

            if (isDisablingRoatation == false)
            {
                isDisablingRoatation = true;
                StartCoroutine(Disable_Rotating_To_Water());
            }
        }

        if (FloatIsInWater == false & canFishing == true & isInFishSwimArea == false)
        {
            Fish_Here_Button.SetActive(true);
            Cant_Fish_Here_Button.SetActive(false);

            if (Input.GetButton("ActionF") & Player.GetComponent<PauseMenuToggleCP>().isInteracting == false)
            {
                pointerDown = true;

                pointerDownTimer += Time.deltaTime;
                if (pointerDownTimer > requiredHoldTime)
                {
                    //Enter
                    Fishing_Action();
                    //Fishing_Main_Script.Entered_Fishing_System_Message();

                    Reset();
                }
                fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
            }
            else
            {
                Reset();
            }
        }
        else
        {
            Fish_Here_Button.SetActive(false);
            Cant_Fish_Here_Button.SetActive(true);
        }
        if (canFishing == false & Input.GetButton("ActionF"))
        {
            Fishing_Main_Script.Can_not_Fishing_Here_Message();
        }

        //Spawn fishing float prefab at mouse posizion
        //Piece following mouse
        if (isMovingFloat == true)
        {
            if (Input.GetMouseButtonDown(0) & canPlaceFloat == true & isMouseOnAimGraphic == true)
            {
                //Place Float
                Place_Fishing_Float();
            }

            if(shouldMoveWithMouse == true)
            {
                Ray ray = Float_Place_Camera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                int layer_mask = LayerMask.GetMask("Overlay");

                if (Physics.Raycast(ray, out hit, 100f, layer_mask))
                {
                    float y = hit.point.y + (Fishing_Float_Prefab.transform.localScale.y / Fishing_Float_High);
                    Vector3 pos = new Vector3(hit.point.x, y, hit.point.z);
                    Fishing_Float_Prefab.transform.position = pos;
                }
            }
            else
            {
                Float_Z_Position += Input.GetAxis("Mouse ScrollWheel");
                Float_Z_Position = Mathf.Clamp(Float_Z_Position, 1, 20);

                Vector3 CenterPos = Float_Place_Camera.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Float_Z_Position));

                Vector3 New_Floater_Position = new Vector3(CenterPos.x, 4.263f, CenterPos.z);

                Fishing_Float_Prefab.transform.position = New_Floater_Position;
            }
        }
        //Spinning
        if (Backpack.isUsingFloater == false & FloatIsInWater == true)
        {
            float Current_Spinner_Distance = Vector3.Distance(Fishing_Float_Prefab.transform.position, Player.transform.position);
            if (Current_Spinner_Distance < minSpinnerDistance)
            {
                Pull_Up_The_Line_Without_Fish();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {//Pull up line if Mouse0 is pressed.
                isMouseKeyDown = true;
                isSpinning = true;
            }
            if (isMouseKeyDown == true)
            {
                //Pull_Up_Line_With_Fish();
                //Player.GetComponent<Animator>().speed = 1;
                if (isFishingUpAnimationPlaying == false)
                {
                    isFishingUpAnimationPlaying = true;
                    Player.GetComponent<Animator>().SetFloat("Fishing_Up_Speed", 1);
                    Player.GetComponent<Animator>().Play("Fishing_Up");
                }
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                isMouseKeyDown = false;
                isSpinning = false;
                isFishingUpAnimationPlaying = false;
                //The fishing rod up animation of the player will move with the fis_bite_Bar fillamount
                //Player.GetComponent<Animator>().Play("Fishing_Up", -1, Fish_Bite_Bar.fillAmount);
                Player.GetComponent<Animator>().SetFloat("Fishing_Up_Speed", 0);
            }
        }
        //End Spinning
        //Pull up empty floater
        if (Backpack.isUsingFloater == true & FloatIsInWater == true & hasFishBite == false)
        {
            float Current_Spinner_Distance = Vector3.Distance(Fishing_Float_Prefab.transform.position, Player.transform.position);
            if (Current_Spinner_Distance < minSpinnerDistance)
            {
                Pull_Up_The_Line_Without_Fish();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {//Pull up line if Mouse0 is pressed.
                isMouseKeyDown = true;
                isPullUpEmptyFloater = true;
            }
            if (isMouseKeyDown == true)
            {
                //Pull_Up_Line_With_Fish();
                //Player.GetComponent<Animator>().speed = 1;
                if (isFishingUpAnimationPlaying == false)
                {
                    isFishingUpAnimationPlaying = true;
                    Player.GetComponent<Animator>().SetFloat("Fishing_Up_Speed", 1);
                    Player.GetComponent<Animator>().Play("Fishing_Up");
                }
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                isMouseKeyDown = false;
                isPullUpEmptyFloater = false;
                isFishingUpAnimationPlaying = false;
                //The fishing rod up animation of the player will move with the fis_bite_Bar fillamount
                //Player.GetComponent<Animator>().Play("Fishing_Up", -1, Fish_Bite_Bar.fillAmount);
                Player.GetComponent<Animator>().SetFloat("Fishing_Up_Speed", 0);
            }
        }
        //End Pull up empty floater
        if (hasFishBite == true)
        {
            isPullUpEmptyFloater = false;
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {//Pull up line if Mouse0 is pressed.
                isMouseKeyDown = true;
            }
            if(isMouseKeyDown == true)
            {
                Pull_Up_Line_With_Fish();
                //Player.GetComponent<Animator>().speed = 1;
                if(isFishingUpAnimationPlaying == false)
                {
                    isFishingUpAnimationPlaying = true;
                    Player.GetComponent<Animator>().SetFloat("Fishing_Up_Speed", 1);
                    Player.GetComponent<Animator>().Play("Fishing_Up");
                }
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                isMouseKeyDown = false;
                isFishingUpAnimationPlaying = false;
                //The fishing rod up animation of the player will move with the fis_bite_Bar fillamount
                //Player.GetComponent<Animator>().Play("Fishing_Up", -1, Fish_Bite_Bar.fillAmount);
                Player.GetComponent<Animator>().SetFloat("Fishing_Up_Speed", 0);
            }
            //Subtract one amount per second
            Fish_Bite_Bar.fillAmount -= 0.001f;
            //The fishing rod up animation of the player will move with the fis_bite_Bar fillamount
            //Player.GetComponent<Animator>().Play("Fishing_Up", -1, Fish_Bite_Bar.fillAmount);

            //if fishing bite bars value is 0.99f for 3 seconds will the line be destroyed.
            if(Fish_Bite_Bar.fillAmount > 0.99f)
            {

            }

            if(Fish_Bite_Bar.fillAmount > 0.99f & isFishInTrigger == true)
            {
                //END the player has catch the fish
                Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Floater_Follow_Fish_Position = false;
                Fishing_Main_Script.Add_Catched_Fishes_Point();
                Fishing_Main_Script.Check_Catched_Fishes();
                Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().current_Strong_Swim_Value = 0;
                Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Play_Splash_Sound_Big();
                Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().canTellFishBite = false;
                Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Trigger_Collider.enabled = false;
                Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Disable_Mesh();
                Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().isSwimmingStrong = false;
                Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().canPerformStrongSwim = true;
                FloatIsInWater = false;
                Fishing_System_Canvas.SetActive(false);
                While_Fishing_Canvas.SetActive(false);
                Change_Camera_To_Player_View();
                Cancel_Fishing_Button.SetActive(false);
                Pull_In_Fishing_Line.SetActive(false);
                Pull_Up_Fishing_Line_Bite.SetActive(false);
                Pull_Up_Fishing_Line_No_Bite.SetActive(false);

                isMovingFloat = false;
                //Player.GetComponent<Animator>().Play("Fishing_Idle");
                canPlaceFloat = false;
                Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Reset_All_Fishes();
                hasFishBite = false;
                isMouseKeyDown = false;
                isFishInTrigger = false;
                Catched_Trigger.GetComponent<Fish_Catched_Trigger>().enabled = false;
                Player.GetComponent<Animator>().SetFloat("Fishing_Up_Speed", 1);
                Player.GetComponent<Animator>().Play("Fish_In_Hand");
                //Fishing_Main_Script.Fishing_Rod.SetActive(false);
                Keep_Release_Fish_Canvas.SetActive(true);
                //Save the fish size.
                //Get current fish size number.
                string Current_Fish_Size;
                int Current_Fish_Size_Number;
                Current_Fish_Size = Fish_Size.text.Substring(0, 2);
                Current_Fish_Size_Number = int.Parse(Current_Fish_Size);
                //Get best fish size number.
                string Best_Fish_Size;
                int Best_Fish_Size_Number;
                if(PlayerPrefs.HasKey("Best_Fish_Size"))
                {
                    Best_Fish_Size = PlayerPrefs.GetString("Best_Fish_Size").Substring(0, 2);
                    Best_Fish_Size_Number = int.Parse(Best_Fish_Size);
                    //Save the size if current fish size is heigher.
                    if (Current_Fish_Size_Number > Best_Fish_Size_Number)
                    {
                        PlayerPrefs.SetString("Best_Fish_Size", Fish_Size.text);
                        PlayerPrefs.SetString("Biggest_Fish_Name", Current_Fish_Name);
                    }
                }
                else
                {
                    PlayerPrefs.SetString("Best_Fish_Size", Fish_Size.text);
                    PlayerPrefs.SetString("Biggest_Fish_Name", Current_Fish_Name);
                }
                //Add xp to level system.
                Progress_Menu.GetComponent<Level_System>().Add_XP_Points(0.5f);
                //Enable fishing holder.
                Fishing_Holder.SetActive(true);
                if (Current_Fish_Name == "Koi Carp")
                {
                    //The player has cathed a Koi_Carp, so enable the koi carp object in the hands
                    Koi_Carp_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Bream")
                {
                    //The player has cathed a Bream, so enable the Bream object in the hands
                    Bream_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Carp")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Carp_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Pike")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Pike_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Shiner")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Shiner_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Trout")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Trout_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Cod")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Cod_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Barracuda")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Barracuda_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Black Spotted Grunt")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Black_Spotted_Grunt_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Butterfly Fish")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Butterfly_Fish_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Clownfish")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Clownfish_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Discus")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Discus_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Emperor")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Emperor_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Foureye Butterfly Fish")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Foureye_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Mackerel")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Mackerel_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Mandarin")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Mandarin_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Moorish Idol")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Moorish_Idol_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Nimbochromis")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Nimbochromis_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Orangespine Unicron Fish")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Orangespine_Unicron_Fish_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Frontosa")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Frontosa_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Rainbow Cichlid")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Rainbow_Cichlid_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Salmon")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Salmon_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Siganus Javus")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Siganus_Javus_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Siganus")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Siganus_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Trewavas Cichlid")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Trewavas_Cichlid_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Great White Shark")
                {
                    //The player has cathed a Carp, so enable the Carp object in the hands
                    Great_White_Shark_Object.SetActive(true);
                }
                if (Current_Fish_Name == "Blue Marlin")
                {
                    //The player has cathed a marlin, so enable the marlin object in the hands
                    //Blue_Marlin_Object.SetActive(true);
                    Blue_Marlin_Catched = GameObject.FindGameObjectWithTag("Catched_Marlin");
                    Blue_Marlin_Catched.GetComponent<Enable_Big_Catched_Fish_Animation>().Enbale_Big_Catched_Fish_Anim();
                }
                Hold_Fish_Camera.SetActive(true);

                if (Fishing_Main_Script.Catched_Fishes > 9)
                {
                }
            }

            if (Fish_Bite_Bar.fillAmount < 0.01f)
            {
                //END the fish has won, he escaping
                Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Floater_Follow_Fish_Position = false;
                Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Play_Splash_Sound_Big();
                Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Current_Fish = null;
                //Pull_Up_The_Line_Without_Fish();
                Fish_Is_Escaped();
            }

            /*float animationTime;

            animationTime = Player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
            animationTime = Fish_Bite_Bar.fillAmount;*/
        }

        if(isDrawingFishingLine == true)
        {
            if(hasFishBite == false)
            {
                //Fish hasn't bitten
                //Set the first line renderer point to the fishing rod line position
                Spawned_Fishing_Line.GetComponent<LineRenderer>().SetPosition(0, Fishing_Rod_Line_Point.transform.position);

                Vector3 NewFishing_Floater_Position = new Vector3(Fishing_Float_Prefab.transform.position.x, Fishing_Float_Prefab.transform.position.y + 0.45f, Fishing_Float_Prefab.transform.position.z);

                //Set the second line renderer position to the floater transform
                Spawned_Fishing_Line.GetComponent<LineRenderer>().SetPosition(1, NewFishing_Floater_Position);
            }
            else
            {
                //Fish has bitten
                //Set the first line renderer point to the fishing rod line position
                Spawned_Fishing_Line.GetComponent<LineRenderer>().SetPosition(0, Fishing_Rod_Line_Point.transform.position);

                Vector3 NewFishing_Floater_Position = new Vector3(Fishing_Float_Prefab.transform.position.x, Fishing_Float_Prefab.transform.position.y + 0.1f, Fishing_Float_Prefab.transform.position.z);

                //Set the second line renderer position to the floater transform
                Spawned_Fishing_Line.GetComponent<LineRenderer>().SetPosition(1, NewFishing_Floater_Position);
            }
        }
    }

    public void Set_Start_Bite_Amount(float Bite_Amount)
    {
        //Adding start bite amount
        Fish_Bite_Bar.fillAmount = Bite_Amount;
    }

    public void Pull_Up_Line_With_Fish()
    {
        //Adding start bite amount
        Fish_Bite_Bar.fillAmount += Player_Fishing_Force;
        /*if(isPlayingUpAnimation == false)
        {
            isPlayingUpAnimation = true;
            Player.GetComponent<Animator>().SetTrigger("Fishing_Catched");
        }*/
    }

    public void Start_Fish_Bite()
    {
        Backpack.isUsingFloater = true;
        isSpinning = false;
        isPullUpEmptyFloater = false;
        //Disable switch camera to floater button
        Change_Camera_To_Player_View();
        Change_To_Float_Camera_Button.SetActive(false);
        Change_To_Player_Camera_Button.SetActive(false);

        //hasFishBite = true;
        Player.GetComponent<Animator>().SetTrigger("Fish_Bite");
        Pull_Up_Fishing_Line_No_Bite.SetActive(false);
        Pull_Up_Fishing_Line_Bite.SetActive(true);
        StartCoroutine(Play_Fishing_Animation());
    }

    IEnumerator Play_Fishing_Animation()
    {
        yield return new WaitForSeconds(1);

        hasFishBite = true;
    }

    public void Change_Camera_To_Float_View()
    {
        Change_To_Float_Camera_Button.SetActive(false);
        Change_To_Player_Camera_Button.SetActive(true);
        Fishing_Camera_To_Player.gameObject.SetActive(false);
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Enable_Float_Camera();
    }
    public void Change_Camera_To_Player_View()
    {
        //Searching the Fishing_Float_Prefab...
        if (GameObject.FindGameObjectWithTag("Fishing_Float") != null)
        {
            //We have found the Fishing_Float_Prefab assign him to the Fishing_Float_Prefab variable
            if (Fishing_Float_Prefab == null)
                Fishing_Float_Prefab = GameObject.FindGameObjectWithTag("Fishing_Float");
        }
        else
        {
            //The Fishing_Float_Prefab is not there
        }
        Change_To_Float_Camera_Button.SetActive(true);
        Change_To_Player_Camera_Button.SetActive(false);
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Disable_Float_Camera();
        Fishing_Camera_To_Player.gameObject.SetActive(true);
    }

    public void Disable_Player_Movement()
    {
        if (Main_Camera == null)
            Main_Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        Cursor.lockState = CursorLockMode.None; //unlock cursor
        Cursor.visible = true; //make mouse visible
        Player.GetComponent<Animator>().enabled = true;
        Player.GetComponent<PauseMenuToggleCP>().DisablePauseMenuUI();
        Player.GetComponent<PauseMenuToggleCP>().enabled = false;
        Main_Camera.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>().enabled = false;
        //Disable player control
        Player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().canControl = false;
    }

    public void Enable_Player_Movement()
    {
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        Cursor.lockState = CursorLockMode.Locked; //lock cursor
        Cursor.visible = false; //disable visible mouse

        Player.GetComponent<Animator>().enabled = true;
        Player.GetComponent<PauseMenuToggleCP>().enabled = true;
        Main_Camera.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>().enabled = true;
        //Enable player control
        Player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().canControl = true;
        Player.GetComponent<PauseMenuToggleCP>().CancelPause();

        Player.GetComponent<Animator>().Play("Walk_W_Fishing_Rod");
    }

    public void Fishing_Action()
    {
        if(Backpack.NoBite == true)
        {
            //Player hasn't selected a bite, so display error message.
            Open_No_Bite_Selected_Warning_Window();
        }
        else
        {   //Has a bite selected.
            //Backpack.Check_Baits();
            Fishing_Main_Script.Entered_Fishing_System_Message();
            Backpack.isUsingFloater = true;
            Rotate_To_Water_Button.SetActive(true);
            Fishing_Main_Script.Fishing_Rod.SetActive(true);

            //FloatIsInWater = true;
            Fishing_System_Canvas.SetActive(false);
            While_Fishing_Canvas.SetActive(true);
            Change_Camera_To_Player_View();
            Change_To_Float_Camera_Button.SetActive(false);
            Change_To_Player_Camera_Button.SetActive(false);
            Cancel_Fishing_Button.SetActive(false);
            Pull_In_Fishing_Line.SetActive(false);
            Pull_Up_Fishing_Line_Bite.SetActive(false);
            Pull_Up_Fishing_Line_No_Bite.SetActive(false);

            Disable_Player_Movement();

            //Searching the Fishing_Float_Prefab...
            if (GameObject.FindGameObjectWithTag("Fishing_Float") != null)
            {
                //We have found the Fishing_Float_Prefab assign him to the Fishing_Float_Prefab variable
                if (Fishing_Float_Prefab == null)
                    Fishing_Float_Prefab = GameObject.FindGameObjectWithTag("Fishing_Float");

                Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Enable_Mesh();
                Fishing_Float_Prefab.GetComponent<Collider>().enabled = true;
            }
            else
            {
                //The Fishing_Float_Prefab is not there
            }

            //Searching the Float_Move_Ground...
            if (GameObject.FindGameObjectWithTag("Float_Move_Ground") != null)
            {
                //We have found the Float_Move_Ground assign him to the Float_Move_Ground variable
                if (Float_Move_Ground == null)
                    Float_Move_Ground = GameObject.FindGameObjectWithTag("Float_Move_Ground");

                //Enable mesh collider
                Float_Move_Ground.GetComponent<Collider>().enabled = true;

                Float_Place_Camera = Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Fishing_Orbit_Camera;
            }
            else
            {
                //The Float_Move_Ground is not there
            }

            Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().canTellFishBite = false;
            Fishing_Float_Prefab.SetActive(true);
            Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Fishing_Orbit_Camera.gameObject.SetActive(true);
            Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Place_Float_3D_Canvas.SetActive(true);
            isMovingFloat = true;
            Player.GetComponent<Animator>().SetTrigger("Fishing_Idle");
            //Disable Head Wight
            Player.GetComponent<Animator>().SetLayerWeight(14, 0);
            //Player.GetComponent<RealisticEyeMovements.EyeAndHeadAnimator>().headWeight = 0;
            //Clear all searched fishes in the array
            Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Fishes = null;
            Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Clear_Fish_List();
            Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().current_Strong_Swim_Value = 0;
            canPlaceFloat = true;
            Aim_Graphic.SetActive(true);
            Main_Camera.gameObject.SetActive(false);
        }
    }

    public void Place_Fishing_Float()
    {
        FloatIsInWater = true;
        Rotate_To_Water_Button.SetActive(false);
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Place_Float_3D_Canvas.SetActive(false);
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Play_Splash_Sound_Small();
        Pull_Up_Fishing_Line_No_Bite.SetActive(false);
        canPlaceFloat = false;
        isMovingFloat = false;
        isMouseOnAimGraphic = false;
        Aim_Graphic.SetActive(false);
        isFishInTrigger = false;
        Catched_Trigger.GetComponent<Fish_Catched_Trigger>().enabled = true;
        //Disable mesh collider
        Float_Move_Ground.GetComponent<Collider>().enabled = false;
        Fishing_Float_Prefab.GetComponent<Collider>().enabled = true;

        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Fishing_Orbit_Camera.gameObject.SetActive(false);
        Fishing_Camera_To_Player.gameObject.SetActive(true);
        Player.GetComponent<Animator>().SetTrigger("Fishing_In");
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Trigger_Collider.enabled = true;
        Change_To_Float_Camera_Button.SetActive(true);
        Change_To_Player_Camera_Button.SetActive(false);
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().canTellFishBite = true;
        //Rotate Player to floater
        Vector3 targetPostition = new Vector3(Fishing_Float_Prefab.transform.position.x,
        Player.transform.position.y, Fishing_Float_Prefab.transform.position.z);
        Player.transform.LookAt(targetPostition);
        //Player.transform.LookAt(Fishing_Float_Prefab.transform.position);
        //Draw fishing line
        //Spawn fishing line
        Spawned_Fishing_Line = Instantiate(Fishing_Line);
        //Assign Fishing_Rod_Line_Point to variable
        if (Fishing_Rod_Line_Point == null)
            Fishing_Rod_Line_Point = GameObject.FindGameObjectWithTag("Fishing_Rod_Line_Point").transform;
        isDrawingFishingLine = true;
        Backpack.Check_Baits();
        //Start fishing idle animation
        StartCoroutine(Start_Fishing_Idle_Animation());
    }

    IEnumerator Start_Fishing_Idle_Animation()
    {
        yield return new WaitForSeconds(2);
        //If fish hasn't bitten, play fishing idle animation.
        if(hasFishBite == false)
        {
            Player.GetComponent<Animator>().Play("Fishing_Idle");
            Player.GetComponent<Animator>().ResetTrigger("Fishing_In");
        }
    }

    public void Pull_Up_The_Line_Without_Fish()
    {
        //Destroy fishing line
        isDrawingFishingLine = false;
        Destroy(Spawned_Fishing_Line);
        Spawned_Fishing_Line = null;

        Backpack.Check_Baits();

        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Floater_Follow_Fish_Position = false;
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().current_Strong_Swim_Value = 0;
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Play_Splash_Sound_Big();
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().AnimController.Play("Float_Idle_Animation");
        Fishing_Main_Script.Fishing_Rod.SetActive(true);
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().canTellFishBite = false;
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Trigger_Collider.enabled = false;
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Disable_Mesh();
        Player.GetComponent<Animator>().SetTrigger("Fishing_Catched");
        FloatIsInWater = false;
        Fishing_System_Canvas.SetActive(false);
        While_Fishing_Canvas.SetActive(true);
        Change_Camera_To_Player_View();
        Pull_In_Fishing_Line.SetActive(true);
        Pull_Up_Fishing_Line_Bite.SetActive(false);
        Pull_Up_Fishing_Line_No_Bite.SetActive(false);
        Cancel_Fishing_Button.SetActive(true);

        isMovingFloat = false;
        //Player.GetComponent<Animator>().Play("Fishing_Idle");
        canPlaceFloat = false;
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Reset_All_Fishes();
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Current_Fish = null;
        hasFishBite = false;
        isMouseKeyDown = false;
        isFishInTrigger = false;
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().isSwimmingStrong = false;
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().canPerformStrongSwim = true;
        Catched_Trigger.GetComponent<Fish_Catched_Trigger>().enabled = false;
        Player.GetComponent<Animator>().SetFloat("Fishing_Up_Speed", 1);
        Fish_Bite_Bar.fillAmount = 0f;
        //Clear all searched fishes in the array
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Fishes = null;
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Clear_Fish_List();
    }

    public void Fish_Is_Escaped()
    {
        Backpack.Check_Baits();

        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Floater_Follow_Fish_Position = false;
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().current_Strong_Swim_Value = 0;
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Play_Splash_Sound_Big();
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().AnimController.Play("Float_Idle_Animation");
        Fishing_System_Canvas.SetActive(false);
        While_Fishing_Canvas.SetActive(true);
        Change_Camera_To_Player_View();
        Pull_In_Fishing_Line.SetActive(false);
        Pull_Up_Fishing_Line_Bite.SetActive(false);
        Pull_Up_Fishing_Line_No_Bite.SetActive(false);
        Cancel_Fishing_Button.SetActive(false);

        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Reset_All_Fishes();
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Current_Fish = null;
        hasFishBite = false;
        isMouseKeyDown = false;
        isFishInTrigger = false;
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().isSwimmingStrong = false;
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().canPerformStrongSwim = true;
        Player.GetComponent<Animator>().SetFloat("Fishing_Up_Speed", 0f);
        Fish_Bite_Bar.fillAmount = 0f;
        //Clear all searched fishes in the array
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Fishes = null;
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Clear_Fish_List();

        Rotate_To_Water_Button.SetActive(false);
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Place_Float_3D_Canvas.SetActive(false);
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Play_Splash_Sound_Small();
        Pull_Up_Fishing_Line_No_Bite.SetActive(false);
        canPlaceFloat = false;
        isMovingFloat = false;
        isMouseOnAimGraphic = false;
        Aim_Graphic.SetActive(false);
        isFishInTrigger = false;
        Catched_Trigger.GetComponent<Fish_Catched_Trigger>().enabled = true;
        //Disable mesh collider
        Float_Move_Ground.GetComponent<Collider>().enabled = false;
        Fishing_Float_Prefab.GetComponent<Collider>().enabled = true;

        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Trigger_Collider.enabled = true;
        Change_To_Float_Camera_Button.SetActive(true);
        Change_To_Player_Camera_Button.SetActive(false);
        //Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().canTellFishBite = true;
        //Rotate Player to floater
        Vector3 targetPostition = new Vector3(Fishing_Float_Prefab.transform.position.x,
        Player.transform.position.y, Fishing_Float_Prefab.transform.position.z);
        Player.transform.LookAt(targetPostition);
        Backpack.Check_Baits();
        Player.GetComponent<Animator>().SetTrigger("Fishing_Idle");
        FloatIsInWater = true;
        StartCoroutine(Can_Tell_Bites_After_Seconds());
    }

    IEnumerator Can_Tell_Bites_After_Seconds()
    {
        yield return new WaitForSeconds(5);

        if(FloatIsInWater == true & hasFishBite == false)
            Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().canTellFishBite = true;
    }

    public void Keep_Fish()
    {
        //Disable fishing holder.
        Fishing_Holder.SetActive(false);
        Hold_Fish_Camera.SetActive(false);
        if (Current_Fish_Name == "Koi Carp")
        {
            //The player has cathed a Koi_Carp, so add a koi carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Koi_Carp_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Koi_Carp, so disable the koi carp object in the hands
            Koi_Carp_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Bream")
        {
            //The player has cathed a Bream, so add a Bream to the backpack and on his hand
            Fish_Spawned = Instantiate(Bream_Prefab);
            Fish_Spawned = null;
            //The player has cathed a bream, so disable the bream object in the hands
            Bream_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Carp")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Carp_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Carp_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Pike")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Pike_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Pike_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Shiner")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Shiner_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Shiner_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Trout")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Trout_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Trout_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Cod")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Cod_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Cod_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Barracuda")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Barracuda_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Barracuda_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Black Spotted Grunt")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Black_Spotted_Grunt_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Black_Spotted_Grunt_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Butterfly Fish")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Butterfly_Fish_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Butterfly_Fish_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Clownfish")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Clownfish_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Clownfish_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Discus")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Discus_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Discus_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Emperor")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Emperor_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Emperor_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Foureye Butterfly Fish")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Foureye_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Foureye_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Mackerel")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Mackerel_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Mackerel_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Mandarin")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Mandarin_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Mandarin_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Moorish Idol")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Moorish_Idol_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Moorish_Idol_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Nimbochromis")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Nimbochromis_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Nimbochromis_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Orangespine Unicron Fish")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Orangespine_Unicron_Fish_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Orangespine_Unicron_Fish_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Frontosa")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Frontosa_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Frontosa_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Rainbow Cichlid")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Rainbow_Cichlid_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Rainbow_Cichlid_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Salmon")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Salmon_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Salmon_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Siganus Javus")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Siganus_Javus_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Siganus_Javus_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Siganus")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Siganus_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Siganus_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Trewavas Cichlid")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Trewavas_Cichlid_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Trewavas_Cichlid_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Great White Shark")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Great_White_Shark_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            Great_White_Shark_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Blue Marlin")
        {
            //The player has cathed a Carp, so add a Carp to the backpack and on his hand
            Fish_Spawned = Instantiate(Blue_Marlin_Prefab);
            Fish_Spawned = null;
            //The player has cathed a Carp, so disable the Carp object in the hands
            //Blue_Marlin_Object.SetActive(false);
            Blue_Marlin_Catched = GameObject.FindGameObjectWithTag("Catched_Marlin");
            Blue_Marlin_Catched.GetComponent<Enable_Big_Catched_Fish_Animation>().Disable_Big_Catched_Fish_Anim();
        }
        Destroy(Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Current_Fish);
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Current_Fish = null;
        //Clear all searched fishes in the array
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Fishes = null;
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Clear_Fish_List();
        Keep_Release_Fish_Canvas.SetActive(false);
        Pull_Up_The_Line_Without_Fish();
    }

    public void Release_Fish()
    {
        //Disable fishing holder.
        Fishing_Holder.SetActive(false);
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Play_Splash_Sound_Big();
        Hold_Fish_Camera.SetActive(false);
        Koi_Carp_Object.SetActive(false);
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Current_Fish = null;

        if (Current_Fish_Name == "Koi Carp")
        {
            //The player has cathed a Koi_Carp, so disable the koi carp object in the hands
            Koi_Carp_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Bream")
        {
            //The player has cathed a Bream, so disable the Bream object in the hands
            Bream_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Carp")
        {
            //The player has cathed a Carp, so disable the Carp object in the hands
            Carp_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Pike")
        {
            //The player has cathed a Carp, so disable the Carp object in the hands
            Pike_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Shiner")
        {
            //The player has cathed a Carp, so disable the Carp object in the hands
            Shiner_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Trout")
        {
            //The player has cathed a Carp, so disable the Carp object in the hands
            Trout_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Cod")
        {
            Cod_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Barracuda")
        {
            Barracuda_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Black Spotted Grunt")
        {
            Black_Spotted_Grunt_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Butterfly Fish")
        {
            Butterfly_Fish_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Clownfish")
        {
            Clownfish_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Discus")
        {
            Discus_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Emperor")
        {
            Emperor_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Foureye Butterfly Fish")
        {
            Foureye_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Mackerel")
        {
            Mackerel_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Mandarin")
        {
            Mandarin_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Moorish Idol")
        {
            Moorish_Idol_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Nimbochromis")
        {
            Nimbochromis_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Orangespine Unicron Fish")
        {
            Orangespine_Unicron_Fish_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Frontosa")
        {
            Frontosa_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Rainbow Cichlid")
        {
            Rainbow_Cichlid_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Salmon")
        {
            Salmon_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Siganus Javus")
        {
            Siganus_Javus_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Siganus")
        {
            Siganus_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Trewavas Cichlid")
        {
            Trewavas_Cichlid_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Great White Shark")
        {
            Great_White_Shark_Object.SetActive(false);
        }
        if (Current_Fish_Name == "Blue Marlin")
        {
            //Blue_Marlin_Object.SetActive(false);
            Blue_Marlin_Catched = GameObject.FindGameObjectWithTag("Catched_Marlin");
            Blue_Marlin_Catched.GetComponent<Enable_Big_Catched_Fish_Animation>().Disable_Big_Catched_Fish_Anim();
        }

        Keep_Release_Fish_Canvas.SetActive(false);
        Pull_Up_The_Line_Without_Fish();
    }

    public void Cancel_Fishing()
    {
        Main_Camera.gameObject.SetActive(true);
        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().Disable_Float_Camera();
        Fishing_Camera_To_Player.gameObject.SetActive(false);
        Fishing_System_Canvas.SetActive(true);
        While_Fishing_Canvas.SetActive(false);
        Cancel_Fishing_Button.SetActive(false);
        Pull_In_Fishing_Line.SetActive(false);
        Pull_Up_Fishing_Line_Bite.SetActive(false);
        Pull_Up_Fishing_Line_No_Bite.SetActive(false);

        Enable_Player_Movement();

        Fishing_Float_Prefab.GetComponent<Fishing_Float_System>().canTellFishBite = false;
        //Eable Head Wight
        Player.GetComponent<Animator>().SetLayerWeight(14, 1);
        //Player.GetComponent<RealisticEyeMovements.EyeAndHeadAnimator>().headWeight = 1;
        Player.GetComponent<Animator>().Rebind();
        Player.GetComponent<Animator>().Play("Walk_W_Fishing_Rod");

        Backpack.Check_Baits();
    }

    public void Rotate_To_Water()
    {
        Rotate_To_Water_Button.SetActive(false);
        isRotating = true;
    }

    IEnumerator Disable_Rotating_To_Water()
    {
        yield return new WaitForSeconds(2);

        isRotating = false;
        isDisablingRoatation = false;
        if(isMovingFloat == true)
            Rotate_To_Water_Button.SetActive(true);
    }

    //No bite selected warning
    public void Open_No_Bite_Selected_Warning_Window()
    {
        No_Bite_Selected_Warning.SetActive(true);
        No_Bite_Selected_Warning.GetComponent<Animator>().Play("Spawn_Frien_Window_In");
        Cursor.lockState = CursorLockMode.None; //unlock cursor
        Cursor.visible = true; //make mouse visible
    }
    public void Close_No_Bite_Selected_Warning_Window()
    {
        No_Bite_Selected_Warning.GetComponent<Animator>().Play("Spawn_Frien_Window_Out");
        Cursor.lockState = CursorLockMode.Locked; //lock cursor
        Cursor.visible = false; //disable visible mouse
        StartCoroutine(Disable_No_Bite_Selected_Warning_Menu_After_Seconds());
    }
    IEnumerator Disable_No_Bite_Selected_Warning_Menu_After_Seconds()
    {
        yield return new WaitForSeconds(1);
        No_Bite_Selected_Warning.SetActive(false);
        No_Bite_Selected_Warning.GetComponent<Animator>().Rebind();
    }
}
