using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CW.Win32 {
	public static class Dwm {
		[DllImport("DwmApi.dll", EntryPoint="DwmExtendFrameIntoClientArea")]
		public static extern int ExtendFrameIntoClientArea(IntPtr hwnd, Margins pMarInset);

		[DllImport("DwmApi.dll", EntryPoint="DwmEnableBlurBehindWindow")]
		public static extern int EnableBlurBehindWindow(IntPtr hWnd, DwmBlurBehind pBlurBehind);
	}

	[StructLayout(LayoutKind.Sequential)]
	public sealed class Margins{
		private int _Left, _Right, _Top, _Bottom;

		public int Left{
			get{
				return this._Left;
			}
			set{
				this._Left = value;
			}
		}

		public int Right{
			get{
				return this._Right;
			}
			set{
				this._Right = value;
			}
		}

		public int Top{
			get{
				return this._Top;
			}
			set{
				this._Top = value;
			}
		}

		public int Bottom{
			get{
				return this._Bottom;
			}
			set{
				this._Bottom = value;
			}
		}
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DwmBlurBehind {
		private DwmBlurBehindOptions _dwFlags;
		private Boolean _fEnable;
		private IntPtr _hRgnBlur;
		private Boolean _fTransitionOnMaximized;

		public DwmBlurBehind() {
		}

		public bool Enabled {
			get { return _fEnable; }
			set {
				_fEnable = value;
				if(value){
					_dwFlags |= DwmBlurBehindOptions.Enable;
				}else{
					_dwFlags ^= (_dwFlags & DwmBlurBehindOptions.Enable);
				}
			}
		}

		public bool TransitionOnMaximized {
			get { return _fTransitionOnMaximized; }
			set {
				_fTransitionOnMaximized = value;
				if(value){
					_dwFlags |= DwmBlurBehindOptions.TransitionOnMaximized;
				}else{
					_dwFlags ^= (_dwFlags & DwmBlurBehindOptions.TransitionOnMaximized);
				}
			}
		}

		public IntPtr Region {
			get { return _hRgnBlur; }
			set {
				_hRgnBlur = value;
				if(value != IntPtr.Zero){
					_dwFlags |= DwmBlurBehindOptions.BlurRegion;
				}else{
					_dwFlags ^= (_dwFlags & DwmBlurBehindOptions.BlurRegion);
				}
			}
		}

		[Serializable, Flags]
		private enum DwmBlurBehindOptions{
			None = 0,
			Enable = 1,
			BlurRegion = 2,
			TransitionOnMaximized = 4,
		}
	}
}
