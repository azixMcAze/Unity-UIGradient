using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVerticalGradient : BaseMeshEffect
{
	public Color m_topColor = Color.white;
	public Color m_bottomColor = Color.white;

    public override void ModifyMesh(VertexHelper vh)
    {
		if(enabled)
		{
			Rect rect = graphic.rectTransform.rect;

			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < vh.currentVertCount; i++) {
				vh.PopulateUIVertex (ref vertex, i);
				vertex.color *= Color.Lerp (m_bottomColor, m_topColor, Mathf.InverseLerp (rect.yMin, rect.yMax, vertex.position.y));
				vh.SetUIVertex (vertex, i);
			}
		}
    }
}
