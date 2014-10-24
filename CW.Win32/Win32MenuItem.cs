using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CW.Win32 {
	public class Win32MenuItem : DisposableObject {
		private IntPtr handle;
		private bool _Owner;

		public Win32MenuItem(IntPtr handle) : this(handle, false) {
		}

		public Win32MenuItem(IntPtr handle, bool owner) {
			if(handle == IntPtr.Zero){
				throw new ArgumentException("handle");
			}
			this.handle = handle;
			this._Owner = owner;
		}

		public static Win32MenuItem CreatePopupMenu(){
			return new Win32MenuItem(User32.CreatePopupMenu(), true);
		}

		public int Show(IntPtr handle, Point pos, TrackPopupMenuOptions options) {
			User32.ClientToScreen(handle, ref pos);
			return User32.TrackPopupMenu(this.handle, options, (int)pos.X, (int)pos.Y, 0, handle, 0);
		}

		public int GetDefaultItem(MenuFoundBy byPos, GetMenuDefaultItemOptions options) {
			return User32.GetMenuDefaultItem(this.handle, byPos, options);
		}

		public IntPtr Handle {
			get {
				return this.handle;
			}
		}

		public int Count {
			get {
				return User32.GetMenuItemCount(this.handle);
			}
		}

		protected override void Dispose(bool disposing) {
			if(this._Owner){
				if(this.handle != IntPtr.Zero) {
					User32.DestroyMenu(this.handle);
					this.handle = IntPtr.Zero;
				}
			}
		}
	}

	public static partial class User32 {
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

		[DllImport("user32.dll", EntryPoint = "ClientToScreen", CallingConvention = CallingConvention.StdCall)]
		public extern static bool ClientToScreen(IntPtr hWnd, ref Point pt);
	}
}
