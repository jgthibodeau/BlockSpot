using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum InputName{
//	PITCH_UP,
//	PITCH_DOWN,
//	ROLL_LEFT,
//	ROLL_RIGHT,
//	WINGS_IN,
//	WINGS_OUT,
//	FLAP,
//	GRAB,
//	CAMERA_LEFT,
//	CAMERA_RIGHT,
//	CAMERA_BACK,
//	PAUSE,
//
//	UI_UP,
//	UI_DOWN,
//	UI_LEFT,
//	UI_RIGHT,
//
//	UI_SELECT,
//	UI_CANCEL
//}
//public enum InputType{
//	AXIS,
//	BUTTON_HELD,
//	BUTTON_PRESS
//}
//public class InputMapping{
//	public string name;
//	public string description;
//	public InputType type;
//
//	public InputMapping(string name, string description, InputType type){
//		this.name = name;
//		this.description = description;
//		this.type = type;
//	}
//}

public class MyInputManager : MonoBehaviour {
//	private string alt = "alt";

//	public Dictionary<InputName,InputMapping> inputMappings;
//
//	string KeyName(InputName inputName, bool altKey){
//		string key = inputName.ToString ();
//		if (altKey) {
//			key += alt;
//		}
//		return key;
//	}
//	void SetInput(InputName inputName, string buttonName, bool altKey){
//		string key = KeyName (inputName, altKey);
//		PlayerPrefs.SetString (key, buttonName);
//	}
//	void UnsetInputByName(InputName inputName, bool altKey){
//		string key = KeyName (inputName, altKey);
//		PlayerPrefs.DeleteKey (key);
//	}
//	void UnsetAllInputsByName(InputName inputName){
//		string key = KeyName (inputName, false);
//		string altkey = KeyName (inputName, true);
//
//		PlayerPrefs.DeleteKey (key);
//		PlayerPrefs.DeleteKey (altkey);
//	}

//	public float GetInput(InputName inputName) {
//		InputMapping inputMapping = inputMappings [inputName];
//
//		string key = KeyName (inputName.ToString (), false);
//		string buttonName = PlayerPrefs.GetString (key);
//
//		switch (inputMapping.type){
//		case InputType.AXIS:
//			return Input.GetAxis (buttonName);
//			break;
//		case InputType.BUTTON_HELD:
//			return Input.GetButton (buttonName);
//			break;
//		case InputType.BUTTON_PRESS:
//			return Input.GetButtonDown (buttonName);
//			break;
//		}
//	}
}
