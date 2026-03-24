using UnityEngine;
using EC.Appliers.Core;

namespace EC.Appliers.Variants
{
    [Applier(typeof(Camera), typeof(float), "Camera", "FieldOfView"), ComponentIcon("Camera Icon")]
    public class CameraFieldOfViewApplier : ApplierBase<Camera, float>
    {
        public override float Value => Target.fieldOfView;
        public override void SetValue(float value) => Target.fieldOfView = value;
    }
    [Applier(typeof(Camera), typeof(float), "Camera", "OrtographicSize"), ComponentIcon("Camera Icon")]
    public class CameraOrtographicSizeApplier : ApplierBase<Camera, float>
    {
        public override float Value => Target.orthographicSize;
        public override void SetValue(float value) => Target.orthographicSize = value;
    }
}
