using Sandbox;

namespace Cinema;

public partial class Player
{
    // default
    public bool isLeftShoulderView = true;
    public bool isRightShoulderView;

    public void ThirdPersonSwapShoulder()
    {
        if(ThirdPersonCamera)
        {
            isRightShoulderView = !isRightShoulderView;
            Log.Info($"Switched to {(isRightShoulderView ? "Right Shoulder View" : "Left Shoulder View")}");
        }
    }

    public void SimulateCamera(IClient cl)
    {
        Camera.Rotation = EyeRotation;

        // Set field of view to whatever the user chose in options
        Camera.FieldOfView = Screen.CreateVerticalFieldOfView(Game.Preferences.FieldOfView);

        if (ThirdPersonCamera)
        {
            Camera.FirstPersonViewer = null;

            Vector3 targetPos;
            var center = Position + Vector3.Up * 64;
            var pos = center;
            var rot = Rotation.FromAxis(Vector3.Up, -16) * Camera.Rotation;

            float distance = 130.0f * Scale;
            targetPos = pos + rot.Right * ((CollisionBounds.Mins.x + 32) * Scale);
            targetPos = pos + rot.Right * ((isRightShoulderView ? 1 : 64) * Scale);
            targetPos += rot.Forward * -distance;

            var tr = Trace.Ray(pos, targetPos)
                .WithAnyTags("solid")
                .Ignore(this)
                .Radius(8)
                .Run();

            Camera.Position = tr.EndPosition;
        }
        else
        {
            Camera.Position = EyePosition;
            Camera.ZNear = 0.5f;

            // Set the first person viewer to this, so it won't render our model
            Camera.FirstPersonViewer = this;
            Camera.Main.SetViewModelCamera(Camera.FieldOfView);

            if (ActiveChild is Carriable carriable)
            {
                carriable.UpdateViewmodelCamera();
                carriable.UpdateCamera();
            }
        }


    }
}
