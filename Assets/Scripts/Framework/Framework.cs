using UnityEngine;
using Framework.Dependency;

namespace Framework
{
	/// <summary>
	/// Each game has one Framework that persists between scenes.
	/// It manages all required modules and initializes them before any other code is ran.
	/// </summary>
	public class Framework : MonoBehaviour
	{
		private static bool isCreated = false;

		/// <summary>
		/// Initializes the Framework. 
		/// Sets up all required modules.
		/// </summary>
		public void Initialize()
		{
			SceneOrganizer.Initialize(new SceneOrganizer());
		}

		private void Awake()
		{
			// Only one Framework can be active in the game.
			Debug.Assert(!isCreated, "Only one Framework is allowed");
			isCreated = true;

			// The Framework should persist between scenes.
			DontDestroyOnLoad(this);
		}
	}
}
