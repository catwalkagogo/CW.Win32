/*
	$Id: FileOperation.cs 287 2011-08-10 10:49:24Z catwalkagogo@gmail.com $
*/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Text;

namespace CW.Win32.Shell {
	using IO = System.IO;
	using Path = System.IO.Path;
	using MemoryStream = System.IO.MemoryStream;
	
	public static class FileOperations{
		[DllImport("Shell32.dll", EntryPoint = "SHFileOperation", CharSet = CharSet.Auto)]
		private static extern int SHFileOperation(ref SHFileOperationStruct lpFileOp);
		
		[StructLayout(LayoutKind.Sequential)]
		internal struct SHFileOperationStruct{
			public IntPtr Handle;
			public FileOperationFunc Func;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string From;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string To;
			public FileOperationOptions Options;
			public bool AnyOperationsAborted;
			public IntPtr NameMappings;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string ProgressTitle;
		}
		
		#region SHFileOperation
		
		private struct SHFileOperationArgs{
			public IntPtr Handle{get; set;}
			public FileOperationFunc Func{get; set;}
			public FileOperationOptions Options{get; set;}
			public string[] From{get; set;}
			public string[] To{get; set;}
			public string ProgressTitle{get; set;}
		}
		
		private static void SHFileOperation(SHFileOperationArgs args){
			SHFileOperationStruct sh = new SHFileOperationStruct();
			sh.Handle = args.Handle;
			sh.Func = args.Func;
			sh.From = String.Join("\0", args.From) + '\0';
			sh.To = String.Join("\0", args.To) + '\0';;
			sh.Options = args.Options;
			sh.AnyOperationsAborted = false;
			sh.NameMappings = IntPtr.Zero;
			sh.ProgressTitle = args.ProgressTitle;

			int errorCode = SHFileOperation(ref sh);
			if(errorCode != 0){
				throw new Win32Exception(errorCode);
			}
			if(sh.AnyOperationsAborted){
				throw new OperationCanceledException();
			}
		}
		
		#endregion
		
		#region 同期処理
		
		public static void Delete(string[] files){
			Delete(files, FileOperationOptions.None, IntPtr.Zero, null);
		}
		
		public static void Delete(string[] files, FileOperationOptions options){
			Delete(files, options, IntPtr.Zero, null);
		}
		
		public static void Delete(string[] files, FileOperationOptions options, IntPtr hwnd){
			Delete(files, options, hwnd, null);
		}
		
		public static void Delete(string[] files, FileOperationOptions options, IntPtr hwnd, string progressTitle){
			if(files == null){
				throw new ArgumentNullException();
			}
			if(files.Length == 0){
				throw new ArgumentException();
			}
			SHFileOperationArgs args = new SHFileOperationArgs();
			args.Handle = hwnd;
			args.Func = FileOperationFunc.Delete;
			args.Options = options;
			args.From = files;
			args.To = null;
			args.ProgressTitle = progressTitle;
			SHFileOperation(args);
		}
		
		public static void Move(string to, params string[] files){
			Move(files, to, FileOperationOptions.None, IntPtr.Zero, null);
		}
		
		public static void Move(string[] files, string to, FileOperationOptions options){
			Move(files, to, options, IntPtr.Zero, null);
		}
		
		public static void Move(string[] files, string to, FileOperationOptions options, IntPtr hwnd){
			Move(files, to, options, hwnd, null);
		}
		
		public static void Move(string[] files, string to, FileOperationOptions options, IntPtr hwnd, string progressTitle){
			if(files == null){
				throw new ArgumentNullException();
			}
			if(files.Length == 0){
				throw new ArgumentException();
			}
			SHFileOperationArgs args = new SHFileOperationArgs();
			args.Handle = hwnd;
			args.Func = FileOperationFunc.Move;
			args.Options = options;
			args.From = files;
			args.To = new []{to};
			args.ProgressTitle = progressTitle;
			SHFileOperation(args);
		}
		
		public static void Copy(string to, params string[] files){
			Copy(files, to, FileOperationOptions.None, IntPtr.Zero, null);
		}
		
		public static void Copy(string[] files, string to, FileOperationOptions options){
			Copy(files, to, options, IntPtr.Zero, null);
		}
		
		public static void Copy(string[] files, string to, FileOperationOptions options, IntPtr hwnd){
			Copy(files, to, options, hwnd, null);
		}
		
		public static void Copy(string[] files, string to, FileOperationOptions options, IntPtr hwnd, string progressTitle){
			if(files == null){
				throw new ArgumentNullException();
			}
			if(files.Length == 0){
				throw new ArgumentException();
			}
			SHFileOperationArgs args = new SHFileOperationArgs();
			args.Handle = hwnd;
			args.Func = FileOperationFunc.Copy;
			args.Options = options;
			args.From = files;
			args.To = new[]{to};
			args.ProgressTitle = progressTitle;
			SHFileOperation(args);
		}
		
		public static void Rename(string from, string to){
			Rename(from, to, FileOperationOptions.None, IntPtr.Zero, null);
		}
		
		public static void Rename(string from, string to, FileOperationOptions options){
			Rename(from, to, options, IntPtr.Zero, null);
		}
		
		public static void Rename(string from, string to, FileOperationOptions options, IntPtr hwnd){
			Rename(from, to, options, hwnd, null);
		}
		
		public static void Rename(string from, string to, FileOperationOptions options, IntPtr hwnd, string progressTitle){
			if((from == null) || (to == null)){
				throw new ArgumentNullException();
			}
			SHFileOperationArgs args = new SHFileOperationArgs();
			args.Handle = hwnd;
			args.Func = FileOperationFunc.Rename;
			args.Options = options;
			args.From = new[]{from};
			args.To = new[]{to};
			args.ProgressTitle = progressTitle;
			SHFileOperation(args);
		}
		
		#endregion

		#region Symbolic Link

		public static void CreateSymbolicLink(string linkToCreate, string target, SymbolicLinkKind kind){
			if(!Kernel32.CreateSymbolicLink(linkToCreate, target, kind)){
				throw new Win32Exception();
			}
		}

		#endregion

		#region Long Path / Short Path

		public static string GetShortPathName(string path){
			for(int count = path.Length; count < 32767; count *= 2){
				StringBuilder sb = new StringBuilder(count);
				if(Kernel32.GetShortPathName(path, sb, count) != 0){
					return sb.ToString();
				}
			}
			return null;
		}

		public static string GetLongPathName(string path){
			for(int count = path.Length + 256; count < 32767; count *= 2){
				StringBuilder sb = new StringBuilder(count);
				if(Kernel32.GetLongPathName(path, sb, count) != 0){
					return sb.ToString();
				}
			}
			return null;
		}

		#endregion

		#region ShellExecute

		public static bool Execute(string verb, string file) {
			return Execute(verb, file, null, null, ShowWindowCommand.ShowNormal, IntPtr.Zero);
		}

		public static bool Execute(string verb, string file, string parameter) {
			return Execute(verb, file, parameter, null, ShowWindowCommand.ShowNormal, IntPtr.Zero);
		}

		public static bool Execute(string verb, string file, string parameter, string directory) {
			return Execute(verb, file, parameter, directory, ShowWindowCommand.ShowNormal, IntPtr.Zero);
		}

		public static bool Execute(string verb, string file, string parameter, string directory, ShowWindowCommand nShow) {
			return Execute(verb, file, parameter, directory, nShow, IntPtr.Zero);
		}

		/// <summary>
		/// ファイルを実行する。
		/// </summary>
		/// <param name="verb">実行名</param>
		/// <param name="file">ファイルパス</param>
		/// <param name="parameter">パラメータ</param>
		/// <param name="directory">起動ディレクトリ</param>
		/// <param name="nShow">ウインドウの状態</param>
		/// <param name="hwnd">ウインドウハンドル</param>
		public static bool Execute(string verb, string file, string parameter, string directory, ShowWindowCommand nShow, IntPtr hwnd) {
			var args = new ShellExecuteInfo();
			args.Handle = hwnd;
			args.Verb = verb;
			args.File = file;
			args.Parameters = parameter;
			args.Directory = directory;
			args.Show = nShow;
			return Shell32.ShellExecuteEx(ref args);
		}

		#endregion

		#region EmptyRecycleBin

		public static OLEError EmptyRecycleBin() {
			return EmptyRecycleBin(null, IntPtr.Zero, SHEmptyRecycleBinOptions.None);
		}

		public static OLEError EmptyRecycleBin(string drive) {
			return EmptyRecycleBin(drive, IntPtr.Zero, SHEmptyRecycleBinOptions.None);
		}

		public static OLEError EmptyRecycleBin(string drive, IntPtr hwnd) {
			return EmptyRecycleBin(drive, hwnd, SHEmptyRecycleBinOptions.None);
		}

		public static OLEError EmptyRecycleBin(string drive, IntPtr hwnd, SHEmptyRecycleBinOptions options) {
			return Shell32.SHEmptyRecycleBin(hwnd, drive, options);
		}
		#endregion
		
		#region ContextMenu

		internal static ContextMenu GetContextMenu(IntPtr hwnd, string[] files) {
			ulong pchEaten = 0, pdwAttributes = 0;

			string parentDir = System.IO.Path.GetDirectoryName(files[0]);

			using(var parent = ShellFolder.GetFolder(hwnd, parentDir)) {
				IntPtr[] apidl = new IntPtr[files.Length];
				try {
					int i = 0;
					foreach(string path in files) {
						string name = System.IO.Path.GetFileName(path);
						if(String.IsNullOrEmpty(name)) {
							name = path;
						}
						parent.Interface.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, name, ref pchEaten, out apidl[i], ref pdwAttributes);
						i++;
					}

					return parent.GetUIObjectOf(apidl, hwnd);
				} finally {
					foreach(IntPtr h in apidl) {
						Marshal.FreeCoTaskMem(h);
					}
				}
			}
		}

		public static bool ShowContextMenu(IntPtr handle, Point pos, params string[] files) {
			files.ThrowIfNull("files");
			using(Win32MenuItem menu = Win32MenuItem.CreatePopupMenu()) {
				using(var contextMenu = GetContextMenu(handle, files)) {
					contextMenu.Interface.QueryContextMenu(menu.Handle, (uint)0, (uint)1, (uint)0x7fff, (uint)0);
					int cmdId = menu.Show(handle, pos, TrackPopupMenuOptions.ReturnCommand | TrackPopupMenuOptions.LeftAlign | TrackPopupMenuOptions.TopAlign);
					if(cmdId != 0) {
						cmdId--;
						contextMenu.InvokeCommand(cmdId, handle, ShowWindowCommand.ShowNormal);
						return true;
					} else {
						return false;
					}
				}
			}
		}

		public static void InvokeCommand(int cmdId, IntPtr hwnd, ShowWindowCommand nShow, params string[] files) {
			files.ThrowIfNull("files");
			using(ContextMenu contextMenu = GetContextMenu(hwnd, files)) {
				using(Win32MenuItem menuItem = Win32MenuItem.CreatePopupMenu()) {
					contextMenu.Interface.QueryContextMenu(menuItem.Handle, 0, 1, 0x7fff, 0);
					contextMenu.InvokeCommand(cmdId, hwnd, nShow);
				}
			}
		}

		public static void ShowProperty(IntPtr hwnd, params string[] files) {
			files.ThrowIfNull("files");
			InvokeCommand(19, hwnd, ShowWindowCommand.ShowNormal, files);
		}

		public static bool ExecuteDefaultAction(IntPtr hwnd, params string[] files) {
			files.ThrowIfNull("files");
			if(files.Length > 0) {
				using(Win32MenuItem menu = Win32MenuItem.CreatePopupMenu()) {
					using(var contextMenu = GetContextMenu(hwnd, files)) {
						contextMenu.Interface.QueryContextMenu(menu.Handle, (uint)0, (uint)1, (uint)0x7fff, (uint)0);
						int cmdId = menu.GetDefaultItem(MenuFoundBy.Command, GetMenuDefaultItemOptions.Normal);
						if(cmdId != -1) {
							cmdId--;
							/*
							try {
								Environment.CurrentDirectory = IO.Path.GetDirectoryName(files[0]);
							} catch(ArgumentException ex) {
								Debug.WriteLine(ex.ToString());
							} catch(IO.IOException ex) {
								Debug.WriteLine(ex.ToString());
							} catch(System.Security.SecurityException ex) {
								Debug.WriteLine(ex.ToString());
							}
							 */
							contextMenu.InvokeCommand(cmdId, IntPtr.Zero, ShowWindowCommand.ShowNormal);
							return true;
						} else {
							return false;
						}
					}
				}
			}
			return false;
		}

		#endregion
	}

	public static partial class Kernel32 {
		[DllImport("kernel32.dll", EntryPoint = "CreateSymbolicLinkW", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool CreateSymbolicLink([In] string lpSymlinkFileName, [In] string lpTargetFileName, SymbolicLinkKind dwFlags);

		[DllImport("kernel32", EntryPoint = "GetShortPathName", CharSet = CharSet.Auto)]
		public static extern int GetShortPathName(string path, StringBuilder shortName, int bufferSize);

		[DllImport("kernel32", EntryPoint = "GetLongPathName", CharSet = CharSet.Ansi)]
		public static extern int GetLongPathName(string path, StringBuilder longName, int bufferSize);
	}

	public static partial class Shell32 {
		[DllImport("shell32.dll", EntryPoint = "SHEmptyRecycleBin", CharSet = CharSet.Auto)]
		public static extern OLEError SHEmptyRecycleBin(IntPtr hwnd, string drive, SHEmptyRecycleBinOptions options);

		[DllImport("shell32.dll", EntryPoint = "ExtractIconEx", CharSet = CharSet.Auto)]
		public static extern int ExtractIconEx([MarshalAs(UnmanagedType.LPTStr)] string file, int index, out IntPtr largeIconHandle, out IntPtr smallIconHandle, int icons);

		[DllImport("shell32.dll", EntryPoint = "SHGetFileInfo", CharSet = CharSet.Auto)]
		public static extern IntPtr SHGetFileInfo(string pszPath, System.IO.FileAttributes attr, ref SHFileInfo psfi, int cbSizeFileInfo, SHGetFileInfoOptions uFlags);

		[DllImport("shell32.dll", EntryPoint = "#727")]
		public extern static int SHGetImageList(ImageListSize iImageList, ref Guid riid, out IImageList ppv);

		[DllImport("shell32.dll", EntryPoint = "ShellExecuteEx", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShellExecuteEx(ref ShellExecuteInfo shinfo);

		[DllImport("shell32.dll", EntryPoint = "SHGetDesktopFolder", CharSet = CharSet.Auto)]
		public static extern int SHGetDesktopFolder(out IntPtr ppshf);

		[DllImport("shell32.dll", EntryPoint = "SHGetSpecialFolderLocation", CharSet = CharSet.Auto)]
		public static extern uint SHGetSpecialFolderLocation(IntPtr hwnd, int CSIDL, out IntPtr pidl);

		[DllImport("shlwapi.dll", EntryPoint = "PathCommonPrefix", CharSet = CharSet.Auto)]
		public static extern int PathCommonPrefix(string path1, string path2, StringBuilder commonPrefix);

		[DllImport("shell32.dll", EntryPoint = "CommandLineToArgvW", CharSet = CharSet.Unicode)]
		internal static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string commmandLine, out int argCount);
	}

	#region IShellFolder
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("000214E6-0000-0000-C000-000000000046")]
	public interface IShellFolder {
		[PreserveSig]
		Int32 ParseDisplayName(IntPtr hwnd, IntPtr pbc, [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, ref ulong pchEaten, out IntPtr ppidl, ref ulong pdwAttributes);

		[PreserveSig]
		Int32 EnumObjects(IntPtr hwnd, Int32 grfFlags, out IntPtr ppenumIDList);

		[PreserveSig]
		Int32 BindToObject(IntPtr pidl, IntPtr pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppv);

		[PreserveSig]
		Int32 BindToStorage(IntPtr pidl, IntPtr pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppv);

		[PreserveSig]
		Int32 CompareIDs(Int32 lParam, IntPtr pidl1, IntPtr pidl2);

		[PreserveSig]
		Int32 CreateViewObject(IntPtr hwndOwner, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppv);

		[PreserveSig]
		Int32 GetAttributesOf(uint cidl, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IntPtr[] apidl, ref uint rgfInOut);

		[PreserveSig]
		Int32 GetUIObjectOf(IntPtr hwndOwner, uint cidl, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, ref uint rgfReserved, out IntPtr ppv);

		[PreserveSig]
		Int32 GetDisplayNameOf(IntPtr pidl, uint uFlags, out StrRet pName);

		[PreserveSig]
		Int32 SetNameOf(IntPtr hwnd, IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)] String pszName, uint uFlags, out IntPtr ppidlOut);
	}
	#endregion

	#region StrRet
	[StructLayout(LayoutKind.Explicit)]
	public struct StrRet {
		[FieldOffset(0)]
		public UInt32 uType;

		[FieldOffset(4)]
		public IntPtr pOleStr;

		[FieldOffset(4)]
		public IntPtr pStr;

		[FieldOffset(4)]
		public uint uOffset;

		[FieldOffset(4)]
		public IntPtr cStr;
	}
	#endregion

	#region IContextMenu
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[GuidAttribute("000214e4-0000-0000-c000-000000000046")]
	public interface IContextMenu {
		[PreserveSig]
		int QueryContextMenu(IntPtr hmenu, uint iMenu, uint idCmdFirst, uint idCmdLast, uint uFlags);

		[PreserveSig]
		int InvokeCommand(ref InvokeCommandInfo info);

		[PreserveSig]
		void GetCommandString(int idcmd, uint uflags, uint reserved, StringBuilder commandstring, uint cch);
	}
	#endregion

	#region InvokeCommandInfo
	[StructLayout(LayoutKind.Sequential)]
	public struct InvokeCommandInfo {
		public int Size;
		public int Mask;
		public IntPtr Hwnd;
		public IntPtr Verb;
		public IntPtr Parameters;
		public IntPtr Directory;
		public ShowWindowCommand Show;
		public int HotKey;
		public IntPtr Icon;
	}
	#endregion

	#region ShellExecuteInfo
	[StructLayoutAttribute(LayoutKind.Sequential)]
	public struct ShellExecuteInfo {
		public int Size;
		public ShellExecuteExMask Mask;
		public IntPtr Handle;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string Verb;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string File;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string Parameters;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string Directory;
		public ShowWindowCommand Show;
		public IntPtr InstApp;
		public IntPtr IDList;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string Class;
		public IntPtr KeyClass;
		public uint HotKey;
		public IntPtr Icon;
		public IntPtr Process;
	}

	#endregion

	#region ShellExecuteExMask
	[Flags]
	public enum ShellExecuteExMask : int {
		None = 0x00000000,
		ClassName = 0x00000001,
		ClassKey = 0x00000003,
		IDList = 0x00000004,
		InvokeIDList = 0x0000000c,
		Icon = 0x00000010,
		Hotkey = 0x00000020,
		NoCloseProcess = 0x00000040,
		ConnectNetDrive = 0x00000080,
		FlagDDEWait = 0x00000100,
		DoEnvironmentSubstring = 0x00000200,
		FlagNoUI = 0x00000400,
		Unicode = 0x00004000,
		NoConsole = 0x00008000,
		HMonitor = 0x00200000,
		FlagLogUsage = 0x04000000,
	}
	#endregion

	#region FileOperationOptions
	/// <summary>
	/// SHFileOperationで行う処理のオプション。
	/// </summary>
	[Flags]
	public enum FileOperationOptions : ushort{
		None = 0x0000,
		MultiDestFiles = 0x0001,
		ConfirmMouse = 0x0002,
		Silent = 0x0004,
		RenameOnCollision = 0x0008,
		NoConfirmation = 0x0010,
		WantMappingHandle = 0x0020,
		AllowUndo = 0x0040,
		FilesOnly = 0x0080,
		SimpleProgress = 0x0100,
		NoConfirmMakeDirectory = 0x0200,
		NoErrorUI = 0x0400,
		NoCopySecurityAttributes = 0x0800,
		NoRecursion = 0x1000,
		NoConectedElements = 0x2000,
		WantNukeWarning = 0x4000,
		NoRecurseParsing = 0x8000,
	}
	#endregion

	#region FileOperationFunc
	public enum FileOperationFunc : uint{
		Move = 0x0001,
		Copy = 0x0002,
		Delete = 0x0003,
		Rename = 0x0004,
	}
	#endregion

	#region SymbolicLinkKind
	public enum SymbolicLinkKind : int{
		File = 0,
		Directory = 1,
	}
	#endregion
}