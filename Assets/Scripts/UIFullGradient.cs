using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

[AddComponentMenu("UI/Effects/Full Gradient")]
public class UIFullGradient : BaseMeshEffect
{
    public Gradient m_gradient;
    [Range(-180f, 180f)]
    public float m_angle = 0f;
    public bool m_ignoreRatio = true;

    static List<float> GetKeyTimes(Gradient gradient)
    {
        List<float> keyList = new List<float>(gradient.alphaKeys.Length + gradient.colorKeys.Length);
        var colorKeys = gradient.colorKeys;
        var alphaKeys = gradient.alphaKeys;
        int nc = colorKeys.Length;
        int na = alphaKeys.Length;
        int ic = 0;
        int ia = 0;
        float t = -1f;
        
        while(ic < nc || ia < na)
        {
            float tk;
            if(ic < nc && ia < na)
            {
                float tc = colorKeys[ic].time;
                float ta = alphaKeys[ia].time;
                if(tc < ta)
                {
                    tk = tc;
                    ic++;
                }
                else
                {
                    tk = ta;
                    ia++;
                }
            }
            else if(ic < nc)
            {
                tk = colorKeys[ic].time;
                ic++;
            }
            else //if(ia < na)
            {
                tk = alphaKeys[ia].time;
                ia++;
            }

            if(tk > t)
            {
                t = tk;
                keyList.Add(tk);
            }
        }
        return keyList;
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if(enabled)
        {
            Rect rect = graphic.rectTransform.rect;

            float angleRad = m_angle * Mathf.Deg2Rad;
            float sin = Mathf.Sin(angleRad);
            float cos = Mathf.Cos(angleRad);

            UIGradientUtils.Matrix2x3 localPositionMatrix = UIGradientUtils.LocalPositionMatrix(rect, cos, sin);

            List<float> keyTimes = GetKeyTimes(m_gradient);
            UIVertex v0 = default(UIVertex);
            UIVertex v1 = default(UIVertex);
            UIVertex v2 = default(UIVertex);
            UIVertex v3 = default(UIVertex);

            UIVertex v01 = default(UIVertex);
            UIVertex v12 = default(UIVertex);
            UIVertex v23 = default(UIVertex);
            UIVertex v30 = default(UIVertex);

            int vertexCount = vh.currentVertCount;
            for (int i = 0; i < vertexCount; i += 4)
            {
                UIGradientUtils.GetQuad(vh, ref v0, ref v1, ref v2, ref v3, i);

                Vector2 pos0 = localPositionMatrix * v0.position;
                Vector2 pos1 = localPositionMatrix * v1.position;
                Vector2 pos2 = localPositionMatrix * v2.position;
                Vector2 pos3 = localPositionMatrix * v3.position;

                // int i0 = keyTimes.BinarySearch(pos0.y);
                // int i1 = keyTimes.BinarySearch(pos1.y);
                // int i2 = keyTimes.BinarySearch(pos2.y);
                // int i3 = keyTimes.BinarySearch(pos3.y);

                // if(i0 < 0)
                // 	i0 = ~i0;
                // if(i1 < 0)
                // 	i1 = ~i1;
                // if(i2 < 0)
                // 	i2 = ~i2;
                // if(i3 < 0)
                // 	i3 = ~i3;

                float slice = keyTimes[1];

                bool side0 = pos0.y < slice;
                bool side1 = pos1.y < slice;
                bool side2 = pos2.y < slice;
                bool side3 = pos3.y < slice;

                if(side0 != side1 || side0 != side2 || side0 != side3)
                {
                    bool split01 = SplitEdgeIfNeeded(v0, v1, pos0.y, pos1.y, slice, side0, side1, ref v01);
                    if(split01)
                    {
                        Vector2 pos01 = localPositionMatrix * v01.position;
                        v01.color *= m_gradient.Evaluate(pos01.y);
                    }
                    bool split12 = SplitEdgeIfNeeded(v1, v2, pos1.y, pos2.y, slice, side1, side2, ref v12);
                    if(split12)
                    {
                        Vector2 pos12 = localPositionMatrix * v12.position;
                        v12.color *= m_gradient.Evaluate(pos12.y);
                    }
                    bool split23 = SplitEdgeIfNeeded(v2, v3, pos2.y, pos3.y, slice, side2, side3, ref v23);
                    if(split23)
                    {
                        Vector2 pos23 = localPositionMatrix * v23.position;
                        v23.color *= m_gradient.Evaluate(pos23.y);
                    }
                    bool split30 = SplitEdgeIfNeeded(v3, v0, pos3.y, pos0.y, slice, side3, side0, ref v30);
                    if(split30)
                    {
                        Vector2 pos30 = localPositionMatrix * v30.position;
                        v30.color *= m_gradient.Evaluate(pos30.y);
                    }

                    v0.color *= m_gradient.Evaluate(pos0.y);
                    v1.color *= m_gradient.Evaluate(pos1.y);
                    v2.color *= m_gradient.Evaluate(pos2.y);
                    v3.color *= m_gradient.Evaluate(pos3.y);

                    if(split12 && split30)
                    {
                        UIGradientUtils.SetQuad(vh, v0, v1, v12, v30, i);
                        UIGradientUtils.AddQuad(vh, v30, v12, v2, v3);
                    }
                    else if(split01 && split23)
                    {
                        UIGradientUtils.SetQuad(vh, v0, v01, v23, v3, i);
                        UIGradientUtils.AddQuad(vh, v01, v1, v2, v23);
                    }
                    else if(split01 && split12)
                    {
                        UIGradientUtils.SetQuad(vh, v0, v12, v2, v3, i);
                        UIGradientUtils.AddQuad(vh, v01, v1, v12, v0);
                    }
                    else if(split12 && split23)
                    {
                        UIGradientUtils.SetQuad(vh, v0, v1, v23, v3, i);
                        UIGradientUtils.AddQuad(vh, v12, v2, v23, v1);
                    }
                    else if(split23 && split30)
                    {
                        UIGradientUtils.SetQuad(vh, v0, v1, v2, v30, i);
                        UIGradientUtils.AddQuad(vh, v30, v2, v23, v3);
                    }
                    else if(split30 && split01)
                    {
                        UIGradientUtils.SetQuad(vh, v01, v1, v2, v3, i);
                        UIGradientUtils.AddQuad(vh, v01, v3, v30, v0);
                    }
                }
                else
                {
                    v0.color *= m_gradient.Evaluate(pos0.y);
                    v1.color *= m_gradient.Evaluate(pos1.y);
                    v2.color *= m_gradient.Evaluate(pos2.y);
                    v3.color *= m_gradient.Evaluate(pos3.y);

                    UIGradientUtils.SetQuad(vh, v0, v1, v2, v3, i);
                }
            }
        }
    }

    static bool SplitEdgeIfNeeded(UIVertex v0, UIVertex v1, float pos0, float pos1, float posSlice, bool side0, bool side1, ref UIVertex v2)
    {
        if(side0 != side1)
        {
            float tSlice  = UIGradientUtils.InverseLerp(pos0, pos1, posSlice);
            UIGradientUtils.Lerp(v0, v1, tSlice, ref v2);
            return true;
        }
        else
        {
            return false;
        }
    }
}
