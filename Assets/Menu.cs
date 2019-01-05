using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
	public Menu previous;
	public Selectable firstSelected;

    private GameObject menuItem;

    void Start()
    {
        menuItem = transform.GetChild(0).gameObject;
    }

	private void Show() {
        menuItem.SetActive (true);
		//firstSelected.Select ();
	}

	public void Show(Menu current) {
		previous = current;
		if (previous != null) {
			previous.Hide ();
		}
		Show ();
	}

	public void Back() {
        menuItem.SetActive (false);
		if (previous != null) {
			previous.Show ();
		}
	}

	public void Hide(){
        menuItem.SetActive (false);
	}
}
