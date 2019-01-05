using UnityEngine;
using System.Collections;
using TouchControlsKit;

public class Pause : MonoBehaviour {
	private ShowPanels showPanels;						//Reference to the ShowPanels script used to hide and show UI panels
	public Menu pauseMenu;
	public bool pauseOnStartup;
	public bool isPaused;								//Boolean to check if the game is paused or not
	private StartOptions startScript;					//Reference to the StartButton script

    //Awake is called before Start()
    void Awake()
    {
        //Get a component reference to ShowPanels attached to this object, store in showPanels variable
        showPanels = GetComponent<ShowPanels> ();
		//Get a component reference to StartButton attached to this object, store in startScript variable
		startScript = GetComponent<StartOptions> ();

		if (pauseOnStartup) {
			DoPause (false);
		}
	}

	// Update is called once per frame
	void Update () {
        bool pauseTriggered = Util.GetButtonDown("Pause") || TCKInput.GetAction("Pause", EActionEvent.Down);

        if (pauseTriggered)
        {
            if (!isPaused && !startScript.inMainMenu && MyGameManager.instance.CanPause())
            {
                DoPause();
            }
            else if (isPaused && !startScript.inMainMenu)
            {
                UnPause();
            }
        }
	
	}

    public void DoPause()
    {
        Debug.Log("Pausing");
        DoPause(true);
        MyGameManager.instance.HandlePause();
    }

    public void DoPause(bool showMenu)
    {
        //Set isPaused to true
        isPaused = true;
        //Set time.timescale to 0, this will cause animations and physics to stop updating
        Time.timeScale = 0;
        //call the ShowPausePanel function of the ShowPanels script
        if (showMenu)
        {
            showPanels.Show(pauseMenu);
        }
    }

    public void UnPause()
    {
        Debug.Log("Unpausing");

        //call the HidePausePanel function of the ShowPanels script
        showPanels.Back();

        if (showPanels.NoMenu())
        {
            //Set isPaused to false
            isPaused = false;
            //Set time.timescale to 1, this will cause animations and physics to continue updating at regular speed
            Time.timeScale = 1;
        }

        MyGameManager.instance.HandleUnPause();
    }
}
