using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/4 Corners Gradient")]
public class UICornersGradient : BaseMeshEffect {
	public Color m_topLeftColor = Color.white;
	public Color m_topRightColor = Color.white;
	public Color m_bottomRightColor = Color.white;
	public Color m_bottomLeftColor = Color.white;

    public override void ModifyMesh(VertexHelper vh)
    {
		if(enabled)
		{
            Rect rect = graphic.rectTransform.rect;
            Vector2 rectMin = rect.min;
			Vector2 recSize = rect.size;
			Vector2 rectInvSize = new Vector2(1f / recSize.x, 1f/ recSize.y);
			
			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < vh.currentVertCount; i++) {
				vh.PopulateUIVertex (ref vertex, i);
				Vector2 normalizedPosition = UIGradientUtils.NormalizedPosition(vertex.position, rectMin, rectInvSize);
				vertex.color *= UIGradientUtils.InterpolatedColor(normalizedPosition, m_topLeftColor, m_topRightColor, m_bottomLeftColor, m_bottomRightColor);
				vh.SetUIVertex (vertex, i);
			}
		}
    }
}
