using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGradient : BaseMeshEffect
{
	// [ColorUsage(true, true, 0f, 8f, 1f/8f, 3f)]
	public Color m_color1 = Color.white;
	// [ColorUsage(true, true, 0f, 8f, 1f/8f, 3f)]
	public Color m_color2 = Color.white;
	[Range(-180f, 180f)]
	public float m_angle = 0f;
	
    public override void ModifyMesh(VertexHelper vh)
    {
		if(enabled)
		{
			Rect rect = graphic.rectTransform.rect;
			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < vh.currentVertCount; i++) {
				vh.PopulateUIVertex (ref vertex, i);
				Vector2 normalizedPosition = GetNormalizedPosition(vertex.position, rect);
				Vector2 rotatedPosition = Rotate(normalizedPosition - new Vector2(0.5f, 0.5f), m_angle) + new Vector2(0.5f, 0.5f);
				vertex.color *= Color.Lerp(m_color1, m_color2, rotatedPosition.y);
				vh.SetUIVertex (vertex, i);
			}
		}
    }

	static Vector2 GetNormalizedPosition(Vector2 position, Rect rect)
	{
		float x = Mathf.InverseLerp (rect.xMin, rect.xMax, position.x);
		float y = Mathf.InverseLerp (rect.yMin, rect.yMax, position.y);
		return new Vector2(x, y);
	}

    static Vector2 Rotate(Vector2 v, float degrees) {
		float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
		float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
		
		float tx = v.x;
		float ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) + (cos * ty);
		return v;
	}
}
