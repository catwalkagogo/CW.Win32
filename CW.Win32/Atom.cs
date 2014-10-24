using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Win32 {
	public class Atom : DisposableObject, IEquatable<Atom>{
		public int Id{get; private set;}

		public Atom(string str){
			str.ThrowIfNull("str");
			var atom = Kernel32.GlobalAddAtom(str);
			this.Id = atom;
		}

		protected override void Dispose(bool disposing) {
			if(!this.IsDisposed){
				Kernel32.GlobalDeleteAtom(this.Id);
			}
			base.Dispose(disposing);
		}

		public override int GetHashCode() {
			return this.Id;
		}

		public override bool Equals(object obj) {
			var atom = obj as Atom;
			if(atom != null){
				return this.Equals(atom);
			}else{
				return base.Equals(obj);
			}
		}

		public bool Equals(Atom atom){
			if(atom != null){
				return this.Id == atom.Id;
			}else{
				return false;
			}
		}
	}
}
