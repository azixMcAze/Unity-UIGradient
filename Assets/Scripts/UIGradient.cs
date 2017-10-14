using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class UIGradient : BaseMeshEffect
{
    public Color32 m_color1 = Color.white;
    public Color32 m_color2 = Color.white;
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

            UIGradientUtils.Matrix2x3 localPositionMatrix = UIGradientUtils.LocalPositionMatrix(rect, cos, sin);

            UIVertex vertex = default(UIVertex);
            for (int i = 0; i < vh.currentVertCount; i++) {
                vh.PopulateUIVertex (ref vertex, i);
                Vector2 localPosition = localPositionMatrix * vertex.position;
                vertex.color = UIGradientUtils.MulLerp(m_color2, m_color1, Mathf.Clamp01(localPosition.y), vertex.color);
                vh.SetUIVertex (vertex, i);
            }
        }
    }
}
