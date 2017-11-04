using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace BattleBeet.Loading
{
    public class Loading : MonoBehaviour
    {
        public static Thread thr;

        public static GameObject hookObj = null;

        public static Hacks.Menu mainMenu = null;

        private static void hook()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(2000);
                    if (hookObj == null)
                    {
                        hookObj = new GameObject();
                        mainMenu = hookObj.AddComponent<Hacks.Menu>();
                        //hookObj.AddComponent<Hacks.lib_Console>();
                        DontDestroyOnLoad(hookObj);
                    }
                    Thread.Sleep(5000);
                }
            }
            catch (Exception ex) { UnityEngine.Debug.LogException(ex); }
        }

        public static void thread()
        {
            try
            {
                thr = new Thread(new ThreadStart(hook));
                thr.Start();
            }
            catch (Exception x) { Debug.Log("ERROR START\n" + x + "\nERROR END"); }
        }
    }
}
