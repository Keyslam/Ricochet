using UnityEngine;

namespace Framework.Pooling
{
	/// <summary>
	/// Base class for poolable objects.
	/// Implemented using CRTP.
	/// </summary>
	/// <typeparam name="T">Class to pool</typeparam>
	public abstract class Poolable<T> : MonoBehaviour where T : Poolable<T>
	{
		/// <summary>
		/// Friend class to give access to private methods.
		/// </summary>
		public class Friend : MonoBehaviour
		{
			protected static void InitializePoolable(Poolable<T> item, ObjectPool<T> objectPool)
			{
				item.Initialize(objectPool);
			}

			protected static void BorrowPoolable(Poolable<T> item)
			{
				item.Borrow();
			}

			protected static void ReturnPoolable(Poolable<T> item)
			{
				item.Return();
			}
		}

		public ObjectPool<T> Pool {
			get;
			private set;
		} = null;

		public bool IsBorrowed {
			get;
			private set;
		} = false;

		private void Initialize(ObjectPool<T> objectPool)
		{
			Pool = objectPool;
		}

		private void Borrow()
		{
			IsBorrowed = true;
		}

		private void Return()
		{
			IsBorrowed = false;
		}
	}
}