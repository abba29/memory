﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	
	private int gridRows, gridCols;
	private float offsetX, offsetY;

	private int[] numbers;

	[SerializeField] private MemoryCard originalCard;
	[SerializeField] private Sprite[] images;
	[SerializeField] private TextMesh scoreLabel;
	
	private MemoryCard _firstRevealed;
	private MemoryCard _secondRevealed;

	private int _score = 0;
	private int maxHealth, currentHealth;

	public HealthBar healthBar;

	// Use this for initialization
	void Start() {
		
		Vector3 startPos = originalCard.transform.position;

		if (PlayerPrefs.GetString("LastGameModeSelected") == "Easy")
		{
			EasyGameInitialization();
        } else
        {
			HardGameInitialization();
			// Adjust the position of the first card to make the spawned cards fit in the screen size
			originalCard.transform.position = new Vector3(startPos.x, startPos.y + 0.4f, startPos.z);
        }

		startPos = originalCard.transform.position;

		currentHealth = maxHealth;
		healthBar.SetMaxHealth(maxHealth);

		// place cards in a grid
		for (int i = 0; i < gridCols; i++) {
			for (int j = 0; j < gridRows; j++) {
				MemoryCard card;

				// use the original for the first grid space
				if (i == 0 && j == 0) {
					card = originalCard;
				} else {
					card = Instantiate(originalCard) as MemoryCard;
				}

				card.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);

				// next card in the list for each grid space
				int index = j * gridCols + i;
				int id = numbers[index];
				card.SetCard(id, images[id]);

				float posX = (offsetX * i) + startPos.x;
				float posY = -(offsetY * j) + startPos.y;
				card.transform.position = new Vector3(posX, posY, startPos.z);
			}
		}
	}

	// Knuth shuffle algorithm
	private int[] ShuffleArray(int[] numbers) {
		int[] newArray = numbers.Clone() as int[];
		for (int i = 0; i < newArray.Length; i++ ) {
			int tmp = newArray[i];
			int r = Random.Range(i, newArray.Length);
			newArray[i] = newArray[r];
			newArray[r] = tmp;
		}
		return newArray;
	}

	public void CardRevealed(MemoryCard card) {
		if (_firstRevealed == null) {
			_firstRevealed = card;
		} else {
			_secondRevealed = card;
			StartCoroutine(CheckMatch());
		}
	}
	
	private IEnumerator CheckMatch() {

		// increment score if the cards match
		if (_firstRevealed.id == _secondRevealed.id) {
			_score++;
			scoreLabel.text = "Score: " + _score;
		}

		// otherwise turn them back over after .5s pause
		else {
			yield return new WaitForSeconds(.5f);

			_firstRevealed.Unreveal();
			_secondRevealed.Unreveal();

			currentHealth -= 1;
			healthBar.SetHealth(currentHealth);

			if (currentHealth == 0)
			{
				Debug.Log("You Lose.");
				Time.timeScale = 0f;
			}
		}
		
		_firstRevealed = null;
		_secondRevealed = null;
	}

	public bool canReveal
	{
		get { return _secondRevealed == null; }
	}

	public void EasyGameInitialization()
    {
		maxHealth = 3;

		gridRows = 2;
		gridCols = 4;
		offsetX = 1.5f;
		offsetY = 1.8f;

		// create shuffled list of cards
		numbers = new int[] { 0, 0, 1, 1, 2, 2, 3, 3 };
		numbers = ShuffleArray(numbers);
	}

	public void HardGameInitialization()
	{
		maxHealth = 5;

		gridRows = 3;
		gridCols = 4;
		offsetX = 1.5f;
		offsetY = 1.5f;

		// create shuffled list of cards
		numbers = new int[] { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5 };
		numbers = ShuffleArray(numbers);
	}
}