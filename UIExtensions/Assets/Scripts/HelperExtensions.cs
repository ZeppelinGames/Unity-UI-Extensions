using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class HelperExtensions {
    public static Color GrayscaleColor(this Color c, float gs) {
        c = new Color(gs, gs, gs);
        return c;
    }

    #region Rect Extensions
    public static Rect Scale(this Rect r, float scale) {
        return new Rect(r.position, new Vector2(r.width * scale, r.height * scale));
    }
    public static Rect ScaleCentered(this Rect r, float scale) {
        Vector2 scaleDiff = new Vector2(r.width - (r.width * scale), r.height - (r.height * scale));
        scaleDiff = new Vector2(scaleDiff.x / 2, scaleDiff.y / 2);

        return new Rect(r.position + scaleDiff, new Vector2(r.width * scale, r.height * scale));
    }

    public static Rect ScaleUniform(this Rect r, float scale, bool matchWidth = true) {
        float newScale = matchWidth ? r.width - (r.width * scale) : r.height - (r.height * scale);

        return new Rect(r.position, new Vector2(r.width - newScale, r.height - newScale));
    }

    public static Rect ScaleUniformCentered(this Rect r, float scale, bool matchWidth = true) {
        float newScale = matchWidth ? r.width - (r.width * scale) : r.height - (r.height * scale);

        Vector2 scaleDiff = Vector2.one * (newScale / 2);

        return new Rect(r.position + scaleDiff, new Vector2(r.width - newScale, r.height - newScale));
    }

    public static Rect PadUniform(this Rect r, int padding) {
        return new Rect(r.position, r.size - (Vector2.one * padding));
    }

    public static Rect PadUniformCentered(this Rect r, int padding) {
        return new Rect(r.position + (Vector2.one * (padding / 2)), r.size - (Vector2.one * padding));
    }

    public static Rect PadRect(this Rect r, Rect pad) { //tblr
        return new Rect(
            new Vector2(r.position.x + pad.width, r.position.y + pad.x - pad.y),
            new Vector2(r.width - pad.width - pad.height, r.height - pad.x - pad.y));
    }
    #endregion

    public static MethodInfo[] GetScriptFunctions(this MonoBehaviour target, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default) {
        List<MethodInfo> methods = new List<MethodInfo>();
        methods.AddRange(target.GetType().GetMethods(flags));
        return methods.ToArray();
    }
}
