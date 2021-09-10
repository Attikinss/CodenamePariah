// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Input/InputMap.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMap : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMap()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMap"",
    ""maps"": [
        {
            ""name"": ""Pariah"",
            ""id"": ""fe51b155-810c-414e-ba9e-193edb85052c"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""fb938c95-86d8-4953-a494-eb9d071fdb4d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""06d42fe4-8f5f-4bd9-a30b-65478cb7acb3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""5a218837-3080-4382-a183-6f476c77f504"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Possession"",
                    ""type"": ""Button"",
                    ""id"": ""2822eab5-a5b5-4e37-9625-c22c7d2cb9d4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Raise"",
                    ""type"": ""Button"",
                    ""id"": ""bad2e9ad-e017-4bf7-8cb4-218300a5a17e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Lower"",
                    ""type"": ""Button"",
                    ""id"": ""16b5e56d-e5d9-43c7-b959-cb11a8cb8555"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5c510ee1-57ef-425e-bf59-1ae4c7495048"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e0a6f370-3e22-4d25-bcdf-6e0deb7209c7"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c763dd09-3dd5-44a4-81ff-1b9742b34b23"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Possession"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""3f7c76aa-cb96-46fc-8df4-679872631eba"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""b2223058-08fc-4bfa-9ffe-18a24bf9bcd0"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""aa0fabff-2f3f-4225-81b8-5633581da822"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8c61dd14-f3b7-4ace-9178-53847c9c946d"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""e6c0c1a2-53ed-49fb-a1ef-4e0caf925967"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""cd48883e-605b-4feb-a522-1f77f8b7fb78"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""28c95b18-ebb2-4ddf-8eda-6f6def5c0c7e"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c86a5501-1e74-4b44-a5f8-ea56424f65f7"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""bdd8d062-d687-4706-8bd0-041935ee61a6"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""fa56d928-4125-4c51-ba20-ee983597a31f"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""f32dccea-9ed7-4dc0-aa9a-3b620162022d"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Look"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f7863801-3499-4a42-a57d-d9b05ef55957"",
                    ""path"": ""<Gamepad>/rightStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""bcdebd9c-ff6e-4baf-988a-c62acce2167c"",
                    ""path"": ""<Gamepad>/rightStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b155efad-c960-45bc-a29b-e00e1746664c"",
                    ""path"": ""<Gamepad>/rightStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""e47f1b98-623b-4794-801a-95ff4219774c"",
                    ""path"": ""<Gamepad>/rightStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""5e9199ff-0f85-4845-ad15-d91b7d2195a2"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=0.1,y=0.1)"",
                    ""groups"": ""KBM"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""34b954f0-161d-4dec-960b-dfa789a39853"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Possession"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2ab8cc32-a24b-47f7-9384-d894f3b28e14"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Raise"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9ebf7aa5-030d-4d2c-a031-85cc4e6e69cf"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Raise"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eb12ab1b-acbe-4082-8f3f-112500243de9"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Lower"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""03ae98bd-2f7f-4e08-9609-5fffbee8a37b"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Lower"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Host"",
            ""id"": ""9adc571d-a01c-495f-833b-e40af1dbe17f"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""9e4acff7-ae78-4116-b41e-dbaecacd7c7a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""cc2e554b-fc5c-4ff4-b0f3-3980d3e865f5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""c8f56a98-9122-4f16-88a0-da29836269f2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Possession"",
                    ""type"": ""Button"",
                    ""id"": ""4c9f5263-4b9a-4806-8e74-f4a34899045e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Melee"",
                    ""type"": ""Button"",
                    ""id"": ""8e59b1b5-e1c5-4e27-8cff-98d0ccff9a08"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ADS"",
                    ""type"": ""Button"",
                    ""id"": ""343d798a-e920-4bbb-9b4a-2a0f63ca8adb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""PassThrough"",
                    ""id"": ""84b6fde1-732d-461b-bc87-2c0ddd54988f"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""e119e84c-d294-4a5a-afb7-7cf29ab71709"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Slide"",
                    ""type"": ""Button"",
                    ""id"": ""93a55832-59b9-416a-87c6-b8f78d496127"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Select1"",
                    ""type"": ""Button"",
                    ""id"": ""0364cb4f-20c1-4e50-829a-718426a89f84"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Select2"",
                    ""type"": ""Button"",
                    ""id"": ""1a369283-ec17-47f9-8107-662f60fdea79"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Select3"",
                    ""type"": ""Button"",
                    ""id"": ""34ac542d-357c-4a6b-a6aa-2be3b09a062e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RecoilTest"",
                    ""type"": ""Button"",
                    ""id"": ""bcf73363-bc80-4831-b974-7150f49040b0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ability2"",
                    ""type"": ""Button"",
                    ""id"": ""f9c58ad8-8a77-4d56-a4a3-4075ad8b7bde"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Reload"",
                    ""type"": ""Button"",
                    ""id"": ""5b49c673-d3b1-41b6-99d5-f9d704f79ca1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ability3"",
                    ""type"": ""Button"",
                    ""id"": ""1c83b672-8c3a-4495-a8a6-adbde018f0a7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Debug"",
                    ""type"": ""Button"",
                    ""id"": ""eae199af-e3a2-4a7c-b07a-7d79b6689faa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ToggleHUD"",
                    ""type"": ""Button"",
                    ""id"": ""ef142069-848d-41fb-81e7-af98a812f8b8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""952a0180-940e-46a8-a352-c1c871a68f5f"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Possession"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""7efef21f-f9a6-4eee-a7b7-9cfd08e5dd34"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6519c53f-34a9-478f-a8af-77703eb551c6"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c15220f6-ea70-425a-9058-e333402d9fa4"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""321727a7-19e6-49cc-ac2d-2f89351122dd"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""c9e6a9dd-638d-4ceb-a4ee-6ae0d998c931"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""eab28111-727e-46b2-8a67-b099fbe2acfd"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""aa9a5c67-4cc1-4fc9-8626-a66a6e313bcb"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f8ec2782-7421-495f-8062-9748a685133c"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""f8a96b1d-79a6-4985-a4f5-6620805fa275"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""c4e8c271-ade7-4517-990f-cf6a80286016"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""08ee35bf-5be1-44a8-850d-9b2074d1147e"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a93b939d-d57b-4525-85f0-e6cf4748c7c9"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""113c2ee9-635b-481d-931b-323955a0d483"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Melee"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""be30fbe1-dcfe-4bae-a084-4dc837dcbf88"",
                    ""path"": ""<Gamepad>/rightStick/down"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Melee"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b45685f2-8365-42e4-b637-81498a84c566"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""14ff3784-6465-45c1-9f5f-f182452bcccd"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""ADS"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""71de11ca-2bc1-476c-8fde-249474715189"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""ADS"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""42e55000-702b-4f15-b0f3-75eefdd353eb"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Look"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c5190f76-0d31-4eff-b649-496b991ea486"",
                    ""path"": ""<Gamepad>/rightStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""889f00dc-e9b6-4e9f-979f-ae45eb9a2b77"",
                    ""path"": ""<Gamepad>/rightStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e09f9c19-26f8-415f-980b-d9ec9410e5ce"",
                    ""path"": ""<Gamepad>/rightStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""19b4c268-23e4-46fb-925b-e3a0750584ac"",
                    ""path"": ""<Gamepad>/rightStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""4a69d2a7-d3fb-4ec1-8307-2fd09f79b56b"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=0.1,y=0.1)"",
                    ""groups"": ""KBM"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""13aea1dd-1775-49b0-943f-f06b15230a71"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a2b87edc-4e45-466f-bd41-9b228d93808f"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Possession"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1d304913-64ab-4625-b041-4816615ebd3a"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""06c416a2-c864-4d01-8a8e-d316b2e038b3"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ddb31c03-aafc-41dc-9146-b181bb57af18"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Slide"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c52b96ba-281a-4f8c-be1d-22fa59d42267"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Slide"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ef6720c9-a014-4fa8-ba5c-a31f7fc09217"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Select1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cbd2250e-468b-4b91-9ca4-2f5f58d84d64"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Select2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fd18cd63-caeb-4989-aefc-5546fc2ed0b5"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""RecoilTest"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7cdb3b23-dc12-467b-b688-42c89ec820fb"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Ability2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c82cdf53-4342-4604-a013-40d6cd52bada"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c94af80d-b167-4b36-995e-12faa5a99128"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Ability3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8f60f675-66e6-4506-87e1-d9b14f5a29c3"",
                    ""path"": ""<Keyboard>/f3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Debug"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b3e6ed3e-811b-47c5-a261-13e34e8b8ea6"",
                    ""path"": ""<Keyboard>/f4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""ToggleHUD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""95045599-4679-4d11-813e-37f2645a4094"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Select3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KBM"",
            ""bindingGroup"": ""KBM"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Controller"",
            ""bindingGroup"": ""Controller"",
            ""devices"": [
                {
                    ""devicePath"": ""<DualShockGamepad>"",
                    ""isOptional"": true,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<XInputController>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Pariah
        m_Pariah = asset.FindActionMap("Pariah", throwIfNotFound: true);
        m_Pariah_Movement = m_Pariah.FindAction("Movement", throwIfNotFound: true);
        m_Pariah_Look = m_Pariah.FindAction("Look", throwIfNotFound: true);
        m_Pariah_Dash = m_Pariah.FindAction("Dash", throwIfNotFound: true);
        m_Pariah_Possession = m_Pariah.FindAction("Possession", throwIfNotFound: true);
        m_Pariah_Raise = m_Pariah.FindAction("Raise", throwIfNotFound: true);
        m_Pariah_Lower = m_Pariah.FindAction("Lower", throwIfNotFound: true);
        // Host
        m_Host = asset.FindActionMap("Host", throwIfNotFound: true);
        m_Host_Movement = m_Host.FindAction("Movement", throwIfNotFound: true);
        m_Host_Look = m_Host.FindAction("Look", throwIfNotFound: true);
        m_Host_Dash = m_Host.FindAction("Dash", throwIfNotFound: true);
        m_Host_Possession = m_Host.FindAction("Possession", throwIfNotFound: true);
        m_Host_Melee = m_Host.FindAction("Melee", throwIfNotFound: true);
        m_Host_ADS = m_Host.FindAction("ADS", throwIfNotFound: true);
        m_Host_Fire = m_Host.FindAction("Fire", throwIfNotFound: true);
        m_Host_Jump = m_Host.FindAction("Jump", throwIfNotFound: true);
        m_Host_Slide = m_Host.FindAction("Slide", throwIfNotFound: true);
        m_Host_Select1 = m_Host.FindAction("Select1", throwIfNotFound: true);
        m_Host_Select2 = m_Host.FindAction("Select2", throwIfNotFound: true);
        m_Host_Select3 = m_Host.FindAction("Select3", throwIfNotFound: true);
        m_Host_RecoilTest = m_Host.FindAction("RecoilTest", throwIfNotFound: true);
        m_Host_Ability2 = m_Host.FindAction("Ability2", throwIfNotFound: true);
        m_Host_Reload = m_Host.FindAction("Reload", throwIfNotFound: true);
        m_Host_Ability3 = m_Host.FindAction("Ability3", throwIfNotFound: true);
        m_Host_Debug = m_Host.FindAction("Debug", throwIfNotFound: true);
        m_Host_ToggleHUD = m_Host.FindAction("ToggleHUD", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Pariah
    private readonly InputActionMap m_Pariah;
    private IPariahActions m_PariahActionsCallbackInterface;
    private readonly InputAction m_Pariah_Movement;
    private readonly InputAction m_Pariah_Look;
    private readonly InputAction m_Pariah_Dash;
    private readonly InputAction m_Pariah_Possession;
    private readonly InputAction m_Pariah_Raise;
    private readonly InputAction m_Pariah_Lower;
    public struct PariahActions
    {
        private @InputMap m_Wrapper;
        public PariahActions(@InputMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Pariah_Movement;
        public InputAction @Look => m_Wrapper.m_Pariah_Look;
        public InputAction @Dash => m_Wrapper.m_Pariah_Dash;
        public InputAction @Possession => m_Wrapper.m_Pariah_Possession;
        public InputAction @Raise => m_Wrapper.m_Pariah_Raise;
        public InputAction @Lower => m_Wrapper.m_Pariah_Lower;
        public InputActionMap Get() { return m_Wrapper.m_Pariah; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PariahActions set) { return set.Get(); }
        public void SetCallbacks(IPariahActions instance)
        {
            if (m_Wrapper.m_PariahActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PariahActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PariahActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PariahActionsCallbackInterface.OnMovement;
                @Look.started -= m_Wrapper.m_PariahActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_PariahActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_PariahActionsCallbackInterface.OnLook;
                @Dash.started -= m_Wrapper.m_PariahActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_PariahActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_PariahActionsCallbackInterface.OnDash;
                @Possession.started -= m_Wrapper.m_PariahActionsCallbackInterface.OnPossession;
                @Possession.performed -= m_Wrapper.m_PariahActionsCallbackInterface.OnPossession;
                @Possession.canceled -= m_Wrapper.m_PariahActionsCallbackInterface.OnPossession;
                @Raise.started -= m_Wrapper.m_PariahActionsCallbackInterface.OnRaise;
                @Raise.performed -= m_Wrapper.m_PariahActionsCallbackInterface.OnRaise;
                @Raise.canceled -= m_Wrapper.m_PariahActionsCallbackInterface.OnRaise;
                @Lower.started -= m_Wrapper.m_PariahActionsCallbackInterface.OnLower;
                @Lower.performed -= m_Wrapper.m_PariahActionsCallbackInterface.OnLower;
                @Lower.canceled -= m_Wrapper.m_PariahActionsCallbackInterface.OnLower;
            }
            m_Wrapper.m_PariahActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
                @Possession.started += instance.OnPossession;
                @Possession.performed += instance.OnPossession;
                @Possession.canceled += instance.OnPossession;
                @Raise.started += instance.OnRaise;
                @Raise.performed += instance.OnRaise;
                @Raise.canceled += instance.OnRaise;
                @Lower.started += instance.OnLower;
                @Lower.performed += instance.OnLower;
                @Lower.canceled += instance.OnLower;
            }
        }
    }
    public PariahActions @Pariah => new PariahActions(this);

    // Host
    private readonly InputActionMap m_Host;
    private IHostActions m_HostActionsCallbackInterface;
    private readonly InputAction m_Host_Movement;
    private readonly InputAction m_Host_Look;
    private readonly InputAction m_Host_Dash;
    private readonly InputAction m_Host_Possession;
    private readonly InputAction m_Host_Melee;
    private readonly InputAction m_Host_ADS;
    private readonly InputAction m_Host_Fire;
    private readonly InputAction m_Host_Jump;
    private readonly InputAction m_Host_Slide;
    private readonly InputAction m_Host_Select1;
    private readonly InputAction m_Host_Select2;
    private readonly InputAction m_Host_Select3;
    private readonly InputAction m_Host_RecoilTest;
    private readonly InputAction m_Host_Ability2;
    private readonly InputAction m_Host_Reload;
    private readonly InputAction m_Host_Ability3;
    private readonly InputAction m_Host_Debug;
    private readonly InputAction m_Host_ToggleHUD;
    public struct HostActions
    {
        private @InputMap m_Wrapper;
        public HostActions(@InputMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Host_Movement;
        public InputAction @Look => m_Wrapper.m_Host_Look;
        public InputAction @Dash => m_Wrapper.m_Host_Dash;
        public InputAction @Possession => m_Wrapper.m_Host_Possession;
        public InputAction @Melee => m_Wrapper.m_Host_Melee;
        public InputAction @ADS => m_Wrapper.m_Host_ADS;
        public InputAction @Fire => m_Wrapper.m_Host_Fire;
        public InputAction @Jump => m_Wrapper.m_Host_Jump;
        public InputAction @Slide => m_Wrapper.m_Host_Slide;
        public InputAction @Select1 => m_Wrapper.m_Host_Select1;
        public InputAction @Select2 => m_Wrapper.m_Host_Select2;
        public InputAction @Select3 => m_Wrapper.m_Host_Select3;
        public InputAction @RecoilTest => m_Wrapper.m_Host_RecoilTest;
        public InputAction @Ability2 => m_Wrapper.m_Host_Ability2;
        public InputAction @Reload => m_Wrapper.m_Host_Reload;
        public InputAction @Ability3 => m_Wrapper.m_Host_Ability3;
        public InputAction @Debug => m_Wrapper.m_Host_Debug;
        public InputAction @ToggleHUD => m_Wrapper.m_Host_ToggleHUD;
        public InputActionMap Get() { return m_Wrapper.m_Host; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(HostActions set) { return set.Get(); }
        public void SetCallbacks(IHostActions instance)
        {
            if (m_Wrapper.m_HostActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_HostActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnMovement;
                @Look.started -= m_Wrapper.m_HostActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnLook;
                @Dash.started -= m_Wrapper.m_HostActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnDash;
                @Possession.started -= m_Wrapper.m_HostActionsCallbackInterface.OnPossession;
                @Possession.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnPossession;
                @Possession.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnPossession;
                @Melee.started -= m_Wrapper.m_HostActionsCallbackInterface.OnMelee;
                @Melee.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnMelee;
                @Melee.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnMelee;
                @ADS.started -= m_Wrapper.m_HostActionsCallbackInterface.OnADS;
                @ADS.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnADS;
                @ADS.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnADS;
                @Fire.started -= m_Wrapper.m_HostActionsCallbackInterface.OnFire;
                @Fire.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnFire;
                @Fire.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnFire;
                @Jump.started -= m_Wrapper.m_HostActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnJump;
                @Slide.started -= m_Wrapper.m_HostActionsCallbackInterface.OnSlide;
                @Slide.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnSlide;
                @Slide.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnSlide;
                @Select1.started -= m_Wrapper.m_HostActionsCallbackInterface.OnSelect1;
                @Select1.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnSelect1;
                @Select1.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnSelect1;
                @Select2.started -= m_Wrapper.m_HostActionsCallbackInterface.OnSelect2;
                @Select2.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnSelect2;
                @Select2.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnSelect2;
                @Select3.started -= m_Wrapper.m_HostActionsCallbackInterface.OnSelect3;
                @Select3.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnSelect3;
                @Select3.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnSelect3;
                @RecoilTest.started -= m_Wrapper.m_HostActionsCallbackInterface.OnRecoilTest;
                @RecoilTest.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnRecoilTest;
                @RecoilTest.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnRecoilTest;
                @Ability2.started -= m_Wrapper.m_HostActionsCallbackInterface.OnAbility2;
                @Ability2.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnAbility2;
                @Ability2.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnAbility2;
                @Reload.started -= m_Wrapper.m_HostActionsCallbackInterface.OnReload;
                @Reload.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnReload;
                @Reload.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnReload;
                @Ability3.started -= m_Wrapper.m_HostActionsCallbackInterface.OnAbility3;
                @Ability3.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnAbility3;
                @Ability3.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnAbility3;
                @Debug.started -= m_Wrapper.m_HostActionsCallbackInterface.OnDebug;
                @Debug.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnDebug;
                @Debug.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnDebug;
                @ToggleHUD.started -= m_Wrapper.m_HostActionsCallbackInterface.OnToggleHUD;
                @ToggleHUD.performed -= m_Wrapper.m_HostActionsCallbackInterface.OnToggleHUD;
                @ToggleHUD.canceled -= m_Wrapper.m_HostActionsCallbackInterface.OnToggleHUD;
            }
            m_Wrapper.m_HostActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
                @Possession.started += instance.OnPossession;
                @Possession.performed += instance.OnPossession;
                @Possession.canceled += instance.OnPossession;
                @Melee.started += instance.OnMelee;
                @Melee.performed += instance.OnMelee;
                @Melee.canceled += instance.OnMelee;
                @ADS.started += instance.OnADS;
                @ADS.performed += instance.OnADS;
                @ADS.canceled += instance.OnADS;
                @Fire.started += instance.OnFire;
                @Fire.performed += instance.OnFire;
                @Fire.canceled += instance.OnFire;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Slide.started += instance.OnSlide;
                @Slide.performed += instance.OnSlide;
                @Slide.canceled += instance.OnSlide;
                @Select1.started += instance.OnSelect1;
                @Select1.performed += instance.OnSelect1;
                @Select1.canceled += instance.OnSelect1;
                @Select2.started += instance.OnSelect2;
                @Select2.performed += instance.OnSelect2;
                @Select2.canceled += instance.OnSelect2;
                @Select3.started += instance.OnSelect3;
                @Select3.performed += instance.OnSelect3;
                @Select3.canceled += instance.OnSelect3;
                @RecoilTest.started += instance.OnRecoilTest;
                @RecoilTest.performed += instance.OnRecoilTest;
                @RecoilTest.canceled += instance.OnRecoilTest;
                @Ability2.started += instance.OnAbility2;
                @Ability2.performed += instance.OnAbility2;
                @Ability2.canceled += instance.OnAbility2;
                @Reload.started += instance.OnReload;
                @Reload.performed += instance.OnReload;
                @Reload.canceled += instance.OnReload;
                @Ability3.started += instance.OnAbility3;
                @Ability3.performed += instance.OnAbility3;
                @Ability3.canceled += instance.OnAbility3;
                @Debug.started += instance.OnDebug;
                @Debug.performed += instance.OnDebug;
                @Debug.canceled += instance.OnDebug;
                @ToggleHUD.started += instance.OnToggleHUD;
                @ToggleHUD.performed += instance.OnToggleHUD;
                @ToggleHUD.canceled += instance.OnToggleHUD;
            }
        }
    }
    public HostActions @Host => new HostActions(this);
    private int m_KBMSchemeIndex = -1;
    public InputControlScheme KBMScheme
    {
        get
        {
            if (m_KBMSchemeIndex == -1) m_KBMSchemeIndex = asset.FindControlSchemeIndex("KBM");
            return asset.controlSchemes[m_KBMSchemeIndex];
        }
    }
    private int m_ControllerSchemeIndex = -1;
    public InputControlScheme ControllerScheme
    {
        get
        {
            if (m_ControllerSchemeIndex == -1) m_ControllerSchemeIndex = asset.FindControlSchemeIndex("Controller");
            return asset.controlSchemes[m_ControllerSchemeIndex];
        }
    }
    public interface IPariahActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnPossession(InputAction.CallbackContext context);
        void OnRaise(InputAction.CallbackContext context);
        void OnLower(InputAction.CallbackContext context);
    }
    public interface IHostActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnPossession(InputAction.CallbackContext context);
        void OnMelee(InputAction.CallbackContext context);
        void OnADS(InputAction.CallbackContext context);
        void OnFire(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnSlide(InputAction.CallbackContext context);
        void OnSelect1(InputAction.CallbackContext context);
        void OnSelect2(InputAction.CallbackContext context);
        void OnSelect3(InputAction.CallbackContext context);
        void OnRecoilTest(InputAction.CallbackContext context);
        void OnAbility2(InputAction.CallbackContext context);
        void OnReload(InputAction.CallbackContext context);
        void OnAbility3(InputAction.CallbackContext context);
        void OnDebug(InputAction.CallbackContext context);
        void OnToggleHUD(InputAction.CallbackContext context);
    }
}
