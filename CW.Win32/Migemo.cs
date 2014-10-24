/*using System;
using System.Runtime.InteropServices;

namespace CW.Win32{
	public class Migemo : InteropObject {
		private IntPtr _MigemoHandle;
		private bool _Disposed = false;

		#region Initialize

		public Migemo(string dllName) : this(dllName, null){}
		public Migemo(string dllName, string dict) : base(dllName){
			this._MigemoHandle = this.migemo_open(dict);
		}

		public Migemo(IntPtr handle) : this(handle, null){}
		public Migemo(IntPtr handle, string dict) : base(handle){
			this._MigemoHandle = this.migemo_open(dict);
		}

		protected override void Dispose(bool disposing) {
			if(!this._Disposed){
				migemo_close(this._MigemoHandle);
				this._Disposed = true;
			}
			base.Dispose(disposing);
		}

		#endregion

		#region NativeMethods

		private delegate IntPtr migemo_openDelegate(string dict);
		private migemo_openDelegate _migemo_openDelegate;
		private IntPtr migemo_open(string dict){
			this.ThrowIfDisposed();
			if(this._migemo_openDelegate == null){
				this._migemo_openDelegate = this.LoadMethod<migemo_openDelegate>("migemo_open");
			}
			return this._migemo_openDelegate(dict);
		}

		private delegate void migemo_closeDelegate(IntPtr handle);
		private migemo_closeDelegate _migemo_closeDelegate;
		private void migemo_close(IntPtr handle){
			this.ThrowIfDisposed();
			if(this._migemo_closeDelegate == null){
				this._migemo_closeDelegate = this.LoadMethod<migemo_closeDelegate>("migemo_close");
			}
			this._migemo_closeDelegate(handle);
		}

		private delegate IntPtr migemo_queryDelegate(IntPtr handle, string query);
		private migemo_queryDelegate _migemo_queryDelegate;
		private IntPtr migemo_query(IntPtr handle, string query){
			this.ThrowIfDisposed();
			if(this._migemo_queryDelegate == null){
				this._migemo_queryDelegate = this.LoadMethod<migemo_queryDelegate>("migemo_query");
			}
			return this._migemo_queryDelegate(handle, query);
		}
		
		private delegate void migemo_releaseDelegate(IntPtr handle, IntPtr pattern);
		private migemo_releaseDelegate _migemo_releaseDelegate;
		private void migemo_release(IntPtr handle, IntPtr pattern){
			this.ThrowIfDisposed();
			if(this._migemo_releaseDelegate == null){
				this._migemo_releaseDelegate = this.LoadMethod<migemo_releaseDelegate>("migemo_release");
			}
			this._migemo_releaseDelegate(handle, pattern);
		}

		private delegate int migemo_loadDelegate(IntPtr handle, MigemoDictionary id, string dict);
		private migemo_loadDelegate _migemo_loadDelegate;
		private int migemo_load(IntPtr handle, MigemoDictionary id, string dict){
			this.ThrowIfDisposed();
			if(this._migemo_loadDelegate == null){
				this._migemo_loadDelegate = this.LoadMethod<migemo_loadDelegate>("migemo_load");
			}
			return this._migemo_loadDelegate(handle, id, dict);
		}

		private delegate int migemo_is_enableDelegate(IntPtr handle);
		private migemo_is_enableDelegate _migemo_is_enableDelegate;
		private int migemo_is_enable(IntPtr handle){
			this.ThrowIfDisposed();
			if(this._migemo_is_enableDelegate == null){
				this._migemo_is_enableDelegate = this.LoadMethod<migemo_is_enableDelegate>("migemo_is_enable");
			}
			return this._migemo_is_enableDelegate(handle);
		}

		private delegate int migemo_set_operatorDelegate(IntPtr handle, MigemoOperator index, string op);
		private migemo_set_operatorDelegate _migemo_set_operatorDelegate;
		private int migemo_set_operator(IntPtr handle, MigemoOperator index, string op){
			this.ThrowIfDisposed();
			if(this._migemo_set_operatorDelegate == null){
				this._migemo_set_operatorDelegate = this.LoadMethod<migemo_set_operatorDelegate>("migemo_set_operator");
			}
			return this._migemo_set_operatorDelegate(handle, index, op);
		}

		private delegate string migemo_get_operatorDelegate(IntPtr handle, MigemoOperator index);
		private migemo_get_operatorDelegate _migemo_get_operatorDelegate;
		private string migemo_get_operator(IntPtr handle, MigemoOperator index){
			this.ThrowIfDisposed();
			if(this._migemo_get_operatorDelegate == null){
				this._migemo_get_operatorDelegate = this.LoadMethod<migemo_get_operatorDelegate>("migemo_get_operator");
			}
			return this._migemo_get_operatorDelegate(handle, index);
		}
		
		private const int MIGEMO_DICTID_INVALID = 0;

		#endregion

		/// <summary>
		/// 正規表現パターンを取得する。
		/// </summary>
		/// <param name="query">正規表現を作成する元の文字列</param>
		/// <returns>正規表現</returns>
		public string GetPattern(string query){
			IntPtr pPattern = migemo_query(this._MigemoHandle, query);
			string pattern = Marshal.PtrToStringAnsi(pPattern);
			migemo_release(this._MigemoHandle, pPattern);
			return pattern;
		}
		
		/// <summary>
		/// Migemoの辞書ファイルをロードする。
		/// </summary>
		/// <param name="id">辞書ファイルの種類</param>
		/// <param name="dict">辞書ファイルのパス</param>
		/// <returns>ロードに成功したかどうか</returns>
		public bool Load(MigemoDictionary id, string dict){
			return (migemo_load(this._MigemoHandle, id, dict) != MIGEMO_DICTID_INVALID);
		}
		
		/// <summary>
		/// Migemoが利用可能かどうかを取得する。
		/// </summary>
		public bool IsEnable{
			get{
				return (migemo_is_enable(this._MigemoHandle) != 0);
			}
		}
		
		/// <summary>
		/// Migemoが解釈する正規表現オペレータを設定する。
		/// </summary>
		/// <param name="index">オペレータ</param>
		/// <param name="op">オペレータとなる文字列</param>
		/// <returns>設定に成功したかどうか</returns>
		public bool SetOperator(MigemoOperator index, string op){
			return (migemo_set_operator(this._MigemoHandle, index, op) != 0);
		}
		
		/// <summary>
		/// Migemoが解釈する正規表現オペレータを取得する。
		/// </summary>
		/// <param name="index">オペレータ</param>
		/// <returns>オペレータとなる文字列</returns>
		public string GetOperator(MigemoOperator index){
			return migemo_get_operator(this._MigemoHandle, index);
		}
	}
	
	/// <summary>
	/// Migemoの辞書ファイルの種類。
	/// </summary>
	public enum MigemoDictionary : int{
		Migemo = 1,
		Roma2Hira = 2,
		Hira2Kata = 3,
		Han2Zen = 4,
	}
	
	/// <summary>
	/// 正規表現オペレータ。
	/// </summary>
	public enum MigemoOperator : int{
		Or = 0,
		In = 1,
		Out = 2,
		SelectIn = 3,
		SelectOut = 4,
		NewLine = 5,
	}
}*/