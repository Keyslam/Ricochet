using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Pooling
{
	/// <summary>
	/// Base class for object pool of prefabs.
	/// </summary>
	/// <typeparam name="T">Type of component for objects</typeparam>
	public class ObjectPool<T> : Poolable<T>.Friend where T : Poolable<T>
	{
		private Queue<T> poolables = null;

		[SerializeField, Required]
		private T prefab = null;

		[SerializeField]
		private int initialCapacity = 100;

		public virtual void Awake()
		{
			poolables = new Queue<T>(initialCapacity);

			// Fill pool with ready to go objects
			for (int i = 0; i < initialCapacity; ++i)
			{
				T poolable = Instantiate(prefab, transform);
				InitializePoolable(poolable, this);

				poolables.Enqueue(poolable);

				poolable.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Borrows an object for a pool.
		/// Makes a new object if the pool is empty.
		/// </summary>
		/// <param name="parent">Transform to put the object in.</param>
		/// <returns>A new object</returns>
		public T BorrowObject(Transform parent = null)
		{
			T poolable = null;

			if (poolables.Count > 0)
				poolable = poolables.Dequeue();
			else
			{
				// Make a new object if the pool is empty
				poolable = Instantiate(prefab);
				InitializePoolable(poolable, this);
			}

			BorrowPoolable(poolable);
			poolable.transform.SetParent(parent);
			poolable.gameObject.SetActive(true);

			return poolable;
		}

		/// <summary>
		/// Returns an object back to the pool.
		/// </summary>
		/// <param name="poolable">Object to return.</param>
		public void ReturnObject(T poolable)
		{
			if (poolable.Pool != this)
			{
				Debug.LogError("Object does not belong to this pool");
				return;
			}

			if (!poolable.IsBorrowed)
			{
				Debug.LogError("Object was already released");
				return;
			}

			poolables.Enqueue(poolable);
			ReturnPoolable(poolable);

			poolable.gameObject.SetActive(false);
			poolable.transform.SetParent(transform);
		}
	}
}
