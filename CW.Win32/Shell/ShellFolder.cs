using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CW.Win32.Shell {
	public class ShellFolder : ComObject<IShellFolder>{
		public static readonly Guid IID = new Guid("{000214E6-0000-0000-C000-000000000046}");
	
		public ShellFolder(IShellFolder shellFolder) : base(shellFolder){}

		public static ShellFolder GetDesktopFolder() {
			IntPtr ptrRet;
			var error = Shell32.SHGetDesktopFolder(out ptrRet);
			Marshal.ThrowExceptionForHR(error);
			return new ShellFolder((IShellFolder)Marshal.GetTypedObjectForIUnknown(ptrRet, typeof(IShellFolder)));
		}

		public static ShellFolder GetFolder(IntPtr hwnd, string path) {
			ulong pchEaten = 0, pdwAttributes = 0;

			using(var desktop = ShellFolder.GetDesktopFolder()) {
				IntPtr ppidl = IntPtr.Zero;
				if(String.IsNullOrEmpty(path)) {
					Shell32.SHGetSpecialFolderLocation(hwnd, 0x0011, out ppidl);
				} else {
					var error = desktop.Interface.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, path, ref pchEaten, out ppidl, ref pdwAttributes);
					Marshal.ThrowExceptionForHR(error);
				}

				using(var idl = new ComTaskMemory(ppidl)) {
					return desktop.BindToObject(ppidl);
				}
			}
		}

		public ShellView CreateViewObject(IntPtr hwnd) {
			IntPtr ptrRet;
			var error = this.Interface.CreateViewObject(hwnd, ShellView.IID, out ptrRet);
			Marshal.ThrowExceptionForHR(error);
			return new ShellView((IShellView)Marshal.GetTypedObjectForIUnknown(ptrRet, typeof(IShellView)));
		}

		public ShellFolder BindToObject(IntPtr ppidl) {
			IntPtr ptrRet;
			var error = this.Interface.BindToObject(ppidl, IntPtr.Zero, ShellFolder.IID, out ptrRet);
			Marshal.ThrowExceptionForHR(error);
			return new ShellFolder((IShellFolder)Marshal.GetTypedObjectForIUnknown(ptrRet, typeof(IShellFolder)));
		}

		public ContextMenu GetUIObjectOf(IntPtr[] apidl, IntPtr hwnd) {
			uint rgfReserved = 0;
			IntPtr ptrRet;
			var error = this.Interface.GetUIObjectOf(hwnd, (uint)apidl.Length, apidl, ContextMenu.IID, ref rgfReserved, out ptrRet);
			Marshal.ThrowExceptionForHR(error);
			return new ContextMenu((IContextMenu)Marshal.GetTypedObjectForIUnknown(ptrRet, typeof(IContextMenu)));
		}
	}
}
