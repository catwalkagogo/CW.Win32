using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;

namespace CW.Win32 {
	using IO = System.IO;

	[StructLayout(LayoutKind.Sequential)]
	public class FileInformation {
		public FileInformation(string file){
			file = IO.Path.GetFullPath(file);
			using(var hFile = OpenFile(file)){
				var info = default(ByHandleFileInformation);
				if(Kernel32.GetFileInformationByHandle(hFile, out info)) {
					this.CreationTime = ToDateTime(info.CreationTime);
					this.LastWriteTime = ToDateTime(info.LastWriteTime);
					this.LastAccessTime = ToDateTime(info.LastAccessTime);
					this.VolumeSerialNumber = info.VolumeSerialNumber;
					this.Length = ToLong(info.FileSizeHigh, info.FileSizeLow);
					this.FileIndex = ToLong(info.FileIndexHigh, info.FileIndexLow);
				}else{
					throw new FileLoadException("GetFileInformationByHandle faild", new Win32Exception(Marshal.GetLastWin32Error()));
				}
			}
		}

		public DateTime CreationTime{get; private set;}
		public DateTime LastWriteTime{get; private set;}
		public DateTime LastAccessTime{get; private set;}
		public int VolumeSerialNumber{get; private set;}
		public long Length{get; private set;}
		public int LinkCount{get; private set;}
		public long FileIndex{get; private set;}

		#region Functions

		private static SafeFileHandle OpenFile(string file){
			var handle = Kernel32.CreateFileW(file, (FileAccess)0, FileShare.Delete | FileShare.Read, IntPtr.Zero, FileMode.Open, FileOptions.None, IntPtr.Zero);
			var eno = Marshal.GetLastWin32Error();
			if(handle == null || handle.IsInvalid){
				throw new FileLoadException("CreateFileW failed", new Win32Exception(eno));
			}
			return handle;
		}

		private static DateTime ToDateTime(FileTime fileTime){
			return (new DateTime(ToLong(fileTime.HighDateTime, fileTime.LowDateTime))).AddYears(1600);
		}

		private static long ToLong(int high, int low){
			return (((long)high) << 32) + low;
		}

		#endregion
	}

	public static partial class Kernel32 {
		[DllImport("Kernel32.dll", SetLastError = true, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetFileInformationByHandle(SafeFileHandle file, out ByHandleFileInformation fileInformation);

		[DllImport("Kernel32.dll", SetLastError = true, ExactSpelling = true)]
		public static extern SafeFileHandle CreateFileW(
			[In, MarshalAs(UnmanagedType.LPWStr)] string fileName,
			FileAccess desiredAccess,
			FileShare shareMode,
			IntPtr securityAttributes,
			FileMode createDisposition,
			FileOptions flagsAndAttributes,
			IntPtr templateFile);
	}

	#region Structs

	public struct ByHandleFileInformation {
		public IO.FileAttributes FileAttributes;
		public FileTime CreationTime;
		public FileTime LastWriteTime;
		public FileTime LastAccessTime;
		public int VolumeSerialNumber;
		public int FileSizeHigh;
		public int FileSizeLow;
		public int NumberOfLinks;
		public int FileIndexHigh;
		public int FileIndexLow;
	}

	public struct FileTime {
		public int LowDateTime;
		public int HighDateTime;
	}

	#endregion
}
