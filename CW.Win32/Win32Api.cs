/*
	$Id: Win32Api.cs 316 2013-12-26 10:16:12Z catwalkagogo@gmail.com $
*/

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace CW.Win32 {
	/*
	/// <summary>
	/// Win32APIを扱う静的クラス。
	/// </summary>
	public static partial class Win32Api {
		#region Shell
		[DllImport("shell32.dll", EntryPoint = "SHEmptyRecycleBin", CharSet = CharSet.Auto)]
		public static extern OLEError SHEmptyRecycleBin(IntPtr hwnd, string drive, SHEmptyRecycleBinOptions options);

		[DllImport("shell32.dll", EntryPoint = "ExtractIconEx", CharSet = CharSet.Auto)]
		public static extern int ExtractIconEx([MarshalAs(UnmanagedType.LPTStr)] string file, int index, out IntPtr largeIconHandle, out IntPtr smallIconHandle, int icons);

		[DllImport("shell32.dll", EntryPoint = "SHGetFileInfo", CharSet = CharSet.Auto)]
		public static extern IntPtr SHGetFileInfo(string pszPath, FileAttributes attr, ref SHFileInfo psfi, int cbSizeFileInfo, SHGetFileInfoOptions uFlags);

		[DllImport("shell32.dll", EntryPoint = "#727")]
		public extern static int SHGetImageList(ImageListSize iImageList, ref Guid riid, out IImageList ppv);

		[DllImport("shell32.dll", EntryPoint = "ShellExecuteEx", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShellExecuteEx(ref ShellExecuteInfo shinfo);

		[DllImport("shell32.dll", EntryPoint = "SHGetDesktopFolder", CharSet = CharSet.Auto)]
		public static extern OLEError SHGetDesktopFolder(out IntPtr ppshf);

		public static readonly Guid IID_IShellFolder = new Guid("{000214E6-0000-0000-C000-000000000046}");

		public static IShellFolder BindToObject(IShellFolder iShellFolder, IntPtr ppidl) {
			IntPtr ptrRet;
			iShellFolder.BindToObject(ppidl, IntPtr.Zero, IID_IShellFolder, out ptrRet);
			if(ptrRet != IntPtr.Zero) {
				return (IShellFolder)Marshal.GetTypedObjectForIUnknown(ptrRet, typeof(IShellFolder));
			} else {
				throw new Win32Exception();
			}
		}

		public static IShellFolder GetDesktopFolder() {
			IntPtr ptrRet;
			SHGetDesktopFolder(out ptrRet);
			if(ptrRet != IntPtr.Zero) {
				return (IShellFolder)Marshal.GetTypedObjectForIUnknown(ptrRet, typeof(IShellFolder));
			} else {
				throw new Win32Exception();
			}
		}

		[DllImport("shell32.dll", EntryPoint = "SHGetSpecialFolderLocation", CharSet = CharSet.Auto)]
		public static extern uint SHGetSpecialFolderLocation(IntPtr hwnd, int CSIDL, out IntPtr pidl);

		public static IContextMenu GetIContextMenu(IntPtr hwnd, string[] files, out IShellFolder desktopFolder, out IShellFolder parentFolder) {
			desktopFolder = null;
			parentFolder = null;
			IContextMenu iContextMenu = null;
			IntPtr ppidl;
			IntPtr[] apidl = new IntPtr[files.Length];
			ulong pchEaten = 0, pdwAttributes = 0;

			string parentDir = System.IO.Path.GetDirectoryName(files[0]);

			desktopFolder = GetDesktopFolder();
			if(String.IsNullOrEmpty(parentDir)) {
				SHGetSpecialFolderLocation(hwnd, 0x0011, out ppidl);
			} else {
				desktopFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, parentDir, ref pchEaten, out ppidl, ref pdwAttributes);
			}
			parentFolder = BindToObject(desktopFolder, ppidl);
			ReleaseItemIdList(ppidl);

			int i = 0;
			foreach(string path in files) {
				string name = System.IO.Path.GetFileName(path);
				if(String.IsNullOrEmpty(name)) {
					name = path;
				}
				parentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, name, ref pchEaten, out apidl[i], ref pdwAttributes);
				i++;
			}
			iContextMenu = GetUIObjectOf(parentFolder, apidl, hwnd);
			ReleaseItemIdList(apidl);
			return iContextMenu;
		}

		public static IContextMenu GetUIObjectOf(IShellFolder iShellFolder, IntPtr[] apidl, IntPtr hwnd) {
			uint rgfReserved = 0;
			IntPtr ptrRet;
			iShellFolder.GetUIObjectOf(hwnd, (uint)apidl.Length, apidl, IID_IContextMenu, ref rgfReserved, out ptrRet);
			if(ptrRet != IntPtr.Zero) {
				return (IContextMenu)Marshal.GetTypedObjectForIUnknown(ptrRet, typeof(IContextMenu));
			} else {
				throw new Win32Exception();
			}
		}

		public static readonly Guid IID_IContextMenu = new Guid("{000214e4-0000-0000-c000-000000000046}");

		public static void ReleaseItemIdList(params IntPtr[] apidl) {
			foreach(IntPtr ppidl in apidl) {
				Marshal.FreeCoTaskMem(ppidl);
			}
		}
		#endregion

		[DllImport("user32.dll", EntryPoint = "SetWindowLong")]
		public extern static IntPtr SetWindowLongPtr(IntPtr hwnd, int nIndex, IntPtr newValue);

		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
		public extern static IntPtr SetWindowLongPtr64(IntPtr hwnd, int nIndex, IntPtr newValue);

		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(String lpFileName);
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetProcAddress(IntPtr hModule, String lpProcName);
		[DllImport("kernel32.dll")]
		public static extern Boolean FreeLibrary(IntPtr hLibModule);

		[DllImport("KERNEL32.DLL", EntryPoint = "CreateFileMapping", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpAttributes, CreateFileMappingOptions flProtect, [MarshalAs(UnmanagedType.U4)] int dwMaximumSizeHigh, [MarshalAs(UnmanagedType.U4)] int dwMaximumSizeLow, string lpName);

		[DllImport("KERNEL32.DLL", EntryPoint = "MapViewOfFile", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, FileMapAccessMode dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, IntPtr dwNumberOfBytesToMap);

		[DllImport("KERNEL32.DLL", EntryPoint = "CloseHandle", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool CloseHandle(IntPtr handle);

		[DllImport("KERNEL32.DLL", EntryPoint = "UnmapViewOfFile", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool UnmapViewOfFile(IntPtr hMap);

		[DllImport("KERNEL32.DLL", EntryPoint = "GetLastError", CharSet = CharSet.Auto)]
		public static extern int GetLastError();

		[DllImport("KERNEL32.DLL", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Auto)]
		public static extern void CopyMemory(IntPtr dst, IntPtr src, IntPtr length);

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

		/// <summary>
		/// IMEのコンテキストを取得する。
		/// </summary>
		/// <param name="hwnd">hwnd</param>
		/// <returns>コンテキストのポインタ</returns>
		[DllImport("imm32.dll", EntryPoint = "ImmGetContext", CharSet = CharSet.Auto)]
		public static extern IntPtr ImmGetContext(IntPtr hwnd);

		/// <summary>
		/// IMEの状態を変更する。
		/// </summary>
		/// <param name="hIMC">IMEのコンテキストのポインタ</param>
		/// <param name="fOpen">IMEを開くかどうか</param>
		/// <returns>bool</returns>
		[DllImport("imm32.dll", EntryPoint = "ImmSetOpenStatus", CharSet = CharSet.Auto)]
		public static extern bool ImmSetOpenStatus(IntPtr hIMC, bool fOpen);

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

		/// <summary>
		/// 仮想キーコードとキーの状態から入力された文字(ユニコード文字列)を取得します。
		/// </summary>
		/// <seealso cref="Win32Api.GetKeyboardState()">Win32.GetKeyboardState</seealso>
		public static string KeyToUnicode(uint virtualKey, byte[] keyState) {
			StringBuilder sb = new StringBuilder(16);
			int result = ToUnicode(virtualKey, 0, keyState, sb, sb.Capacity, 0);
			if(result > 0) {
				return sb.ToString();
			} else {
				return null;
			}
		}

		[DllImport("user32.dll", EntryPoint = "GetKeyboardState", CharSet = CharSet.Auto)]
		internal static extern int GetKeyboardState(byte[] lpKeyState);

		public static byte[] GetKeyboardState() {
			byte[] keyState = new byte[256];
			int result = GetKeyboardState(keyState);
			if(result == 0) {
				throw new Win32Exception();
			}
			return keyState;
		}

		[DllImport("user32.dll", EntryPoint = "MessageBeep", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool MessageBeep(MessageBeep messageBeep);

		[DllImport("shlwapi.dll", EntryPoint = "PathMatchSpec", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PathMatchSpec([MarshalAs(UnmanagedType.LPTStr)] string path, [MarshalAs(UnmanagedType.LPTStr)] string spec);

		[DllImport("user32.dll", EntryPoint = "CreatePopupMenu", CharSet = CharSet.Auto)]
		public static extern IntPtr CreatePopupMenu();

		[DllImport("user32.dll", EntryPoint = "TrackPopupMenu", CharSet = CharSet.Auto)]
		public static extern int TrackPopupMenu(IntPtr hMenu, TrackPopupMenuOptions wFlags, int x, int y, int nReserved, IntPtr hwnd, int lprc);

		[DllImport("user32.dll", EntryPoint = "DestroyMenu", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DestroyMenu(IntPtr hMenu);

		[DllImport("user32.dll", EntryPoint = "GetMenuDefaultItem", CharSet = CharSet.Auto)]
		public static extern int GetMenuDefaultItem(IntPtr hMenu, MenuFoundBy byPos, GetMenuDefaultItemOptions options);

		[DllImport("user32.dll", EntryPoint = "GetMenuItemCount", CharSet = CharSet.Auto)]
		public static extern int GetMenuItemCount(IntPtr hMenu);

		[DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetProcessWorkingSetSize(IntPtr procHandle, IntPtr min, IntPtr max);

		[DllImport("user32.dll", EntryPoint = "GetDesktopWindow", CharSet = CharSet.Auto)]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll", EntryPoint = "GetDC", CharSet = CharSet.Auto)]
		public static extern IntPtr GetDeviceContext(IntPtr hWnd);

		[DllImport("user32.dll", EntryPoint = "ReleaseDC", CharSet = CharSet.Auto)]
		public static extern IntPtr ReleaseDeviceContext(IntPtr hWnd, IntPtr hDc);

		[DllImport("gdi32.dll", EntryPoint = "BitBlt", CharSet = CharSet.Auto)]
		public static extern bool BitBlt(IntPtr hdcDst, int xDst, int yDsk, int width, int height, IntPtr hdcSrc, int xSrc, int ySrc, int rasterOp);
		[DllImport("USER32.DLL", EntryPoint = "DestroyIcon", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DestroyIcon(IntPtr hIcon);

		[DllImport("kernel32", EntryPoint = "GetLogicalDrives", CharSet = CharSet.Auto)]
		internal static extern long InternalGetLogicalDrives();

		[Obsolete]
		public static string[] GetLogicalDrives() {
			List<string> drives = new List<string>();
			long d = Win32Api.InternalGetLogicalDrives();
			for(int i = 0; i < 26; i++) {
				if(((1 << i) & d) > 0) {
					drives.Add(((char)('A' + i)) + ":\\");
				}
			}
			return drives.ToArray();
		}

		[DllImport("kernel32", EntryPoint = "GetVolumeInformation", CharSet = CharSet.Auto)]
		public static extern bool GetVolumeInformation(string drivePath, StringBuilder volumeNameBuffer, int volumeNameBufferSize, int volumeSerialNumber, int maximumComponentLength, int fileSystemFlags, StringBuilder fileSystemNameBuffer, int fileSystemNameSize);

		[Obsolete]
		public static bool GetVolumeInformation(string drivePath, out string volumeLabel, out string fileSystemName) {
			StringBuilder volumeNameBuffer = new StringBuilder(64);
			StringBuilder fileSystemNameBuffer = new StringBuilder(64);
			bool result = GetVolumeInformation(drivePath, volumeNameBuffer, volumeNameBuffer.Capacity, 0, 260, 0, fileSystemNameBuffer, fileSystemNameBuffer.Capacity);
			volumeLabel = volumeNameBuffer.ToString();
			fileSystemName = fileSystemNameBuffer.ToString();
			return result;
		}

		[DllImport("kernel32", EntryPoint = "GetShortPathName", CharSet = CharSet.Auto)]
		public static extern int GetShortPathName(string path, StringBuilder shortName, int bufferSize);

		[DllImport("kernel32", EntryPoint = "GetLongPathName", CharSet = CharSet.Ansi)]
		public static extern int GetLongPathName(string path, StringBuilder longName, int bufferSize);

		[DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
		public extern static Int32 SetWindowTheme(IntPtr hwnd, String subAppName, String subIdList);

		[DllImport("winmm.dll", CharSet = CharSet.Auto, EntryPoint = "mciSendString")]
		public extern static int MciSendString(string command, StringBuilder returnString, int bufferLength, IntPtr hwndCallback);

		[DllImport("kernel32.dll", EntryPoint = "CreateSymbolicLinkW", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool CreateSymbolicLink([In] string lpSymlinkFileName, [In] string lpTargetFileName, SymbolicLinkKind dwFlags);

		[DllImport("user32.dll", EntryPoint = "ClientToScreen", CallingConvention = CallingConvention.StdCall)]
		public extern static bool ClientToScreen(IntPtr hWnd, ref Point pt);
	}*/
}