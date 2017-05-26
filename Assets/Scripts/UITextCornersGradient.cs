using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Text 4 Corners Gradient")]
public class UITextCornersGradient : BaseMeshEffect {
	public Color m_topLeftColor = Color.white;
	public Color m_topRightColor = Color.white;
	public Color m_bottomRightColor = Color.white;
	public Color m_bottomLeftColor = Color.white;

	static Vector2[] ms_verticesPositions = new Vector2[] { Vector2.up, Vector2.one, Vector2.right, Vector2.zero };

    public override void ModifyMesh(VertexHelper vh)
    {
		if(enabled)
		{
			Rect rect = graphic.rectTransform.rect;

			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < vh.currentVertCount; i++) {
				vh.PopulateUIVertex (ref vertex, i);
				Vector2 normalizedPosition = ms_verticesPositions[i % 4];
				vertex.color *= InterpolatedColor(normalizedPosition, m_topLeftColor, m_topRightColor, m_bottomLeftColor, m_bottomRightColor);
				vh.SetUIVertex (vertex, i);
			}
		}
    }
	
	public static Color InterpolatedColor(Vector2 normalizedPosition, Color topLeft, Color topRight, Color bottomLeft, Color bottomRight)
	{
		Color top = Color.Lerp(topLeft, topRight, normalizedPosition.x);
		Color bottom = Color.Lerp(bottomLeft, bottomRight, normalizedPosition.x);
		return Color.Lerp(bottom, top, normalizedPosition.y);
	}

}
