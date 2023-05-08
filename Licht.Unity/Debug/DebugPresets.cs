using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Licht.Unity.Debug
{
    public static class DebugPresets
    {
        public static void DrawRectangle(Vector3 position, Vector2 size, Color color)
        {
            var topLeft = new Vector3(size.x * -0.5f, size.y * 0.5f);
            var topRight = new Vector3(size.x * 0.5f, size.y * 0.5f);
            var bottomLeft = new Vector3(size.x * -0.5f, size.y * -0.5f);
            var bottomRight = new Vector3(size.x * 0.5f, size.y * -0.5f);

            UnityEngine.Debug.DrawLine(position + topLeft, position + topRight, color);
            UnityEngine.Debug.DrawLine(position + topRight, position + bottomRight, color);
            UnityEngine.Debug.DrawLine(position + bottomRight, position + bottomLeft, color);
            UnityEngine.Debug.DrawLine(position + bottomLeft, position + topLeft, color);
        }

        public static void DrawEllipse(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, int segments, Color color, float duration = 0)
        {
            var angle = 0f;
            var rot = Quaternion.LookRotation(forward, up);
            var lastPoint = Vector3.zero;
            var thisPoint = Vector3.zero;

            for (var i = 0; i < segments + 1; i++)
            {
                thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
                thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

                if (i > 0)
                {
                    UnityEngine.Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
                }

                lastPoint = thisPoint;
                angle += 360f / segments;
            }
        }

    }
}
