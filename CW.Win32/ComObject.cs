using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CW.Win32 {
	public class ComObject<T> : IDisposable{
		private T _Interface;
		public T Interface{
			get {
				this.ThrowIfDisposed();
				return this._Interface;
			}
		}

		public ComObject(T obj){
			obj.ThrowIfNull("obj");
			if(!obj.GetType().IsCOMObject) {
				throw new ArgumentException("obj is not a ComObject");
			}
			this._Interface = obj;
		}

		~ComObject(){
			this.Dispose(false);
		}

		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private bool _IsDisposed = false;
		protected virtual void Dispose(bool disposing) {
			if(!this._IsDisposed){
				Marshal.FinalReleaseComObject(this.Interface);
				this._IsDisposed = true;
			}
		}

		protected void ThrowIfDisposed() {
			if(this._IsDisposed) {
				throw new ObjectDisposedException(this.GetType().Name);
			}
		}
	}
}
