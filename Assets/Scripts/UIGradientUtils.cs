using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIGradientUtils
{
    public struct Matrix2x3
    {
        public float m00, m01, m02, m10, m11, m12;
        public Matrix2x3(float m00, float m01, float m02, float m10, float m11, float m12)
        {
            this.m00 = m00;
            this.m01 = m01;
            this.m02 = m02;
            this.m10 = m10;
            this.m11 = m11;
            this.m12 = m12;
        }

        public static Vector2 operator*(Matrix2x3 m, Vector2 v)
        {
            float x = (m.m00 * v.x) + (m.m01 * v.y) + m.m02;
            float y = (m.m10 * v.x) + (m.m11 * v.y) + m.m12;
            return new Vector2(x, y);
        }
    }

    public static Matrix2x3 LocalPositionMatrix(Rect rect, float cos, float sin)
    {
        Vector2 rectMin = rect.min;
        Vector2 rectSize = rect.size;
        float c = 0.5f;
        float ax = rectMin.x / rectSize.x + c;
        float ay = rectMin.y / rectSize.y + c;
        float m00 = cos / rectSize.x;
        float m01 = -sin / rectSize.y;
        float m02 = -(ax * cos - ay * sin - c);
        float m10 = sin / rectSize.x;
        float m11 = cos / rectSize.y;		
        float m12 = -(ax * sin + ay * cos - c);
        return new Matrix2x3(m00, m01, m02, m10, m11, m12);
    }

    static Vector2[] ms_verticesPositions = new Vector2[] { Vector2.up, Vector2.one, Vector2.right, Vector2.zero };
    public static Vector2[] VerticesPositions
    {
        get { return ms_verticesPositions; }
    }

    public static float InverseLerp (float a, float b, float v)
    {
        return a != b ? (v - a) / (b - a) : 0f;
    }

    public static Color Bilerp(Color a1, Color a2, Color b1, Color b2, Vector2 t)
    {
        Color a = Color.LerpUnclamped(a1, a2, t.x);
        Color b = Color.LerpUnclamped(b1, b2, t.x);
        return Color.LerpUnclamped(a, b, t.y);
    }

    public static void Lerp(UIVertex a, UIVertex b, float t, ref UIVertex c)
    {
        c.position = Vector3.LerpUnclamped(a.position, b.position, t);
        c.normal = Vector3.LerpUnclamped(a.normal, b.normal, t);
        c.color = Color32.LerpUnclamped(a.color, b.color, t);
        c.tangent = Vector3.LerpUnclamped(a.tangent, b.tangent, t);
        c.uv0 = Vector3.LerpUnclamped(a.uv0, b.uv0, t);
        c.uv1 = Vector3.LerpUnclamped(a.uv1, b.uv1, t);
        // c.uv2 = Vector3.LerpUnclamped(a.uv2, b.uv2, t);
        // c.uv3 = Vector3.LerpUnclamped(a.uv3, b.uv3, t);
    }

    static byte Mul(byte a, byte b)
    {
        float mul = ((float)a * (float)b) / 255f;
        return (byte)mul;
    }

    public static Color32 Mul(Color32 c1, Color32 c2)
    {
        c1.r = Mul(c1.r, c2.r);
        c1.g = Mul(c1.g, c2.g);
        c1.b = Mul(c1.b, c2.b);
        c1.a = Mul(c1.a, c2.a);
        return c1;
    }

    public static int AddVert(VertexHelper vh, UIVertex v)
    {
        int i = vh.currentVertCount;
        vh.AddVert(v);
        return i;
    }

    public static int AddVert(List<UIVertex> verts, UIVertex v)
    {
        int i = verts.Count;
        verts.Add(v);
        return i;
    }

    public static void AddQuad(VertexHelper vh, UIVertex v0, UIVertex v1, UIVertex v2, UIVertex v3)
    {
        int i0 = AddVert(vh, v0);
        int i1 = AddVert(vh, v1);
        int i2 = AddVert(vh, v2);
        int i3 = AddVert(vh, v3);
        vh.AddTriangle(i0, i1, i2);
        vh.AddTriangle(i2, i3, i0);
    }

    public static void AddQuad(List<UIVertex> verts, UIVertex v0, UIVertex v1, UIVertex v2, UIVertex v3)
    {
        verts.Add(v0);
        verts.Add(v1);
        verts.Add(v2);
        verts.Add(v2);
        verts.Add(v3);
        verts.Add(v0);
    }

    public static void SetQuad(VertexHelper vh, UIVertex v0, UIVertex v1, UIVertex v2, UIVertex v3, int i)
    {
        vh.SetUIVertex (v0, i);
        vh.SetUIVertex (v1, i + 1);
        vh.SetUIVertex (v2, i + 2);
        vh.SetUIVertex (v3, i + 3);
    }

    public static void SetQuad(List<UIVertex> verts, UIVertex v0, UIVertex v1, UIVertex v2, UIVertex v3, int i)
    {
        verts[i] = v0;
        verts[i + 1] = v1;
        verts[i + 2] = v2;
        verts[i + 3] = v3;
    }

    public static void GetQuad(VertexHelper vh, ref UIVertex v0, ref UIVertex v1, ref UIVertex v2, ref UIVertex v3, int i)
    {
        vh.PopulateUIVertex (ref v0, i);
        vh.PopulateUIVertex (ref v1, i + 1);
        vh.PopulateUIVertex (ref v2, i + 2);
        vh.PopulateUIVertex (ref v3, i + 3);
    }

    public static void GetQuad(List<UIVertex> verts, ref UIVertex v0, ref UIVertex v1, ref UIVertex v2, ref UIVertex v3, int i)
    {
        v0 = verts[i];
        v1 = verts[i + 1];
        v2 = verts[i + 2];
        v3 = verts[i + 3];
    }
}
