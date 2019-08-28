using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Framework.Values
{
	/// <summary>
	/// Base class for 'Values'.
	/// Values implement the observer pattern, emiting an event when the value is changed.
	/// </summary>
	/// <typeparam name="T">Type of value to create.</typeparam>
	[Serializable]
	public abstract class BaseValue<T> : ScriptableObject where T : IEquatable<T>
	{
		/// <summary>
		/// Called when the value is changed.
		/// </summary>
		/// <param name="newValue">The new value.</param>
		public delegate void ValueChangedHandler(T newValue);
		public event ValueChangedHandler ValueChanged;

		[SerializeField, ReadOnly]
		private T value;
		public T Value
		{
			get { return value; }
			set
			{
				// Only change the value and emit an event if it was actually changed.
				if (!this.value.Equals(value))
				{
					this.value = value;
					ValueChanged?.Invoke(value);
				}
			}
		}

		[SerializeField]
		private T defaultValue = default;
		public T DefaultValue
		{
			get { return defaultValue; }
		}

		private void OnEnable()
		{
			value = defaultValue;
		}
	}
}

