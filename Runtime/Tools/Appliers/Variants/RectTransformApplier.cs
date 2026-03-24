using UnityEngine;
using EC.Appliers.Core;

namespace EC.Appliers.Variants
{
    // Position
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "PositionX"), ComponentIcon("RectTransform Icon")]
    public class RectTransformPositionXApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.position.x;
        public override void SetValue(float value) => Target.position = new Vector3(value, Target.position.y, Target.position.z);
    }
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "PositionY"), ComponentIcon("RectTransform Icon")]
    public class RectTransformPositionYApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.position.y;
        public override void SetValue(float value) => Target.position = new Vector3(Target.position.x, value, Target.position.z);
    }
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "PositionZ"), ComponentIcon("RectTransform Icon")]
    public class RectTransformPositionZApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.position.z;
        public override void SetValue(float value) => Target.position = new Vector3(Target.position.x, Target.position.y, value);
    }
    [Applier(typeof(RectTransform), typeof(Vector3), "RectTransform", "Position"), ComponentIcon("RectTransform Icon")]
    public class RectTransformPositionApplier : ApplierBase<RectTransform, Vector3>
    {
        public override Vector3 Value => Target.position;
        public override void SetValue(Vector3 value) => Target.position = value;
    }

    // Local Position
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "LocalPositionX"), ComponentIcon("RectTransform Icon")]
    public class RectTransformLocalPositionXApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.localPosition.x;
        public override void SetValue(float value) => Target.localPosition = new Vector3(value, Target.localPosition.y, Target.localPosition.z);
    }
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "LocalPositionY"), ComponentIcon("RectTransform Icon")]
    public class RectTransformLocalPositionYApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.localPosition.y;
        public override void SetValue(float value) => Target.localPosition = new Vector3(Target.localPosition.x, value, Target.localPosition.z);
    }
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "LocalPositionZ"), ComponentIcon("RectTransform Icon")]
    public class RectTransformLocalPositionZApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.localPosition.z;
        public override void SetValue(float value) => Target.localPosition = new Vector3(Target.localPosition.x, Target.localPosition.y, value);
    }
    [Applier(typeof(RectTransform), typeof(Vector3), "RectTransform", "LocalPosition"), ComponentIcon("RectTransform Icon")]
    public class RectTransformLocalPositionApplier : ApplierBase<RectTransform, Vector3>
    {
        public override Vector3 Value => Target.localPosition;
        public override void SetValue(Vector3 value) => Target.localPosition = value;
    }

    // Anchored Position
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "AnchoredPositionX"), ComponentIcon("RectTransform Icon")]
    public class RectTransformAnchoredPositionXApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.anchoredPosition.x;
        public override void SetValue(float value) => Target.anchoredPosition = new Vector3(value, Target.anchoredPosition.y);
    }
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "AnchoredPositionY"), ComponentIcon("RectTransform Icon")]
    public class RectTransformAnchoredPositionYApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.anchoredPosition.y;
        public override void SetValue(float value) => Target.anchoredPosition = new Vector2(Target.anchoredPosition.x, value);
    }
    [Applier(typeof(RectTransform), typeof(Vector2), "RectTransform", "AnchoredPosition"), ComponentIcon("RectTransform Icon")]
    public class RectTransformAnchoredPositionApplier : ApplierBase<RectTransform, Vector2>
    {
        public override Vector2 Value => Target.anchoredPosition;
        public override void SetValue(Vector2 value) => Target.anchoredPosition = value;
    }

    // Local Scale
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "LocalScaleX"), ComponentIcon("RectTransform Icon")]
    public class RectTransformLocalScaleXApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.localScale.x;
        public override void SetValue(float value) => Target.localScale = new Vector3(value, Target.localScale.y, Target.localScale.z);
    }
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "LocalScaleY"), ComponentIcon("RectTransform Icon")]
    public class RectTransformLocalScaleYApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.localScale.y;
        public override void SetValue(float value) => Target.localScale = new Vector3(Target.localScale.x, value, Target.localScale.z);
    }
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "LocalScaleZ"), ComponentIcon("RectTransform Icon")]
    public class RectTransformLocalScaleZApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.localScale.z;
        public override void SetValue(float value) => Target.localScale = new Vector3(Target.localScale.x, Target.localScale.y, value);
    }
    [Applier(typeof(RectTransform), typeof(Vector3), "RectTransform", "LocalScale"), ComponentIcon("RectTransform Icon")]
    public class RectTransformLocalScaleApplier : ApplierBase<RectTransform, Vector3>
    {
        public override Vector3 Value => Target.localScale;
        public override void SetValue(Vector3 value) => Target.localScale = value;
    }

    // Size Delta
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "SizeDeltaX"), ComponentIcon("RectTransform Icon")]
    public class RectTransformSizeDeltaXApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.sizeDelta.x;
        public override void SetValue(float value) => Target.sizeDelta = new Vector3(value, Target.sizeDelta.y);
    }
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "SizeDeltaY"), ComponentIcon("RectTransform Icon")]
    public class RectTransformSizeDeltaYApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.sizeDelta.y;
        public override void SetValue(float value) => Target.sizeDelta = new Vector2(Target.sizeDelta.x, value);
    }
    [Applier(typeof(RectTransform), typeof(Vector2), "RectTransform", "SizeDelta"), ComponentIcon("RectTransform Icon")]
    public class RectTransformSizeDeltaApplier : ApplierBase<RectTransform, Vector2>
    {
        public override Vector2 Value => Target.sizeDelta;
        public override void SetValue(Vector2 value) => Target.sizeDelta = value;
    }

    // Rotation
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "RotationX"), ComponentIcon("RectTransform Icon")]
    public class RectTransformRotationXApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.rotation.eulerAngles.x;
        public override void SetValue(float value) => Target.rotation = Quaternion.Euler(new Vector3(value, Target.rotation.eulerAngles.y, Target.rotation.eulerAngles.z));
    }
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "RotationY"), ComponentIcon("RectTransform Icon")]
    public class RectTransformRotationYApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.rotation.eulerAngles.y;
        public override void SetValue(float value) => Target.rotation = Quaternion.Euler(new Vector3(Target.rotation.eulerAngles.x, value, Target.rotation.eulerAngles.z));
    }
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "RotationZ"), ComponentIcon("RectTransform Icon")]
    public class RectTransformRotationZApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.rotation.eulerAngles.z;
        public override void SetValue(float value) => Target.rotation = Quaternion.Euler(new Vector3(Target.rotation.eulerAngles.x, Target.rotation.eulerAngles.y, value));
    }
    [Applier(typeof(RectTransform), typeof(Vector3), "RectTransform", "RotationEuler"), ComponentIcon("RectTransform Icon")]
    public class RectTransformRotationEulerApplier : ApplierBase<RectTransform, Vector3>
    {
        public override Vector3 Value => Target.rotation.eulerAngles;
        public override void SetValue(Vector3 value) => Target.rotation = Quaternion.Euler(value);
    }
    [Applier(typeof(RectTransform), typeof(Quaternion), "RectTransform", "Rotation"), ComponentIcon("RectTransform Icon")]
    public class RectTransformRotationApplier : ApplierBase<RectTransform, Quaternion>
    {
        public override Quaternion Value => Target.rotation;
        public override void SetValue(Quaternion value) => Target.rotation = value;
    }

    // Local Rotation
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "LocalRotationX"), ComponentIcon("RectTransform Icon")]
    public class RectTransformLocalRotationXApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.localRotation.eulerAngles.x;
        public override void SetValue(float value) => Target.localRotation = Quaternion.Euler(new Vector3(value, Target.localRotation.eulerAngles.y, Target.localRotation.eulerAngles.z));
    }
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "LocalRotationY"), ComponentIcon("RectTransform Icon")]
    public class RectTransformLocalRotationYApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.localRotation.eulerAngles.y;
        public override void SetValue(float value) => Target.localRotation = Quaternion.Euler(new Vector3(Target.localRotation.eulerAngles.x, value, Target.localRotation.eulerAngles.z));
    }
    [Applier(typeof(RectTransform), typeof(float), "RectTransform", "LocalRotationZ"), ComponentIcon("RectTransform Icon")]
    public class RectTransformLocalRotationZApplier : ApplierBase<RectTransform, float>
    {
        public override float Value => Target.localRotation.eulerAngles.z;
        public override void SetValue(float value) => Target.localRotation = Quaternion.Euler(new Vector3(Target.localRotation.eulerAngles.x, Target.localRotation.eulerAngles.y, value));
    }
    [Applier(typeof(RectTransform), typeof(Vector3), "RectTransform", "LocalRotationEuler"), ComponentIcon("RectTransform Icon")]
    public class RectTransformLocalRotationEulerApplier : ApplierBase<RectTransform, Vector3>
    {
        public override Vector3 Value => Target.localRotation.eulerAngles;
        public override void SetValue(Vector3 value) => Target.localRotation = Quaternion.Euler(value);
    }
    [Applier(typeof(RectTransform), typeof(Quaternion), "RectTransform", "LocalRotation"), ComponentIcon("RectTransform Icon")]
    public class RectTransformLocalRotationApplier : ApplierBase<RectTransform, Quaternion>
    {
        public override Quaternion Value => Target.localRotation;
        public override void SetValue(Quaternion value) => Target.localRotation = value;
    }
}
