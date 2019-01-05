using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadButtons : MonoBehaviour {
	private enum GAMEPAD_TYPE {
		XBOX
	}

	private static string joystickbutton = "Joystick1Button";

	private static Dictionary<RuntimePlatform, Dictionary<GAMEPAD_TYPE, Dictionary<string, string>>> mappings = new Dictionary<RuntimePlatform, Dictionary<GAMEPAD_TYPE, Dictionary<string, string>>> ();

	private void Awake() {
		SetMappings ();
	}

	public string GetMapping(string button) {
		GAMEPAD_TYPE inputType = GAMEPAD_TYPE.XBOX;
		string map = "";
		if (mappings.ContainsKey (Application.platform)
		    && mappings [Application.platform].ContainsKey (inputType)
		    && mappings [Application.platform] [inputType].ContainsKey (button)) {
			map = mappings [Application.platform] [inputType] [button];
		}
        Debug.Log("converted '" + button+"' to '" + map + "'");
		return map;
	}

	private void SetMappings(){
		mappings [RuntimePlatform.WindowsEditor] = new Dictionary<GAMEPAD_TYPE, Dictionary<string, string>> ();
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX] = new Dictionary<string, string> ();
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add (joystickbutton + "0", "A");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add (joystickbutton + "1", "B");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add (joystickbutton + "2", "X");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add (joystickbutton + "3", "Y");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add (joystickbutton + "4", "Left Bumper");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add (joystickbutton + "5", "Right Bumper");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add (joystickbutton + "6", "Back");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add (joystickbutton + "7", "Start");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add (joystickbutton + "8", "Left Stick Click");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add (joystickbutton + "9", "Right Stick Click");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add ("0", "Left Stick X");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add ("1", "Left Stick Y");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add ("2", "Triggers");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add ("3", "Right Stick X");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add ("4", "Right Stick Y");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add ("5", "D-Pad X");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add ("6", "D-Pad Y");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add ("8", "Left Trigger");
		mappings [RuntimePlatform.WindowsEditor] [GAMEPAD_TYPE.XBOX].Add ("9", "Right Trigger");

		mappings [RuntimePlatform.WindowsPlayer] = mappings [RuntimePlatform.WindowsEditor];
		mappings [RuntimePlatform.WebGLPlayer] = mappings [RuntimePlatform.WindowsEditor];
	}
}