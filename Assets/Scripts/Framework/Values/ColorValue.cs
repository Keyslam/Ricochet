using System;
using UnityEngine;

namespace Framework.Values
{
	[CreateAssetMenu(fileName = "Color Value", menuName = "Values/Color Value", order = 7), Serializable]
	public class ColorValue : BaseValue<Color> { }
}
