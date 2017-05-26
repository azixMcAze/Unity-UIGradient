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
				Vector2 normalizedPosition = UIGradientUtils.VerticePositions[i % 4];
				Vector2 rotatedPosition = UIGradientUtils.Rotate(normalizedPosition - center, cos, sin) + center;
				vertex.color *= Color.Lerp(m_color2, m_color1, rotatedPosition.y);
				vh.SetUIVertex (vertex, i);
			}
		}
    }
}
