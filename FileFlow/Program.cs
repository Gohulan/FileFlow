using System;
using System.IO;

class Program
{
    static string primaryLocation = @"C:\Users\gohul\source\repos";
    static string backupLocation = @"D:\DRIVE\repos";
    static string logFilePath = AppDomain.CurrentDomain.BaseDirectory + "FileMonitorLog.txt";

    static void Main(string[] args)
    {
        try
        {
            MonitorDirectories(primaryLocation, backupLocation);
        }
        catch (Exception ex)
        {
            Log($"Error: {ex.Message}");
        }
    }

    static void MonitorDirectories(string primary, string backup)
    {
        var primaryFiles = Directory.GetFiles(primary, "*", SearchOption.AllDirectories);

        foreach (var primaryFile in primaryFiles)
        {
            string relativePath = primaryFile.Substring(primary.Length);
            string backupFile = Path.Combine(backup, relativePath.TrimStart(Path.DirectorySeparatorChar));

            if (File.Exists(backupFile))
            {
                DateTime primaryLastModified = File.GetLastWriteTimeUtc(primaryFile);
                DateTime backupLastModified = File.GetLastWriteTimeUtc(backupFile);

                if (primaryLastModified > backupLastModified)
                {
                    File.Copy(primaryFile, backupFile, true);
                    Log($"Updated: {backupFile} from {primaryFile}");
                }
            }
            else 
            {
                Directory.CreateDirectory(Path.GetDirectoryName(backupFile));
                File.Copy(primaryFile, backupFile);
                Log($"Copied: {primaryFile} to {backupFile}");
                
            }
        }
    }

    static void Log(string message)
    {
        using (StreamWriter sw = new StreamWriter(logFilePath, true))
        {
            sw.WriteLine($"{DateTime.Now}: {message}");
        }
    }
}
