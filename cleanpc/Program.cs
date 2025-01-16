using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PCCleaner
{
    class Program
    {
        private static Dictionary<string, long> spaceSavings = new Dictionary<string, long>();

        static void DisplayHeader()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════╗
║                     PC CLEANER PRO v1.0                      ║
║           Utilitaire de nettoyage système avancé             ║
╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        static void DisplayMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n═══════════════ MENU PRINCIPAL ═══════════════");
            Console.ResetColor();

            string[] menuItems = {
                "1. 🔍 Analyser l'espace récupérable",
                "2. 🌐 Nettoyer le cache navigateurs",
                "3. 🪟 Nettoyer le cache Windows",
                "4. 📁 Nettoyer les fichiers temporaires",
                "5. 🗑️ Nettoyer la corbeille",
                "6. 🔄 Nettoyer les fichiers de mise à jour Windows",
                "7. 👤 Nettoyer les profils obsolètes",
                "8. 🖼️ Nettoyer les miniatures",
                "9. 📊 Voir l'espace total récupérable",
                "10. 🧹 Tout nettoyer",
                "11. ❌ Quitter"
            };

            foreach (string item in menuItems)
            {
                if (item.StartsWith("10") || item.StartsWith("11"))
                {
                    Console.ForegroundColor = item.StartsWith("11") ? ConsoleColor.Red : ConsoleColor.Green;
                    Console.WriteLine(item);
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(item);
                }
            }

            Console.WriteLine("\n═══════════════════════════════════════════════");
        }

        static void DisplayProgressBar(int percentage)
        {
            Console.Write("[");
            var pos = Console.CursorLeft;
            Console.Write(new string('═', 50));
            Console.CursorLeft = pos;
            Console.Write(new string('█', (int)(percentage / 2)));
            Console.Write("] {0,3:0}%", percentage);
        }

        static void DisplayDiskInfo()
        {
            Console.Clear();
            DisplayHeader();

            var drives = DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Fixed);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n╔═══════════════ INFORMATIONS DU DISQUE ════════════════╗");
            Console.ResetColor();

            foreach (var drive in drives)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"║ Disque {drive.Name.TrimEnd('\\')}");
                Console.ResetColor();

                long used = drive.TotalSize - drive.AvailableFreeSpace;
                int usedPercentage = (int)((double)used / drive.TotalSize * 100);

                // Barre d'utilisation
                Console.Write("║ [");
                for (int i = 0; i < 50; i++)
                {
                    if (i < usedPercentage / 2)
                    {
                        Console.ForegroundColor = usedPercentage > 90 ? ConsoleColor.Red :
                                                usedPercentage > 70 ? ConsoleColor.Yellow :
                                                ConsoleColor.Green;
                        Console.Write("█");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("░");
                    }
                }
                Console.Write($"] {usedPercentage}%");
                Console.ResetColor();
                Console.WriteLine();

                Console.WriteLine($"║ Espace total: {FormatSize(drive.TotalSize)}");
                Console.WriteLine($"║ Espace utilisé: {FormatSize(used)}");
                Console.WriteLine($"║ Espace libre: {FormatSize(drive.AvailableFreeSpace)}");
                Console.WriteLine("║" + new string('─', 52));
            }

            // Afficher l'espace récupérable si analysé
            if (spaceSavings.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("║ ESPACE RÉCUPÉRABLE PAR CATÉGORIE:");
                Console.ResetColor();

                foreach (var item in spaceSavings)
                {
                    Console.WriteLine($"║ ├─ {item.Key}: {FormatSize(item.Value)}");
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"║ └─ TOTAL RÉCUPÉRABLE: {FormatSize(spaceSavings.Values.Sum())}");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        static void Main(string[] args)
        {
            Console.Title = "PC Cleaner Pro v1.0";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (!IsAdministrator())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n⚠️  ATTENTION: Droits administrateur requis!");
                Console.WriteLine("    Veuillez relancer le programme en tant qu'administrateur.");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            while (true)
            {
                DisplayDiskInfo();  // Afficher les informations du disque
                DisplayMenu();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nVotre choix (1-11): ");
                Console.ResetColor();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AnalyzeAllCleanableSpace();
                        break;
                    case "2":
                        CleanBrowserCache();
                        break;
                    case "3":
                        CleanWindowsCache();
                        break;
                    case "4":
                        CleanTempFiles();
                        break;
                    case "5":
                        CleanRecycleBin();
                        break;
                    case "6":
                        CleanWindowsUpdate();
                        break;
                    case "7":
                        CleanObsoleteProfiles();
                        break;
                    case "8":
                        CleanThumbnails();
                        break;
                    case "9":
                        ShowTotalSpace();
                        break;
                    case "10":
                        CleanAll();
                        break;
                    case "11":
                        return;
                    default:
                        Console.WriteLine("Option invalide. Veuillez réessayer.");
                        break;
                }

                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
        }

        static void AnalyzeAllCleanableSpace()
        {
            Console.WriteLine("\nAnalyse de l'espace récupérable...");
            spaceSavings.Clear();

            // Analyse du cache des navigateurs
            spaceSavings["Navigateurs"] = CalculateBrowserCacheSize();

            // Analyse du cache Windows
            spaceSavings["Cache Windows"] = CalculateWindowsCacheSize();

            // Analyse des fichiers temporaires
            spaceSavings["Fichiers temporaires"] = CalculateTempFilesSize();

            // Analyse de la corbeille
            spaceSavings["Corbeille"] = CalculateRecycleBinSize();

            // Analyse des fichiers de mise à jour Windows
            spaceSavings["Windows Update"] = CalculateWindowsUpdateSize();

            // Analyse des miniatures
            spaceSavings["Miniatures"] = CalculateThumbnailsSize();

            // Affichage des résultats
            Console.WriteLine("\nEspace récupérable par catégorie:");
            foreach (var item in spaceSavings)
            {
                Console.WriteLine($"{item.Key}: {FormatSize(item.Value)}");
            }

            Console.WriteLine($"\nEspace total récupérable: {FormatSize(spaceSavings.Values.Sum())}");
        }

        static long CalculateBrowserCacheSize()
        {
            long total = 0;
            string[] browserPaths = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google\\Chrome\\User Data\\Default\\Cache"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Mozilla\\Firefox\\Profiles"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\Edge\\User Data\\Default\\Cache")
            };

            foreach (string path in browserPaths)
            {
                if (Directory.Exists(path))
                    total += CalculateFolderSize(path);
            }
            return total;
        }

        static void CleanBrowserCache()
        {
            Console.WriteLine("\nNettoyage du cache des navigateurs...");
            long sizeBefore = CalculateBrowserCacheSize();

            try
            {
                string[] browserPaths = {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google\\Chrome\\User Data\\Default\\Cache"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Mozilla\\Firefox\\Profiles"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\Edge\\User Data\\Default\\Cache")
                };

                foreach (string path in browserPaths)
                {
                    if (Directory.Exists(path))
                    {
                        DeleteDirectoryContents(path);
                    }
                }

                long sizeAfter = CalculateBrowserCacheSize();
                Console.WriteLine($"Espace libéré: {FormatSize(sizeBefore - sizeAfter)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du nettoyage du cache des navigateurs: {ex.Message}");
            }
        }

        static void CleanWindowsCache()
        {
            Console.WriteLine("\nNettoyage du cache Windows...");
            long sizeBefore = CalculateWindowsCacheSize();

            try
            {
                string[] cachePaths = {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SoftwareDistribution\\Download")
                };

                foreach (string path in cachePaths)
                {
                    if (Directory.Exists(path))
                    {
                        DeleteDirectoryContents(path);
                    }
                }

                // Vider le cache DNS
                System.Diagnostics.Process.Start("ipconfig", "/flushdns").WaitForExit();

                long sizeAfter = CalculateWindowsCacheSize();
                Console.WriteLine($"Espace libéré: {FormatSize(sizeBefore - sizeAfter)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du nettoyage du cache Windows: {ex.Message}");
            }
        }

        static void CleanAll()
        {
            Console.WriteLine("\nNettoyage complet en cours...");

            CleanBrowserCache();
            CleanWindowsCache();
            CleanTempFiles();
            CleanRecycleBin();
            CleanWindowsUpdate();
            CleanThumbnails();

            Console.WriteLine("\nNettoyage complet terminé!");
        }

        static void ShowTotalSpace()
        {
            if (spaceSavings.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n⚠️  Veuillez d'abord analyser l'espace récupérable (option 1)");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n╔════════════ RÉCAPITULATIF DE L'ESPACE ════════════╗");
            Console.ResetColor();

            foreach (var item in spaceSavings)
            {
                Console.Write($"║ {item.Key}: ");
                Console.CursorLeft = 30;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{FormatSize(item.Value),15} ║");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╠══════════════════════════════════════════════════╣");
            Console.Write("║ TOTAL: ");
            Console.CursorLeft = 30;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write($"{FormatSize(spaceSavings.Values.Sum()),15}");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" ║");
            Console.WriteLine("╚══════════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        static void CleanRecycleBin()
        {
            Console.WriteLine("\nNettoyage de la corbeille...");
            long sizeBefore = CalculateRecycleBinSize();

            try
            {
                System.Diagnostics.Process.Start("cmd", "/c rd /s /q C:\\$Recycle.Bin").WaitForExit();

                long sizeAfter = CalculateRecycleBinSize();
                Console.WriteLine($"Espace libéré: {FormatSize(sizeBefore - sizeAfter)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du nettoyage de la corbeille: {ex.Message}");
            }
        }

        static void CleanTempFiles()
        {
            Console.WriteLine("\nNettoyage des fichiers temporaires...");
            long sizeBefore = CalculateTempFilesSize();

            try
            {
                string[] tempPaths = new string[]
                {
                    Path.GetTempPath(),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Temp")
                };

                foreach (string path in tempPaths)
                {
                    if (Directory.Exists(path))
                    {
                        DeleteDirectoryContents(path);
                    }
                }

                long sizeAfter = CalculateTempFilesSize();
                Console.WriteLine($"Espace libéré: {FormatSize(sizeBefore - sizeAfter)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du nettoyage des fichiers temporaires: {ex.Message}");
            }
        }

        static void CleanObsoleteProfiles()
        {
            Console.WriteLine("\nRecherche des profils obsolètes...");

            try
            {
                string usersPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "..", "Users");
                var currentUser = Environment.UserName;

                var userFolders = Directory.GetDirectories(usersPath)
                    .Where(d => !d.EndsWith("Public") &&
                               !d.EndsWith(currentUser) &&
                               !d.EndsWith("Default") &&
                               !d.EndsWith("Default User") &&
                               !d.EndsWith("All Users"));

                if (!userFolders.Any())
                {
                    Console.WriteLine("Aucun profil obsolète trouvé.");
                    return;
                }

                Console.WriteLine("\nProfils trouvés:");
                foreach (var folder in userFolders)
                {
                    long size = CalculateFolderSize(folder);
                    Console.WriteLine($"- {Path.GetFileName(folder)} ({FormatSize(size)})");
                }

                Console.Write("\nVoulez-vous supprimer tous ces profils? (O/N): ");
                if (Console.ReadLine().Trim().ToUpper() == "O")
                {
                    long totalSize = 0;
                    foreach (var folder in userFolders)
                    {
                        try
                        {
                            totalSize += CalculateFolderSize(folder);
                            Directory.Delete(folder, true);
                            Console.WriteLine($"Profil {Path.GetFileName(folder)} supprimé.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erreur lors de la suppression de {Path.GetFileName(folder)}: {ex.Message}");
                        }
                    }
                    Console.WriteLine($"\nEspace total libéré: {FormatSize(totalSize)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du nettoyage des profils: {ex.Message}");
            }
        }

        static void CleanWindowsUpdate()
        {
            Console.WriteLine("\nNettoyage des fichiers de mise à jour Windows...");
            long sizeBefore = CalculateWindowsUpdateSize();

            try
            {
                string windowsUpdatePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SoftwareDistribution\\Download");
                if (Directory.Exists(windowsUpdatePath))
                {
                    DeleteDirectoryContents(windowsUpdatePath);
                }

                long sizeAfter = CalculateWindowsUpdateSize();
                Console.WriteLine($"Espace libéré: {FormatSize(sizeBefore - sizeAfter)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du nettoyage des fichiers de mise à jour Windows: {ex.Message}");
            }
        }

        static void CleanThumbnails()
        {
            Console.WriteLine("\nNettoyage du cache des miniatures...");
            long sizeBefore = CalculateThumbnailsSize();

            try
            {
                string thumbnailPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Microsoft\\Windows\\Explorer");

                var thumbFiles = Directory.GetFiles(thumbnailPath, "thumbcache_*.db");
                foreach (var file in thumbFiles)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }

                long sizeAfter = CalculateThumbnailsSize();
                Console.WriteLine($"Espace libéré: {FormatSize(sizeBefore - sizeAfter)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du nettoyage des miniatures: {ex.Message}");
            }
        }

        static long CalculateFolderSize(string folder)
        {
            long size = 0;
            try
            {
                DirectoryInfo di = new DirectoryInfo(folder);
                size += di.GetFiles().Sum(fi => fi.Length);
                size += di.GetDirectories().Sum(dir => CalculateFolderSize(dir.FullName));
            }
            catch { }
            return size;
        }

        static long CalculateWindowsCacheSize()
        {
            long total = 0;
            string[] paths = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch")
            };

            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                    total += CalculateFolderSize(path);
            }
            return total;
        }

        static long CalculateTempFilesSize()
        {
            return CalculateFolderSize(Path.GetTempPath());
        }

        static long CalculateRecycleBinSize()
        {
            return CalculateFolderSize("C:\\$Recycle.Bin");
        }

        static long CalculateWindowsUpdateSize()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SoftwareDistribution\\Download");
            return Directory.Exists(path) ? CalculateFolderSize(path) : 0;
        }

        static long CalculateThumbnailsSize()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Microsoft\\Windows\\Explorer");
            return Directory.Exists(path) ?
                Directory.GetFiles(path, "thumbcache_*.db").Sum(f => new FileInfo(f).Length) : 0;
        }

        static void DeleteDirectoryContents(string path)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch { }
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    try
                    {
                        dir.Delete(true);
                    }
                    catch { }
                }
            }
            catch { }
        }

        static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static string FormatSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;

            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }

            return $"{number:n1} {suffixes[counter]}";
        }
    }
}