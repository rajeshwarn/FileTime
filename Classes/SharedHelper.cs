﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using genBTC.FileTime.mViewModels;
using genBTC.FileTime.Properties;

namespace genBTC.FileTime.Classes
{
    //This needs to be renamed to something once we figure out why all this stuff is allowed to be seperate, Sort it. Refactor it.
    class SharedHelper
    {
        public static readonly char Seperator = Path.DirectorySeparatorChar;
        public static readonly string UserDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public static string CurrExten;
        public List<string> Filextlist = new List<string>();

        public static bool FindCurExt(String s)
        {
            return s == CurrExten;
        }

        public static FileAttributes RemoveAttributes(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }

        //native call to do string compare like the OS
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int StrCmpLogicalW(String x, String y);
        public static IComparer<string> explorerStringComparer()
        {
            return new ExplorerComparerstringHelper();
        }
        /// <summary>  Reasons to be invisible  </summary>
        public static FileAttributes SyncSettingstoInvisibleFlag()
        {
            FileAttributes reasonsToBeInvisible = (Settings.Default.ShowHidden ? 0 : FileAttributes.Hidden) |
                                                  (Settings.Default.ShowSystem ? 0 : FileAttributes.System) |
                                                  (Settings.Default.ShowReadOnly ? 0 : FileAttributes.ReadOnly);
            return reasonsToBeInvisible;
        }

        /// <summary> Return 1 if bool=true (Directory) otherwise 0=false (File) </summary>
        public static int Bool2Int(bool fileOrDir)
        {
            return fileOrDir ? 1 : 0;
        }

        /// <summary>
        /// Display the Folder Browser Dialog and then display the selected
        /// file path and the directories and files in the folder.
        /// </summary>
        public static string OpenFile(string path)
        {
            //Feed in a path to start in or use current path as dialog path:
            if (path == null)
                return null;
            //start a new filebrowser dialog thread.
            var t = new Thread(() =>
            {
                var openFile = new FolderBrowserDialog
                {
                    ShowNewFolderButton = false,
                    SelectedPath = path,
                    Description = "Select the folder you want to view/change the subfolders of:"
                };

                //openFile.RootFolder = System.Environment.SpecialFolder.MyComputer;
                //openFile.ShowNewFolderButton = true;
                if (openFile.ShowDialog() == DialogResult.Cancel)
                    return;
                //re-use the path variable to return what was selected
                path = openFile.SelectedPath;
            });
            //TODO: check if Code is needed for when running as MultiThreaded App. [MTAThread]
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            return path;
        }
    }
}