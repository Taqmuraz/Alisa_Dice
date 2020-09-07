using System.Collections.Generic;

public sealed partial class DiceGame
{
	private sealed class QueueGameRules : GameRulesProvider
	{
		List<Player> playersList = new List<Player>();
		bool lockPlayer;
		int player = -1;

		public QueueGameRules(DiceGame game) : base(game)
		{
			playersList = new List<Player>(game.players);
		}

		public override void OnStartGame()
		{
			game.ShowThrowWindow();
		}

		public override int GetDicesCount()
		{
			return 2;
		}

		public override bool MoveNextPlayer()
		{
			if (playersList.Count <= 1)
			{
				game.ShowWinnerWindow();
				return false;
			}

			if (lockPlayer) return true;

			player++;
			if (player >= game.players.Count) player = 0;
			while (!playersList.Contains(game.players[player])) player++;

			return true;
		}

		public override Player GetCurrentPlayer()
		{
			return game.players[player];
		}

		public override string GetPlayerDesc(Player player)
		{
			return string.Format("{0} ({1})", player.name, playersList.Contains(player) ? "Играет" : "Проиграл");
		}

		public override IEnumerable<Player> GetWinners()
		{
			return playersList;
		}

		public override void ScoreCallback(int score)
		{
			if (lockPlayer)
			{
				switch (score)
				{
					case 7: lockPlayer = false; playersList.Remove(GetCurrentPlayer()); break;

					case 2: return;
					case 8: return;
					case 12: return;
					case 11: return;

					default:
						GetCurrentPlayer().scores += score;
						lockPlayer = false;
						break;
				}
			}
			else
			{
				switch (score)
				{
					case 7: GetCurrentPlayer().scores += score; break;
					case 11: GetCurrentPlayer().scores += score; break;

					case 2: playersList.Remove(GetCurrentPlayer()); return;
					case 8: playersList.Remove(GetCurrentPlayer()); return;
					case 12: playersList.Remove(GetCurrentPlayer()); return;

					default:
						lockPlayer = true;
						break;
				}
			}
		}

		public override string GetScoreDesc(int score)
		{
			string win = "Победа";
			string loose = "Проигрыш";
			string locked = "Ещё один ход";

			if (lockPlayer)
			{
				switch (score)
				{
					case 7: return loose;

					case 2: return locked;
					case 8: return locked;
					case 12: return locked;
					case 11: return locked;

					default: return win;
				}
			}
			else
			{
				switch (score)
				{
					case 7: return win;
					case 11: return win;

					case 2: return loose;
					case 8: return loose;
					case 12: return loose;

					default: return locked;
				}
			}
		}
	}
}
