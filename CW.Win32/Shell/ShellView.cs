using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CW.Win32.Shell {
	public class ShellView : ComObject<IShellView>{
		public static readonly Guid IID = new Guid("{000214E3-0000-0000-c000-000000000046}");

		public ShellView(IShellView view)
			: base(view) {

		}
	}

	[ComImport]
	[Guid("000214E3-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IShellView {
		void GetWindow(out IntPtr windowHandle);

		void ContextSensitiveHelp(bool fEnterMode);

		[PreserveSig]
		long TranslateAcceleratorA(IntPtr message);

		void EnableModeless(bool enable);

		void UIActivate(uint activtionState);

		void Refresh();

		void CreateViewWindow([In, MarshalAs(UnmanagedType.Interface)] IShellView previousShellView, IntPtr folderSetting, IntPtr shellBrowser, [In] ref Rectangle bounds, [In, Out] ref IntPtr handleOfCreatedWindow);

		void DestroyViewWindow();

		void GetCurrentInfo(IntPtr pfs);

		void AddPropertySheetPages(uint reserved, ref IntPtr functionPointer, IntPtr lparam);

		void SaveViewState();

		void SelectItem(IntPtr pidlItem, uint flags);

		[PreserveSig]
		long GetItemObject(uint uItem, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppv);
	}
}
