using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LanguageTools.Word {
    public class WinHook {
        public enum HookType : int {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_SHELL = 10,
            WH_DEBUG = 9,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14,
            WH_MSGFILTER = -1
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SetWindowsHookEx(int idHook, HookProcInternal lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate int HookProcInternal(int nCode, IntPtr wParam, IntPtr lParam);
        public delegate void CallbackFunc(int nCode, IntPtr wParam, IntPtr lParam);

        private HookType type;
        private int handle = 0;
        private CallbackFunc callback;

        public WinHook(HookType type, CallbackFunc callback) {
            this.type = type;
            this.callback = callback;
        }

        ~WinHook() {
            ClearHook();
        }

        public void InstallHook(IntPtr hMod, int threadId) {
            handle = SetWindowsHookEx((int)type, CallbackWrapper, hMod, threadId);
            if(handle == 0) {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public void ClearHook() {
            if(handle != 0) {
                bool success = UnhookWindowsHookEx(handle);
                if(!success) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            handle = 0;
        }

        private int CallbackWrapper(int nCode, IntPtr wParam, IntPtr lParam) {
            if(nCode >= 0) {
                callback(nCode, wParam, lParam);
            }
            return CallNextHookEx(handle, nCode, wParam, lParam);
        }
    }
}