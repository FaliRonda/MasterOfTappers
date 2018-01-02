using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorPersonaje : MonoBehaviour {
	protected Animator animator;
	protected AudioSource sonidoEspada;
	protected AudioSource sonidoMuerte;
	protected bool atacando;
	protected bool siendoAtacado;
	protected bool muriendo;

	protected virtual void Start(){
		siendoAtacado = false;
		muriendo = false;
		atacando = false;
	}

	public bool estaAtacando(){
		return atacando;
	}

	public bool estaSiendoAtacado(){
		return siendoAtacado;
	}

	public bool estaMuriendo(){
		return muriendo;
	}

	public IEnumerator morir(string nombreAnimacion){
		muriendo = true;
		animator = this.GetComponent<Animator>();
		animator.SetTrigger("morir");
		sonidoMuerte = GetComponents<AudioSource> ()[1];
		if(sonidoMuerte !=null)
			sonidoMuerte.Play ();
//		yield return new WaitWhile (() => !animator.GetCurrentAnimatorStateInfo (0).IsName (nombreAnimacion));
		yield return new WaitForSeconds(1);
		muriendo = false;
	}

	public IEnumerator serAtacado(string nombreAnimacion){
		siendoAtacado = true;
		animator = this.GetComponent<Animator>();
		animator.SetTrigger("serAtacado");
//		yield return new WaitWhile (() => !animator.GetCurrentAnimatorStateInfo (0).IsName (nombreAnimacion));
		yield return new WaitForSeconds(0.2f);
		siendoAtacado = false;
	}

	public IEnumerator atacar(string nombreAnimacion){
		yield return atacar (nombreAnimacion, false);
	}

	public IEnumerator atacar(string nombreAnimacion, bool rematar){
		atacando = true;
		animator = this.GetComponent<Animator>();
		if (rematar) {
			animator.ResetTrigger ("atacar");
			animator.SetTrigger ("rematar");
		} else
			animator.SetTrigger("atacar");
		sonidoEspada = GetComponent<AudioSource> ();
		sonidoEspada.Play ();
//		yield return new WaitWhile (() => !animator.GetCurrentAnimatorStateInfo (0).IsName (nombreAnimacion));
		yield return new WaitForSeconds(0.2f);
		atacando = false;
	}
}
