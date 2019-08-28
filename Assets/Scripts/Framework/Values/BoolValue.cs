using System;
using UnityEngine;

namespace Framework.Values
{
	[CreateAssetMenu(fileName = "Bool Value", menuName = "Values/Bool Value", order = 2), Serializable]
	public class BoolValue : BaseValue<bool> { }
}
