using UnityEngine;
using UnityEngine.UI;

namespace Framework.Extensions.UI
{
	/// <summary>
	/// Flow Layout Group is evolved version of Grid Layout Group for Unity UI. 
	/// It allows to build Tag like element group. 
	/// Follow the question on StackOverflow : http://stackoverflow.com/questions/38336835/correct-flowlayoutgroup-in-unity3d-as-per-horizontallayoutgroup-etc/38479097#38479097
	/// Taken from: https://github.com/Unitarian/Unity-Extensions/tree/master/FlowLayoutGroup
	/// 
	/// Instructions:
	/// Attach this script to your panel just like you would use other layout groups like GridViewLayout
	/// Add UI element(Buttons, Images etc) as child of Panel.
    /// Add ContentSizeFitter component to children and set Horizontal Fit and Vertical Fit properties to Preferred Size
    /// Add Layout Element component to child and set Preferred Width and Preferred Height values.These values will control size of UI Element.
    /// Add as many elements as you want and apply same procedure to get desired size.";
	/// </summary>
	[AddComponentMenu("Layout/Flow Layout Group", 153)]
	public class FlowLayoutGroup : LayoutGroup
	{
		public enum Corner
		{
			UPPER_LEFT = 0,
			UPPER_RIGHT = 1,
			LOWER_LEFT = 2,
			LOWER_RIGHT = 3,
		}

		public enum Constraint
		{
			FLEXIBLE = 0,
			FIXED_COLUMN_COUNT = 1,
			FIXED_ROW_COUNT = 2,
		}

		[SerializeField]
		private Vector2 cellSize = new Vector2(100, 100);
		public Vector2 CellSize
		{
			get { return cellSize; }
			set { SetProperty(ref cellSize, value); }
		}

		[SerializeField]
		private Vector2 spacing = Vector2.zero;
		public Vector2 Spacing
		{
			get { return spacing; }
			set { SetProperty(ref spacing, value); }
		}

		[SerializeField]
		private bool horizontal = true;
		public bool Horizontal
		{
			get { return horizontal; }
			set { SetProperty(ref horizontal, value); }
		}

		private int cellsPerMainAxis = 0;
		private int actualCellCountX = 0;
		private int actualCellCountY = 0;

		private float totalWidth = 0.0f;
		private float totalHeight = 0.0f;

		private float lastMax = 0.0f;

		private FlowLayoutGroup()
		{

		}

		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();

			int minColumns = 0;
			int preferredColumns = 0;

			minColumns = 1;
			preferredColumns = Mathf.CeilToInt(Mathf.Sqrt(rectChildren.Count));

			float totalMin = padding.horizontal + (CellSize.x + Spacing.x) * minColumns - Spacing.x;
			float totalPreferred = padding.horizontal + (CellSize.x + Spacing.x) * preferredColumns - Spacing.x;

			SetLayoutInputForAxis(totalMin, totalPreferred, -1, 0);
		}

		public override void CalculateLayoutInputVertical()
		{
			int minRows = 1;
			float minSpace = padding.vertical + (CellSize.y + Spacing.y) * minRows - Spacing.y;

			SetLayoutInputForAxis(minSpace, minSpace, -1, 1);
		}

		public override void SetLayoutHorizontal()
		{
			SetCellsAlongAxis();
		}

		public override void SetLayoutVertical()
		{
			SetCellsAlongAxis();
		}

		private void SetCellsAlongAxis()
		{
			// Normally a Layout Controller should only set horizontal values when invoked for the horizontal axis
			// and only vertical values when invoked for the vertical axis.
			// However, in this case we set both the horizontal and vertical position when invoked for the vertical axis.
			// Since we only set the horizontal position and not the size, it shouldn't affect children's layout,
			// and thus shouldn't break the rule that all horizontal layout must be calculated before all vertical layout.

			float width = rectTransform.rect.size.x;
			float height = rectTransform.rect.size.y;

			int cellCountX = 1;
			int cellCountY = 1;

			if (CellSize.x + Spacing.x <= 0)
				cellCountX = int.MaxValue;
			else
				cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + Spacing.x + 0.001f) / (CellSize.x + Spacing.x)));

			if (CellSize.y + Spacing.y <= 0)
				cellCountY = int.MaxValue;
			else
				cellCountY = Mathf.Max(1, Mathf.FloorToInt((height - padding.vertical + Spacing.y + 0.001f) / (CellSize.y + Spacing.y)));

			cellsPerMainAxis = cellCountX;
			actualCellCountX = Mathf.Clamp(cellCountX, 1, rectChildren.Count);
			actualCellCountY = Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(rectChildren.Count / (float)cellsPerMainAxis));

			Vector2 requiredSpace = new Vector2(
				actualCellCountX * CellSize.x + (actualCellCountX - 1) * Spacing.x,
				actualCellCountY * CellSize.y + (actualCellCountY - 1) * Spacing.y
			);
			Vector2 startOffset = new Vector2(
				GetStartOffset(0, requiredSpace.x),
				GetStartOffset(1, requiredSpace.y)
			);

			totalWidth = 0;
			totalHeight = 0;
			Vector2 currentSpacing = Vector2.zero;
			for (int i = 0; i < rectChildren.Count; i++)
			{
				SetChildAlongAxis(rectChildren[i], 0, startOffset.x + totalWidth /*+ currentSpacing[0]*/, rectChildren[i].rect.size.x);
				SetChildAlongAxis(rectChildren[i], 1, startOffset.y + totalHeight  /*+ currentSpacing[1]*/, rectChildren[i].rect.size.y);

				currentSpacing = Spacing;

				if (Horizontal)
				{
					totalWidth += rectChildren[i].rect.width + currentSpacing[0];
					if (rectChildren[i].rect.height > lastMax)
						lastMax = rectChildren[i].rect.height;

					if (i < rectChildren.Count - 1)
					{
						if (totalWidth + rectChildren[i + 1].rect.width + currentSpacing[0] > width - padding.horizontal)
						{
							totalWidth = 0;
							totalHeight += lastMax + currentSpacing[1];
							lastMax = 0;
						}
					}
				}
				else
				{
					totalHeight += rectChildren[i].rect.height + currentSpacing[1];
					if (rectChildren[i].rect.width > lastMax)
						lastMax = rectChildren[i].rect.width;

					if (i < rectChildren.Count - 1)
					{
						if (totalHeight + rectChildren[i + 1].rect.height + currentSpacing[1] > height - padding.vertical)
						{
							totalHeight = 0;
							totalWidth += lastMax + currentSpacing[0];
							lastMax = 0;
						}
					}
				}
			}
		}
	}
}