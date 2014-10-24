using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CW.Win32 {
	public class InteropObject : IDisposable{
		protected IntPtr Handle{get; private set;}

		public InteropObject(string dllName) : this(Kernel32.LoadLibrary(dllName)) { }
		public InteropObject(IntPtr handle){
			if(handle == IntPtr.Zero){
				throw new ArgumentException("handle");
			}
			this.Handle = handle;
		}

		protected void ThrowIfDisposed(){
			if(this._IsDisposed){
				throw new ObjectDisposedException("Handle");
			}
		}

		~InteropObject(){
			this.Dispose(false);
		}

		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private bool _IsDisposed = false;
		protected virtual void Dispose(bool disposing) {
			if(!this._IsDisposed){
				Kernel32.FreeLibrary(this.Handle);
				this._IsDisposed = true;
			}
		}

		public virtual T LoadMethod<T>(string name) where T : class{
			return LoadMethod<T>(name, this.Handle);
		}

		private static T LoadMethod<T>(string name, IntPtr hModule) where T : class{
			return Marshal.GetDelegateForFunctionPointer(Kernel32.GetProcAddress(hModule, name), typeof(T)) as T;
		}
	}

	public static partial class Kernel32 {
		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(String lpFileName);
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetProcAddress(IntPtr hModule, String lpProcName);
		[DllImport("kernel32.dll")]
		public static extern Boolean FreeLibrary(IntPtr hLibModule);
	}
}
