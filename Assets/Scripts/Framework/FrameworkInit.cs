using UnityEngine;

namespace Framework
{
	/// <summary>
	/// This class uses 'RuntimeInitializeOnLoadMethod' to initialize a Framework object before any other code is ran.
	/// The prefab is loaded from a Resources folder. Typically 'Framework/Resources'
	/// </summary>
	public static class FrameworkInit
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void RuntimeInitializeOnLoad()
		{
			Framework framework = Resources.Load("Framework", typeof(Framework)) as Framework;

			if (framework == null)
			{
				Debug.LogError("Framework prefab not found in Resources folder");
				return;
			}

			Object.Instantiate(framework);
			framework.Initialize();
		}
	}
}