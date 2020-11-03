namespace RBOT_UnityPlugin
{
    public abstract class RBOT_EventArgs
    {
        public string Msg { get; }
        public ResultCode Result { get; }

        public RBOT_EventArgs(string msg, ResultCode result)
        {
            this.Msg = msg;
            this.Result = result;
        }
    }
    public class RBOT_PoseEstimationEventArgs : RBOT_EventArgs
    {
        public float[] PoseData { get; }

        public RBOT_PoseEstimationEventArgs(string msg, ResultCode result, float[] poseData) : base(msg, result)
        {
            PoseData = poseData;
        }
    }
    public class RBOT_ObjectAddedEventArgs : RBOT_EventArgs
    {
        public Object3D object3D{ get; }

        public RBOT_ObjectAddedEventArgs(string msg, ResultCode result, Object3D _object3D) : base(msg, result)
        {
            this.object3D = _object3D;
        }
    }
    public class RBOT_ObjectRemovedEventArgs : RBOT_EventArgs
    {
        public int Index { get; }

        public RBOT_ObjectRemovedEventArgs(string msg, ResultCode result, int _index) : base(msg, result)
        {
            this.Index = _index;
        }
    }
    public class RBOT_PoseEstimatorCreatedEventArgs : RBOT_EventArgs
    {
        public RBOT_PoseEstimatorCreatedEventArgs(string msg, ResultCode result) : base(msg, result)
        {

        }
    }

    public delegate void RBOT_PoseEstimationEventHandler(RBOT_PoseEstimationEventArgs e);
    public delegate void RBOT_ObjectAddedEventHandler(RBOT_ObjectAddedEventArgs e);
    public delegate void RBOT_ObjectRomevedEventHandler(RBOT_ObjectRemovedEventArgs e);
    public delegate void RBOT_PoseEstimatorCreatedEventHandler(RBOT_PoseEstimatorCreatedEventArgs e);
}