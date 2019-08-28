using System;
using UnityEngine;

namespace Framework.Values
{
	[CreateAssetMenu(fileName = "Quaternion Value", menuName = "Values/Quaternion Value", order = 6), Serializable]
	public class QuaternionValue : BaseValue<Quaternion> { }
}
