﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace genBTC.FileTime.Classes.Native
{
    /// <summary> Grabs the filesystem or system icon cache for a string filePath.</summary>
    // only used once on ExplorerTree.cs line 581 to scroll programmatically.
    // http://www.pinvoke.net/default.aspx/shell32.shgetfileinfo
    internal class NativeExtractIcon
    {
        /// <summary>Maximal Length of unmanaged Windows-Path-strings</summary>
        private const int MAX_PATH = 260;

        /// <summary>Maximal Length of unmanaged Typename</summary>
        private const int MAX_TYPE = 80;

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetFileInfo(
            string pszPath,
            int dwFileAttributes,
            out SHFILEINFO psfi,
            uint cbfileInfo,
            SHGFI uFlags);

        /// <summary>
        /// Get the associated Icon for a file or application, this method always returns
        /// an icon.  If the strPath is invalid or there is no idonc the default icon is returned
        /// </summary>
        /// <param name="strPath">full path to the file</param>
        /// <param name="bSmall">if true, the 16x16 icon is returned otherwise the 32x32</param>
        /// <returns></returns>
        public static Icon GetIcon(string strPath, bool bSmall)
        {
            var info = new SHFILEINFO();
            int cbFileInfo = Marshal.SizeOf(info);
            SHGFI flags = SHGFI.Icon | SHGFI.UseFileAttributes;
            if (bSmall)
                flags = flags | SHGFI.SmallIcon;
            else
                flags = flags | SHGFI.LargeIcon;

            SHGetFileInfo(strPath, 256, out info, (uint)cbFileInfo, flags);
            try
            {
                return Icon.FromHandle(info.hIcon);
            }
            catch (ArgumentException) { return null; }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public SHFILEINFO(bool b = true)
            {
                hIcon = IntPtr.Zero;
                int iIcon = 0;
                uint dwAttributes = 0;
                string szDisplayName = "";
                string szTypeName = "";
            }

            public readonly IntPtr hIcon;
        };

        [Flags]
        private enum SHGFI
        {
            /// <summary>get icon</summary>
            Icon = 0x000000100,
            /// <summary>get display name</summary>
            DisplayName = 0x000000200,
            /// <summary>get type name</summary>
            TypeName = 0x000000400,
            /// <summary>get attributes</summary>
            Attributes = 0x000000800,
            /// <summary>get icon location</summary>
            IconLocation = 0x000001000,
            /// <summary>return exe type</summary>
            ExeType = 0x000002000,
            /// <summary>get system icon index</summary>
            SysIconIndex = 0x000004000,
            /// <summary>put a link overlay on icon</summary>
            LinkOverlay = 0x000008000,
            /// <summary>show icon in selected state</summary>
            Selected = 0x000010000,
            /// <summary>get only specified attributes</summary>
            Attr_Specified = 0x000020000,
            /// <summary>get large icon</summary>
            LargeIcon = 0x000000000,
            /// <summary>get small icon</summary>
            SmallIcon = 0x000000001,
            /// <summary>get open icon</summary>
            OpenIcon = 0x000000002,
            /// <summary>get shell size icon</summary>
            ShellIconSize = 0x000000004,
            /// <summary>pszPath is a pidl</summary>
            PIDL = 0x000000008,
            /// <summary>use passed dwFileAttribute</summary>
            UseFileAttributes = 0x000000010,
            /// <summary>apply the appropriate overlays</summary>
            AddOverlays = 0x000000020,
            /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
            OverlayIndex = 0x000000040,
        }
    }
}