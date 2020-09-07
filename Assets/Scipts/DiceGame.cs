using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public sealed partial class DiceGame : MonoBehaviour
{
	[SerializeField] GameObject dicePrefab;
	[SerializeField] Transform cubeStartPlace;
	[SerializeField] Button throwButton;
	[SerializeField] Text scoreText;
	[SerializeField] GameObject scoreWindow;
	[SerializeField] GameObject lobbyWindow;
	[SerializeField] GameObject throwWindow;
	[SerializeField] GameObject winnerWindow;
	[SerializeField] Button createPlayerButton;
	[SerializeField] InputField playerName;
	[SerializeField] GameObject playerListElementPrefab;
	[SerializeField] RectTransform playerListElementParent;
	[SerializeField] Button oneRoundGameButton;
	[SerializeField] Button queueGameButton;
	[SerializeField] Text activePlayerText;
	[SerializeField] Text scoreList;
	[SerializeField] Text winnerText;
	[SerializeField] Button toLobbyMenuButton;
	[SerializeField] Button quitButton;

	ManagedList<Player> players = new ManagedList<Player>();
	List<GameObject> menus = new List<GameObject>();
	GameRulesProvider gameRules;

	private void ShowMenu(GameObject menuRoot)
	{
		foreach (var menu in menus)
		{
			menu.SetActive(menuRoot == menu);
		}
	}

	private void RefreshScoreList()
	{
		string text = string.Empty;
		foreach (var player in players)
		{
			text += string.Format("{0}\n", gameRules.GetPlayerDesc(player));
		}
		scoreList.text = text;
		activePlayerText.text = string.Format("Ход игрока {0}", gameRules.GetCurrentPlayer().name);
	}

	private void ThrowDicesByPlayer(Player player, int count, System.Func<int, int> scoresToAdd, Action callback)
	{
		Action<int> score_callback = i =>
		{
			i = scoresToAdd(i);
			player.scores += i;
			//RefreshScoreList();
			callback();
		};

		score_callback += EndGameRound;
		StartCoroutine(ThrowCubesAndWaitScore(count, score_callback));
	}

	private void GameRound()
	{
		scoreText.text = string.Empty;
		StartCoroutine(ThrowCubesAndWaitScore(2, EndGameRound));
	}

	private void ShowScoreWindow(int score)
	{
		scoreText.text = string.Format("{0}\n{1}", score.ToString(), gameRules.GetScoreDesc(score));
		ShowMenu(scoreWindow);
	}
	private void ShowThrowWindow()
	{
		if (gameRules.MoveNextPlayer())
		{
			RefreshScoreList();
			ShowMenu(throwWindow);
		}
	}

	private void ShowLobbyWindow()
	{
		foreach (var player in players)
		{
			player.Reset();
		}
		ShowMenu(lobbyWindow);
	}

	private void ShowWinnerWindow()
	{
		var winners = gameRules.GetWinners();

		int count = winners.Count();
		string text;
		if (count > 0) text = count > 1 ? "Победители :" : "Победитель :";
		else text = "Никто";
		

		foreach (var winner in winners)
		{
			text += string.Format("\n{0}", winner.name);
		}
		winnerText.text = text;

		ShowMenu(winnerWindow);

		Invoke("ShowLobbyWindow", 2f);
	}

	private IEnumerator ScoreWindowRoutine(int score)
	{
		ShowScoreWindow(score);
		yield return new WaitForSeconds(1f);
		ShowThrowWindow();
	}

	private void EndGameRound(int score)
	{
		StartCoroutine(ScoreWindowRoutine(score));
		gameRules.ScoreCallback(score);
	}

	private void CreatePlayer(string name)
	{
		if (players.Count >= 8) return;

		Player player;
		players.Add(player = new Player(name));

		GameObject listElement = Instantiate(playerListElementPrefab, playerListElementParent);
		listElement.GetComponentInChildren<Button>().onClick.AddListener(() =>
		{
			Destroy(listElement.gameObject);
			players.Remove(player);
		});
		listElement.GetComponentInChildren<Text>().text = name;
	}

	private void OnPlayerNameEdit(string name)
	{
		createPlayerButton.interactable = !string.IsNullOrEmpty(name);
	}

	private void OnPlayerNameEndEdit(string name)
	{
		if (!string.IsNullOrEmpty(name))
		{
			CreatePlayer(playerName.text);
			playerName.text = string.Empty;
		}
	}

	private void StartGame(GameRulesProvider gameRules)
	{
		this.gameRules = gameRules;
		this.gameRules.OnStartGame();
	}

	private void Start()
	{
		menus.Add(lobbyWindow);
		menus.Add(throwWindow);
		menus.Add(scoreWindow);
		menus.Add(winnerWindow);

		players.ListChanged += () =>
		{
			oneRoundGameButton.interactable = queueGameButton.interactable = players.Count > 1;
		};

		queueGameButton.onClick.AddListener(() => StartGame(new QueueGameRules(this)));
		oneRoundGameButton.onClick.AddListener(() => StartGame(new OneRoundGameRules(this)));

		playerName.onValueChanged.AddListener(OnPlayerNameEdit);
		playerName.onEndEdit.AddListener(OnPlayerNameEndEdit);
		createPlayerButton.interactable = false;
		createPlayerButton.onClick.AddListener(() => CreatePlayer(playerName.text));
		throwButton.onClick.AddListener(GameRound);
		toLobbyMenuButton.onClick.AddListener(ShowLobbyWindow);
		quitButton.onClick.AddListener(Application.Quit);

		ShowLobbyWindow();
	}

	private IEnumerator ThrowCubesAndWaitScore(int cubesCount, System.Action<int> callback)
	{
		var wait = new WaitForEndOfFrame();

		int cubesBack = 0;
		int score = 0;

		DiceCube[] cubes = new DiceCube[cubesCount];

		for (int i = 0; i < cubesCount; i++)
		{
			Vector3 positionOffset = dicePrefab.transform.TransformVector(new Vector3(i - (cubesCount / 2), 0f, 0f));
			DiceCube diceCube = Instantiate(dicePrefab, cubeStartPlace.position + positionOffset, cubeStartPlace.rotation).GetComponent<DiceCube>();
			cubes[i] = diceCube;
		}

		yield return wait;

		System.Action<int> cubes_callback = i =>
		{
			cubesBack++;
			score += i;
		};

		for (int i = 0; i < cubesCount; i++) cubes[i].ThrowAndWaitValue(cubes_callback);

		while (cubesBack < cubesCount)
		{
			yield return wait;
		}

		yield return new WaitForSeconds(1f);

		for (int i = 0; i < cubesCount; i++) Destroy(cubes[i].gameObject);

		callback(score);
	}
}
