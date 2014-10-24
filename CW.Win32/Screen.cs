using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CW.Win32 {
	public static class Screen {
		public static IEnumerable<ScreenInfo> GetMonitors(){
			var list = new List<ScreenInfo>();
			EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (hMonitor, hdcMonitor, lprcMonitor, dwData) => {
				var info = new MonitorInfoEx{
					cbSize = Marshal.SizeOf(typeof(MonitorInfoEx))
				};
				GetMonitorInfo(hMonitor, ref info);
				list.Add(new ScreenInfo(info));
			}, IntPtr.Zero);
			return list;
		}

		[DllImport("User32.dll")]
		private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr rect, EnumDisplayMonitorsCallback callback, IntPtr dwData);

		private delegate void EnumDisplayMonitorsCallback(IntPtr hMonir, IntPtr hdcMonitor, IntPtr lprcMonitor, IntPtr dwData);

		[DllImport("User32.dll")]
		private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx info);

		[StructLayout(LayoutKind.Sequential)]
		internal struct MonitorInfoEx{
			public int cbSize;
			public Rectangle rcMonitor;
			public Rectangle rcWork;
			public int dwFlags;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szMonitor;
		}

		public static ScreenInfo GetCurrentMonitor(Int32Rect rect){
			return GetMonitors().Select(mon => new {Rect = mon.ScreenArea.Intersect(rect), Monitor = mon})
				.OrderByDescending(x => x.Rect.Area)
				.Select(x => x.Monitor)
				.First();
		}
	}

	public class ScreenInfo : IEquatable<ScreenInfo>{
		public Int32Rect ScreenArea{get; private set;}
		public Int32Rect WorkingArea{get; private set;}
		public string Name{get; private set;}

		internal ScreenInfo(Screen.MonitorInfoEx info){
			this.ScreenArea = new Int32Rect(info.rcMonitor.Left, info.rcMonitor.Top, info.rcMonitor.Right - info.rcMonitor.Left, info.rcMonitor.Bottom - info.rcMonitor.Top);
			this.WorkingArea = new Int32Rect(info.rcWork.Left, info.rcWork.Top, info.rcWork.Right - info.rcWork.Left, info.rcWork.Bottom - info.rcWork.Top);
			this.Name = info.szMonitor;
		}

		#region IEquatable<MonitorInfo> Members

		public bool Equals(ScreenInfo other) {
			if(other == null){
				return false;
			}else{
				return (this.ScreenArea == other.ScreenArea && this.WorkingArea == other.WorkingArea && this.Name == other.Name);
			}
		}

		public override bool Equals(object obj) {
			if(obj == null){
				return false;
			}else{
				return this.Equals(obj as ScreenInfo);
			}
		}

		public override int GetHashCode() {
			return this.ScreenArea.GetHashCode() ^ this.WorkingArea.GetHashCode() ^ this.Name.GetHashCode();
		}

		public static bool operator ==(ScreenInfo a, ScreenInfo b){
			if(Object.ReferenceEquals(a, b)){
				return true;
			}else if(Object.ReferenceEquals(a, null)){
				return false;
			}else{
				return a.Equals(b);
			}
		}

		public static bool operator !=(ScreenInfo a, ScreenInfo b){
			return !(a == b);
		}

		#endregion
	}
}
