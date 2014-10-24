using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.IO;
using Microsoft.Win32;

namespace CW.Win32 {
	/// <summary>
	/// Flow
	/// 
	/// * = need Administrator rights
	/// -init app
	/// RegisterApplication* (if needed)
	/// 
	/// -init ext
	/// RegisterFileAssociationCapability* (if needed)
	/// RegisterAppRegisterName* (if needed)
	/// 
	/// -each file
	/// RegisterAppRegisterNameToCurrentUser (if needed)
	/// SetAppAsDefault
	/// </summary>
	public static class ApplicationAssociation {
		private static WeakReference _Instance;
		private static IApplicationAssociationRegistration Instance{
			get{
				object instance;
				if(_Instance == null || (instance = _Instance.Target) == null){
					instance = new ComObject<IApplicationAssociationRegistration>((IApplicationAssociationRegistration)new ApplicationAssociationRegistration());
					_Instance = new WeakReference(instance);
				}
				return ((ComObject<IApplicationAssociationRegistration>)instance).Interface;
			}
		}
		
		private static bool IsVistaOrHigher{
			get{
				return (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6);
			}
		}

		/// <summary>
		/// Supports Vista or higher.
		/// </summary>
		/// <param name="extOrProto"></param>
		/// <param name="assocType"></param>
		/// <param name="assocLevel"></param>
		/// <returns></returns>
		public static string GetCurrentDefault(string extOrProto, AssociationType assocType, AssociationLevel assocLevel){
			if(IsVistaOrHigher){
				string name;
				Marshal.ThrowExceptionForHR(Instance.QueryCurrentDefault(extOrProto, assocType, assocLevel, out name));
				return name;
			}else{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Supports Vista or higher.
		/// </summary>
		/// <param name="appRegisterName"></param>
		/// <param name="extOrProto"></param>
		/// <param name="assocType"></param>
		/// <param name="assocLevel"></param>
		/// <returns></returns>
		public static bool IsAppDefault(string appRegisterName, string extOrProto, AssociationType assocType, AssociationLevel assocLevel){
			if(IsVistaOrHigher){
				try{
					bool b;
					Marshal.ThrowExceptionForHR(Instance.QueryAppIsDefault(extOrProto, assocType, assocLevel, appRegisterName, out b));
					return b;
				}catch(FileNotFoundException){
					return false;
				}
			}else{
				object obj;
				Marshal.ThrowExceptionForHR(AssocCreate(CLSID_QueryAssociations, ref IID_IQueryAssociations, out obj));
				var assoc = (IQueryAssociations)obj;
				
				assoc.Init(AssociationInitializeOptions.None, extOrProto, IntPtr.Zero, IntPtr.Zero);

				StringBuilder sb = null;
				const AssociationString en = AssociationString.FriendlyApplicationName;
				int len;
				assoc.GetString(AssociationOptions.None, en, null, null, out len);
				sb = new StringBuilder(len);
				assoc.GetString(AssociationOptions.None, en, null, sb, out len);
				sb = null;

				return sb.ToString() == appRegisterName;
			}
		}

		private static readonly Guid CLSID_QueryAssociations = new Guid("a07034fd-6caa-4954-ac3f-97a27216f98a");
		private static Guid IID_IQueryAssociations = new Guid("c46ca590-3c3f-11d2-bee6-0000f805ca57");

		[DllImport("shlwapi.dll")]
		private static extern int AssocCreate(Guid clsid, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppv);

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// Supports Vista or higher.
		/// </remarks>
		/// <param name="appRegisterName"></param>
		/// <param name="assocLevel"></param>
		/// <returns></returns>
		public static bool IsAppDefaultAll(string appRegisterName, AssociationLevel assocLevel){
			if(IsVistaOrHigher){
				bool b;
				Marshal.ThrowExceptionForHR(Instance.QueryAppIsDefaultAll(assocLevel, appRegisterName, out b));
				return b;
			}else{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Set app as default
		/// </summary>
		/// <remarks>
		/// Supports Windows XP or higher (might be 95 or higher)
		/// </remarks>
		/// <param name="appRegisterName"></param>
		/// <param name="extOrProto">extension starts with dot or protocol name</param>
		/// <param name="assocType"></param>
		public static void SetAppAsDefault(string appRegisterName, string extOrProto, AssociationType assocType, string progId){
			if(IsVistaOrHigher){
				Marshal.ThrowExceptionForHR(Instance.SetAppAsDefault(appRegisterName, extOrProto, assocType));
			}else{
				using(var regExtKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + extOrProto)){
					regExtKey.SetValue("ProgID", progId);
				}
				//using(var regExtKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + extOrProto)){
				//	regExtKey.SetValue(null, appRegisterName);
				//}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// Supports Vista or higher.
		/// </remarks>
		/// <param name="appRegisterName"></param>
		public static void SetAppAsDefaultAll(string appRegisterName){
			if(IsVistaOrHigher){
				Marshal.ThrowExceptionForHR(Instance.SetAppAsDefaultAll(appRegisterName));
			}else{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// Supports Vista or higher.
		/// </remarks>
		public static void ClearUserAssociations(){
			if(IsVistaOrHigher){
				Marshal.ThrowExceptionForHR(Instance.ClearUserAssociations());
			}else{
				throw new NotSupportedException();
			}
		}

		public static void ShowAssociationRegistrationUI(string appRegisterName){
			if(IsVistaOrHigher){
				IApplicationAssociationRegistrationUI ui = null;
				try{
					ui = (IApplicationAssociationRegistrationUI)new ApplicationAssociationRegistrationUI();
					Marshal.ThrowExceptionForHR(ui.LaunchAdvancedAssociationUI(appRegisterName));
				}finally{
					if(ui != null){
						Marshal.ReleaseComObject(ui);
					}
				}
			}else{
				throw new NotSupportedException();
			}
		}

		private const string RegisteredApplicationsPath = @"Software\RegisteredApplications";

		/// <summary>
		/// Register Application's Capabilities.
		/// </summary>
		/// <param name="appName"></param>
		/// <param name="company"></param>
		/// <param name="description"></param>
		public static void RegisterApplication(string appName, string company, string description){
			var companyAppName = (String.IsNullOrWhiteSpace(company)) ? appName : company + "\\" + appName;
			var softwareRegPath = @"Software\" + companyAppName;
			var capabilitiesPath = softwareRegPath + @"\Capabilities";

			// Capabilities
			using(var regCapKey = Registry.LocalMachine.CreateSubKey(capabilitiesPath)){
				regCapKey.SetValue("ApplicationName", appName);
				regCapKey.SetValue("ApplicationDescription", description);
			}

			// RegisteredApplications
			using(var regAppsKey = Registry.LocalMachine.CreateSubKey(RegisteredApplicationsPath)){
				regAppsKey.SetValue(appName, capabilitiesPath);
			}
		}

		public static bool IsApplicationRegistered(string appName, string company, string description){
			var companyAppName = (String.IsNullOrWhiteSpace(company)) ? appName : company + "\\" + appName;
			var softwareRegPath = @"Software\" + companyAppName;
			var capabilitiesPath = softwareRegPath + @"\Capabilities";

			// Capabilities
			var regCapKey = Registry.LocalMachine.OpenSubKey(capabilitiesPath);
			if(regCapKey == null){
				return false;
			}
			bool b = true;
			using(regCapKey){
				b &= (regCapKey.GetValue("ApplicationName") as string) == appName;
				b &= (regCapKey.GetValue("ApplicationDescription") as string) == description;
			}

			if(!b){
				return false;
			}

			// RegisteredApplications
			var regAppsKey = Registry.LocalMachine.OpenSubKey(RegisteredApplicationsPath);
			if(regAppsKey == null){
				return false;
			}
			using(regAppsKey){
				b &= (regAppsKey.GetValue(appName) as string) == capabilitiesPath;
			}
			return b;
		}

		/// <summary>
		/// Register File Associations into Capavilities.
		/// </summary>
		/// <param name="appName"></param>
		/// <param name="company"></param>
		/// <param name="appRegisterName"></param>
		/// <param name="extension"></param>
		/// <param name="description"></param>
		/// <param name="verb"></param>
		public static void RegisterFileAssociationCapability(string appName, string company, string appRegisterName, string extension, string description, string verb){
			var companyAppName = (String.IsNullOrWhiteSpace(company)) ? appName : company + "\\" + appName;
			var softwareRegPath = @"Software\" + companyAppName;
			var capabilitiesPath = softwareRegPath + @"\Capabilities";
			using(var regAsscKey = Registry.ClassesRoot.CreateSubKey(capabilitiesPath + @"\FileAssociations")){
				regAsscKey.SetValue(extension, appRegisterName);
			}
		}

		public static bool IsFileAssociationCapabilityRegistered(string appName, string company, string appRegisterName, string extension, string description, string verb){
			var companyAppName = (String.IsNullOrWhiteSpace(company)) ? appName : company + "\\" + appName;
			var softwareRegPath = @"Software\" + companyAppName;
			var capabilitiesPath = softwareRegPath + @"\Capabilities";
			var regAsscKey = Registry.ClassesRoot.CreateSubKey(capabilitiesPath + @"\FileAssociations");
			if(regAsscKey == null){
				return false;
			}
			using(regAsscKey){
				return (regAsscKey.GetValue(extension) as string) == appRegisterName;
			}
		}

		public static void RegisterUrlAssociationCapability(string appName, string company, string appRegisterName, string protocol, string description, string verb){
			var companyAppName = (String.IsNullOrWhiteSpace(company)) ? appName : company + "\\" + appName;
			var softwareRegPath = @"Software\" + companyAppName;
			var capabilitiesPath = softwareRegPath + @"\Capabilities";
			using(var regAsscKey = Registry.LocalMachine.CreateSubKey(capabilitiesPath + @"\URLAssociations")){
				regAsscKey.SetValue(protocol, appRegisterName);
			}
		}

		/// <summary>
		/// Register App Register Name Into HKCR
		/// </summary>
		/// <param name="appName"></param>
		/// <param name="progId"></param>
		/// <param name="description"></param>
		/// <param name="verb"></param>
		public static void RegisterProgId(string appName, string progId, string description, string verb){
			using(var regAppKey = Registry.ClassesRoot.CreateSubKey(progId)){
				regAppKey.SetValue(null, description);
				using(var regOpenComKey = regAppKey.CreateSubKey(@"shell\open\command")){
					regOpenComKey.SetValue(null, verb);
				}
				using(var regAppComKey = regAppKey.CreateSubKey(@"shell\" + appName + @"\command")){
					regAppComKey.SetValue(null, verb);
				}
			}
		}

		public static bool IsProgIdRegistered(string appName, string progId, string description, string verb){
			using(var regAppKey = Registry.ClassesRoot.CreateSubKey(progId)){
				regAppKey.SetValue(null, description);
				var regOpenComKey = regAppKey.OpenSubKey(@"shell\open\command");
				if(regOpenComKey == null){
					return false;
				}
				var b = true;
				using(regOpenComKey){
					 b &= (regOpenComKey.GetValue(null) as string) == verb;
				}

				var regAppComKey = regAppKey.OpenSubKey(@"shell\" + appName + @"\command");
				using(regAppComKey){
					b &= (regAppComKey.GetValue(null) as string) == verb;
				}
				return b;
			}
		}


		public static void RegisterProgIdToCurrentUser(string appName, string progId, string description, string verb){
			using(var regAppKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + progId)){
				regAppKey.SetValue(null, description);
				using(var regOpenComKey = regAppKey.CreateSubKey(@"shell\open\command")){
					regOpenComKey.SetValue(null, verb);
				}
				using(var regAppComKey = regAppKey.CreateSubKey(@"shell\" + appName + @"\command")){
					regAppComKey.SetValue(null, verb);
				}
			}
		}

		public static bool IsProgIdToCurrentUserRegistered(string appName, string progId, string description, string verb){
			var regAppKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + progId);
			if(regAppKey == null){
				return false;
			}
			using(regAppKey){
				var b = true;
				b = (regAppKey.GetValue(null) as string) == description;

				var regOpenComKey = regAppKey.CreateSubKey(@"shell\open\command");
				if(regOpenComKey == null){
					return false;
				}
				using(regOpenComKey){
					b &= (regOpenComKey.GetValue(null) as string) == verb;
				}

				var regAppComKey = regAppKey.CreateSubKey(@"shell\" + appName + @"\command");
				if(regAppComKey == null){
					return false;
				}
				using(regAppComKey){
					b &= (regAppComKey.GetValue(null) as string) == verb;
				}
				return b;
			}
		}

		public static bool IsEnableUac{
			get{
				if(!IsVistaOrHigher){
					return false;
				}
				using(var regSysKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\\Windows\CurrentVersion\Policies\System", false)){
					return (int)regSysKey.GetValue("EnableLUA", 0) > 0;
				}
			}
		}

		public static bool IsAdministrator{
			get{
				WindowsPrincipal windowsPricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
				return windowsPricipal.IsInRole(WindowsBuiltInRole.Administrator);
			}
		}
	}

	[ComImport]
	[Guid("4e530b0a-e611-4c77-a3ac-9031d022281b")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IApplicationAssociationRegistration{
		int QueryCurrentDefault(string pszQuery, AssociationType atQueryType, AssociationLevel alQueryLevel, out string ppszAssociation);
		int QueryAppIsDefault(string pszQuery, AssociationType atQueryType, AssociationLevel alQueryLevel, string pszAppRegistryName, out bool pfDefault);
		int QueryAppIsDefaultAll(AssociationLevel alQueryLevel, string pszAppRegistryName, out bool pfDefault);
		int SetAppAsDefault(string pszAppRegistryName, string pszSet, AssociationType atSetType);
		int SetAppAsDefaultAll(string pszAppRegistryName);
		int ClearUserAssociations();
	}

	// TODO : Wrapper
	[ComImport]
	[Guid("591209c7-767b-42b2-9fba-44ee4615f2c7")]
	public class ApplicationAssociationRegistration{
	}

	public enum AssociationLevel : int{
		Machine = 0,
		Effective = 1,
		User = 2,
	};

	public enum AssociationType : int{
		FileExtension = 0,
		UrlProtocol = 1,
		StartMenuClient = 2,
		MimeType = 3,
	};

	[ComImport]
	[Guid("1f76a169-f994-40ac-8fc8-0959e8874710")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IApplicationAssociationRegistrationUI{
		int LaunchAdvancedAssociationUI(string appRegisterName);
	}

	[ComImport]
	[Guid("1968106d-f3b5-44cf-890e-116fcb9ecef1")]
	public class ApplicationAssociationRegistrationUI{
	}

	[ComImport]
	[Guid("c46ca590-3c3f-11d2-bee6-0000f805ca57")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IQueryAssociations{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="options"></param>
		/// <param name="assoc">extension, guid, progid or executanble name</param>
		/// <param name="hkey">(optional)The HKEY value of the subkey that is used as a root key. The search looks only below this key. If a value is specified for pwszAssoc, set this parameter to NULL.</param>
		/// <param name="hwnd">(optional)</param>
		/// <returns></returns>
		int Init(AssociationInitializeOptions options, string assoc, IntPtr hkey, IntPtr hwnd);

		int GetString(AssociationOptions options, AssociationString str, string extra, StringBuilder outStr, out int length);
	}

	[Flags]
	public enum AssociationOptions : int{
		None = 0x00000000,

		/// <summary>
		/// dont use HKCU
		/// </summary>
		NoUserSettings              = 0x00000010,
		/// <summary>
		/// dont truncate the return string
		/// </summary>
		NoTruncated                  = 0x00000020,
		/// <summary>
		/// verify data is accurate (DISK HITS)
		/// </summary>
		Verify                      = 0x00000040,
		/// <summary>
		/// actually gets info about rundlls target if applicable
		/// </summary>
		RemapRunDll                 = 0x00000080,
		/// <summary>
		/// attempt to fix errors if found
		/// </summary>
		NoFixups                    = 0x00000100,
		/// <summary>
		/// dont recurse into the baseclass
		/// </summary>
		IgnoreBassClass             = 0x00000200,

	}

	[Flags]
	public enum AssociationInitializeOptions : int{
		None = 0,

		/// <summary>
		/// executable is being passed in
		/// </summary>
		ByExeName = 0x00000002,
		/// <summary>
		/// treat "*" as the BaseClass
		/// </summary>
		DefaultToStar = 0x00000004,
		/// <summary>
		/// treat "Folder" as the BaseClass
		/// </summary>
		DefaultToFolder = 0x00000008,
	}


	public enum AssociationString : int{
		/// <summary>
		/// shell\verb\command string
		/// </summary>
		Command      = 1,
		/// <summary>
		/// the executable part of command string
		/// </summary>
		Executable,
		/// <summary>
		/// friendly name of the document type
		/// </summary>
		FriendlyDocumentName,
		/// <summary>
		/// friendly name of executable
		/// </summary>
		FriendlyApplicationName,
		/// <summary>
		/// noopen value
		/// </summary>
		NoOpen,
		/// <summary>
		/// query values under the shellnew key
		/// </summary>
		ShellNewValue,
		/// <summary>
		/// template for DDE commands
		/// </summary>
		DDECommand,
		/// <summary>
		/// DDECOMMAND to use if just create a process
		/// </summary>
		DDEIfExec,
		/// <summary>
		/// Application name in DDE broadcast
		/// </summary>
		DDEApplication,
		/// <summary>
		/// Topic Name in DDE broadcast
		/// </summary>
		DDETopic,
		/// <summary>
		/// info tip for an item, or list of properties to create info tip from
		/// </summary>
		InfoTip,

		// IE6 or later

		/// <summary>
		/// same as ASSOCSTR_INFOTIP, except, this list contains only quickly retrievable properties
		/// </summary>
		QuickTip,
		/// <summary>
		/// similar to ASSOCSTR_INFOTIP - lists important properties for tileview
		/// </summary>
		TileInfo,
		/// <summary>
		/// MIME Content type
		/// </summary>
		ContentType,
		/// <summary>
		/// Default icon source
		/// </summary>
		DefaultIcon,
		/// <summary>
		/// Guid string pointing to the Shellex\Shellextensionhandler value.
		/// </summary>
		ShellExtension,

		// IE8 or later

		/// <summary>
		/// The CLSID of DropTarget
		/// </summary>
		DropTarget,
		/// <summary>
		/// The CLSID of DelegateExecute
		/// </summary>
		DelegateExecute,
	}
}
