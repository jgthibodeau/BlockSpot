using UnityEngine;
using System.Collections;

public class ShowPanels : MonoBehaviour {
	public Menu current = null;
    public static ShowPanels instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public bool NoMenu()
    {
        return current == null;
    }

    public bool InMenu()
    {
        return current != null;
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

    public void DoubleBack()
    {
        Back();
        Back();
    }

    public void Close()
    {
        if (current != null)
        {
            if (current.previous != null)
            {
                current.previous = null;
            }
            current.Hide();
            current = null;
        }
    }
}
