using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

namespace SearchReplaceRename
{
    public partial class ConsoleWindow : Window
    {
        public ConsoleWindow(string path, string search, string replace)
        {
            Path = path;
            Search = search;
            Replace = replace;

            InitializeComponent();
        }

        public string Path { get; set; }
        public string Search { get; set; }
        public string Replace { get; set; }

        private void Window_Initialized(object sender, EventArgs e)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            MessageBox.Show("Replacement completed!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DoWork(object sender, DoWorkEventArgs args)
        {
            if (!Directory.Exists(Path))
            {
                WriteLine("ERROR: Path does not exist. Please select a valid path and try again");
            }
            else
            {
                WriteLine(string.Format(@"Renaming all files and directories containing '{0}' to have '{1}' instead in path '{2}'", Search, Replace, Path));
                SearchAndReplaceRename(Search, Replace, Path);
            }
        }

        private void WriteLineSync(string line)
        {
            textBox1.Text += "\n" + line;
        }

        private void WriteLine(string line)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(WriteLineSync), line);
        }

        private void SearchAndReplaceRename(string search, string replace, string path)
        {
            string[] excludedFolders = { "bin", ".vs", ".git", "Logs" };
            string[] allowedExtensions = { ".cs", ".cshtml", ".csproj", ".csproj.user", ".sln" };

            if (excludedFolders.Any(folder => LastPathSegment(path).Equals(folder, StringComparison.OrdinalIgnoreCase)))
            {
                WriteLine($"Skipping folder: {path}");
                return;
            }

            foreach (var file in Directory.GetFiles(path).Where(file => allowedExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase))))
            {
                RenameIfMatch(file, search, replace);
                ReplaceContent(file, search, replace);
            }

            foreach (var directory in Directory.GetDirectories(path))
            {
                SearchAndReplaceRename(search, replace, directory);
                RenameIfMatch(directory, search, replace);
            }
        }

        private static string LastPathSegment(string path)
        {
            var parts = path.Split('\\');
            return parts.Length > 0 ? parts[parts.Length - 1] : path;
        }

        private static void RenameIfMatch(string path, string search, string replace)
        {
            try
            {
                var name = System.IO.Path.GetFileName(path);
                if (name.Contains(search))
                {
                    var newName = name.Replace(search, replace);
                    var newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), newName);
                    Directory.Move(path, newPath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

        }

        private static void ReplaceContent(string file, string search, string replace)
        {
            try
            {
                var content = File.ReadAllText(file);
                if (content.Contains(search))
                {
                    content = content.Replace(search, replace);
                    File.WriteAllText(file, content);
                }

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message); ;
            }
        }
    }
}