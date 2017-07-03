using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class UIGradient : BaseMeshEffect
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
                UIGradientUtils.CompensateAspectRatio(rect, cos, sin, out cos, out sin);
            }

            UIGradientUtils.Matrix2x3 localPositionMatrix = UIGradientUtils.LocalPositionMatrix(rect, cos, sin);

            UIVertex vertex = default(UIVertex);
            for (int i = 0; i < vh.currentVertCount; i++) {
                vh.PopulateUIVertex (ref vertex, i);
                Vector2 localPosition = localPositionMatrix * vertex.position;
                vertex.color *= Color.Lerp(m_color2, m_color1, localPosition.y);
                vh.SetUIVertex (vertex, i);
            }
        }
    }
}
