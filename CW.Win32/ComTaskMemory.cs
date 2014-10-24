using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CW.Win32 {
	public class ComTaskMemory : DisposableObject{
		public IntPtr Handle { get; private set; }

		public ComTaskMemory(IntPtr handle) {
			this.Handle = handle;
		}

		protected override void Dispose(bool disposing) {
			if(this.Handle != IntPtr.Zero) {
				Marshal.FreeCoTaskMem(this.Handle);
				this.Handle = IntPtr.Zero;
			}
		}
	}
}
