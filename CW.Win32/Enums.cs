using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CW.Win32 {
	public enum IconSize{
		Large = 0x0,
		Small = 0x1,
	}

	[Flags]
	public enum SHGetFileInfoOptions : uint{
		Icon                = 0x000000100,    // get icon
		DisplayName         = 0x000000200,    // get display name
		TypeName            = 0x000000400,    // get type name
		Attributes          = 0x000000800,    // get attributes
		IconLocation        = 0x000001000,    // get icon location
		ExeType             = 0x000002000,    // return exe type
		SysIconIndex        = 0x000004000,    // get system icon index
		LinkOverlay         = 0x000008000,    // put a link overlay on icon
		Selected            = 0x000010000,    // show icon in selected state
			                                    // (NTDDI_VERSION >= NTDDI_WIN2K)
		SpecifiedAttributes = 0x000020000,    // get only specified attributes
		LargeIcon           = 0x000000000,    // get large icon
		SmallIcon           = 0x000000001,    // get small icon
		OpenIcon            = 0x000000002,    // get open icon
		ShellIconSize       = 0x000000004,    // get shell size icon
		Pidl                = 0x000000008,    // pszPath is a pidl
		UseFileAttributes   = 0x000000010,    // use passed dwFileAttribute
			                                    // (_WIN32_IE >= 0x0500)
		AddOverlays         = 0x000000020,    // apply the appropriate overlays
		OverlayIndex        = 0x000000040,    // Get the index of the overlay
	}

	public enum GetWindowOption : uint{
		First          = 0,
		Last           = 1,
		Next           = 2,
		Prev           = 3,
		Owner          = 4,
		Child          = 5,
		EnabledPopup   = 6,
	}
	
	public enum GetWindowLongOption : int{
		WndProc         = (-4),
		HInstance       = (-6),
		HwndParent      = (-8),
		Style           = (-16),
		ExStyle         = (-20),
		UserData        = (-21),
		Id              = (-12),
	}
	
	public enum GetClassLongOption : int{
		MenuName      = (-8),
		HbrBackground = (-10),
		HCursor       = (-12),
		HIcon         = (-14),
		HModule       = (-16),
		WndProc       = (-24),
		HIconSmall    = (-34),
	}
	
	[Flags]
	public enum WindowStyle : uint{
		Overlapped       = 0x00000000,
		Popup            = 0x80000000,
		Child            = 0x40000000,
		Minimize         = 0x20000000,
		Visible          = 0x10000000,
		Disabled         = 0x08000000,
		ClipSiblings     = 0x04000000,
		ClipChildren     = 0x02000000,
		Maximize         = 0x01000000,
		Caption          = 0x00C00000,     /* WS_BORDER | WS_DLGFRAME  */
		Border           = 0x00800000,
		DialogFrame      = 0x00400000,
		VScroll          = 0x00200000,
		HScroll          = 0x00100000,
		SystemMenu       = 0x00080000,
		ThickFrame       = 0x00040000,
		Group            = 0x00020000,
		TabStop          = 0x00010000,
		
		MinimizeBox      = 0x00020000,
		MaximizeBox      = 0x00010000,
		
		Tiled            = Overlapped,
		Iconic           = Minimize,
		SizeBox          = ThickFrame,
		TiledWindows     = OverlappedWindow,
		
		/*
		 * Common Window Styles
		 */
		OverlappedWindow = (Overlapped | Caption | SystemMenu | ThickFrame | MinimizeBox | MaximizeBox),
		PopupWindow      = (Popup | Border | SystemMenu),
		ChildWindow      = (Child),
	}
	
	[Flags]
	public enum SetWindowPosOptions : uint{
		NoSize          = 0x0001,
		NoMove          = 0x0002,
		NoZOrder        = 0x0004,
		NoRedraw        = 0x0008,
		NoActivate      = 0x0010,
		FrameChanged    = 0x0020,  /* The frame changed: send WM_NCCALCSIZE */
		ShowWindow      = 0x0040,
		HideWindow      = 0x0080,
		NoCopyBits      = 0x0100,
		NoOwnerZOrder   = 0x0200,  /* Don't do owner Z ordering */
		NoSendChanging  = 0x0400,  /* Don't send WM_WINDOWPOSCHANGING */
		DrawFrane       = FrameChanged,
		NoReposition    = NoOwnerZOrder,
		Defererase      = 0x2000,
		AsyncWindowPosition = 0x4000,
	}
	
	public enum WindowMessage : uint{
		Null = 0x0000,
		Create = 0x0001,
		Destroy = 0x0002,
		Move = 0x0003,
		Size = 0x0005,
		Activate = 0x0006,
		Setfocus = 0x0007,
		Killfocus = 0x0008,
		Enable = 0x000A,
		Setredraw = 0x000B,
		SetText = 0x000C,
		GetText = 0x000D,
		GetTextlength = 0x000E,
		Paint = 0x000F,
		Close = 0x0010,
		QueryEndSession = 0x0011,
		QueryOpen = 0x0013,
		EndSession = 0x0016,
		Quit = 0x0012,
		EraseBackground = 0x0014,
		SystemColorChange = 0x0015,
		ShowWindow = 0x0018,
		WinIniChange = 0x001A,
		DeviceModeChange = 0x001B,
		ActivateApplication = 0x001C,
		FontChange = 0x001D,
		TimeChange = 0x001E,
		CancelMode = 0x001F,
		SetCursor = 0x0020,
		MouseActivate = 0x0021,
		ChildActivate = 0x0022,
		QueueSync = 0x0023,
		GetMinMaxInfo = 0x0024,
		PaintIcon = 0x0026,
		IconEraseBackground = 0x0027,
		NextDialogControl = 0x0028,
		SpoolerStatus = 0x002A,
		DrawItem = 0x002B,
		MeasureItem = 0x002C,
		DeleteItem = 0x002D,
		VKeyToItem = 0x002E,
		CharToItem = 0x002F,
		SetFont = 0x0030,
		GetFont = 0x0031,
		SetHotKey = 0x0032,
		GetHotKey = 0x0033,
		QueryDragIcon = 0x0037,
		CompareItem = 0x0039,
		GetObject = 0x003D,
		Compacting = 0x0041,
		Commnotify = 0x0044  /* no longer suported */,
		WindowPositionChanging = 0x0046,
		WindowPositionChanged = 0x0047,
		Power = 0x0048,
		CopyData = 0x004A,
		CancelJournal = 0x004B,
		Notify = 0x004E,
		InputLanguageChangeRequest = 0x0050,
		InputLanguageChange = 0x0051,
		TCard = 0x0052,
		Help = 0x0053,
		UserChanged = 0x0054,
		NotifyFormat = 0x0055,
		ContextMenu = 0x007B,
		StyleChanging = 0x007C,
		StyleChanged = 0x007D,
		DisplayChange = 0x007E,
		GetIcon = 0x007F,
		SetIcon = 0x0080,
		NCCreate = 0x0081,
		NCDestroy = 0x0082,
		NCCalcSize = 0x0083,
		NCHitTest = 0x0084,
		NCPaint = 0x0085,
		NCActivate = 0x0086,
		GetDialogCode = 0x0087,
		SyncPaint = 0x0088,
		NCMouseMove = 0x00A0,
		NCLButtonDown = 0x00A1,
		NCLButtonUp = 0x00A2,
		NCLButtonDoubleClick = 0x00A3,
		NCRButtonDown = 0x00A4,
		NCRButtonUp = 0x00A5,
		NCRButtonDoubleClick = 0x00A6,
		NCMButtonDown = 0x00A7,
		NCMButtonUp = 0x00A8,
		NCMButtonDoubleClick = 0x00A9,
		NCXButtonDown = 0x00AB,
		NCXButtonUp = 0x00AC,
		NCXButtonDoubleClick = 0x00AD,
		Input = 0x00FF,
		KeyFirst = 0x0100,
		KeyDown = 0x0100,
		KeyUp = 0x0101,
		@Char = 0x0102,
		DeadChar = 0x0103,
		SystemKeyDown = 0x0104,
		SystemKeyUp = 0x0105,
		SystemChar = 0x0106,
		SystemDeadChar = 0x0107,
		UniChar = 0x0109,
		//keyLast = 0x0109,
		KeyLast = 0x0108,
		Ime_StartComposition = 0x010D,
		Ime_EndComposition = 0x010E,
		Ime_Composition = 0x010F,
		Ime_KeyLast = 0x010F,
		InitDialog = 0x0110,
		Command = 0x0111,
		SysCommand = 0x0112,
		Timer = 0x0113,
		HScroll = 0x0114,
		VScroll = 0x0115,
		InitMenu = 0x0116,
		InitMenuPopup = 0x0117,
		MenuSelect = 0x011F,
		MenuChar = 0x0120,
		EnterIdle = 0x0121,
		MenuRButtonUp = 0x0122,
		MenuDrag = 0x0123,
		MenuGetObject = 0x0124,
		UninitMenuPopup = 0x0125,
		MenuCommand = 0x0126,
		ChangeUIState = 0x0127,
		UpdateUIState = 0x0128,
		QueryUIState = 0x0129,
		CtlColorMessageBox = 0x0132,
		CtlColorEdit = 0x0133,
		CtlColorListBox = 0x0134,
		CtlColorButton = 0x0135,
		CtlColorDialog = 0x0136,
		CtlColorScrollbar = 0x0137,
		CtlColorStatic = 0x0138,
//		MouseFirst = 0x0200,
		MouseMove = 0x0200,
		LButtonDown = 0x0201,
		LButtonUp = 0x0202,
		LButtonDoubleClick = 0x0203,
		RButtonDown = 0x0204,
		RButtonUp = 0x0205,
		RButtonDoubleClick = 0x0206,
		MButtonDown = 0x0207,
		MButtonUp = 0x0208,
		MButtonDoubleClick = 0x0209,
		MouseWheel = 0x020A,
		XButtonDown = 0x020B,
		XButtonUp = 0x020C,
		XButtonDoubleClick = 0x020D,
//		MouseLast = 0x020D,
//		MouseLast = 0x020A,
//		MouseLast = 0x0209,
		ParentNotify = 0x0210,
		EnterMenuloop = 0x0211,
		ExitMenuloop = 0x0212,
		NextMenu = 0x0213,
		Sizing = 0x0214,
		CaptureChanged = 0x0215,
		Moving = 0x0216,
		MdiCreate = 0x0220,
		MdiDestroy = 0x0221,
		MdiActivate = 0x0222,
		MdiRestore = 0x0223,
		MdiNext = 0x0224,
		MdiMaximize = 0x0225,
		MdiTile = 0x0226,
		MdiCascade = 0x0227,
		MdiIconArrange = 0x0228,
		MdiGetActive = 0x0229,
		MdiSetMenu = 0x0230,
		EnterSizeMove = 0x0231,
		ExitSizeMove = 0x0232,
		DropFiles = 0x0233,
		MdiRefreshMenu = 0x0234,
		Ime_Setconrext = 0x0281,
		Ime_Notify = 0x0282,
		Ime_Control = 0x0283,
		Ime_CompositionFull = 0x0284,
		Ime_Select = 0x0285,
		Ime_Char = 0x0286,
		Ime_Request = 0x0288,
		Ime_Keydown = 0x0290,
		Ime_Keyup = 0x0291,
		MouseHover = 0x02A1,
		MouseLeave = 0x02A3,
		NcMouseHover = 0x02A0,
		NcMouseLeave = 0x02A2,
		WTSsessionChange = 0x02B1,
		TabletFirst = 0x02c0,
		TabletLast = 0x02df,
		Cut = 0x0300,
		Copy = 0x0301,
		Paste = 0x0302,
		Clear = 0x0303,
		Undo = 0x0304,
		RenderFormat = 0x0305,
		RenderAllFormats = 0x0306,
		DestroyClipboard = 0x0307,
		DrawClipboard = 0x0308,
		PaintClipboard = 0x0309,
		VScrollClipboard = 0x030A,
		SizeClipboard = 0x030B,
		AskCbformAtName = 0x030C,
		ChangeClipboardChain = 0x030D,
		HScrollClipboard = 0x030E,
		QueryNewPalette = 0x030F,
		PaletteIsChanging = 0x0310,
		PaletteChanged = 0x0311,
		HotKey = 0x0312,
		Print = 0x0317,
		PrintClient = 0x0318,
		AppCommand = 0x0319,
		ThemeChanged = 0x031A,
		HandHeldFirst = 0x0358,
		HandHeldLast = 0x035F,
		AfxFirst = 0x0360,
		AfxLast = 0x037F,
		PenWinFirst = 0x0380,
		PenWinLast = 0x038F,
		App = 0x8000,
		User = 0x0400,
	}
	
	public enum ListViewMessage : int{
		First = 0x1000,
		SetSelectedColumn = First + 140,
		GetSelectedColumn = First + 174,
		GetHeader = First + 31,
	}
	
	public enum HeaderMessage : int{
		First = 0x1200,
		GetItemA = First + 3,
		GetItemW = First + 11,
		GetItem = GetItemW,
		SetItemA = First + 4,
		SetItemW = First + 12,
		SetItem = SetItemW,
	}
	
	[Flags]
	public enum ListViewColumnFormat : int{
		Left               = 0x0000,
		Right              = 0x0001,
		Center             = 0x0002,
		JustifyMark        = 0x0003,
		RightToLeftReading = 0x0004,
		Bitmap             = 0x2000,
		String             = 0x4000,
		OwnerDraw          = 0x8000,
		
		// (_WIN32_IE >= 0x0300)
		Image              = 0x0800,
		BitmapOnRight      = 0x1000,
		
		// (_WIN32_WINNT >= 0x0501)
		SortUp             = 0x0400,
		SortDown           = 0x0200,
		
		// _WIN32_WINNT >= 0x0600
		CheckBox           = 0x0040,
		Checked            = 0x0080,
		FixedWidth         = 0x0100,
		SplitButton        = 0x1000000,
	}
	
	[Flags]
	public enum HeaderItemMask : uint{
		Width              = 0x0001,
		Height             = Width,
		Text               = 0x0002,
		Format             = 0x0004,
		LParam             = 0x0008,
		Bitmap             = 0x0010,
		
		// (_WIN32_IE >= 0x0300)
		Image              = 0x0020,
		DISetItem          = 0x0040,
		Order              = 0x0080,
		
		// (_WIN32_IE >= 0x0500)
		Filter             = 0x0100,
		
		// _WIN32_WINNT >= 0x0600
		State             = 0x0200,
	}
	
	public enum MessageBeep : uint{
		Okey = 0x00000000,
		IconHand = 0x00000010,
		IconQuestion = 0x00000020,
		IconExclamation = 0x00000030,
		IconAsterisk = 0x00000040,
		Beep = 0xFFFFFFFF
	}

	public enum ShowWindowCommand : int{
		Hide = 0,
		ShowNormal = 1,
		ShowMinimized = 2,
		ShowMaximized = 3,
		ShowNoActivate = 4,
		Show = 5,
		Minimize = 6,
		ShowMinNoActivete = 7,
		ShowNA = 8,
		Restore = 9,
		ShowDefault = 10,
		ForceMinimize = 11
	}
	
	[Flags]
	public enum TrackPopupMenuOptions : uint{
		LeftButton    = 0x0000,
		RightButton   = 0x0002,
		LeftAlign     = 0x0000,
		CenterAlign   = 0x0004,
		RightAlign    = 0x0008,
		TopAlign      = 0x0000,
		VCenterAlign  = 0x0010,
		BottomAlign   = 0x0020,
		Horizontal    = 0x0000,
		Vertical      = 0x0040,
		Monotify      = 0x0080,
		ReturnCommand = 0x0100,
	}
	
	public enum MenuFoundBy : uint{
		Command = 0x0000,
		Position = 0x0400,
	}
	
	[Flags]
	public enum GetMenuDefaultItemOptions : uint{
		Normal = 0x0000,
		UseDisabled = 0x0001,
		GoIntoPopups = 0x0002,
	}
	/*
	public class PinnedObject : DisposableObject{
		private GCHandle handle;
		private bool disposed = false;
		
		public PinnedObject(object obj){
			this.handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
		}
		
		public IntPtr Address{
			get{
				return this.handle.AddrOfPinnedObject();
			}
		}
		
		protected override void Dispose(bool disposing){
			if(!(this.disposed)){
				this.handle.Free();
				this.disposed = true;
			}
		}
	}
	*/
	[Flags]
	public enum CreateFileMappingOptions : uint{
		None = 0x00,
		PageReadOnly = 0x02,
		PageReadWrite = 0x04,
		PageWriteCopy = 0x08,
		SecImage = 0x1000000,
		SecReserve = 0x4000000,
		SecCommit = 0x8000000,
		SecNoCache = 0x10000000,
	}

	public enum FileMapAccessMode : uint{
		None = 0x0000,
		Copy = 0x0001,
		Write = 0x0002,
		Read = 0x0004,
		AllAccess = 0x000F0000 | Copy | Write | Read | 0x0008 | 0x0010,
		Execute = 0x0020 // not included in SECTION_ALL_ACCESS
	}

	public enum OLEError : uint {
		NoError = 0,
		Abort = 0x80004004,
		AccessDenied = 0x80070005,
		Fail = 0x80004005,
		Handle = 0x80070006,
		InvalidArg = 0x80070057,
		NoInterface = 0x80004002,
		NoTimpl = 0x80004001,
		OutOfMemory = 0x8007000E,
		Pointer = 0x80004003,
		Unexpected = 0x8000FFFF,
	}

	[Flags]
	public enum SHEmptyRecycleBinOptions : int {
		None = 0x00,
		NoConfirmation = 0x01,
		NoProgressUI = 0x02,
		NoSound = 0x04,
	}
}
