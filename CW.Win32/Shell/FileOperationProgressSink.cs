using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CW.Win32.Shell {
	public class FileOperationProgressSink {
		public FileOperationProgressSink() {
			this.Interface = new FileOperationProgressSinkImpl(this);
		}

		#region Started

		public event EventHandler Started;
		protected virtual void OnStarted(EventArgs e) {
			var handler = this.Started;
			if(handler != null) {
				handler(this, e);
			}
		}

		#endregion

		#region Completed

		public event EventHandler<FileOperationResultEventArgs> Completed;
		protected virtual void OnCompleted(FileOperationResultEventArgs e) {
			var handler = this.Completed;
			if(handler != null) {
				handler(this, e);
			}
		}

		#endregion

		#region Delete

		public event EventHandler<FileOperationItemEventArgs> Deleting;
		protected virtual void OnDeleting(FileOperationItemEventArgs e) {
			var handler = this.Deleting;
			if(handler != null) {
				handler(this, e);
			}
		}

		public event EventHandler<FileOperationResultItemEventArgs> Deleted;
		protected virtual void OnDeleted(FileOperationResultItemEventArgs e) {
			var handler = this.Deleted;
			if(handler != null) {
				handler(this, e);
			}
		}

		#endregion

		#region Rename

		public event EventHandler<FileOperationItemEventArgs> Renaming;
		protected virtual void OnRenaming(FileOperationItemEventArgs e) {
			var handler = this.Renaming;
			if(handler != null) {
				handler(this, e);
			}
		}

		public event EventHandler<FileOperationResultItemEventArgs> Renamed;
		protected virtual void OnRenamed(FileOperationResultItemEventArgs e) {
			var handler = this.Renamed;
			if(handler != null) {
				handler(this, e);
			}
		}

		#endregion

		#region Copy

		public event EventHandler<FileOperationItemEventArgs> Copying;
		protected virtual void OnCopying(FileOperationItemEventArgs e) {
			var handler = this.Copying;
			if(handler != null) {
				handler(this, e);
			}
		}

		public event EventHandler<FileOperationResultItemEventArgs> Copied;
		protected virtual void OnCopied(FileOperationResultItemEventArgs e) {
			var handler = this.Copied;
			if(handler != null) {
				handler(this, e);
			}
		}

		#endregion

		#region Move

		public event EventHandler<FileOperationItemEventArgs> Moving;
		protected virtual void OnMoving(FileOperationItemEventArgs e) {
			var handler = this.Moving;
			if(handler != null) {
				handler(this, e);
			}
		}

		public event EventHandler<FileOperationResultItemEventArgs> Moved;
		protected virtual void OnMoved(FileOperationResultItemEventArgs e) {
			var handler = this.Moved;
			if(handler != null) {
				handler(this, e);
			}
		}

		#endregion

		#region NewItem

		public event EventHandler<FileOperationItemEventArgs> Creating;
		protected virtual void OnCreating(FileOperationItemEventArgs e) {
			var handler = this.Creating;
			if(handler != null) {
				handler(this, e);
			}
		}

		public event EventHandler<FileOperationResultItemEventArgs> Created;
		protected virtual void OnCreated(FileOperationResultItemEventArgs e) {
			var handler = this.Created;
			if(handler != null) {
				handler(this, e);
			}
		}

		#endregion

		#region Progress

		public event EventHandler<FileOperationProgressEventArgs> ProgressChanged;

		public void OnProgressChanged(FileOperationProgressEventArgs e) {
			var handler = this.ProgressChanged;
			if(handler != null) {
				handler(this, e);
			}
		}

		#endregion

		internal FileOperationProgressSinkImpl Interface { get; private set; }

		internal class FileOperationProgressSinkImpl : IFileOperationProgressSink {
			private FileOperationProgressSink _Sink;

			internal FileOperationProgressSinkImpl(FileOperationProgressSink sink) {
				sink.ThrowIfNull("sink");
				this._Sink = sink;
			}

			#region IFileOperationProgressSink Members

			public void StartOperations() {
				this._Sink.OnStarted(EventArgs.Empty);
			}

			public void FinishOperations(int hrResult) {
				this._Sink.OnCompleted(new FileOperationResultEventArgs(hrResult));
			}

			private const int S_OK = 0x00000000;
			private const int S_FALSE = 0x00000001;

			public int PreRenameItem(TransferSourceFlags dwFlags, IShellItem psiItem, string pszNewName) {
				var e = new FileOperationItemEventArgs(dwFlags, psiItem, pszNewName, null);
				this._Sink.OnRenaming(e);
				return e.Cancel ? S_FALSE : S_OK;
			}

			public int PostRenameItem(TransferSourceFlags dwFlags, IShellItem psiItem, string pszNewName, int hrRename, IShellItem psiNewlyCreated) {
				var e = new FileOperationResultItemEventArgs(hrRename, dwFlags, psiItem, pszNewName, psiNewlyCreated, null);
				this._Sink.OnRenamed(e);
				return e.Cancel ? S_FALSE : S_OK;
			}

			public int PreMoveItem(TransferSourceFlags dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, string pszNewName) {
				var e = new FileOperationItemEventArgs(dwFlags, psiItem, pszNewName, psiDestinationFolder);
				this._Sink.OnMoving(e);
				return e.Cancel ? S_FALSE : S_OK;
			}

			public int PostMoveItem(TransferSourceFlags dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, string pszNewName, int hrMove, IShellItem psiNewlyCreated) {
				var e = new FileOperationResultItemEventArgs(hrMove, dwFlags, psiItem, pszNewName, psiNewlyCreated, psiDestinationFolder);
				this._Sink.OnMoved(e);
				return e.Cancel ? S_FALSE : S_OK;
			}

			public int PreCopyItem(TransferSourceFlags dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, string pszNewName) {
				var e = new FileOperationItemEventArgs(dwFlags, psiItem, pszNewName, psiDestinationFolder);
				this._Sink.OnCopying(e);
				return e.Cancel ? S_FALSE : S_OK;
			}

			public int PostCopyItem(TransferSourceFlags dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, string pszNewName, int hrCopy, IShellItem psiNewlyCreated) {
				var e = new FileOperationResultItemEventArgs(hrCopy, dwFlags, psiItem, pszNewName, psiNewlyCreated, psiDestinationFolder);
				this._Sink.OnCopied(e);
				return e.Cancel ? S_FALSE : S_OK;
			}

			public int PreDeleteItem(TransferSourceFlags dwFlags, IShellItem psiItem) {
				var e = new FileOperationItemEventArgs(dwFlags, psiItem, null, null);
				this._Sink.OnDeleting(e);
				return e.Cancel ? S_FALSE : S_OK;
			}

			public int PostDeleteItem(TransferSourceFlags dwFlags, IShellItem psiItem, int hrDelete, IShellItem psiNewlyCreated) {
				var e = new FileOperationResultItemEventArgs(hrDelete, dwFlags, psiItem, null, psiNewlyCreated, null);
				this._Sink.OnDeleted(e);
				return e.Cancel ? S_FALSE : S_OK;
			}

			public int PreNewItem(TransferSourceFlags dwFlags, IShellItem psiDestinationFolder, string pszNewName) {
				var e = new FileOperationItemEventArgs(dwFlags, null, pszNewName, psiDestinationFolder);
				this._Sink.OnCreating(e);
				return e.Cancel ? S_FALSE : S_OK;
			}

			public int PostNewItem(TransferSourceFlags dwFlags, IShellItem psiDestinationFolder, string pszNewName, string pszTemplateName, System.IO.FileAttributes dwFileAttributes, int hrNew, IShellItem psiNewItem) {
				var e = new FileOperationResultItemEventArgs(hrNew, dwFlags, null, pszNewName, psiNewItem, psiDestinationFolder);
				this._Sink.OnCreated(e);
				return e.Cancel ? S_FALSE : S_OK;
			}

			public void UpdateProgress(int iWorkTotal, int iWorkSoFar) {
				this._Sink.OnProgressChanged(new FileOperationProgressEventArgs(iWorkTotal, iWorkSoFar));
			}

			public void ResetTimer() {
			}

			public void PauseTimer() {
			}

			public void ResumeTimer() {
			}

			#endregion
		}
	}

	#region EventArgs

	public class FileOperationProgressEventArgs : EventArgs {
		public int WorkSoFar { get; private set; }
		public int WorkTotal { get; private set; }
		public double Progress {
			get {
				return (double)this.WorkSoFar / (double)this.WorkTotal;
			}
		}

		public FileOperationProgressEventArgs(int workTotal, int workSoFar) {
			this.WorkTotal = workTotal;
			this.WorkSoFar = workSoFar;
		}
	}

	public class FileOperationResultEventArgs : CancelEventArgs {
		public int HResult { get; private set; }

		public FileOperationResultEventArgs(int hresult){
			this.HResult = hresult;
		}
	}

	public class FileOperationResultItemEventArgs : FileOperationResultEventArgs {
		public TransferSourceFlags TransferSourceFlags { get; private set; }
		public IShellItem Item{get; private set;}
		public string NewName { get; private set; }
		public IShellItem NewItem{get; private set;}
		public IShellItem DestinationFolder { get; private set; }

		public FileOperationResultItemEventArgs(int hresult, TransferSourceFlags flags, IShellItem item, string newName, IShellItem newItem, IShellItem destinationFolder)
			: base(hresult) {
			this.TransferSourceFlags = flags;
			this.Item = item;
			this.NewName = newName;
			this.NewItem = newItem;
			this.DestinationFolder = destinationFolder;
		}
	}

	public class FileOperationItemEventArgs : CancelEventArgs {
		public TransferSourceFlags TransferSourceFlags { get; private set; }
		public IShellItem Item { get; private set; }
		public string NewName { get; private set; }
		public IShellItem DestinationFolder { get; private set; }

		public FileOperationItemEventArgs(TransferSourceFlags flags, IShellItem item, string newName, IShellItem destinationFolder) {
			this.TransferSourceFlags = flags;
			this.Item = item;
			this.NewName = newName;
			this.DestinationFolder = destinationFolder;
		}
	}

	#endregion

	#region IFileOperationProgressSink

	[ComImport]
	[Guid("04b0f1a7-9490-44bc-96e1-4296a31252e2")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IFileOperationProgressSink {
		void StartOperations();
		void FinishOperations(int hrResult);

		int PreRenameItem(TransferSourceFlags dwFlags, IShellItem psiItem,
			[MarshalAs(UnmanagedType.LPWStr)] string pszNewName);
		int PostRenameItem(TransferSourceFlags dwFlags, IShellItem psiItem,
			[MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
			int hrRename, IShellItem psiNewlyCreated);

		int PreMoveItem(TransferSourceFlags dwFlags, IShellItem psiItem, IShellItem
			psiDestinationFolder,
			[MarshalAs(UnmanagedType.LPWStr)] string pszNewName);
		int PostMoveItem(TransferSourceFlags dwFlags, IShellItem psiItem,
			IShellItem psiDestinationFolder,
			[MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
			int hrMove, IShellItem psiNewlyCreated);

		int PreCopyItem(TransferSourceFlags dwFlags, IShellItem psiItem,
			IShellItem psiDestinationFolder,
			[MarshalAs(UnmanagedType.LPWStr)] string pszNewName);
		int PostCopyItem(TransferSourceFlags dwFlags, IShellItem psiItem,
			IShellItem psiDestinationFolder,
			[MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
			int hrCopy, IShellItem psiNewlyCreated);

		int PreDeleteItem(TransferSourceFlags dwFlags, IShellItem psiItem);
		int PostDeleteItem(TransferSourceFlags dwFlags, IShellItem psiItem, int hrDelete,
			IShellItem psiNewlyCreated);

		int PreNewItem(TransferSourceFlags dwFlags, IShellItem psiDestinationFolder,
			[MarshalAs(UnmanagedType.LPWStr)] string pszNewName);
		int PostNewItem(TransferSourceFlags dwFlags, IShellItem psiDestinationFolder,
			[MarshalAs(UnmanagedType.LPWStr)] string pszNewName,
			[MarshalAs(UnmanagedType.LPWStr)] string pszTemplateName,
			System.IO.FileAttributes dwFileAttributes, int hrNew, IShellItem psiNewItem);

		void UpdateProgress(int iWorkTotal, int iWorkSoFar);

		void ResetTimer();
		void PauseTimer();
		void ResumeTimer();
	}

	#endregion

	#region TransferSourceFlags

	[Flags]
	public enum TransferSourceFlags : uint {
		Normal = 0,
		FailExist = 0,
		RenameExist = 0x1,
		OverwriteExists = 0x2,
		AllowDecryption = 0x4,
		NoSecurity = 0x8,
		CopyCreationTime = 0x10,
		CopyWriteTime = 0x20,
		UseFullAccess = 0x40,
		DeleteRecycleIfPossible = 0x80,
		CopyHardLink = 0x100,
		CopyLocalizedName = 0x200,
		MoveAsCopyDelete = 0x400,
		SuspendShellEvents = 0x800
	}

	#endregion
}
