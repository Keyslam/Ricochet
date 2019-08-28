using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Dependency
{
	/// <summary>
	/// The SceneOrganizer is the only allowed Singleton in the game.
	/// It manages all other globally accessible objects by making them register themself by component type.
	/// </summary>
	public class SceneOrganizer
	{
		// Implementation of singleton pattern.
		public static SceneOrganizer Instance
		{
			get;
			private set;
		}

		// Dictionary of all registered objects.
		private Dictionary<Type, Component> components = new Dictionary<Type, Component>();

		public static void Initialize(SceneOrganizer instance)
		{
			// Implementation of singleton pattern.
			if (Instance != null)
				Debug.LogError("SceneOrganizer was already initialized");

			Instance = instance;
		}

		/// <summary>
		/// Register an object on the SceneOrganizer.
		/// </summary>
		/// <param name="component">Component to register under.</param>
		public static void Register(Component component)
		{
			Instance.RegisterInternal(component);
		}

		private void RegisterInternal(Component component)
		{
			Type type = component.GetType();

			if (components.ContainsKey(type))
			{
				Debug.LogError("Object was already registered");
				return;
			}

			components[type] = component;
		}

		/// <summary>
		/// Unregister an object from the SceneOrganizer.
		/// </summary>
		/// <param name="component">Component to unregister under.</param>
		public static void Unregister(Component component)
		{
			Instance.UnregisterInternal(component);
		}

		private void UnregisterInternal(Component component)
		{
			Type type = component.GetType();

			if (!components.ContainsKey(type))
			{
				Debug.LogError("Object was already unregistered");
				return;
			}

			components[type] = null;
		}

		/// <summary>
		/// Gets an object with a component of type T.
		/// </summary>
		/// <typeparam name="T">Type of component to get.</typeparam>
		/// <returns>A registered component with type T.</returns>
		public static T Get<T>() where T : Component
		{
			return Instance.GetInternal<T>();
		}

		private T GetInternal<T>() where T : Component
		{
			components.TryGetValue(typeof(T), out Component component);
			return component as T;
		}

		/// <summary>
		/// Tries to get an object with a component of type T.
		/// </summary>
		/// <typeparam name="T">Type of component to get.</typeparam>
		/// <param name="component">Reference to component to populate.</param>
		/// <returns>True if object was found, false otherwise.</returns>
		public static bool TryGet<T>(T component) where T : Component
		{
			return Instance.TryGetInternal<T>(component);		
		}

		private bool TryGetInternal<T>(Component component) where T : Component
		{
			return components.TryGetValue(typeof(T), out component);
		}
	}
}
