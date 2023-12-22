using Emin.SaveFile;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace SIM
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct OpenFileName
    {
        public int lStructSize;
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpstrFilter;
        public string lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public string lpstrFile;
        public int nMaxFile;
        public string lpstrFileTitle;
        public int nMaxFileTitle;
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt;
        public IntPtr lCustData;
        public IntPtr lpfnHook;
        public string lpTemplateName;
        public IntPtr pvReserved;
        public int dwReserved;
        public int flagsEx;
    }

    class Program
    {

        public static bool IsProgramInitialized = false;
        public static string ProgramCurrentDirectory = Environment.CurrentDirectory;
        public static string SpecifiedGamePlatform = string.Empty;
        public static string SpecifiedbhvrSession = string.Empty;
        public static string SpecifiedSaveFile = string.Empty;
        public static sbyte SpecifiedWorkingMode = 0;

        [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool GetOpenFileName(ref OpenFileName ofn);

        private static string ShowDialog()
        {
            var filedialog = new OpenFileName();
            filedialog.lStructSize = Marshal.SizeOf(filedialog);
            filedialog.lpstrFilter = "FullProfile.txt (*.*)\0*.*\0";
            filedialog.lpstrFile = new string(new char[256]);
            filedialog.nMaxFile = filedialog.lpstrFile.Length;
            filedialog.lpstrFileTitle = new string(new char[64]);
            filedialog.nMaxFileTitle = filedialog.lpstrFileTitle.Length;
            filedialog.lpstrTitle = "Open File Dialog...";
            if (GetOpenFileName(ref filedialog))
                return filedialog.lpstrFile;
            return string.Empty;
        }

        static void Main()
        {
            if (!IsProgramInitialized)
                IncreaseConsoleBufferSize();

            Console.Title = "SIM v1.0.0";
            Console.Clear();
            Console.WriteLine("Save injector Mobile\n");
            Console.WriteLine("Creator By CO0K1E\n");

            Console.WriteLine("Select your platform:\n");
            Console.WriteLine("1) Global\n");
            Console.WriteLine("2) Netease (Not working, soon)\n");

            switch (Console.ReadLine())
            {
                default:
                    Main();
                    break;

                case "1":
                    SpecifiedGamePlatform = "latest";
                    break;

                case "2":
                    Main();
                    SpecifiedGamePlatform = "";
                    break;
            }

            if (SpecifiedbhvrSession.Length == 0)
            {
                Console.Clear();
                Console.WriteLine("Save injector Mobile\n");
                Console.WriteLine("Creator By CO0K1E\n");

                Console.Write("\nbhvrSession=");
                SpecifiedbhvrSession = Console.ReadLine();

                if (SpecifiedbhvrSession.Length > 255)
                {
                    Console.Clear();
                    Console.WriteLine("Save injector Mobile\n");
                    Console.WriteLine("Creator By CO0K1E\n");
                    Console.WriteLine("Menu\n");
                    Console.WriteLine("1) Inject SaveFile");
                    Console.WriteLine("2) Reset SaveFile");
                    Console.WriteLine("3) Dump SaveFile\n");

                    switch (Console.ReadLine())
                    {
                        default:
                            Main();
                            break;

                        case "1":
                            SpecifiedWorkingMode = 0;
                            break;

                        case "2":
                            SpecifiedWorkingMode = 1;
                            break;

                        case "3":
                            SpecifiedWorkingMode = 2;
                            break;
                    }
                }
                else
                {
                    Console.Write("\n\nERROR: Invalid bhvrSession\n\nPlease ENTER to continue...");
                    SpecifiedGamePlatform = string.Empty;
                    SpecifiedbhvrSession = string.Empty;
                    Console.ReadLine();
                    Main();
                }

                if (SpecifiedWorkingMode == 0)
                    SpecifiedSaveFile = InitializeSaveFile(string.Empty);
                else if (SpecifiedWorkingMode == 1)
                    SpecifiedSaveFile = Properties.DefaultSave.OFFLINE_SAVEFILE;
                else if (SpecifiedWorkingMode == 2)
                    SpecifiedSaveFile = "NONE";

                if (SpecifiedSaveFile.Length != 0)
                {
                    SpecifiedbhvrSession = SpecifiedbhvrSession.Replace("bhvrSession", "").Replace("=", "").Replace(" ", "");
                    string saveFileVersion = NetServices.REQUEST_GET_HEADER($"https://{SpecifiedGamePlatform}.live.dbdena.com/api/v1/players/me/states/FullProfile/binary", $"bhvrSession={SpecifiedbhvrSession}");
                    if (saveFileVersion == "ERROR")
                    {
                        Console.Write("\nERROR: Something went wrong, make sure bhvrSession is valid & properly pasted\n\nPress ENTER to continue...");
                        SpecifiedbhvrSession = string.Empty;
                        SpecifiedSaveFile = string.Empty;
                        Console.ReadLine();
                        Main();
                    }

                    Console.WriteLine($"\nProfile Version: {saveFileVersion}\n\nTrying To Obtain playerUID...");
                    string saveFileUserID = NetServices.REQUEST_GET($"https://{SpecifiedGamePlatform}.live.dbdena.com/api/v1/players/me/states/FullProfile/binary", $"bhvrSession={SpecifiedbhvrSession}");
                    if (SpecifiedWorkingMode == 2)
                    {
                        InitializeSaveFileDump(EminFile.DecryptSavefile(saveFileUserID));
                        SpecifiedbhvrSession = string.Empty;
                        SpecifiedSaveFile = string.Empty;
                        Console.Write("\nSaveFile was successfully obtained & stored on PC \n\nPress ENTER to continue...");
                        Console.ReadLine();
                        Main();
                    }

                    InitializeSaveFileBackup(saveFileUserID);
                    if (saveFileUserID == "ERROR")
                    {
                        Console.Write("\nERROR: Something went wrong when program tried to obtain playerUID\n\nPress ENTER to continue...");
                        SpecifiedbhvrSession = string.Empty;
                        SpecifiedSaveFile = string.Empty;
                        Console.ReadLine();
                        Main();
                    }

                    var JsFullProfile = JObject.Parse(EminFile.DecryptSavefile(saveFileUserID));
                    saveFileUserID = (string)JsFullProfile["playerUId"];

                    Console.WriteLine($"\nUserID: {saveFileUserID}\n\nTrying To Inject SaveFile...");
                    string saveFileResponse = NetServices.REQUEST_POST($"https://{SpecifiedGamePlatform}.live.dbdena.com/api/v1/players/me/states/binary?schemaVersion=0&stateName=FullProfile&version={(Convert.ToInt32(saveFileVersion) + 1).ToString()}", $"bhvrSession={SpecifiedbhvrSession}", SpecifiedSaveFile, saveFileUserID);
                    if (saveFileResponse == "ERROR")
                    {
                        Console.Write("\nERROR: Something went wrong, make sure that bhvrSession is validated\n\nPress ENTER to continue...");
                        SpecifiedbhvrSession = string.Empty;
                        SpecifiedSaveFile = string.Empty;
                        Console.ReadLine();
                        Main();
                    }
                    Console.Write("\nSuccess!\n\nPress ENTER to continue...");
                    Console.ReadLine();
                    SpecifiedbhvrSession = string.Empty;
                    SpecifiedGamePlatform = string.Empty;
                    SpecifiedSaveFile = string.Empty;
                    Main();
                }
            }
        }

        private static string InitializeSaveFile(string input)
        {
            try
            {
                if (input.Length == 0)
                {
                    using (StreamReader sr = new StreamReader(ShowDialog()))
                    {
                        string InputFileContent = sr.ReadToEnd();
                        if (InputFileContent.IsDeadByDaylightCryptoString())
                            return EminFile.DecryptSavefile(InputFileContent);

                        else if (InputFileContent.IsBase64String())
                            return System.Text.Encoding.ASCII.GetString(
                                System.Convert.FromBase64String(InputFileContent));

                        else
                            return InputFileContent;
                    }
                }
                else
                {
                    string InputFileContent = input;
                    if (InputFileContent.IsDeadByDaylightCryptoString())
                        return EminFile.DecryptSavefile(InputFileContent);

                    else if (InputFileContent.IsBase64String())
                        return System.Text.Encoding.ASCII.GetString(
                            System.Convert.FromBase64String(InputFileContent));

                    else
                        return InputFileContent;
                }
            }
            catch { return string.Empty; }
        }

        private static void InitializeSaveFileBackup(string input)
        {
            try
            {
                if (!Directory.Exists($"{ProgramCurrentDirectory}\\Backups"))
                    Directory.CreateDirectory($"{ProgramCurrentDirectory}\\Backups");
                File.WriteAllText($"{ProgramCurrentDirectory}\\Backups\\[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString().Replace(":", "։")}] fullProfile.backup.txt", input);
            }
            catch { }
        }
        private static void InitializeSaveFileDump(string input)
        {
            try
            {
                if (!Directory.Exists($"{ProgramCurrentDirectory}\\Dumped SaveFiles"))
                    Directory.CreateDirectory($"{ProgramCurrentDirectory}\\Dumped SaveFiles");
                File.WriteAllText($"{ProgramCurrentDirectory}\\Dumped SaveFiles\\[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString().Replace(":", "։")}] fullProfile.dump.txt", input);
            }
            catch { }
        }

        private static void IncreaseConsoleBufferSize()
        {
            Console.SetIn(new StreamReader(Console.OpenStandardInput(),
                               Console.InputEncoding,
                               false,
                               bufferSize: 1024));

            IsProgramInitialized = true;
        }
    }
}
