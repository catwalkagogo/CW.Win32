/*
	$Id: ShellIcon.cs 273 2011-07-31 18:50:59Z cs6m7y@bma.biglobe.ne.jp $
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using Microsoft.Win32;

namespace CW.Win32.Shell {
	[Obsolete]
	public static class ShellIcon{
		#region GetIcon
		
		public static Icon GetIcon(string path, IconSize size){
			IntPtr hIcon;
			GetIcon(path, size, out hIcon);
			if(hIcon != IntPtr.Zero){
				using(Icon icon = Icon.FromHandle(hIcon)){
					Icon copiedIcon = (Icon)icon.Clone();
					User32.DestroyIcon(icon.Handle);
					return copiedIcon;
				}
			}else{
				return null;
			}
		}
		
		public static Image GetIconImage(string path, IconSize size){
			IntPtr hIcon;
			GetIcon(path, size, out hIcon);
			if(hIcon != IntPtr.Zero){
				Icon icon = Icon.FromHandle(hIcon);
				if(icon != null){
					using(icon){
						Image image = icon.ToBitmap();
						User32.DestroyIcon(icon.Handle);
						return image;
					}
				}else{
					return null;
				}
			}
			return null;
		}

		public static Bitmap GetIconBitmap(string path, IconSize size){
			IntPtr hIcon;
			GetIcon(path, size, out hIcon);
			if(hIcon != IntPtr.Zero){
				Icon icon = Icon.FromHandle(hIcon);
				if(icon != null){
					using(icon){
						Bitmap bmp = icon.ToBitmap();
						User32.DestroyIcon(icon.Handle);
						return bmp;
					}
				}else{
					return null;
				}
			}
			return null;
		}
		/*
		public static ImageSource GetIconImageSource(string path, IconSize size){
			IntPtr hIcon;
			GetIcon(path, size, out hIcon);
			if(hIcon != IntPtr.Zero){
				int s = (size == IconSize.Large) ? 32 : 16;
				var image = Imaging.CreateBitmapSourceFromHIcon(hIcon, new System.Windows.Int32Rect(0, 0, s, s), BitmapSizeOptions.FromWidthAndHeight(s, s));
				image.Freeze();
				return image;
			}
			return null;
		}
		*/
		public static void GetIcon(string path, IconSize size, out IntPtr hIcon){
			object value;
			string ext = Path.GetExtension(path);
			hIcon = IntPtr.Zero;
			if(String.IsNullOrEmpty(ext)){
				if((
					((path.Length == 2) && (Char.IsLetter(path, 0)) && (path[1] == ':')) ||
					((path.Length == 3) && (Char.IsLetter(path, 0)) && (path[1] == ':') && (path[2] == System.IO.Path.DirectorySeparatorChar)))){      // ドライブアイコン
					GetIconShellInternal(path, size, out hIcon);
				}else{                      // 不明のアイコン
					IntPtr hLargeIcon;
					IntPtr hSmallIcon;
					GetUnknownIconHandle(out hLargeIcon, out hSmallIcon);
					if(size == IconSize.Large){
						hIcon = hLargeIcon;
						User32.DestroyIcon(hSmallIcon);
					}else{
						hIcon = hSmallIcon;
						User32.DestroyIcon(hLargeIcon);
					}
				}
			}else{
				RegistryKey root = Registry.ClassesRoot;
				try{
					// typeName取得
					string typeName = GetTypeName(root, ext);
					
					string clsid = null;
					if(typeName != null){
						RegistryKey typeKey = root.OpenSubKey(typeName, false);
						if(typeKey != null){	// ファイルタイプキー
							try{
								RegistryKey defaultIconKey = typeKey.OpenSubKey("DefaultIcon", false);
								if(defaultIconKey != null){	// ファイルタイプ\DefaultIconキー
									try{
										value = defaultIconKey.GetValue("");
										if(value != null){
											string spec = value.ToString();
											IntPtr hLargeIcon;
											IntPtr hSmallIcon;
											GetIconFromSpec(spec, path, out hLargeIcon, out hSmallIcon);
											if(size == IconSize.Large){
												hIcon = hLargeIcon;
												User32.DestroyIcon(hSmallIcon);
											}else{
												hIcon = hSmallIcon;
												User32.DestroyIcon(hLargeIcon);
											}
										}else{
											//Debug.WriteLine(ext + " -> " + typeName + " value is null.");
										}
									}finally{
										defaultIconKey.Close();
									}
								}else{
									//Debug.WriteLine(ext + " -> " + typeName + " -> DefaultIcon is not found.");
								}
								
								// typeName -> DefaultIconで取得できなかったとき、clsidを取得。
								if(hIcon == IntPtr.Zero){
									// 実行ファイルの時
									if(typeName.Equals("exefile", StringComparison.OrdinalIgnoreCase)){
										IntPtr smallIconHandle;
										IntPtr largeIconHandle;
										GetExecutableIconHandle(out largeIconHandle, out smallIconHandle);
										if(size == IconSize.Large){
											hIcon = largeIconHandle;
											User32.DestroyIcon(smallIconHandle);
										}else{
											hIcon = smallIconHandle;
											User32.DestroyIcon(largeIconHandle);
										}
									}else{	// CLSID
										RegistryKey typeClsidKey = typeKey.OpenSubKey("CLSID");
										if(typeClsidKey != null){
											try{
												value = typeClsidKey.GetValue("");
												if(value != null){
													clsid = value.ToString();
												}else{
													//Debug.WriteLine(ext + " -> " + typeName + " -> CLSID value is null.");
												}
											}finally{
												typeClsidKey.Close();
											}
										}else{
											//Debug.WriteLine(ext + " -> " + typeName + " CLSID is not found.");
										}
									}
								}
							}finally{
								typeKey.Close();
							}
						}else{
							//Debug.WriteLine(ext + " -> " + typeName + " is not found.");
						}
					}
					
					// CLSIDから取得。
					if(clsid != null){
						RegistryKey clsidKey = root.OpenSubKey("CLSID");
						if(clsidKey != null){
							try{
								RegistryKey typeClsidKey2 = clsidKey.OpenSubKey(clsid);
								if(typeClsidKey2 != null){
									try{
										RegistryKey defaultIconKey2 = typeClsidKey2.OpenSubKey("DefaultIcon");
										if(defaultIconKey2 != null){
											try{
												value = defaultIconKey2.GetValue("");
												if(value != null){
													string spec = value.ToString();
													IntPtr hLargeIcon;
													IntPtr hSmallIcon;
													GetIconFromSpec(spec, path, out hLargeIcon, out hSmallIcon);
													if(size == IconSize.Large){
														hIcon = hLargeIcon;
														User32.DestroyIcon(hSmallIcon);
													}else{
														hIcon = hSmallIcon;
														User32.DestroyIcon(hLargeIcon);
													}
												}else{
													//Debug.WriteLine(ext + " -> " + typeName + " CLSID -> " + clsid + " -> DefaultIcon value is null.");
												}
											}finally{
												defaultIconKey2.Close();
											}
										}else{
											//Debug.WriteLine(ext + " -> " + typeName + " CLSID -> " + clsid + " -> DefaultIcon is not found.");
										}
									}finally{
										typeClsidKey2.Close();
									}
								}else{
									//Debug.WriteLine(ext + " -> " + typeName + " CLSID -> " + clsid + " is not found.");
								}
							}finally{
								clsidKey.Close();
							}
						}else{
							//Debug.WriteLine("CLSID is not found.");
						}
					}
				}finally{
					root.Close();
				}
			}
			
			// 結局取得できなかったとき。
			if(hIcon == IntPtr.Zero){
				IntPtr hLargeIcon;
				IntPtr hSmallIcon;
				GetUnknownIconHandle(out hLargeIcon, out hSmallIcon);
				if(size == IconSize.Large){
					hIcon = hLargeIcon;
					User32.DestroyIcon(hSmallIcon);
				}else{
					hIcon = hSmallIcon;
					User32.DestroyIcon(hLargeIcon);
				}
			}
		}
		
		private static string GetTypeName(RegistryKey root, string ext){
			RegistryKey extKey = root.OpenSubKey(ext, false);
			if(extKey != null){	// .拡張子キー
				try{
					object value = extKey.GetValue("");
					if(value != null){
						return value.ToString();
					}else{
						//Debug.WriteLine(ext + " value is null.");
					}
				}finally{
					extKey.Close();
				}
			}else{
				//Debug.WriteLine(ext + " is not found.");
			}
			return null;
		}
		
		private static void GetIconFromSpec(string spec, string path, out IntPtr hLargeIcon, out IntPtr hSmallIcon){
			int sepIdx = spec.LastIndexOf(',');
			string res;
			int idx = 0;
			if(sepIdx == -1){
				res = spec;
			}else{
				if(Int32.TryParse(spec.Substring(sepIdx + 1), out idx)){
					res = spec.Substring(0, sepIdx);
				}else{
					res = spec;
					idx = 0;
				}
			}
			res = Environment.ExpandEnvironmentVariables(res.Replace("%1", path));

			Shell32.ExtractIconEx(res, idx, out hLargeIcon, out hSmallIcon, 1);
		}

		public static void GetUnknownIconImage(out Image largeIcon, out Image smallIcon){
			IntPtr hLargeIcon;
			IntPtr hSmallIcon;
			GetUnknownIconHandle(out hLargeIcon, out hSmallIcon);
			if(hLargeIcon != IntPtr.Zero){
				using(Icon icon = (Icon)Icon.FromHandle(hLargeIcon)){
					largeIcon = icon.ToBitmap();
					User32.DestroyIcon(hLargeIcon);
				}
			}else{
				largeIcon = null;
			}
			if(hSmallIcon != IntPtr.Zero){
				using(Icon icon = (Icon)Icon.FromHandle(hSmallIcon)){
					smallIcon = icon.ToBitmap();
					User32.DestroyIcon(hSmallIcon);
				}
			}else{
				smallIcon = null;
			}
		}

		public static Icon GetUnknownIconImage(IconSize size){
			IntPtr hLargeIcon;
			IntPtr hSmallIcon;
			GetUnknownIconHandle(out hLargeIcon, out hSmallIcon);
			if(size == IconSize.Small){
				User32.DestroyIcon(hLargeIcon);
				return Icon.FromHandle(hSmallIcon);
			}else{
				User32.DestroyIcon(hSmallIcon);
				return Icon.FromHandle(hLargeIcon);
			}
		}

		private static void GetUnknownIconHandle(out IntPtr hLargeIcon, out IntPtr hSmallIcon){
			if(Environment.OSVersion.Version.Major >= 6){
				if(Environment.OSVersion.Version.Minor >= 1){
					Shell32.ExtractIconEx("imageres.dll", 2, out hLargeIcon, out hSmallIcon, 1);
				}else{
					Shell32.ExtractIconEx("imageres.dll", 1, out hLargeIcon, out hSmallIcon, 1);
				}
			}else{
				Shell32.ExtractIconEx("Shell32.dll", 0, out hLargeIcon, out hSmallIcon, 1);
			}
		}
		
		public static void GetExecutableIconImage(out Image largeIcon, out Image smallIcon){
			IntPtr hLargeIcon;
			IntPtr hSmallIcon;
			GetExecutableIconHandle(out hLargeIcon, out hSmallIcon);
			if(hLargeIcon != IntPtr.Zero){
				using(Icon icon = (Icon)Icon.FromHandle(hLargeIcon)){
					largeIcon = icon.ToBitmap();
					User32.DestroyIcon(hLargeIcon);
				}
			}else{
				largeIcon = null;
			}
			if(hSmallIcon != IntPtr.Zero){
				using(Icon icon = (Icon)Icon.FromHandle(hSmallIcon)){
					smallIcon = icon.ToBitmap();
					User32.DestroyIcon(hSmallIcon);
				}
			}else{
				smallIcon = null;
			}
		}
		
		private static void GetExecutableIconHandle(out IntPtr hLargeIcon, out IntPtr hSmallIcon){
			if(Environment.OSVersion.Version.Major >= 6){
				if(Environment.OSVersion.Version.Minor >= 1){
					Shell32.ExtractIconEx("imageres.dll", 11, out hLargeIcon, out hSmallIcon, 1);
				}else{
					Shell32.ExtractIconEx("imageres.dll", 10, out hLargeIcon, out hSmallIcon, 1);
				}
			}else{
				Shell32.ExtractIconEx("Shell32.dll", 2, out hLargeIcon, out hSmallIcon, 1);
			}
		}
		
		private static IntPtr GetIconShellInternal(string path, IconSize size, out IntPtr hIcon){
			SHFileInfo shinfo = new SHFileInfo();
			SHGetFileInfoOptions flags = SHGetFileInfoOptions.Icon;
			if(size == IconSize.Small){
				flags |= SHGetFileInfoOptions.SmallIcon;
			}else{
				flags |= SHGetFileInfoOptions.LargeIcon;
			}
			IntPtr r = Shell32.SHGetFileInfo(path, FileAttributes.Normal, ref shinfo, Marshal.SizeOf(shinfo), flags);
			hIcon = shinfo.hIcon;
			return r;
		}
		
		#endregion

		#region ExtractIcon

		public static Icon[] ExtractIcon(string path, int index){
			Icon[] icons = new Icon[2];
			IntPtr largeIconHandle;
			IntPtr smallIconHandle;
			Shell32.ExtractIconEx(path, index, out largeIconHandle, out smallIconHandle, 1);
			if(largeIconHandle != IntPtr.Zero){
				Icon icon = Icon.FromHandle(largeIconHandle);
				using(icon){
					icons[0] = (Icon)icon.Clone();
					User32.DestroyIcon(largeIconHandle);
				}
			}
			if(largeIconHandle != IntPtr.Zero){
				Icon icon = Icon.FromHandle(smallIconHandle);
				using(icon){
					icons[1] = (Icon)icon.Clone();
					User32.DestroyIcon(smallIconHandle);
				}
			}
			return icons;
		}
		
		public static Image[] ExtractIconImage(string path, int index){
			Image[] icons = new Image[2];
			IntPtr largeIconHandle;
			IntPtr smallIconHandle;
			Shell32.ExtractIconEx(path, index, out largeIconHandle, out smallIconHandle, 1);
			if(largeIconHandle != IntPtr.Zero){
				using(Icon icon = Icon.FromHandle(largeIconHandle)){
					icons[0] = icon.ToBitmap();
					User32.DestroyIcon(largeIconHandle);
				}
			}
			if(largeIconHandle != IntPtr.Zero){
				using(Icon icon = Icon.FromHandle(smallIconHandle)){
					icons[1] = icon.ToBitmap();
					User32.DestroyIcon(smallIconHandle);
				}
			}
			return icons;
		}
		
		public static void ExtractIconImage(string path, int index, out Image largeIconImage, out Image smallIconImage){
			largeIconImage = null;
			smallIconImage = null;
			IntPtr largeIconHandle;
			IntPtr smallIconHandle;
			Shell32.ExtractIconEx(path, index, out largeIconHandle, out smallIconHandle, 1);
			if(largeIconHandle != IntPtr.Zero){
				using(Icon icon = Icon.FromHandle(largeIconHandle)){
					largeIconImage = icon.ToBitmap();
					User32.DestroyIcon(largeIconHandle);
				}
			}
			if(largeIconHandle != IntPtr.Zero){
				using(Icon icon = Icon.FromHandle(smallIconHandle)){
					smallIconImage = icon.ToBitmap();
					User32.DestroyIcon(smallIconHandle);
				}
			}
		}

		#endregion
	}
}

namespace CW.Win32 {
	public static partial class User32 {
		[DllImport("USER32.DLL", EntryPoint = "DestroyIcon", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DestroyIcon(IntPtr hIcon);
	}
}