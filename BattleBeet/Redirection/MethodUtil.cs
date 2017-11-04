﻿using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace i.Redirection
{
    public static class RedirectionHelper
    {
        [DllImport("mono.dll", CallingConvention = CallingConvention.FastCall, EntryPoint = "mono_domain_get")]
        private static extern IntPtr mono_domain_get();

        [DllImport("mono.dll", CallingConvention = CallingConvention.FastCall, EntryPoint = "mono_method_get_header")]
        private static extern IntPtr mono_method_get_header(IntPtr method);

        public static void RedirectCalls(MethodInfo from, MethodInfo to)
        {
            var fptr1 = from.MethodHandle.GetFunctionPointer();
            var fptr2 = to.MethodHandle.GetFunctionPointer();
            Debug.Log("Patching from " + from.Name + " to " + to.Name);
            PatchJumpTo(fptr1, fptr2);
        }
        public static void RedirectCalls(ConstructorInfo from, ConstructorInfo to)
        {
            var fptr1 = from.MethodHandle.GetFunctionPointer();
            var fptr2 = to.MethodHandle.GetFunctionPointer();
            Debug.Log("Patching from " + from.Name + " to " + to.Name);
            PatchJumpTo(fptr1, fptr2);
        }
        private static void RedirectCall(MethodInfo from, MethodInfo to)
        {
            IntPtr methodPtr1 = from.MethodHandle.Value;
            IntPtr methodPtr2 = to.MethodHandle.Value;
            from.MethodHandle.GetFunctionPointer();
            to.MethodHandle.GetFunctionPointer();

            IntPtr domain = mono_domain_get();
            unsafe
            {
                byte* jitCodeHash = ((byte*)domain.ToPointer() + 0xE8);
                long** jitCodeHashTable = *(long***)(jitCodeHash + 0x20);
                uint tableSize = *(uint*)(jitCodeHash + 0x18);

                void* jitInfoFrom = null, jitInfoTo = null;

                long mptr1 = methodPtr1.ToInt64();
                uint index1 = ((uint)mptr1) >> 3;
                for (long* value = jitCodeHashTable[index1 % tableSize];
                    value != null;
                    value = *(long**)(value + 1))
                {
                    if (mptr1 == *value)
                    {
                        jitInfoFrom = value;
                        break;
                    }
                }

                long mptr2 = methodPtr2.ToInt64();
                uint index2 = ((uint)mptr2) >> 3;
                for (long* value = jitCodeHashTable[index2 % tableSize];
                    value != null;
                    value = *(long**)(value + 1))
                {
                    if (mptr2 == *value)
                    {
                        jitInfoTo = value;
                        break;
                    }
                }
                if (jitInfoFrom == null || jitInfoTo == null)
                {
                    Debug.Log("Could not find methods");
                    return;
                }

                ulong* fromPtr, toPtr;
                fromPtr = (ulong*)jitInfoFrom;
                toPtr = (ulong*)jitInfoTo;
                *(fromPtr + 2) = *(toPtr + 2);
                *(fromPtr + 3) = *(toPtr + 3);
            }
        }

        private static void PatchJumpTo(IntPtr site, IntPtr target)
        {
            unsafe
            {
                byte* sitePtr = (byte*)site.ToPointer();
                *sitePtr = 0x49; // mov r11, target
                *(sitePtr + 1) = 0xBB;
                *((ulong*)(sitePtr + 2)) = (ulong)target.ToInt64();
                *(sitePtr + 10) = 0x41; // jmp r11
                *(sitePtr + 11) = 0xFF;
                *(sitePtr + 12) = 0xE3;
            }
        }

        private static void RedirectCallIL(MethodInfo from, MethodInfo to)
        {
            IntPtr methodPtr1 = from.MethodHandle.Value;
            IntPtr methodPtr2 = to.MethodHandle.Value;

            mono_method_get_header(methodPtr2);

            unsafe
            {
                byte* monoMethod1 = (byte*)methodPtr1.ToPointer();
                byte* monoMethod2 = (byte*)methodPtr2.ToPointer();
                *((IntPtr*)(monoMethod1 + 40)) = *((IntPtr*)(monoMethod2 + 40));
            }
        }
    }

    public static class MethodUtil
    {
        [DllImport("mono.dll", CallingConvention = CallingConvention.FastCall, EntryPoint = "mono_domain_get")]
        private static extern IntPtr mono_domain_get();

        [DllImport("mono.dll", CallingConvention = CallingConvention.FastCall, EntryPoint = "mono_method_get_header")]
        private static extern IntPtr mono_method_get_header(IntPtr method);

        public static void RedirectCalls(MethodInfo from, MethodInfo to)
        {
            IntPtr fptr1 = from.MethodHandle.GetFunctionPointer();
            IntPtr fptr2 = to.MethodHandle.GetFunctionPointer();
            UnityEngine.Debug.Log(string.Format("Patching {0} to {1}", from.Name, to.Name));
            PatchJumpTo(fptr1, fptr2);
        }

        public static void RedirectCalls(ConstructorInfo from, ConstructorInfo to)
        {
            IntPtr fptr1 = from.MethodHandle.GetFunctionPointer();
            IntPtr fptr2 = to.MethodHandle.GetFunctionPointer();

            PatchJumpTo(fptr1, fptr2);
        }

        private static void PatchJumpTo(IntPtr site, IntPtr target)
        {
            unsafe
            {
                byte* sitePtr = (byte*)site.ToPointer();
                *sitePtr = 0x49; // mov r11, target
                *(sitePtr + 1) = 0xBB;
                *((ulong*)(sitePtr + 2)) = (ulong)target.ToInt64();
                *(sitePtr + 10) = 0x41; // jmp r11
                *(sitePtr + 11) = 0xFF;
                *(sitePtr + 12) = 0xE3;
            }
        }
    }
}