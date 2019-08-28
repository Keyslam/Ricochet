using System;
using UnityEngine;

namespace Framework.Values
{
	[CreateAssetMenu(fileName = "Vector3 Value", menuName = "Values/Vector3 Value", order = 5), Serializable]
	public class Vector3Value : BaseValue<Vector3> { }
}
