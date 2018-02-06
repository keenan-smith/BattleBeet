using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Reflection;
using UnityEngine;

namespace BattleBeet.Hacks
{

    public class Menu : MonoBehaviour
    {
        public static Rect WindowRect = new Rect(20, 20, 200, 600);
        public static Vector2 scrollPos = new Vector2();
        public static Camera mainCamera = Camera.main;

        public static KeyCode menukey = KeyCode.Delete;
        public static KeyCode updatecamerakey = KeyCode.End;
        public static KeyCode givepointskey = KeyCode.PageDown;
        public static float movementspeed = 1f;
        public static float jumpheight = 1f;
        public static float fallspeed = 20f;
        public static float gravity = 9.81f;
        public static bool IsInMenu = false;
        public static bool IsBanned = false;

        public void Start()
        {
        }

        public void Update()
        {
            if (Input.GetKeyDown(menukey))
                IsInMenu = !IsInMenu;

        }

        public void OnGUI()
        {

            GUI.Label(new Rect(20, 20, 200, 200), "BattleBeet");


            if (Event.current.type == EventType.repaint)
            {
                try
                {
                    foreach (PlayerNetwork client in RoomManager.OtherPlayers)
                    {

                        if (client == RoomManager.LocalPlayer)
                            continue;

                        Vector3 pos = mainCamera.WorldToScreenPoint(client.transform.position);
                        pos.y = Screen.height - pos.y;
                        if (pos.z > 0)
                        {
                            bool hit = Physics.Linecast(RoomManager.LocalPlayer.transform.position, client.transform.position, out RaycastHit rhit, -1 | 15);
                            if (client.Player.PlayersTeam == RoomManager.LocalPlayer.Player.PlayersTeam)
                            {
                                if (hit)
                                    GUI.Label(new Rect(pos.x, pos.y, 200, 200), "<size=12><color=#0000ffff>" + client.Player.Name + "\nHealth: " + client.Health + "\nDistance: " + Mathf.RoundToInt(Utils.GetDistance(client.transform)) + "\nHit: " + rhit.collider.gameObject.layer + "</color></size>");
                                else
                                    GUI.Label(new Rect(pos.x, pos.y, 200, 200), "<size=12><color=#0000ffff>" + client.Player.Name + "\nHealth: " + client.Health + "\nDistance: " + Mathf.RoundToInt(Utils.GetDistance(client.transform)) + "</color></size>");
                            }
                            else
                            {
                                if (hit)
                                    GUI.Label(new Rect(pos.x, pos.y, 200, 200), "<size=12><color=#ff0000ff>" + client.Player.Name + "\nHealth: " + client.Health + "\nDistance: " + Mathf.RoundToInt(Utils.GetDistance(client.transform)) + "\nHit: " + rhit.collider.gameObject.layer + "</color></size>");
                                else
                                    GUI.Label(new Rect(pos.x, pos.y, 200, 200), "<size=12><color=#ff0000ff>" + client.Player.Name + "\nHealth: " + client.Health + "\nDistance: " + Mathf.RoundToInt(Utils.GetDistance(client.transform)) + "</color></size>");
                            }
                        }
                    }
                }
                catch (Exception e) { Debug.LogException(e); }

                try
                {

                    if (RoomManager.LocalPlayer != null)
                    {
                        FPSManager fps = RoomManager.LocalPlayer.FPManager;
                        fps.RecoilEffect = 0;
                        fps.RecoilFixSpeed = 0;
                        fps.MaxSway = 0;
                        fps.MaxVerticalRecoil = 0;
                        fps.CharacterMotor.movement.maxForwardSpeed = 3f * movementspeed;
                        fps.CharacterMotor.movement.maxSidewaysSpeed = 2f * movementspeed;
                        fps.CharacterMotor.movement.maxBackwardsSpeed = 2f * movementspeed;
                        fps.CharacterMotor.movement.gravity = gravity;
                        fps.CharacterMotor.movement.maxFallSpeed = fallspeed;
                        fps.CharacterMotor.jumping.baseHeight = jumpheight;
                        GUI.Label(new Rect(220, 80, 200, 200), "HP: " + RoomManager.LocalPlayer.Health);
                        GUI.Label(new Rect(220, 100, 200, 200), "X " + fps.MouseLook.X_Value + " | Y " + fps.MouseLook.Y_Value);
                        try
                        {
                            fps.RadialBlur.enabled = false;
                            fps.BloodScreen.enabled = false;
                            fps.BloodScreenVignette.enabled = false;
                            fps.MenuStop.enabled = false;
                        }
                        catch (Exception e) { Debug.LogException(e); }
                        //fps.Breath.Stamina = 100f;
                        Debug.LogError("test1");
                        Transform sway = fps.WeaponSway;
                        if (sway != null)
                        {
                            Debug.LogError("test12");
                            GUI.Label(new Rect(20, 80, 200, 200), "Sway isnt null!");
                            sway.localEulerAngles = new Vector3(0, 0, 0);
                        }
                        else
                        {
                            Debug.LogError("test13");
                            GUI.Label(new Rect(20, 80, 200, 200), "Sway is null!");
                        }

                        Debug.LogError("test14");
                        BetterReflect.SetPrivateFieldValue(fps, "stamina", 100f);
                        BetterReflect.SetPrivateFieldValue(fps, "staminaRecoilEffect", 0f);
                    }
                }
                catch (Exception e) { Debug.LogException(e); }
            }

            try { 
                if (WeaponManager.Instace != null)
                {
                    GUI.Label(new Rect(20, 40, 200, 200), "WM isnt null!");

                    try
                    {
                        WeaponMagazineStore wms = BetterReflect.GetPrivateFieldValue<WeaponMagazineStore>(WeaponManager.Instace, "mag");
                        if (wms != null)
                        {
                            RaundMagazine curmag = BetterReflect.GetPrivateFieldValue<RaundMagazine>(wms, "currentClip");
                            if (curmag != null)
                            {
                                int ammo = curmag.BulletCount;
                                GUI.Label(new Rect(220, 60, 200, 200), "Ammo: " + ammo);
                                try
                                {
                                    ObscuredInt bullets = BetterReflect.GetPrivateFieldValue<ObscuredInt>(curmag, "bulletCount");
                                    bullets = curmag.MaxBulletCount;
                                    BetterReflect.SetPrivateFieldValue<ObscuredInt>(curmag, "bulletCount", bullets);
                                }
                                catch(Exception e) { Debug.LogException(e); }
                            }
                        }
                    }
                    catch (Exception e) { Debug.LogException(e); }
                }
                else
                {
                    GUI.Label(new Rect(20, 40, 200, 200), "WM is null!");
                }
            }
            catch(Exception e) { Debug.LogException(e); }

            if (IsInMenu)
            {
                try
                {
                    WindowRect = GUI.Window(0, WindowRect, MainWindow, "BattleBeet");
                }
                catch (Exception e) { Debug.LogException(e); }
            }
        }

        public void MainWindow(int id)
        {
            mainCamera = Camera.main;
            //scrollPos = GUILayout.BeginScrollView(scrollPos);
            if (GUILayout.Button("Set Points", GUILayout.MaxHeight(25)))
            {
                try
                {
                    var things = FindObjectsOfType(typeof(LoadoutSelectionManager));
                    if (things == null) Debug.LogError("things is null!");
                    GUI.Label(new Rect(20, 40, 200, 200), things.Length.ToString());
                    BitNetworking.Me.BitPlayer.PointCount = 10;

                    if (things.Length > 0)
                    {
                        LoadoutSelectionManager manager = (LoadoutSelectionManager)things[0];

                    }
                }
                catch { }
            }

            if (GUILayout.Button($"{movementspeed} | {jumpheight} | {fallspeed} | {gravity}"))
            {
                movementspeed = 1;
                jumpheight = 1;
                fallspeed = 20;
                gravity = 9.81f;
            }

            movementspeed = GUILayout.HorizontalSlider(movementspeed, 0, 30);
            jumpheight = GUILayout.HorizontalSlider(jumpheight, 1, 150);
            fallspeed = GUILayout.HorizontalSlider(fallspeed, 20, 0);
            gravity = GUILayout.HorizontalSlider(gravity, 0, 30);

            //GUILayout.EndScrollView();

            GUI.DragWindow(new Rect(0, 0, 200, 600));
        }
    }

}
