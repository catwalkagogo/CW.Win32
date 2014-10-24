using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace CW.Win32.Shell {
	public class ShellItemArray : ComObject<IShellItemArray>{
		public ShellItemArray(IShellItemArray array) : base(array){}

		public static ShellItemArray FromFiles(string[] files) {
			files.ThrowIfNull("files");

			IntPtr[] rgpidl = new IntPtr[files.Length];
			try {
				var i = 0;
				foreach(var file in files) {
					rgpidl[i] = Shell32.ILCreateFromPath(file);
					i++;
				}

				IShellItemArray array;
				Marshal.ThrowExceptionForHR(Shell32.SHCreateShellItemArrayFromIDLists(rgpidl.Length, rgpidl, out array));
				return new ShellItemArray(array);
			} finally {
				foreach(var pidl in rgpidl) {
					if(pidl != IntPtr.Zero) {
						Shell32.ILFree(pidl);
					}
				}
			}
		}
	}

	public static partial class Shell32 {
		[DllImport("shell32")]
		public static extern int SHCreateShellItemArray(IntPtr pidlParent, IShellFolder psf, uint cidl, IntPtr[] ppidl, out IShellItemArray ppsiItemArray);

		[DllImport("shell32")]
		public static extern int SHCreateShellItemArrayFromIDLists(int cidl, IntPtr[] rgpidl, out IShellItemArray ppsiItemArray);

		[DllImport("shell32")]
		public static extern IntPtr ILCreateFromPath(string path);

		[DllImport("shell32")]
		public static extern void ILFree(IntPtr pidl);
	}

	[Guid("B63EA76D-1F85-456F-A19C-48159EFA858B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IShellItemArray {
		[return: MarshalAs(UnmanagedType.Interface)]
		object BindToHandler(IBindCtx pbc, [In] ref Guid rbhid, [In] ref Guid riid);
		[return: MarshalAs(UnmanagedType.Interface)]
		object GetPropertyStore(int flags, [In] ref Guid riid);
		[return: MarshalAs(UnmanagedType.Interface)]
		object GetPropertyDescriptionList([In] ref PKEY keyType, [In] ref Guid riid);
		uint GetAttributes(SIATTRIBFLAGS dwAttribFlags, uint sfgaoMask);
		uint GetCount();
		IShellItem GetItemAt(uint dwIndex);
		[return: MarshalAs(UnmanagedType.Interface)]
		object EnumItems();
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct PKEY {
		private readonly Guid _fmtid;
		private readonly uint _pid;
		public static readonly PKEY Title = new PKEY(new Guid("F29F85E0-4FF9-1068-AB91-08002B27B3D9"), 2u);
		public static readonly PKEY AppUserModel_ID = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5u);
		public static readonly PKEY AppUserModel_IsDestListSeparator = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 6u);
		public static readonly PKEY AppUserModel_RelaunchCommand = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 2u);
		public static readonly PKEY AppUserModel_RelaunchDisplayNameResource = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 4u);
		public static readonly PKEY AppUserModel_RelaunchIconResource = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 3u);
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public PKEY(Guid fmtid, uint pid) {
			this._fmtid = fmtid;
			this._pid = pid;
		}
	}

	public enum SIATTRIBFLAGS {
		AND = 1,
		OR,
		APPCOMPAT
	}
}
