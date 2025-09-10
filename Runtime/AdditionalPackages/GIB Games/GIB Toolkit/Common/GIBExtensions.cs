using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GIB
{
    /// <summary>
    ///     C# Extensions created by and for GIB Games.
    /// </summary>
    public static class GIBExtensions
    {
        #region Camera Extensions

        /// <summary>
        /// Check if the target Renderer is visible.
        /// </summary>
        /// <param name="renderer">Target renderer.</param>
        /// <returns></returns>
        public static bool IsObjectVisible(this Camera @this, Renderer renderer) =>
            GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(@this), renderer.bounds);

        #endregion

        #region Transform Extensions

        /// <summary>
        ///     Sets the transform position x property.
        /// </summary>
        public static Vector3 ChangeX(this Transform transform, float x)
        {
            Vector3 position = transform.position;
            position.x = x;
            transform.position = position;
            return position;
        }

        /// <summary>
        ///     Sets the transform position y property.
        /// </summary>
        public static Vector3 ChangeY(this Transform transform, float y)
        {
            Vector3 position = transform.position;
            position.y = y;
            transform.position = position;
            return position;
        }

        /// <summary>
        ///     Sets the transform position z property.
        /// </summary>
        public static Vector3 ChangeZ(this Transform transform, float z)
        {
            Vector3 position = transform.position;
            position.z = z;
            transform.position = position;
            return position;
        }

        // These extensions allow additional data from Vector3s.

        /// <summary>
        ///     Returns a new Vector3 that ignores the Y axis.
        /// </summary>
        public static Vector3 Flatten(this Vector3 vector) =>
            new Vector3(vector.x, 0f, vector.z);

        /// <summary>
        ///     Returns Vector3 Distance that ignores the Y axis.
        /// </summary>
        public static float FlatDistance(this Vector3 origin, Vector3 destination) =>
            Vector3.Distance(origin.Flatten(), destination.Flatten());

        /// <summary>
        ///     Get a Vector3 which is the target distance along the Forward axis from the transform.
        /// </summary>
        public static Vector3 ForwardPoint(this Transform origin, float distance) =>
            origin.position + (origin.forward * distance);

        /// <summary>
        ///     Get the rotation to look at a Vector3.
        /// </summary>
        /// <returns>A <see cref="Quaternion" /> representing the rotation to look at <paramref name="target" />.</returns>
        public static Quaternion LookAtRotation(this Transform self, Vector3 target) =>
            Quaternion.LookRotation(target - self.position);

        /// <summary>
        ///     Get the rotation to look at a Transform.
        /// </summary>
        /// <returns>A <see cref="Quaternion" /> representing the rotation to look at <paramref name="target" />.</returns>
        public static Quaternion LookAtRotation(this Transform self, Transform target) =>
            LookAtRotation(self, target.position);

        /// <summary>
        ///     Get the rotation to look at a GameObject.
        /// </summary>
        /// <returns>A <see cref="Quaternion" /> representing the rotation to look at <paramref name="target" />.</returns>
        public static Quaternion LookAtRotation(this Transform self, GameObject target) =>
            LookAtRotation(self, target.transform.position);

        /// <summary>
        /// Returns a location around a target point.
        /// </summary>
        /// <param name="point">Target point.</param>
        /// <param name="pivot">Amount of pivot over time.</param>
        /// <param name="angle">Target angle.</param>
        /// <returns></returns>
        public static Vector3 RotatePivotPoint(Vector3 point, Vector3 pivot, Quaternion angle)
        {
            return angle * (point - pivot) + pivot;
        }



        #endregion

        #region Unity Extensions
        /// <summary>
        /// Checks if a GameObject has a MonoBehaviour.
        /// </summary>
        /// <typeparam name="T">A MonoBehaviour.</typeparam>
        /// <param name="gameObject">Target GameObject.</param>
        public static bool HasComponent<T>(this GameObject gameObject) where T : MonoBehaviour =>
            gameObject.GetComponent<T>() != null;

        /// <summary>
        /// Change a Rigidbody's direction without altering its velocity.
        /// </summary>
        /// <param name="direction">Target direction.</param>
        public static void SetDirection(this Rigidbody rb, Vector3 direction)
        {
            rb.linearVelocity = direction.normalized * rb.linearVelocity.magnitude;
        }

        /// <summary>
        /// Checks if a LayerMask contains a specific layer.
        /// </summary>
        /// <param name="layerNumber">Target mask layer.</param>
        public static bool Contains(this LayerMask mask, int layerNumber) =>
            mask == (mask | (1 << layerNumber));

        #endregion

        #region List Extensions

        /// <summary>
        ///     Add multiple values to a List at once.
        /// </summary>
        /// <example>
        /// Example usage:
        /// <code>SomeList.AddMulti("a", "z");</code>
        /// </example>
        public static void AddMulti<T>(this List<T> list, params T[] elements) =>
            list.AddRange(elements);

        /// <summary>
        ///     Get list containing all of the children of the called GameObject.
        /// </summary>
        /// <example>
        /// Example usage:
        /// <code>List<GameObject> targetChildren = targetGameObject.GetChildren();</code>
        /// </example>
        /// <param name="go">Target <see cref="GameObject" />.</param>
        public static List<GameObject> GetChildren(this GameObject go) =>
            (from Transform tran in go.transform select tran.gameObject).ToList();



        #endregion

        #region String Extensions

        /// <summary>
        /// Returns the left part of this string instance.
        /// </summary>
        /// <param name="chars">Number of characters to return.</param>
        public static string Left(this string input, int chars)
        {
            return input.Substring(0, Math.Min(input.Length, chars));
        }

        /// <summary>
        /// Returns the right part of the string instance.
        /// </summary>
        /// <param name="chars">Number of characters to return.</param>
        public static string Right(this string input, int chars)
        {
            return input.Substring(Math.Max(input.Length - chars, 0), Math.Min(chars, input.Length));
        }

        #endregion

        #region Mesh Extensions

        public static Mesh CreatePlaneFromBox(float width, float height)
        {
            Mesh mesh = new Mesh
            {
                vertices = new Vector3[]
                {
                new Vector3(-width * 0.5f, height * 0.5f, 0f),
                new Vector3(width * 0.5f, height * 0.5f, 0f),
                new Vector3(width * 0.5f, -height * 0.5f, 0f),
                new Vector3(-width * 0.5f, -height * 0.5f, 0f),
                },
                triangles = new int[] { 0, 2, 1, 3, 2, 0 }
            };
            mesh.RecalculateNormals();
            return mesh;
        }

        #endregion

        #region Draw Extensions
        public static class DrawArrow
        {
            public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Gizmos.DrawRay(pos, direction);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
                Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
            }

            public static void ForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Gizmos.color = color;
                Gizmos.DrawRay(pos, direction);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
                Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
            }

            public static void ForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Debug.DrawRay(pos, direction);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Debug.DrawRay(pos + direction, right * arrowHeadLength);
                Debug.DrawRay(pos + direction, left * arrowHeadLength);
            }
            public static void ForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Debug.DrawRay(pos, direction, color);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
                Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
            }
        }
        #endregion
    }

    #region Expanded Quaternion
    public class ExpandedQuaternion : PropertyAttribute
    {
        // You can add properties here if needed
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ExpandedQuaternion))]
    public class DrawBothPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2f + 4f; // Adding a little padding
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Check if the property is actually a quaternion
            if (property.propertyType != SerializedPropertyType.Quaternion)
            {
                EditorGUI.LabelField(position, label.text, "Use ExpandedQuaternion only with Quaternion.");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // Draw the default quaternion field (it'll show as Euler angles)
            var r0 = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(r0, property);

            // Draw xyzw representation
            var r1 = new Rect(position.x, position.y + r0.height + 2f, position.width, EditorGUIUtility.singleLineHeight);
            Quaternion quaternionValue = property.quaternionValue;

            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 150;

            EditorGUI.LabelField(r1, "Quaternion (raw)");
            r1.x += EditorGUIUtility.labelWidth;
            r1.width -= EditorGUIUtility.labelWidth;

            string xyzw = $"x: {quaternionValue.x:F3}, y: {quaternionValue.y:F3}, z: {quaternionValue.z:F3}, w: {quaternionValue.w:F3}";
            EditorGUI.LabelField(r1, xyzw);

            EditorGUIUtility.labelWidth = oldLabelWidth;  // Restore the old label width

            EditorGUI.EndProperty();
        }
    }
#endif
    #endregion
}