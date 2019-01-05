using UnityEngine;
using System.Collections;

public class ShowPanels : MonoBehaviour {
	public Menu current = null;

	public bool NoMenu() {
		return current == null;
	}

	public void Show(Menu menu) {
		menu.Show (current);
		current = menu;
	}

	public void Back() {
		if (current != null) {
			current.Back ();
			current = current.previous;
		}
	}

    public void Close()
    {
        current.previous = null;
        current.Hide();
        current = null;
    }
}
