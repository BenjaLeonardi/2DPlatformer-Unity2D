using UnityEngine;
using Cinemachine;

public class LockCameraAxis : CinemachineExtension
{
    enum AxisToLock
    {
        X,
        Y,
        Z
    };
 
    [SerializeField] private AxisToLock lockedAxis;
    [SerializeField, Tooltip("Bloquea el eje seleccionado en la posicion especificada")] private float lockedAxisPosition = 10;
 
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime
    )
    {
        if (enabled && stage == CinemachineCore.Stage.Body)
        {
            var pos = state.RawPosition;
            switch (lockedAxis)
            {
                case AxisToLock.X:
                    pos.x = lockedAxisPosition;
                    break;
                case AxisToLock.Y:
                    pos.y = lockedAxisPosition;
                    break;
                case AxisToLock.Z:
                    pos.z = lockedAxisPosition;
                    break;
            }
            state.RawPosition = pos;
        }
    }
}
