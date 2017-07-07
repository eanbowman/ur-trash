using UnityEngine;  
using System.Collections;  
using UnityEngine.EventSystems;  
using UnityEngine.UI;

public class HoverInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	public Color colour;
	public Color hover = Color.red;
	public Color click = Color.blue;
	public Text theText;

	void Start() {
		colour = theText.color;
		gameObject.GetComponent<Button>().onClick.AddListener(TaskOnClick);
	}

	public void TaskOnClick() {
		theText.color = click;
		Debug.Log (gameObject.name + "Clicked!");
	}

	public void OnPointerEnter(PointerEventData eventData) {
		theText.color = hover;
	}

	public void OnPointerExit(PointerEventData eventData) {
		theText.color = colour;
	}
}
