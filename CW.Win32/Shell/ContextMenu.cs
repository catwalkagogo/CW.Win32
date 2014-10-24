using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CW.Win32.Shell {
	public class ContextMenu : ComObject<IContextMenu>{
		public readonly static Guid IID = new Guid("{000214e4-0000-0000-c000-000000000046}");

		public ContextMenu(IContextMenu contextMenu) : base(contextMenu){}

		public void InvokeCommand(int cmdId, IntPtr hwnd, ShowWindowCommand nShow) {
			InvokeCommandInfo info = new InvokeCommandInfo();
			info.Hwnd = hwnd;
			info.Verb = new IntPtr(cmdId);
			info.Size = Marshal.SizeOf(info);
			info.Show = ShowWindowCommand.ShowNormal;

			this.Interface.InvokeCommand(ref info);
		}
	}
}
