using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.IO;

namespace CW.Win32.Shell {
	public class FileOperation : ComObject<IFileOperation>{
		private static readonly Guid CLSID_FileOperation = new Guid("3ad05575-8857-4850-9277-11b85bdb8e09");
		private static readonly Type FileOperationType = Type.GetTypeFromCLSID(CLSID_FileOperation);
		private static Guid ShellItemGuid = typeof(IShellItem).GUID;

		private uint _SinkCookie = 0;
		public IntPtr Owner { get; set; }
		private FileOperationProgressSink _ProgressSink;

		public FileOperation() : base((IFileOperation)Activator.CreateInstance(FileOperationType)){
			this._ProgressSink = new FileOperationProgressSink();
			this._SinkCookie = this.Interface.Advise(this._ProgressSink.Interface);
		}

		public FileOperationProgressSink ProgressSink {
			get {
				return this._ProgressSink;
			}
		}

		public void Copy(string[] source, string destination) {
			this.ThrowIfDisposed();
			source.ThrowIfNull("source");
			destination.ThrowIfNull("destination");

			using(var array = ShellItemArray.FromFiles(source)) {
				using(var dest = ShellItem.FromPath(destination)){
					this.Interface.CopyItems(array.Interface, dest.Interface);
					Marshal.ThrowExceptionForHR(this.Interface.PerformOperations());
				}
			}
		}

		public void Move(string[] source, string destination) {
			this.ThrowIfDisposed();
			source.ThrowIfNull("source");
			destination.ThrowIfNull("destination");

			using(var array = ShellItemArray.FromFiles(source)) {
				using(var dest = ShellItem.FromPath(destination)) {
					this.Interface.MoveItems(array.Interface, dest.Interface);
					Marshal.ThrowExceptionForHR(this.Interface.PerformOperations());
				}
			}
		}

		public void Delete(string[] source) {
			this.ThrowIfDisposed();
			source.ThrowIfNull("source");

			using(var array = ShellItemArray.FromFiles(source)) {
				this.Interface.DeleteItems(array.Interface);
				Marshal.ThrowExceptionForHR(this.Interface.PerformOperations());
			}
		}

		public void Rename(string source, string newName) {
			this.ThrowIfDisposed();
			source.ThrowIfNull("source");
			newName.ThrowIfNull("newName");

			using(var item = ShellItem.FromPath(source)) {
				this.Interface.RenameItem(item.Interface, newName, null);
				Marshal.ThrowExceptionForHR(this.Interface.PerformOperations());
			}
		}

		public void Create(string parent, string name, FileAttributes attr, string templateName = null) {
			using(var parentItem = ShellItem.FromPath(parent)) {
				this.Interface.NewItem(parentItem.Interface, attr, name, templateName, null);
				Marshal.ThrowExceptionForHR(this.Interface.PerformOperations());
			}
		}

		public bool IsOperationAborted {
			get {
				return this.Interface.GetAnyOperationsAborted();
			}
		}

		protected override void Dispose(bool disposing) {
			if(this._SinkCookie != 0) {
				this.Interface.Unadvise(this._SinkCookie);
				this._SinkCookie = 0;
			}
			base.Dispose(disposing);
		}

		private FileOperationFlags _OperationFlags = FileOperationFlags.None;
		public FileOperationFlags OperationFlags {
			get {
				return this._OperationFlags;
			}
			set {
				this._OperationFlags = value;
				this.Interface.SetOperationFlags(value);
			}
		}
	}

	#region IFIleOperation

	[ComImport]
	[Guid("947aab5f-0a5c-4c13-b4d6-4bf7836fc9f8")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IFileOperation {
		uint Advise(IFileOperationProgressSink pfops);
		void Unadvise(uint dwCookie);

		void SetOperationFlags(FileOperationFlags dwOperationFlags);
		void SetProgressMessage(
			[MarshalAs(UnmanagedType.LPWStr)] string pszMessage);
		void SetProgressDialog(
			[MarshalAs(UnmanagedType.Interface)] object popd);
		void SetProperties(
			[MarshalAs(UnmanagedType.Interface)] object pproparray);
		void SetOwnerWindow(uint hwndParent);

		void ApplyPropertiesToItem(IShellItem psiItem);
		void ApplyPropertiesToItems(
			[MarshalAs(UnmanagedType.Interface)] object punkItems);

		void RenameItem(IShellItem psiItem,
			[MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
			IFileOperationProgressSink pfopsItem);
		void RenameItems(
			[MarshalAs(UnmanagedType.Interface)] object pUnkItems,
			[MarshalAs(UnmanagedType.LPWStr)] string pszNewName);

		void MoveItem(
			IShellItem psiItem,
			IShellItem psiDestinationFolder,
			[MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
			IFileOperationProgressSink pfopsItem);
		void MoveItems(
			[MarshalAs(UnmanagedType.Interface)] object punkItems,
			IShellItem psiDestinationFolder);

		void CopyItem(
			IShellItem psiItem,
			IShellItem psiDestinationFolder,
			[MarshalAs(UnmanagedType.LPWStr)] string pszCopyName,
			IFileOperationProgressSink pfopsItem);
		void CopyItems(
			[MarshalAs(UnmanagedType.Interface)] object punkItems,
			IShellItem psiDestinationFolder);

		void DeleteItem(
			IShellItem psiItem,
			IFileOperationProgressSink pfopsItem);
		void DeleteItems(
			[MarshalAs(UnmanagedType.Interface)] object punkItems);

		uint NewItem(
			IShellItem psiDestinationFolder,
			FileAttributes dwFileAttributes,
			[MarshalAs(UnmanagedType.LPWStr)] string pszName,
			[MarshalAs(UnmanagedType.LPWStr)] string pszTemplateName,
			IFileOperationProgressSink pfopsItem);

		int PerformOperations();

		[return: MarshalAs(UnmanagedType.Bool)]
		bool GetAnyOperationsAborted();
	}

	#endregion

	#region FileOperationFlags

	/// <summary>
	/// SHFileOperationで行う処理のオプション。
	/// </summary>
	[Flags]
	public enum FileOperationFlags : uint {
		None = 0x0000,
		MultiDestFiles = 0x0001,
		ConfirmMouse = 0x0002,
		Silent = 0x0004,
		RenameOnCollision = 0x0008,
		NoConfirmation = 0x0010,
		WantMappingHandle = 0x0020,
		AllowUndo = 0x0040,
		FilesOnly = 0x0080,
		SimpleProgress = 0x0100,
		NoConfirmMakeDirectory = 0x0200,
		NoErrorUI = 0x0400,
		NoCopySecurityAttributes = 0x0800,
		NoRecursion = 0x1000,
		NoConectedElements = 0x2000,
		WantNukeWarning = 0x4000,
		NoRecurseParsing = 0x8000,

		NoSkipJunktions = 0x00010000,  // Don't avoid binding to junctions (like Task folder, Recycle-Bin)
		PreferHardLink = 0x00020000,  // Create hard link if possible
		ShowElevationPrompt = 0x00040000,  // Show elevation prompts when error UI is disabled (use with FOF_NOERRORUI)
		EarlyFailture = 0x00100000,  // Fail operation as soon as a single error occurs rather than trying to process other items (applies only when using FOF_NOERRORUI)
		PreserveFileExtensions = 0x00200000,  // Rename collisions preserve file extns (use with FOF_RENAMEONCOLLISION)
		KeepNewerFiles = 0x00400000,  // Keep newer file on naming conflicts
		NoCopyHooks = 0x00800000,  // Don't use copy hooks
		NoMinimizeBox = 0x01000000,  // Don't allow minimizing the progress dialog
		MoveClsAcrossVolume = 0x02000000,  // Copy security information when performing a cross-volume move operation
		DontDisplaySourcePath = 0x04000000,  // Don't display the path of source file in progress dialog
		DontDisplayDestPath = 0x08000000,  // Don't display the path of destination file in progress dialog
	}

	#endregion
}
