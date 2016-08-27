﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

namespace aggrathon.ld36
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager instance;

		public GameObject carPrefab;
		public GameObject cameraPrefab;
		public GameObject aiPrefab;

		public Transform[] spawnLocations;

		public GameObject outroScreen;
		public Text victoryText;

		[NonSerialized] public CarController[] cars;

		void Awake()
		{
			instance = this;
			cars = new CarController[PlayerData.Players.Length];
			for (int i = 0; i < PlayerData.Players.Length; i++)
			{
				if (PlayerData.Players[i].controller == PlayerData.Controller.ai)
				{
					GameObject go = Instantiate(aiPrefab);
					go.name = PlayerData.Players[i].name;
					cars[i] = (Instantiate(carPrefab, spawnLocations[i].position, spawnLocations[i].rotation, go.transform) as GameObject).GetComponent<CarController>();
					go.GetComponent<AiController>().Setup(cars[i]);
				}
				else
				{
					GameObject go = new GameObject(PlayerData.Players[i].name);
					cars[i] = (Instantiate(carPrefab, spawnLocations[i].position, spawnLocations[i].rotation, go.transform) as GameObject).GetComponent<CarController>();
					CameraController cam = (Instantiate(cameraPrefab, spawnLocations[i].position, spawnLocations[i].rotation, go.transform) as GameObject).GetComponent<CameraController>();
					go.AddComponent<PlayerController>().Setup(cars[i], cam, PlayerData.Players[i].controller);
				}
			}
			for (int i = 0; i < cars.Length; i++)
			{
				cars[i].onDestroyed += (car) =>
				{
					cars = (from c in cars where c != car select c).ToArray();
					if(cars.Length == 1)
					{
						victoryText.text = string.Format(victoryText.text, cars[0].transform.parent.gameObject.name);
						outroScreen.SetActive(true);
					}
				};
			}
		}

		void Update()
		{
			if(Input.GetButton("Cancel"))
			{
				MainMenu();
			}
		}

		public void LockAllCars()
		{
			for (int i = 0; i < cars.Length; i++)
			{
				cars[i].rigidbody.isKinematic = true;
			}
		}

		public void UnlockAllCars()
		{
			for (int i = 0; i < cars.Length; i++)
			{
				cars[i].rigidbody.isKinematic = false;
			}
		}

		public void Restart()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		public void MainMenu()
		{
			SceneManager.LoadScene(0);
		}
	}
}
