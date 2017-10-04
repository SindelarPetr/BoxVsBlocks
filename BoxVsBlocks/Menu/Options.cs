using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PCLStorage;

namespace BoxVsBlock.Menu
{
	public static class Options
	{
		private static int _bestScore;
		private const string ALL_SCORE_FILE_NAME = "AllScore";

		//public static Score AllScore { get; private set; }
		public static int BestScore
		{
			get => _bestScore;
			set
			{
				if (_bestScore >= value) return;

				_bestScore = value;
			}
		}

		/// <summary>
		/// Loades saved information and decides if the game is started for the first time (if file exist then it has been started already)
		/// </summary>
		/// <returns>Whether the game is started for the first time.</returns>
		public static bool Load()
		{
			// Check if the file has been already created.
			if (!(FileSystem.Current.LocalStorage.CheckExistsAsync(ALL_SCORE_FILE_NAME)).Result.HasFlag(
				ExistenceCheckResult.FileExists))
				return true;

			// Game is started for the first time.
			var file = FileSystem.Current.LocalStorage.GetFileAsync(ALL_SCORE_FILE_NAME).Result;
			var serializedScore = file.ReadAllTextAsync().Result;

			try
			{
				_bestScore = JsonConvert.DeserializeObject<int>(serializedScore);
			}
			catch (Exception ex)
			{
				return false;
			};


			return false;
		}

		/// <summary>
		/// Saves information.
		/// </summary>
		public static async Task Save()
		{
			var serializedScore = JsonConvert.SerializeObject(BestScore);

			var file = await FileSystem.Current.LocalStorage.CreateFileAsync(ALL_SCORE_FILE_NAME,
				CreationCollisionOption.ReplaceExisting);

			await file.WriteAllTextAsync(serializedScore);
		}
	}
}
