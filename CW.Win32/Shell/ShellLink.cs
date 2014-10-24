/*
	$Id: ShellLink.cs 269 2011-07-28 10:41:57Z catwalkagogo@gmail.com $
*/
using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace CW.Win32.Shell {
	using ComTypes = System.Runtime.InteropServices.ComTypes;
	
	// based on http://smdn.invisiblefulmoon.net/ikimasshoy/dotnettips/tips043.html
	public sealed class ShellLink : ComObject<IShellLink>{
		// カレントファイル
		private string currentFile;
		
		// 各種定数
		internal const int MAX_PATH = 260;
		
		internal const uint SLGP_SHORTPATH   = 0x0001; // 短い形式(8.3形式)のファイル名を取得する
		internal const uint SLGP_UNCPRIORITY = 0x0002; // UNCパス名を取得する
		internal const uint SLGP_RAWPATH	 = 0x0004; // 環境変数などが変換されていないパス名を取得する
		
		#region "コンストラクション・デストラクション"
		public ShellLink() : base(GetShellLink()){
			this.currentFile = "";
		}

		private static IShellLink GetShellLink() {
			return (IShellLink)(new ShellLinkObject());
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="linkFile">ショートカットファイル</param>
		public ShellLink(string linkFile) : this(){
			this.Load(linkFile);
		}

		#endregion

		#region "プロパティ"

		/// <summary>
		/// カレントファイル。
		/// </summary>
		public string CurrentFile{
			get{
				return currentFile;
			}
		}

		/// <summary>
		/// ショートカットのリンク先。
		/// </summary>
		public string TargetPath{
			get{		
				StringBuilder targetPath = new StringBuilder(MAX_PATH, MAX_PATH);
				ShellLinkFindData data = new ShellLinkFindData();
				this.Interface.GetPath(targetPath, targetPath.Capacity, ref data, SLGP_UNCPRIORITY);
				return targetPath.ToString();
			}
			set{
				this.Interface.SetPath(value);
			}
		}

		/// <summary>
		/// 作業ディレクトリ。
		/// </summary>
		public string WorkingDirectory{
			get{
				StringBuilder workingDirectory = new StringBuilder(MAX_PATH, MAX_PATH);
				this.Interface.GetWorkingDirectory(workingDirectory, workingDirectory.Capacity);
				return workingDirectory.ToString();
			}
			set{
				this.Interface.SetWorkingDirectory(value);	
			}
		}

		/// <summary>
		/// コマンドライン引数。
		/// </summary>
		public string Arguments{
			get{
				StringBuilder arguments = new StringBuilder(MAX_PATH, MAX_PATH);
				this.Interface.GetArguments(arguments, arguments.Capacity);
				return arguments.ToString();
			}
			set{
				this.Interface.SetArguments(value);	
			}
		}

		/// <summary>
		/// ショートカットの説明。
		/// </summary>
		public string Description{
			get{
				StringBuilder description = new StringBuilder(MAX_PATH, MAX_PATH);
				this.Interface.GetDescription(description, description.Capacity);
				return description.ToString();
			}
			set{
				this.Interface.SetDescription(value);	
			}
		}

		/// <summary>
		/// アイコンのファイル。
		/// </summary>
		public string IconFile{
			get{
				int iconIndex = 0;
				string iconFile = "";
				this.GetIconLocation(out iconFile, out iconIndex);
				return iconFile;
			}
			set{
				int iconIndex = 0;
				string iconFile = "";
				this.GetIconLocation(out iconFile, out iconIndex);
				this.SetIconLocation(value, iconIndex);
			}
		}

		/// <summary>
		/// アイコンのインデックス。
		/// </summary>
		public int IconIndex{
			get{
				int iconIndex = 0;
				string iconPath = "";
				this.GetIconLocation(out iconPath, out iconIndex);
				return iconIndex;
			}
			set{
				int iconIndex = 0;
				string iconPath = "";
				this.GetIconLocation(out iconPath, out iconIndex);
				this.SetIconLocation(iconPath, value);
			}
		}

		/// <summary>
		/// アイコンのファイルとインデックスを取得する
		/// </summary>
		/// <param name="iconFile">アイコンのファイル</param>
		/// <param name="iconIndex">アイコンのインデックス</param>
		private void GetIconLocation(out string iconFile, out int iconIndex){
			StringBuilder iconFileBuffer = new StringBuilder(MAX_PATH, MAX_PATH);
			this.Interface.GetIconLocation(iconFileBuffer, iconFileBuffer.Capacity, out iconIndex);
			iconFile = iconFileBuffer.ToString();
		}

		/// <summary>
		/// アイコンのファイルとインデックスを設定する
		/// </summary>
		/// <param name="iconFile">アイコンのファイル</param>
		/// <param name="iconIndex">アイコンのインデックス</param>
		private void SetIconLocation(string iconFile, int iconIndex){
			this.Interface.SetIconLocation(iconFile, iconIndex);
		}

		/// <summary>
		/// 実行時のウィンドウの大きさ。
		/// </summary>
		public ShellLinkDisplayMode DisplayMode{
			get{
				int showCmd = 0;
				this.Interface.GetShowCmd(out showCmd);	
				return (ShellLinkDisplayMode)showCmd;
			}
			set
			{
				this.Interface.SetShowCmd((int)value);
			}
		}

		/// <summary>
		/// ホットキー。
		/// </summary>
		public int HotKey{
			get{
				ushort hotKey = 0;
				this.Interface.GetHotkey(out hotKey);
				return (int)hotKey;
			}
			set{
				this.Interface.SetHotkey((ushort)value);
			}
		}

		#endregion

		#region "保存と読み込み"

		/// <summary>
		/// IShellLinkインターフェイスからキャストされたIPersistFileインターフェイスを取得します。
		/// </summary>
		/// <returns>IPersistFileインターフェイス。　取得できなかった場合はnull。</returns>
		private ComTypes.IPersistFile GetIPersistFile(){
			return this.Interface as ComTypes.IPersistFile;
		}

		/// <summary>
		/// カレントファイルにショートカットを保存します。
		/// </summary>
		/// <exception cref="COMException">IPersistFileインターフェイスを取得できませんでした。</exception>
		public void Save()
		{
			this.Save(currentFile);
		}

		/// <summary>
		/// 指定したファイルにショートカットを保存します。
		/// </summary>
		/// <param name="linkFile">ショートカットを保存するファイル</param>
		/// <exception cref="COMException">IPersistFileインターフェイスを取得できませんでした。</exception>
		public void Save(string linkFile){
			// IPersistFileインターフェイスを取得して保存
			ComTypes.IPersistFile persistFile = GetIPersistFile();
			if(persistFile == null) throw new COMException("IPersistFileインターフェイスを取得できませんでした。");
			persistFile.Save(linkFile, true);
			
			// カレントファイルを保存
			this.currentFile = linkFile;
		}

		/// <summary>
		/// 指定したファイルからショートカットを読み込みます。
		/// </summary>
		/// <param name="linkFile">ショートカットを読み込むファイル</param>
		/// <exception cref="FileNotFoundException">ファイルが見つかりません。</exception>
		/// <exception cref="COMException">IPersistFileインターフェイスを取得できませんでした。</exception>
		public void Load(string linkFile){
			this.Load(linkFile, IntPtr.Zero, ShellLinkResolveFlags.AnyMatch | ShellLinkResolveFlags.NoUI, 1);
		}

		/// <summary>
		/// 指定したファイルからショートカットを読み込みます。
		/// </summary>
		/// <param name="linkFile">ショートカットを読み込むファイル</param>
		/// <param name="hWnd">このコードを呼び出したオーナーのウィンドウハンドル</param>
		/// <param name="resolveFlags">ショートカット情報の解決に関する動作を表すフラグ</param>
		/// <exception cref="FileNotFoundException">ファイルが見つかりません。</exception>
		/// <exception cref="COMException">IPersistFileインターフェイスを取得できませんでした。</exception>
		public void Load(string linkFile, IntPtr hWnd, ShellLinkResolveFlags resolveFlags){
			this.Load(linkFile, hWnd, resolveFlags, 1);
		}

		/// <summary>
		/// 指定したファイルからショートカットを読み込みます。
		/// </summary>
		/// <param name="linkFile">ショートカットを読み込むファイル</param>
		/// <param name="hWnd">このコードを呼び出したオーナーのウィンドウハンドル</param>
		/// <param name="resolveFlags">ショートカット情報の解決に関する動作を表すフラグ</param>
		/// <param name="timeOut">SLR_NO_UIを指定したときのタイムアウト値(ミリ秒)</param>
		/// <exception cref="FileNotFoundException">ファイルが見つかりません。</exception>
		/// <exception cref="COMException">IPersistFileインターフェイスを取得できませんでした。</exception>
		public void Load(string linkFile, IntPtr hWnd, ShellLinkResolveFlags resolveFlags, TimeSpan timeOut){
			this.Load(linkFile, hWnd, resolveFlags, (int)timeOut.TotalMilliseconds);
		}
		
		/// <summary>
		/// 指定したファイルからショートカットを読み込みます。
		/// </summary>
		/// <param name="linkFile">ショートカットを読み込むファイル</param>
		/// <param name="hWnd">このコードを呼び出したオーナーのウィンドウハンドル</param>
		/// <param name="resolveFlags">ショートカット情報の解決に関する動作を表すフラグ</param>
		/// <param name="timeOutMilliseconds">SLR_NO_UIを指定したときのタイムアウト値(ミリ秒)</param>
		/// <exception cref="FileNotFoundException">ファイルが見つかりません。</exception>
		/// <exception cref="COMException">IPersistFileインターフェイスを取得できませんでした。</exception>
		public void Load(string linkFile, IntPtr hWnd, ShellLinkResolveFlags resolveFlags, int timeOutMilliseconds){
			if(!File.Exists(linkFile)) throw new FileNotFoundException("ファイルが見つかりません。", linkFile);

			// IPersistFileインターフェイスを取得
			ComTypes.IPersistFile persistFile = GetIPersistFile();

			if(persistFile == null) throw new COMException("IPersistFileインターフェイスを取得できませんでした。");

			// 読み込み
			persistFile.Load(linkFile, 0x00000000);

			// フラグを処理
			uint flags = (uint)resolveFlags;

			if((resolveFlags & ShellLinkResolveFlags.NoUI) == ShellLinkResolveFlags.NoUI){
				flags |= (uint)(timeOutMilliseconds << 16);
			}

			this.Interface.Resolve(hWnd, flags);

			// カレントファイルを指定
			currentFile = linkFile;
		}
		#endregion
	}

	[ComImport]
	[Guid("00021401-0000-0000-C000-000000000046")]
	[ClassInterface(ClassInterfaceType.None)]
	public class ShellLinkObject{
	}
	
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("000214F9-0000-0000-C000-000000000046")]
	public interface IShellLink{
		void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cch, [MarshalAs(UnmanagedType.Struct)] ref ShellLinkFindData pfd, uint fFlags);
		void GetIDList(out IntPtr ppidl);
		void SetIDList(IntPtr pidl);
		void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cch);
		void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
		void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cch);
		void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
		void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cch);
		void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
		void GetHotkey(out ushort pwHotkey);
		void SetHotkey(ushort wHotkey);
		void GetShowCmd(out int piShowCmd);
		void SetShowCmd(int iShowCmd);
		void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cch, out int piIcon);
		void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
		void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);
		void Resolve(IntPtr hwnd, uint flags);
		void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
	}
	
	[StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
	public struct ShellLinkFindData{
		public const int MAX_PATH = 260;
		public uint dwFileAttributes;
		public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
		public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
		public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
		public uint nFileSizeHigh;
		public uint nFileSizeLow;
		public uint dwReserved0;
		public uint dwReserved1;
		
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
		public string cFileName;
		
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
		public string cAlternateFileName;
	}
	
	/// <summary>
	/// 実行時のウィンドウの表示方法を表す列挙型です。
	/// </summary>
	public enum ShellLinkDisplayMode : int{
		/// <summary>通常の大きさのウィンドウで起動します。</summary>
		Normal = 1,
		/// <summary>最大化された状態で起動します。</summary>
		Maximized = 3,
		/// <summary>最小化された状態で起動します。</summary>
		Minimized = 7,
	}
	
	/// <summary></summary>
	[Flags]
	public enum ShellLinkResolveFlags : int{
		/// <summary></summary>
		AnyMatch = 0x2,
		/// <summary></summary>
		InvokeMsi = 0x80,
		/// <summary></summary>
		NoLinkInfo = 0x40,
		/// <summary></summary>
		NoUI = 0x1,
		/// <summary></summary>
		NoUIWithMessagePump = 0x101,
		/// <summary></summary>
		NoUpdate = 0x8,
		/// <summary></summary>
		NoSearch = 0x10,
		/// <summary></summary>
		NoTrack = 0x20,
		/// <summary></summary>
		Update  = 0x4
	}
}