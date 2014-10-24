using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace CW.Win32.Shell {
	public class ShellItem : ComObject<IShellItem>{
		public ShellItem(IShellItem item) : base(item){

		}

		private static Guid ShellItemGuid = typeof(IShellItem).GUID;

		public static ShellItem FromPath(string path) {
			return new ShellItem((IShellItem)Shell32.SHCreateItemFromParsingName(path, null, ref ShellItemGuid));
		}

	}

	public static partial class Shell32 {
		[DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode, PreserveSig = false)]
		[return: MarshalAs(UnmanagedType.Interface)]
		public static extern object SHCreateItemFromParsingName(
			[MarshalAs(UnmanagedType.LPWStr)] string pszPath, IBindCtx pbc, ref Guid riid);
	}

	#region IShellItem

	[ComImport]
	[Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IShellItem {
		[return: MarshalAs(UnmanagedType.Interface)]
		object BindToHandler(IBindCtx pbc, ref Guid bhid, ref Guid riid);

		IShellItem GetParent();

		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetDisplayName(SIGDN sigdnName);

		uint GetAttributes(uint sfgaoMask);

		int Compare(IShellItem psi, uint hint);
	}

	#endregion

	#region SIGDN

	public enum SIGDN : uint {
		SIGDN_NORMALDISPLAY = 0x00000000,
		SIGDN_PARENTRELATIVEPARSING = 0x80018001,
		SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8001c001,
		SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,
		SIGDN_PARENTRELATIVEEDITING = 0x80031001,
		SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,
		SIGDN_FILESYSPATH = 0x80058000,
		SIGDN_URL = 0x80068000
	}

	#endregion
}
