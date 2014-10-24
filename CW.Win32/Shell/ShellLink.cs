/*
	$Id: ShellLink.cs 269 2011-07-28 10:41:57Z catwalkagogo@gmail.com $
*/
using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace CW.Win32.Shell {
	using ComTypes = System.Runtime.InteropServices.ComTypes;
	
	// based on http://smdn.invisiblefulmoon.net/ikimasshoy/dotnettips/tips043.html
	public sealed class ShellLink : ComObject<IShellLink>{
		// �J�����g�t�@�C��
		private string currentFile;
		
		// �e��萔
		internal const int MAX_PATH = 260;
		
		internal const uint SLGP_SHORTPATH   = 0x0001; // �Z���`��(8.3�`��)�̃t�@�C�������擾����
		internal const uint SLGP_UNCPRIORITY = 0x0002; // UNC�p�X�����擾����
		internal const uint SLGP_RAWPATH	 = 0x0004; // ���ϐ��Ȃǂ��ϊ�����Ă��Ȃ��p�X�����擾����
		
		#region "�R���X�g���N�V�����E�f�X�g���N�V����"
		public ShellLink() : base(GetShellLink()){
			this.currentFile = "";
		}

		private static IShellLink GetShellLink() {
			return (IShellLink)(new ShellLinkObject());
		}

		/// <summary>
		/// �R���X�g���N�^
		/// </summary>
		/// <param name="linkFile">�V���[�g�J�b�g�t�@�C��</param>
		public ShellLink(string linkFile) : this(){
			this.Load(linkFile);
		}

		#endregion

		#region "�v���p�e�B"

		/// <summary>
		/// �J�����g�t�@�C���B
		/// </summary>
		public string CurrentFile{
			get{
				return currentFile;
			}
		}

		/// <summary>
		/// �V���[�g�J�b�g�̃����N��B
		/// </summary>
		public string TargetPath{
			get{		
				StringBuilder targetPath = new StringBuilder(MAX_PATH, MAX_PATH);
				ShellLinkFindData data = new ShellLinkFindData();
				this.Interface.GetPath(targetPath, targetPath.Capacity, ref data, SLGP_UNCPRIORITY);
				return targetPath.ToString();
			}
			set{
				this.Interface.SetPath(value);
			}
		}

		/// <summary>
		/// ��ƃf�B���N�g���B
		/// </summary>
		public string WorkingDirectory{
			get{
				StringBuilder workingDirectory = new StringBuilder(MAX_PATH, MAX_PATH);
				this.Interface.GetWorkingDirectory(workingDirectory, workingDirectory.Capacity);
				return workingDirectory.ToString();
			}
			set{
				this.Interface.SetWorkingDirectory(value);	
			}
		}

		/// <summary>
		/// �R�}���h���C�������B
		/// </summary>
		public string Arguments{
			get{
				StringBuilder arguments = new StringBuilder(MAX_PATH, MAX_PATH);
				this.Interface.GetArguments(arguments, arguments.Capacity);
				return arguments.ToString();
			}
			set{
				this.Interface.SetArguments(value);	
			}
		}

		/// <summary>
		/// �V���[�g�J�b�g�̐����B
		/// </summary>
		public string Description{
			get{
				StringBuilder description = new StringBuilder(MAX_PATH, MAX_PATH);
				this.Interface.GetDescription(description, description.Capacity);
				return description.ToString();
			}
			set{
				this.Interface.SetDescription(value);	
			}
		}

		/// <summary>
		/// �A�C�R���̃t�@�C���B
		/// </summary>
		public string IconFile{
			get{
				int iconIndex = 0;
				string iconFile = "";
				this.GetIconLocation(out iconFile, out iconIndex);
				return iconFile;
			}
			set{
				int iconIndex = 0;
				string iconFile = "";
				this.GetIconLocation(out iconFile, out iconIndex);
				this.SetIconLocation(value, iconIndex);
			}
		}

		/// <summary>
		/// �A�C�R���̃C���f�b�N�X�B
		/// </summary>
		public int IconIndex{
			get{
				int iconIndex = 0;
				string iconPath = "";
				this.GetIconLocation(out iconPath, out iconIndex);
				return iconIndex;
			}
			set{
				int iconIndex = 0;
				string iconPath = "";
				this.GetIconLocation(out iconPath, out iconIndex);
				this.SetIconLocation(iconPath, value);
			}
		}

		/// <summary>
		/// �A�C�R���̃t�@�C���ƃC���f�b�N�X���擾����
		/// </summary>
		/// <param name="iconFile">�A�C�R���̃t�@�C��</param>
		/// <param name="iconIndex">�A�C�R���̃C���f�b�N�X</param>
		private void GetIconLocation(out string iconFile, out int iconIndex){
			StringBuilder iconFileBuffer = new StringBuilder(MAX_PATH, MAX_PATH);
			this.Interface.GetIconLocation(iconFileBuffer, iconFileBuffer.Capacity, out iconIndex);
			iconFile = iconFileBuffer.ToString();
		}

		/// <summary>
		/// �A�C�R���̃t�@�C���ƃC���f�b�N�X��ݒ肷��
		/// </summary>
		/// <param name="iconFile">�A�C�R���̃t�@�C��</param>
		/// <param name="iconIndex">�A�C�R���̃C���f�b�N�X</param>
		private void SetIconLocation(string iconFile, int iconIndex){
			this.Interface.SetIconLocation(iconFile, iconIndex);
		}

		/// <summary>
		/// ���s���̃E�B���h�E�̑傫���B
		/// </summary>
		public ShellLinkDisplayMode DisplayMode{
			get{
				int showCmd = 0;
				this.Interface.GetShowCmd(out showCmd);	
				return (ShellLinkDisplayMode)showCmd;
			}
			set
			{
				this.Interface.SetShowCmd((int)value);
			}
		}

		/// <summary>
		/// �z�b�g�L�[�B
		/// </summary>
		public int HotKey{
			get{
				ushort hotKey = 0;
				this.Interface.GetHotkey(out hotKey);
				return (int)hotKey;
			}
			set{
				this.Interface.SetHotkey((ushort)value);
			}
		}

		#endregion

		#region "�ۑ��Ɠǂݍ���"

		/// <summary>
		/// IShellLink�C���^�[�t�F�C�X����L���X�g���ꂽIPersistFile�C���^�[�t�F�C�X���擾���܂��B
		/// </summary>
		/// <returns>IPersistFile�C���^�[�t�F�C�X�B�@�擾�ł��Ȃ������ꍇ��null�B</returns>
		private ComTypes.IPersistFile GetIPersistFile(){
			return this.Interface as ComTypes.IPersistFile;
		}

		/// <summary>
		/// �J�����g�t�@�C���ɃV���[�g�J�b�g��ۑ����܂��B
		/// </summary>
		/// <exception cref="COMException">IPersistFile�C���^�[�t�F�C�X���擾�ł��܂���ł����B</exception>
		public void Save()
		{
			this.Save(currentFile);
		}

		/// <summary>
		/// �w�肵���t�@�C���ɃV���[�g�J�b�g��ۑ����܂��B
		/// </summary>
		/// <param name="linkFile">�V���[�g�J�b�g��ۑ�����t�@�C��</param>
		/// <exception cref="COMException">IPersistFile�C���^�[�t�F�C�X���擾�ł��܂���ł����B</exception>
		public void Save(string linkFile){
			// IPersistFile�C���^�[�t�F�C�X���擾���ĕۑ�
			ComTypes.IPersistFile persistFile = GetIPersistFile();
			if(persistFile == null) throw new COMException("IPersistFile�C���^�[�t�F�C�X���擾�ł��܂���ł����B");
			persistFile.Save(linkFile, true);
			
			// �J�����g�t�@�C����ۑ�
			this.currentFile = linkFile;
		}

		/// <summary>
		/// �w�肵���t�@�C������V���[�g�J�b�g��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="linkFile">�V���[�g�J�b�g��ǂݍ��ރt�@�C��</param>
		/// <exception cref="FileNotFoundException">�t�@�C����������܂���B</exception>
		/// <exception cref="COMException">IPersistFile�C���^�[�t�F�C�X���擾�ł��܂���ł����B</exception>
		public void Load(string linkFile){
			this.Load(linkFile, IntPtr.Zero, ShellLinkResolveFlags.AnyMatch | ShellLinkResolveFlags.NoUI, 1);
		}

		/// <summary>
		/// �w�肵���t�@�C������V���[�g�J�b�g��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="linkFile">�V���[�g�J�b�g��ǂݍ��ރt�@�C��</param>
		/// <param name="hWnd">���̃R�[�h���Ăяo�����I�[�i�[�̃E�B���h�E�n���h��</param>
		/// <param name="resolveFlags">�V���[�g�J�b�g���̉����Ɋւ��铮���\���t���O</param>
		/// <exception cref="FileNotFoundException">�t�@�C����������܂���B</exception>
		/// <exception cref="COMException">IPersistFile�C���^�[�t�F�C�X���擾�ł��܂���ł����B</exception>
		public void Load(string linkFile, IntPtr hWnd, ShellLinkResolveFlags resolveFlags){
			this.Load(linkFile, hWnd, resolveFlags, 1);
		}

		/// <summary>
		/// �w�肵���t�@�C������V���[�g�J�b�g��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="linkFile">�V���[�g�J�b�g��ǂݍ��ރt�@�C��</param>
		/// <param name="hWnd">���̃R�[�h���Ăяo�����I�[�i�[�̃E�B���h�E�n���h��</param>
		/// <param name="resolveFlags">�V���[�g�J�b�g���̉����Ɋւ��铮���\���t���O</param>
		/// <param name="timeOut">SLR_NO_UI���w�肵���Ƃ��̃^�C���A�E�g�l(�~���b)</param>
		/// <exception cref="FileNotFoundException">�t�@�C����������܂���B</exception>
		/// <exception cref="COMException">IPersistFile�C���^�[�t�F�C�X���擾�ł��܂���ł����B</exception>
		public void Load(string linkFile, IntPtr hWnd, ShellLinkResolveFlags resolveFlags, TimeSpan timeOut){
			this.Load(linkFile, hWnd, resolveFlags, (int)timeOut.TotalMilliseconds);
		}
		
		/// <summary>
		/// �w�肵���t�@�C������V���[�g�J�b�g��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="linkFile">�V���[�g�J�b�g��ǂݍ��ރt�@�C��</param>
		/// <param name="hWnd">���̃R�[�h���Ăяo�����I�[�i�[�̃E�B���h�E�n���h��</param>
		/// <param name="resolveFlags">�V���[�g�J�b�g���̉����Ɋւ��铮���\���t���O</param>
		/// <param name="timeOutMilliseconds">SLR_NO_UI���w�肵���Ƃ��̃^�C���A�E�g�l(�~���b)</param>
		/// <exception cref="FileNotFoundException">�t�@�C����������܂���B</exception>
		/// <exception cref="COMException">IPersistFile�C���^�[�t�F�C�X���擾�ł��܂���ł����B</exception>
		public void Load(string linkFile, IntPtr hWnd, ShellLinkResolveFlags resolveFlags, int timeOutMilliseconds){
			if(!File.Exists(linkFile)) throw new FileNotFoundException("�t�@�C����������܂���B", linkFile);

			// IPersistFile�C���^�[�t�F�C�X���擾
			ComTypes.IPersistFile persistFile = GetIPersistFile();

			if(persistFile == null) throw new COMException("IPersistFile�C���^�[�t�F�C�X���擾�ł��܂���ł����B");

			// �ǂݍ���
			persistFile.Load(linkFile, 0x00000000);

			// �t���O������
			uint flags = (uint)resolveFlags;

			if((resolveFlags & ShellLinkResolveFlags.NoUI) == ShellLinkResolveFlags.NoUI){
				flags |= (uint)(timeOutMilliseconds << 16);
			}

			this.Interface.Resolve(hWnd, flags);

			// �J�����g�t�@�C�����w��
			currentFile = linkFile;
		}
		#endregion
	}

	[ComImport]
	[Guid("00021401-0000-0000-C000-000000000046")]
	[ClassInterface(ClassInterfaceType.None)]
	public class ShellLinkObject{
	}
	
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("000214F9-0000-0000-C000-000000000046")]
	public interface IShellLink{
		void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cch, [MarshalAs(UnmanagedType.Struct)] ref ShellLinkFindData pfd, uint fFlags);
		void GetIDList(out IntPtr ppidl);
		void SetIDList(IntPtr pidl);
		void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cch);
		void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
		void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cch);
		void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
		void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cch);
		void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
		void GetHotkey(out ushort pwHotkey);
		void SetHotkey(ushort wHotkey);
		void GetShowCmd(out int piShowCmd);
		void SetShowCmd(int iShowCmd);
		void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cch, out int piIcon);
		void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
		void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);
		void Resolve(IntPtr hwnd, uint flags);
		void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
	}
	
	[StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
	public struct ShellLinkFindData{
		public const int MAX_PATH = 260;
		public uint dwFileAttributes;
		public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
		public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
		public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
		public uint nFileSizeHigh;
		public uint nFileSizeLow;
		public uint dwReserved0;
		public uint dwReserved1;
		
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
		public string cFileName;
		
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
		public string cAlternateFileName;
	}
	
	/// <summary>
	/// ���s���̃E�B���h�E�̕\�����@��\���񋓌^�ł��B
	/// </summary>
	public enum ShellLinkDisplayMode : int{
		/// <summary>�ʏ�̑傫���̃E�B���h�E�ŋN�����܂��B</summary>
		Normal = 1,
		/// <summary>�ő剻���ꂽ��ԂŋN�����܂��B</summary>
		Maximized = 3,
		/// <summary>�ŏ������ꂽ��ԂŋN�����܂��B</summary>
		Minimized = 7,
	}
	
	/// <summary></summary>
	[Flags]
	public enum ShellLinkResolveFlags : int{
		/// <summary></summary>
		AnyMatch = 0x2,
		/// <summary></summary>
		InvokeMsi = 0x80,
		/// <summary></summary>
		NoLinkInfo = 0x40,
		/// <summary></summary>
		NoUI = 0x1,
		/// <summary></summary>
		NoUIWithMessagePump = 0x101,
		/// <summary></summary>
		NoUpdate = 0x8,
		/// <summary></summary>
		NoSearch = 0x10,
		/// <summary></summary>
		NoTrack = 0x20,
		/// <summary></summary>
		Update  = 0x4
	}
}