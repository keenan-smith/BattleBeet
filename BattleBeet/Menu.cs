using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BattleBeet.Hacks
{
    public class Menu : MonoBehaviour
    {
        public static Rect WindowRect = new Rect(20, 20, 200, 600);
        public static Vector2 scrollPos = new Vector2();
        
        private static MethodInfo _Method1;
        private static MethodInfo Override_Method1;

        public void Start()
        { 
            BitNetworking.HostOrConnectOnLocal();
        }

        public void OnGUI()
        {
            GUI.Label(new Rect(20, 20, 200, 200), "BattleBeet");
            var things = FindObjectsOfType(typeof(LoadoutSelectionManager));
            if (things == null) Debug.LogError("things is null!");
            GUI.Label(new Rect(20, 40, 200, 200), things.Length.ToString());
            BitNetworking.Me.BitPlayer.PointCount = 999;
            if (things.Length > 0)
            {
                LoadoutSelectionManager manager = (LoadoutSelectionManager)things[0];

            }
            Camera mainCamera = Camera.main;
            foreach (PlayerNetwork client in RoomManager.OtherPlayers)
            {
                if (client == RoomManager.LocalPlayer)
                    continue;

                Vector3 pos = mainCamera.WorldToScreenPoint(client.transform.position);
                pos.y = Screen.height - pos.y;
                if (pos.z > 0)
                {
                    if (client.Player.PlayersTeam == RoomManager.LocalPlayer.Player.PlayersTeam)
                    {
                        GUI.Label(new Rect(pos.x, pos.y, 200, 200), "<size=12><color=#0000ffff>" + client.Player.Name + "\nHealth: " + client.Health + "\nDistance: " + Mathf.RoundToInt(Utils.GetDistance(client.transform)) + "</color></size>");
                    }
                    else
                    {
                        GUI.Label(new Rect(pos.x, pos.y, 200, 200), "<size=12><color=#ff0000ff>" + client.Player.Name + "\nHealth: " + client.Health + "\nDistance: " + Mathf.RoundToInt(Utils.GetDistance(client.transform)) + "</color></size>");
                    }
                }
            }
            WindowRect = GUI.Window(0, WindowRect, MainWindow, "BattleBeet");
        }

        public void MainWindow(int id)
        {
            //scrollPos = GUILayout.BeginScrollView(scrollPos);

            if (GUI.Button(new Rect(0, 20, 200, 200), "Host"))
            {
                Debug.LogError("Pressed \"Host\"!");
                BitNetworking.Disconnect();
            }
            
            //GUILayout.EndScrollView();

            GUI.DragWindow(new Rect(0, 0, 200, 600));
        }
    }

}
