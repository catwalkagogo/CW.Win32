using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CW.Win32 {
	public static class WindowUtils {
		public static IEnumerable<IntPtr> OrderByZOrder(this IEnumerable<IntPtr> windows){
			var hash = new HashSet<IntPtr>(windows);

			for(IntPtr hWnd = User32.GetTopWindow(IntPtr.Zero); hWnd != IntPtr.Zero; hWnd = User32.GetNextWindow(hWnd, GW_HWNDNEXT)) {
				if(hash.Contains(hWnd)){
					yield return hWnd;
				}
			}
		}

		private const uint GW_HWNDNEXT = 2;

		public static IEnumerable<IntPtr> GetWindows(){
			var list = new List<IntPtr>();
			User32.EnumWindows(GetWindowsProc, list);
			return list;
		}

		private static bool GetWindowsProc(IntPtr hwnd, object o){
			var list = (List<IntPtr>)o;
			list.Add(hwnd);
			return true;
		}

		public static IntPtr GetWindow(IntPtr hwnd, GetWindowOption option){
			return User32.GetWindow(hwnd, option);
		}

		public static void Show(IntPtr hwnd, ShowWindowCommand cmd){
			User32.ShowWindow(hwnd, cmd);
		}

		public static void SetForeground(IntPtr hwnd){
			User32.SetForegroundWindow(hwnd);
		}

		public static IntPtr GetForeground(){
			return User32.GetForegroundWindow();
		}

		public static void Activate(IntPtr hwnd){
			User32.SetActiveWindow(hwnd);
		}

		public static IntPtr GetActive(){
			return User32.GetActiveWindow();
		}
	}
}
