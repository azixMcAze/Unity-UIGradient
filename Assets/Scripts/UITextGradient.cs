using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Text Gradient")]
public class UITextGradient : BaseMeshEffect
{
	public Color m_color1 = Color.white;
	public Color m_color2 = Color.white;
	[Range(-180f, 180f)]
	public float m_angle = 0f;
	public bool m_ignoreRatio = true;

	static Vector2[] ms_verticesPositions = new Vector2[] { Vector2.up, Vector2.one, Vector2.right, Vector2.zero };

    public override void ModifyMesh(VertexHelper vh)
    {
		if(enabled)
		{
			Rect rect = graphic.rectTransform.rect;

			float angleRad = m_angle * Mathf.Deg2Rad;
			float sin = Mathf.Sin(angleRad);
			float cos = Mathf.Cos(angleRad);

			if (!m_ignoreRatio) {
				float ratio = rect.height / rect.width;
				cos *= ratio;
				float norm = Mathf.Sqrt (cos * cos + sin * sin);
				cos /= norm;
				sin /= norm;
			}
			
			Vector2 center = new Vector2 (0.5f, 0.5f);
			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < vh.currentVertCount; i++) {

				vh.PopulateUIVertex (ref vertex, i);
				Vector2 normalizedPosition = ms_verticesPositions[i % 4];
				Vector2 rotatedPosition = Rotate(normalizedPosition - center, cos, sin) + center;
				vertex.color *= Color.Lerp(m_color2, m_color1, rotatedPosition.y);
				vh.SetUIVertex (vertex, i);
			}
		}
    }

	static Vector2 NormalizedPosition(Vector2 position, Rect rect)
	{
		float x = Mathf.InverseLerp (rect.xMin, rect.xMax, position.x);
		float y = Mathf.InverseLerp (rect.yMin, rect.yMax, position.y);
		return new Vector2(x, y);
	}

	static Vector2 Rotate(Vector2 v, float cos, float sin)
	{
		float x = (cos * v.x) - (sin * v.y);
		float y = (sin * v.x) + (cos * v.y);
		return new Vector2(x, y);
	}
}
