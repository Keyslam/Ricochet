using System;
using UnityEngine;

namespace Framework.Values
{
	[CreateAssetMenu(fileName = "Int Value", menuName = "Values/Int Value", order = 0), Serializable]
	public class IntValue : BaseValue<int> { }
}
