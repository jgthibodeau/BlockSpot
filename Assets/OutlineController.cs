using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
	public Color color;
	public HighlightsFX.SortingType depthType;
	public List<GameObject> highlightedObjects;

//    [System.Serializable]
//    public class OutlineData
//    {
//        public Color color = Color.white;
//        public HighlightsFX.SortingType depthType;
//        public Renderer renderer;
//    }

    public HighlightsFX outlinePostEffect;
//    public OutlineData[] outliners;

//    private void Start()
//    {
//        foreach (var obj in outliners)
//        {
//            outlinePostEffect.AddRenderers(
//                new List<Renderer>() { obj.renderer }, 
//                obj.color, 
//                obj.depthType);
//        }
//    }

    void Update()
    {
        int highlightCount = highlightedObjects.Count;
        highlightedObjects.RemoveAll(item => item == null);

        //if (highlightCount != highlightedObjects.Count)
        //{
            UpdateHighlight();
        //}
    }

	public void SetRenderers(GameObject go) {
		if (go == null) {
			Debug.Log ("removing highlight");
			outlinePostEffect.ClearOutlineData ();
		}
		else/* if (highlightedObject != go)*/ {
			Debug.Log ("highlighting " + go);
			outlinePostEffect.ClearOutlineData ();
			if (go != null) {
				List<Renderer> renderers = new List<Renderer> (go.GetComponentsInChildren<Renderer> ());
				for (int i = renderers.Count - 1; i >= 0; i--) {
					if (renderers [i] is ParticleSystemRenderer) {
						renderers.RemoveAt (i);
					}
//					Debug.Log(renderers[i]);
				}
				outlinePostEffect.AddRenderers (renderers, color, depthType);
			}
		}
	}

    public void AddObject(GameObject go)
    {
        if (!highlightedObjects.Contains(go))
        {
            highlightedObjects.Add(go);
            UpdateHighlight();
        }
    }

    public void RemoveObject(GameObject go)
    {
        if (highlightedObjects.Contains(go))
        {
            highlightedObjects.Remove(go);
            UpdateHighlight();
        }
    }

	public void UpdateHighlight() {
		outlinePostEffect.ClearOutlineData ();
        foreach (GameObject go in highlightedObjects)
        {
            List<Renderer> renderers = new List<Renderer>(go.GetComponentsInChildren<Renderer>());
            HighlightRenderers(renderers, color);
        }
	}

	public void HighlightRenderers(List<Renderer> renderers, Color color){
		for (int i = renderers.Count - 1; i >= 0; i--) {
			if (renderers [i] is ParticleSystemRenderer) {
				renderers.RemoveAt (i);
			}
		}
		outlinePostEffect.AddRenderers (renderers, color, depthType);
	}
}
