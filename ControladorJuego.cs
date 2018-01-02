using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControladorJuego : MonoBehaviour {
	public float tiempoNivel = 45.0f;

	public GameObject cargando;

	public Slider barraTiempo;
	public Image imagenDerrota;
	public Image imagenVictoria;
	public Image letreroOpciones;
	public Button botonReset;
	public Button botonSalir;

	public Button botonMago;
	public Button botonGuerrero;
	public Button botonArquera;
	public Button botonBerserker;

	private GameObject fondo;
	private GameObject UI;
	private GameObject combo;
	private ControladorCombo controladorCombo;
	private GameObject guerrero;
	private GameObject arquera;
	private GameObject mago;
	private GameObject berserker;
	private ControladorPersonaje controladorHeroe;
	private GameObject enemigo;
	private ControladorPersonaje controladorEnemigo;
	private GameObject vida;
	private int contadorVida;
	private int contadorEnemigoCombo;
	private bool acabaDeRecibirDaño = false;
	private bool haPerdido = false;
	private bool haGanado = false;
	private List<string> enemigos = new List<string> ();
	private const string PREFABS_RAIZ = "Prefabs/";
	private const string ENEMIGOS_RAIZ = PREFABS_RAIZ + "Enemigos/";
	private const string COMBOS_RAIZ = PREFABS_RAIZ + "Combos/Combo";

	private void Start(){
		if (SceneManager.GetActiveScene ().name != "Inicio") {
			barraTiempo.maxValue = tiempoNivel;
			barraTiempo.value = barraTiempo.maxValue;

			fondo = GameObject.FindGameObjectWithTag ("Fondo");
			UI = GameObject.FindGameObjectWithTag ("UI");
			combo = GameObject.FindGameObjectWithTag ("Combo");
			guerrero = GameObject.FindGameObjectWithTag ("Guerrero");
			arquera = GameObject.FindGameObjectWithTag ("Arquera");
			mago = GameObject.FindGameObjectWithTag ("Mago");
			berserker = GameObject.FindGameObjectWithTag ("Berserker");
			enemigo = GameObject.FindGameObjectWithTag ("Enemigo");
			vida = GameObject.FindGameObjectWithTag ("Vida");
			contadorVida = 3;
			contadorEnemigoCombo = 0;

			enemigos.Add ("Platonio");
			enemigos.Add ("Naranjado");
			enemigos.Add ("Limonado");
			enemigos.Add ("Cerdonio");
			enemigos.Add ("Nathanoria");
			enemigos.Add ("Naranjado");
			enemigos.Add ("Platonio");
			enemigos.Add ("Cerdonio");
			enemigos.Add ("Nathanoria");
			enemigos.Add ("Yasaichi");

			cargarProximoEnemigo ();
		}
	}

	void Update(){
		StartCoroutine (actualizarTiempo ());
	}

	IEnumerator actualizarTiempo(){
		if (barraTiempo != null && !haPerdido && !haGanado) {
			barraTiempo.value -= Time.deltaTime;

			if (barraTiempo.value <= 0) {
				haPerdido = true;
				controlesActivos (false);

				vida.gameObject.SetActive (false);
				combo.SetActive (false);
				fondo.GetComponents<AudioSource> () [0].Stop ();
				fondo.GetComponents<AudioSource> () [1].Play ();
				imagenDerrota.gameObject.SetActive (true);
				yield return new WaitForSeconds(0.5f);
				letreroOpciones.gameObject.SetActive (true);
				botonSalir.gameObject.SetActive (true);
				botonReset.gameObject.SetActive (true);
			}
		}
	}

	public void cargarEscenaBatalla(){
		GameObject.FindGameObjectWithTag ("Fondo").GetComponent<AudioSource> ().Stop ();
		this.GetComponent<AudioSource> ().Play ();
		SceneManager.LoadScene ("Assets/_Scenes/BatallaEjemplo.unity");
	}

	public void cargarEscenaInicio(){
		GameObject.FindGameObjectWithTag ("Fondo").GetComponent<AudioSource> ().Stop ();
		this.GetComponent<AudioSource> ().Play ();
		SceneManager.LoadScene ("Assets/_Scenes/Inicio.unity");
	}

	public void botonPulsado(Button boton){
		StartCoroutine (controlarPulsacion (boton.tag));
	}

	IEnumerator controlarPulsacion(string tagBoton){
		GameObject personajeSeleccionado = null;

		switch (tagBoton) {
		case "BotonMago":
			personajeSeleccionado = mago;
			break;
		case "BotonGuerrero":
			personajeSeleccionado = guerrero;
			break;
		case "BotonArquera":
			personajeSeleccionado = arquera;
			break;
		case "BotonBerserker":
			personajeSeleccionado = berserker;
			break;
		}
		controladorCombo = (ControladorCombo) combo.GetComponent(typeof(ControladorCombo));
		controladorHeroe = (ControladorPersonaje)personajeSeleccionado.GetComponent (typeof(ControladorPersonaje));
		controladorEnemigo = (ControladorPersonaje)enemigo.GetComponent (typeof(ControladorPersonaje));

		if (controladorCombo.pulsacionCorrecta (tagBoton)) {
			controladorCombo.marcarAcierto ();

			acabaDeRecibirDaño = false;
			bool comboCompletado = controladorCombo.completado ();

			if (comboCompletado) {
				controlesActivos (false);

				if (contadorEnemigoCombo >= 10)
					haGanado = true;

				StartCoroutine (controladorHeroe.atacar ("Rogue_attack_01", comboCompletado));
				yield return new WaitWhile (() => controladorHeroe.estaAtacando ());

				StartCoroutine (controladorEnemigo.morir ("Rogue_death_01"));
				yield return new WaitWhile (() => controladorEnemigo.estaMuriendo ());

				Destroy (enemigo);
				Destroy (combo);

				if (contadorEnemigoCombo < 10) {
					cargarProximoEnemigo ();
					controlesActivos (true);
				} else {
					vida.SetActive (false);
					imagenVictoria.gameObject.SetActive (true);
					yield return new WaitForSeconds(2);
					letreroOpciones.gameObject.SetActive (true);
					botonSalir.gameObject.SetActive (true);
					botonReset.gameObject.SetActive (true);
				}
			} else {
				StartCoroutine (controladorHeroe.atacar ("Rogue_attack_03", comboCompletado));
				StartCoroutine (controladorEnemigo.serAtacado ("Rogue_hit_01"));
				yield return new WaitWhile (() => controladorEnemigo.estaSiendoAtacado ());
			}
		} else if (!acabaDeRecibirDaño) {
			controlesActivos (false);
			acabaDeRecibirDaño = true;

			contadorVida--;
			vida.transform.GetChild (contadorVida).gameObject.SetActive (false);

			StartCoroutine (controladorEnemigo.atacar ("Rogue_attack_03"));
			yield return new WaitWhile (() => controladorEnemigo.estaAtacando ());

			if (contadorVida > 0) {
				StartCoroutine (controladorHeroe.serAtacado ("Rogue_hit_01"));
				yield return new WaitWhile (() => controladorHeroe.estaSiendoAtacado ());

				controladorCombo.reiniciarCombo ();
				controlesActivos (true);
			} else {
				combo.SetActive (false);
				haPerdido = true;
				controladorHeroe = (ControladorPersonaje)guerrero.GetComponent (typeof(ControladorPersonaje));
				StartCoroutine (controladorHeroe.morir ("Rogue_death_01"));
				controladorHeroe = (ControladorPersonaje)arquera.GetComponent (typeof(ControladorPersonaje));
				StartCoroutine (controladorHeroe.morir ("Rogue_death_01"));
				controladorHeroe = (ControladorPersonaje)mago.GetComponent (typeof(ControladorPersonaje));
				StartCoroutine (controladorHeroe.morir ("Rogue_death_01"));
				controladorHeroe = (ControladorPersonaje)berserker.GetComponent (typeof(ControladorPersonaje));
				StartCoroutine (controladorHeroe.morir ("Rogue_death_01"));

				fondo.GetComponents<AudioSource> () [0].Stop ();
				fondo.GetComponents<AudioSource> () [1].Play ();
				imagenDerrota.gameObject.SetActive (true);

				yield return new WaitForSeconds(0.5f);
				letreroOpciones.gameObject.SetActive (true);
				botonSalir.gameObject.SetActive (true);
				botonReset.gameObject.SetActive (true);
			}
		}
	}

	private void cargarProximoEnemigo(){
		enemigo = (GameObject)Instantiate (Resources.Load (ENEMIGOS_RAIZ + enemigos.ToArray()[contadorEnemigoCombo]));
		combo = (GameObject)Instantiate (Resources.Load (COMBOS_RAIZ + enemigos.ToArray()[contadorEnemigoCombo]), UI.transform);

		contadorEnemigoCombo++;
	}

	public void controlesActivos(bool activos){
		botonMago.interactable = activos;
		botonGuerrero.interactable = activos;
		botonArquera.interactable = activos;
		botonBerserker.interactable = activos;
	}
}