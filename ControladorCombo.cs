using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorCombo : MonoBehaviour {
	private List<string> elementosCombo;
	private int cont;
	private Image[] spritesCombo;
	private bool comboCompletado;
	private AudioSource audioExito;

	private void Start(){
		spritesCombo = GetComponentsInChildren<Image> ();
		elementosCombo = new List<string> ();

		foreach (Image sprite in spritesCombo){
			if(sprite.tag != "Combo")
				elementosCombo.Add (sprite.tag);
		}

		cont = 0;
		comboCompletado = false;
		audioExito = GetComponent<AudioSource> ();
	}

	public bool pulsacionCorrecta(string tagBoton){
		bool res;

		if(tagBoton.Equals(elementosCombo[cont])) {
			cont++;
			comboCompletado = cont == elementosCombo.Count;
			res = true;
		} else {
			res = false;
		}

		return res;
	}

	public bool completado(){
		if (comboCompletado)
			audioExito.Play ();
		return comboCompletado;
	}

	public void marcarAcierto(){
		Color color = spritesCombo [cont-1].color;
		color.a = 0.5f;
		spritesCombo [cont-1].color = color;
	}

	public void reiniciarCombo(){
		for (int i = cont; i > 0; i--) {
			Color color = spritesCombo [i-1].color;
			color.a = 1f;
			spritesCombo [i-1].color = color;
		}

		cont = 0;
	}
}