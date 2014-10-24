using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CW.Win32 {
	public class HotKeyManager : DisposableObject{
		private IDictionary<int, HotKey> _HotKeys = new Dictionary<int,HotKey>();
		public IntPtr Handle{get; set;}

		public HotKeyManager(IntPtr hwnd){
			//hwnd.ThrowIfNull("hwnd");
		}

		public event EventHandler<HotKeyEventArgs> HotKeyPressed;

		protected virtual void OnHotKeyPressed(HotKeyEventArgs e){
			var handler = this.HotKeyPressed;
			if(handler != null){
				handler(this, e);
			}
		}
		public HotKey Register(HotKeyModifiers mods, Keys key){
			return this.Register(mods, key, null);
		}
		public HotKey Register(HotKeyModifiers mods, Keys key, EventHandler<HotKeyEventArgs> pressed){
			this.ThrowIfDisposed();

			var atom = this.GetAtom(mods, key);
			bool success = User32.RegisterHotKey(this.Handle, atom.Id, mods, key);
			HotKey hotkey;
			if(success){
				hotkey = new HotKey(this, atom, mods, key);
				this._HotKeys.Add(atom.Id, hotkey);
			}else{
				hotkey = this._HotKeys[atom.Id];
			}
			if(pressed != null){
				hotkey.Pressed += pressed;
			}
			return hotkey;
		}

		public void Unregister(HotKey hotkey){
			this.ThrowIfDisposed();

			if(hotkey.Manager != this){
				throw new ArgumentException("hotkey");
			}
			hotkey.Dispose();
		}

		public void Unregister(HotKey hotkey, EventHandler<HotKeyEventArgs> pressed){
			hotkey.Pressed -= pressed;
		}

		public void Unregister(HotKeyModifiers mods, Keys key, EventHandler<HotKeyEventArgs> pressed){
			using(var atom = this.GetAtom(mods, key)){
				HotKey hotkey;
				if(this._HotKeys.TryGetValue(atom.Id, out hotkey)){
					hotkey.Pressed -= pressed;
				}
			}
		}

		private Atom GetAtom(HotKeyModifiers mods, Keys key){
			return new Atom(this.Handle.ToString() + "_" + mods.ToString() + "_" + key.ToString());
		}


		public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled){
			const int WM_HOTKEY = (int)WindowMessage.HotKey;

			if(msg == WM_HOTKEY){
				HotKey hotkey;
				if(this._HotKeys.TryGetValue(wParam.ToInt32(), out hotkey)){
					var e = new HotKeyEventArgs(hotkey);
					hotkey.OnPressed(e);
					this.OnHotKeyPressed(e);
				}
			}
			return IntPtr.Zero;
		}

		public IEnumerable<HotKey> HotKeys{
			get{
				return this._HotKeys.Values;
			}
		}

		protected override void Dispose(bool disposing) {
			if(!this.IsDisposed){
				var hotkeys = this.HotKeys.ToArray();
				foreach(var hotkey in hotkeys){
					this.Unregister(hotkey);
				}
			}
			base.Dispose(disposing);
		}

		internal void UnregisterInternal(HotKey hotkey) {
			User32.UnregisterHotKey(this.Handle, hotkey.Atom.Id);
			this._HotKeys.Remove(hotkey.Atom.Id);
			hotkey.Atom.Dispose();
		}
	}

	public class HotKey : DisposableObject{
		public HotKeyModifiers Modifiers{get; private set;}
		public Keys Keys{get; private set;}
		internal Atom Atom{get; private set;}
		internal HotKeyManager Manager{get; private set;}
		internal EventHandler<HotKeyEventArgs> Pressed;

		internal void OnPressed(HotKeyEventArgs e){
			var handler = this.Pressed;
			if(handler != null){
				handler(this.Manager, e);
			}
		}

		internal HotKey(HotKeyManager manager, Atom atom, HotKeyModifiers mods, Keys keys){
			this.Manager = manager;
			this.Modifiers = mods;
			this.Keys = keys;
			this.Atom = atom;
		}

		protected override void Dispose(bool disposing) {
			if(!this.IsDisposed){
				this.Manager.UnregisterInternal(this);
			}
			base.Dispose(disposing);
		}
	}

	public class HotKeyEventArgs : EventArgs{
		public HotKey HotKey{get; private set;}

		public HotKeyEventArgs(HotKey hotkey){
			hotkey.ThrowIfNull("hotkey");
			this.HotKey = hotkey;
		}
	}

	public static partial class User32{
		[DllImport("user32")]
		public static extern bool RegisterHotKey(IntPtr hwnd, int id, HotKeyModifiers mods, Keys keys);
		[DllImport("user32")]
		public static extern bool UnregisterHotKey(IntPtr hwnd, int id);
	}

	public static partial class Kernel32{
		[DllImport("kernel32")]
		public static extern int GlobalAddAtom(string str);
		[DllImport("kernel32")]
		public static extern int GlobalDeleteAtom(int atom);
	}

	[Flags]
	public enum HotKeyModifiers{
		Alt = 0x0001,
		Control = 0x0002,
		Shift = 0x0004,
		Windows = 0x0008,
	}

	[Flags]
	public enum Keys
	{
		KeyCode = 65535,
		Modifiers = -65536,
		None = 0,
		LButton = 1,
		RButton = 2,
		Cancel = 3,
		MButton = 4,
		XButton1 = 5,
		XButton2 = 6,
		Back = 8,
		Tab = 9,
		LineFeed = 10,
		Clear = 12,
		Return = 13,
		Enter = 13,
		ShiftKey = 16,
		ControlKey = 17,
		Menu = 18,
		Pause = 19,
		Capital = 20,
		CapsLock = 20,
		KanaMode = 21,
		HanguelMode = 21,
		HangulMode = 21,
		JunjaMode = 23,
		FinalMode = 24,
		HanjaMode = 25,
		KanjiMode = 25,
		Escape = 27,
		IMEConvert = 28,
		IMENonconvert = 29,
		IMEAccept = 30,
		IMEAceept = 30,
		IMEModeChange = 31,
		Space = 32,
		Prior = 33,
		PageUp = 33,
		Next = 34,
		PageDown = 34,
		End = 35,
		Home = 36,
		Left = 37,
		Up = 38,
		Right = 39,
		Down = 40,
		Select = 41,
		Print = 42,
		Execute = 43,
		Snapshot = 44,
		PrintScreen = 44,
		Insert = 45,
		Delete = 46,
		Help = 47,
		D0 = 48,
		D1 = 49,
		D2 = 50,
		D3 = 51,
		D4 = 52,
		D5 = 53,
		D6 = 54,
		D7 = 55,
		D8 = 56,
		D9 = 57,
		A = 65,
		B = 66,
		C = 67,
		D = 68,
		E = 69,
		F = 70,
		G = 71,
		H = 72,
		I = 73,
		J = 74,
		K = 75,
		L = 76,
		M = 77,
		N = 78,
		O = 79,
		P = 80,
		Q = 81,
		R = 82,
		S = 83,
		T = 84,
		U = 85,
		V = 86,
		W = 87,
		X = 88,
		Y = 89,
		Z = 90,
		LWin = 91,
		RWin = 92,
		Apps = 93,
		Sleep = 95,
		NumPad0 = 96,
		NumPad1 = 97,
		NumPad2 = 98,
		NumPad3 = 99,
		NumPad4 = 100,
		NumPad5 = 101,
		NumPad6 = 102,
		NumPad7 = 103,
		NumPad8 = 104,
		NumPad9 = 105,
		Multiply = 106,
		Add = 107,
		Separator = 108,
		Subtract = 109,
		Decimal = 110,
		Divide = 111,
		F1 = 112,
		F2 = 113,
		F3 = 114,
		F4 = 115,
		F5 = 116,
		F6 = 117,
		F7 = 118,
		F8 = 119,
		F9 = 120,
		F10 = 121,
		F11 = 122,
		F12 = 123,
		F13 = 124,
		F14 = 125,
		F15 = 126,
		F16 = 127,
		F17 = 128,
		F18 = 129,
		F19 = 130,
		F20 = 131,
		F21 = 132,
		F22 = 133,
		F23 = 134,
		F24 = 135,
		NumLock = 144,
		Scroll = 145,
		LShiftKey = 160,
		RShiftKey = 161,
		LControlKey = 162,
		RControlKey = 163,
		LMenu = 164,
		RMenu = 165,
		BrowserBack = 166,
		BrowserForward = 167,
		BrowserRefresh = 168,
		BrowserStop = 169,
		BrowserSearch = 170,
		BrowserFavorites = 171,
		BrowserHome = 172,
		VolumeMute = 173,
		VolumeDown = 174,
		VolumeUp = 175,
		MediaNextTrack = 176,
		MediaPreviousTrack = 177,
		MediaStop = 178,
		MediaPlayPause = 179,
		LaunchMail = 180,
		SelectMedia = 181,
		LaunchApplication1 = 182,
		LaunchApplication2 = 183,
		OemSemicolon = 186,
		Oem1 = 186,
		Oemplus = 187,
		Oemcomma = 188,
		OemMinus = 189,
		OemPeriod = 190,
		OemQuestion = 191,
		Oem2 = 191,
		Oemtilde = 192,
		Oem3 = 192,
		OemOpenBrackets = 219,
		Oem4 = 219,
		OemPipe = 220,
		Oem5 = 220,
		OemCloseBrackets = 221,
		Oem6 = 221,
		OemQuotes = 222,
		Oem7 = 222,
		Oem8 = 223,
		OemBackslash = 226,
		Oem102 = 226,
		ProcessKey = 229,
		Packet = 231,
		Attn = 246,
		Crsel = 247,
		Exsel = 248,
		EraseEof = 249,
		Play = 250,
		Zoom = 251,
		NoName = 252,
		Pa1 = 253,
		OemClear = 254,
		Shift = 65536,
		Control = 131072,
		Alt = 262144
	}
}
