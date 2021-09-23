using UnityEngine;
using UnityEngine.InputSystem;

public class Haptics
{
    public static void Rumble(float intensity)
    {
        GetGamepad()?.SetMotorSpeeds(intensity, intensity);
    }

    public static void StopRumble()
    {
        GetGamepad()?.SetMotorSpeeds(0.0f, 0.0f);
    }

    private static Gamepad GetGamepad()
    {
        return Gamepad.current;
        //return Gamepad.all.FirstOrDefault(gamepad => m_Input.devices.Value.Any(device => device.deviceId == gamepad.deviceId));
    }
}
