using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Win32 {
	public class Win32Window : DisposableObject{
		public IntPtr Handle{get; private set;}
		private bool _Owner;

		public Win32Window(IntPtr handle) : this(handle, false){}

		public Win32Window(IntPtr handle, bool owner){
			if(handle == IntPtr.Zero){
				throw new ArgumentException("handle");
			}
			this.Handle = handle;
			this._Owner = owner;
		}

		public void Show(ShowWindowCommand cmd){
			User32.ShowWindow(this.Handle, cmd);
		}

		public void SetForeground(){
			User32.SetForegroundWindow(this.Handle);
		}

		public void Activate(){
			User32.SetActiveWindow(this.Handle);
		}
	}
}
