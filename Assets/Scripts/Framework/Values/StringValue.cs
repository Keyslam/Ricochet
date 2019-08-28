using System;
using UnityEngine;

namespace Framework.Values
{
	[CreateAssetMenu(fileName = "String Value", menuName = "Values/String Value", order = 3), Serializable]
	public class StringValue : BaseValue<string> { }
}
