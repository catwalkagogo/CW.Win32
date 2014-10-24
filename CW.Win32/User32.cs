using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CW.Win32 {
	public static partial class User32 {
		[DllImport("User32")]
		public static extern IntPtr GetTopWindow(IntPtr hWnd);

		[DllImport("User32", EntryPoint = "GetWindow")]
		public static extern IntPtr GetNextWindow(IntPtr hWnd, uint wCmd);

		public delegate bool EnumWindowsProc(IntPtr hWnd, object lParam);

		[DllImport("user32", EntryPoint = "EnumWindows", CharSet = CharSet.Auto)]
		public static extern int EnumWindows(EnumWindowsProc lpEnumFunc, object lParam);

		[DllImport("user32", EntryPoint = "GetWindowThreadProcessId", CharSet = CharSet.Auto)]
		public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int ProcessId);

		[DllImport("user32", EntryPoint = "GetWindow", CharSet = CharSet.Auto)]
		public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowOption option);

		[DllImport("user32", EntryPoint = "GetWindowText", CharSet = CharSet.Auto)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32", EntryPoint = "SetWindowText", CharSet = CharSet.Auto)]
		public static extern bool SetWindowText(IntPtr hWnd, [MarshalAs(UnmanagedType.LPTStr)] string text);

		[DllImport("user32", EntryPoint = "SetWindowPos", CharSet = CharSet.Auto)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hwndAfter, int x, int y, int width, int height, SetWindowPosOptions options);
		public static readonly IntPtr HWND_TOP = IntPtr.Zero;
		public static readonly IntPtr HWND_BOTTOM = (IntPtr)1;
		public static readonly IntPtr HWND_TOPMOST = (IntPtr)(-1);
		public static readonly IntPtr HWND_NOTOPMOST = (IntPtr)(-2);

		[DllImport("user32", EntryPoint = "GetWindowRect", CharSet = CharSet.Auto)]
		public static extern int GetWindowRect(IntPtr hWnd, ref Rectangle rect);

		[DllImport("user32", EntryPoint = "IsWindow", CharSet = CharSet.Auto)]
		public static extern bool IsWindow(IntPtr hWnd);

		[DllImport("user32", EntryPoint = "GetWindowPlacement", CharSet = CharSet.Auto)]
		public static extern bool GetWindowPlacement(IntPtr Handle, ref WindowPlacement placement);

		[DllImport("user32", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
		public static extern uint GetWindowLong(IntPtr Handle, GetWindowLongOption option);

		[DllImport("user32", EntryPoint = "IsWindowVisible", CharSet = CharSet.Auto)]
		public static extern bool IsWindowVisible(IntPtr Handle);

		[DllImport("user32", EntryPoint = "IsWindowEnabled", CharSet = CharSet.Auto)]
		public static extern bool IsWindowEnabled(IntPtr Handle);

		[DllImport("user32", EntryPoint = "EnableWindow", CharSet = CharSet.Auto)]
		public static extern bool IsWindowEnabled(IntPtr Handle, bool enable);

		[DllImport("user32.dll", EntryPoint = "ToUnicode", CharSet = CharSet.Auto)]
		public static extern int ToUnicode(uint wVirtKey, uint wScanCode, byte[] lpKeyState, StringBuilder pwszBuff, int cchBuff, uint wFlags);

		[DllImport("USER32.DLL", EntryPoint = "GetActiveWindow", CharSet = CharSet.Auto)]
		public static extern IntPtr GetActiveWindow();

		[DllImport("USER32.DLL", EntryPoint = "SetActiveWindow", CharSet = CharSet.Auto)]
		public static extern IntPtr SetActiveWindow(IntPtr hwnd);

		[DllImport("USER32.DLL", EntryPoint = "GetClassLong", CharSet = CharSet.Auto)]
		public static extern IntPtr GetClassLong(IntPtr hwnd, GetClassLongOption nIndex);

		/// <summary>
		/// ウインドウを表示する。
		/// </summary>
		/// <param name="hwnd">表示するウインドウのハンドル</param>
		/// <param name="cmdShow">ウインドウの状態</param>
		/// <returns>int</returns>
		[DllImport("USER32.DLL", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
		public static extern int ShowWindow(IntPtr hwnd, ShowWindowCommand cmdShow);

		/// <summary>
		/// ウインドウを前面に表示する。
		/// </summary>
		/// <param name="hWnd">前面い表示するウインドウのハンドル</param>
		/// <returns>bool</returns>
		[DllImport("USER32.DLL", EntryPoint = "SetForegroundWindow", CharSet = CharSet.Auto)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("USER32.DLL", EntryPoint = "GetForegroundWindow", CharSet = CharSet.Auto)]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("USER32.DLL", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hwnd, WindowMessage msg, int wParam, int lParam);

		[DllImport("USER32.DLL", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hwnd, int msg, int wParam, int lParam);

		[DllImport("USER32.DLL", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hwnd, int msg, int wParam, ref HeaderItem lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childAfter, string className, string windowName);

		[DllImport("user32.dll", EntryPoint = "FindWindow")]
		public static extern IntPtr FindWindowWin32(string className, string windowName);

		[DllImport("user32.dll")]
		public static extern int PostMessage(IntPtr window, int message, int wparam, int lparam);

		[DllImport("user32.dll")]
		public static extern int PostMessage(IntPtr window, WindowMessage message, int wparam, int lparam);

		[DllImport("user32.dll")]
		public static extern bool BringWindowToTop(IntPtr window);


		[DllImport("user32.dll")]
		public static extern IntPtr GetParent(IntPtr window);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll")]
		public static extern IntPtr GetLastActivePopup(IntPtr window);

		[DllImport("user32.dll")]
		public static extern int GetWindowTextLength(IntPtr window);

		[DllImport("user32.dll")]
		public static extern bool EnumChildWindows(
			IntPtr window, EnumWindowsProc callback, object o);

		[DllImport("user32.dll")]
		public static extern bool EnumThreadWindows(
			int threadId, EnumWindowsProc callback, object o);

		[DllImport("user32.dll")]
		public static extern int GetWindowThreadProcessId(IntPtr window, IntPtr ptr);

		[DllImport("user32.dll")]
		public static extern bool IsChild(IntPtr parent, IntPtr window);

		[DllImport("user32.dll")]
		public static extern bool IsIconic(IntPtr window);

		[DllImport("user32.dll")]
		public static extern bool IsZoomed(IntPtr window);

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr hwnd);

		[DllImport("user32.dll")]
		public static extern int ReleaseDC(IntPtr hwnd, IntPtr dc);
	}
}
