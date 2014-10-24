using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Win32 {
	public class CachedInteropObject : InteropObject {
		private IDictionary<string, object> _Cache = new Dictionary<string, object>();

		public CachedInteropObject(string dllName): base(dllName){}
		public CachedInteropObject(IntPtr handle) : base(handle){}

		public override T LoadMethod<T>(string name) {
			object obj;
			T method;
			if(!this._Cache.TryGetValue(name, out obj)){
				method = base.LoadMethod<T>(name);
				this._Cache[name] = method;
			}else{
				method = (T)obj;
			}

			return method;
		}
	}
}
