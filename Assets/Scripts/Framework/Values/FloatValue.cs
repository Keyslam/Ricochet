using System;
using UnityEngine;

namespace Framework.Values
{
	[CreateAssetMenu(fileName = "Float Value", menuName = "Values/Float Value", order = 1), Serializable]
	public class FloatValue : BaseValue<float> { }
}
