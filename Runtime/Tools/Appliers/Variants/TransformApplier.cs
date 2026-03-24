using UnityEngine;
using EC.Appliers.Core;

namespace EC.Appliers.Variants
{
    // Position
    [Applier(typeof(Transform), typeof(float), "Transform", "PositionX"), ComponentIcon("Transform Icon")]
    public class TransformPositionXApplier : ApplierBase<Transform, float>
    {
        public override float Value => Target.position.x;
        public override void SetValue(float value) => Target.position = new Vector3(value, Target.position.y, Target.position.z);
    }
    [Applier(typeof(Transform), typeof(float), "Transform", "PositionY"), ComponentIcon("Transform Icon")]
    public class TransformPositionYApplier : ApplierBase<Transform, float>
    {
        public override float Value => Target.position.y;
        public override void SetValue(float value) => Target.position = new Vector3(Target.position.x, value, Target.position.z);
    }
    [Applier(typeof(Transform), typeof(float), "Transform", "PositionZ"), ComponentIcon("Transform Icon")]
    public class TransformPositionZApplier : ApplierBase<Transform, float>
    {
        public override float Value => Target.position.z;
        public override void SetValue(float value) => Target.position = new Vector3(Target.position.x, Target.position.y, value);
    }
    [Applier(typeof(Transform), typeof(Vector3), "Transform", "Position"), ComponentIcon("Transform Icon")]
    public class TransformPositionApplier : ApplierBase<Transform, Vector3>
    {
        public override Vector3 Value => Target.position;
        public override void SetValue(Vector3 value) => Target.position = value;
    }

    // Local Position
    [Applier(typeof(Transform), typeof(float), "Transform", "LocalPositionX"), ComponentIcon("Transform Icon")]
    public class TransformLocalPositionXApplier : ApplierBase<Transform, float>
    {
        public override float Value => Target.localPosition.x;
        public override void SetValue(float value) => Target.localPosition = new Vector3(value, Target.localPosition.y, Target.localPosition.z);
    }
    [Applier(typeof(Transform), typeof(float), "Transform", "LocalPositionY"), ComponentIcon("Transform Icon")]
    public class TransformLocalPositionYApplier : ApplierBase<Transform, float>
    {
        public override float Value => Target.localPosition.y;
        public override void SetValue(float value) => Target.localPosition = new Vector3(Target.localPosition.x, value, Target.localPosition.z);
    }
    [Applier(typeof(Transform), typeof(float), "Transform", "LocalPositionZ"), ComponentIcon("Transform Icon")]
    public class TransformLocalPositionZApplier : ApplierBase<Transform, float>
    {
        public override float Value => Target.localPosition.z;
        public override void SetValue(float value) => Target.localPosition = new Vector3(Target.localPosition.x, Target.localPosition.y, value);
    }
    [Applier(typeof(Transform), typeof(Vector3), "Transform", "LocalPosition"), ComponentIcon("Transform Icon")]
    public class TransformLocalPositionApplier : ApplierBase<Transform, Vector3>
    {
        public override Vector3 Value => Target.localPosition;
        public override void SetValue(Vector3 value) => Target.localPosition = value;
    }

    // Local Scale
    [Applier(typeof(Transform), typeof(float), "Transform", "LocalScaleX"), ComponentIcon("Transform Icon")]
    public class TransformLocalScaleXApplier : ApplierBase<Transform, float>
    {
        public override float Value => Target.localScale.x;
        public override void SetValue(float value) => Target.localScale = new Vector3(value, Target.localScale.y, Target.localScale.z);
    }
    [Applier(typeof(Transform), typeof(float), "Transform", "LocalScaleY"), ComponentIcon("Transform Icon")]
    public class TransformLocalScaleYApplier : ApplierBase<Transform, float>
    {
        public override float Value => Target.localScale.y;
        public override void SetValue(float value) => Target.localScale = new Vector3(Target.localScale.x, value, Target.localScale.z);
    }
    [Applier(typeof(Transform), typeof(float), "Transform", "LocalScaleZ"), ComponentIcon("Transform Icon")]
    public class TransformLocalScaleZApplier : ApplierBase<Transform, float>
    {
        public override float Value => Target.localScale.z;
        public override void SetValue(float value) => Target.localScale = new Vector3(Target.localScale.x, Target.localScale.y, value);
    }
    [Applier(typeof(Transform), typeof(Vector3), "Transform", "LocalScale"), ComponentIcon("Transform Icon")]
    public class TransformLocalScaleApplier : ApplierBase<Transform, Vector3>
    {
        public override Vector3 Value => Target.localScale;
        public override void SetValue(Vector3 value) => Target.localScale = value;
    }

    // Rotation
    [Applier(typeof(Transform), typeof(float), "Transform", "RotationX"), ComponentIcon("Transform Icon")]
    public class TransformRotationXApplier : ApplierBase<Transform, float>
    {
        public override float Value => Target.rotation.eulerAngles.x;
        public override void SetValue(float value) => Target.rotation = Quaternion.Euler(new Vector3(value, Target.rotation.eulerAngles.y, Target.rotation.eulerAngles.z));
    }
    [Applier(typeof(Transform), typeof(float), "Transform", "RotationY"), ComponentIcon("Transform Icon")]
    public class TransformRotationYApplier : ApplierBase<Transform, float>
    {
        public override float Value => Target.rotation.eulerAngles.y;
        public override void SetValue(float value) => Target.rotation = Quaternion.Euler(new Vector3(Target.rotation.eulerAngles.x, value, Target.rotation.eulerAngles.z));
    }
    [Applier(typeof(Transform), typeof(float), "Transform", "RotationZ"), ComponentIcon("Transform Icon")]
    public class TransformRotationZApplier : ApplierBase<Transform, float>
    {
        public override float Value => Target.rotation.eulerAngles.z;
        public override void SetValue(float value) => Target.rotation = Quaternion.Euler(new Vector3(Target.rotation.eulerAngles.x, Target.rotation.eulerAngles.y, value));
    }
    [Applier(typeof(Transform), typeof(Vector3), "Transform", "RotationEuler"), ComponentIcon("Transform Icon")]
    public class TransformRotationEulerApplier : ApplierBase<Transform, Vector3>
    {
        public override Vector3 Value => Target.rotation.eulerAngles;
        public override void SetValue(Vector3 value) => Target.rotation = Quaternion.Euler(value);
    }
    [Applier(typeof(Transform), typeof(Quaternion), "Transform", "Rotation"), ComponentIcon("Transform Icon")]
    public class TransformRotationApplier : ApplierBase<Transform, Quaternion>
    {
        public override Quaternion Value => Target.rotation;
        public override void SetValue(Quaternion value) => Target.rotation = value;
    }

    // Local Rotation
    [Applier(typeof(Transform), typeof(float), "Transform", "LocalRotationX"), ComponentIcon("Transform Icon")]
    public class TransformLocalRotationXApplier : ApplierBase<Transform, float>
    {
        public override float Value => Target.localRotation.eulerAngles.x;
        public override void SetValue(float value) => Target.localRotation = Quaternion.Euler(new Vector3(value, Target.localRotation.eulerAngles.y, Target.localRotation.eulerAngles.z));
    }
    [Applier(typeof(Transform), typeof(float), "Transform", "LocalRotationY"), ComponentIcon("Transform Icon")]
    public class TransformLocalRotationYApplier : ApplierBase<Transform, float>
    {
        public override float Value => Target.localRotation.eulerAngles.y;
        public override void SetValue(float value) => Target.localRotation = Quaternion.Euler(new Vector3(Target.localRotation.eulerAngles.x, value, Target.localRotation.eulerAngles.z));
    }
    [Applier(typeof(Transform), typeof(float), "Transform", "LocalRotationZ"), ComponentIcon("Transform Icon")]
    public class TransformLocalRotationZApplier : ApplierBase<Transform, float>
    {
        public override float Value => Target.localRotation.eulerAngles.z;
        public override void SetValue(float value) => Target.localRotation = Quaternion.Euler(new Vector3(Target.localRotation.eulerAngles.x, Target.localRotation.eulerAngles.y, value));
    }
    [Applier(typeof(Transform), typeof(Vector3), "Transform", "LocalRotationEuler"), ComponentIcon("Transform Icon")]
    public class TransformLocalRotationEulerApplier : ApplierBase<Transform, Vector3>
    {
        public override Vector3 Value => Target.localRotation.eulerAngles;
        public override void SetValue(Vector3 value) => Target.localRotation = Quaternion.Euler(value);
    }
    [Applier(typeof(Transform), typeof(Quaternion), "Transform", "LocalRotation"), ComponentIcon("Transform Icon")]
    public class TransformLocalRotationApplier : ApplierBase<Transform, Quaternion>
    {
        public override Quaternion Value => Target.localRotation;
        public override void SetValue(Quaternion value) => Target.localRotation = value;
    }
}
