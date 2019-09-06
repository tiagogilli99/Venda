using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Text;

namespace VendaService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            FileSystemWatcher fileWatcher = new FileSystemWatcher(ConfigurationManager.AppSettings["WatchPath"]);

            fileWatcher.Created += new FileSystemEventHandler(Created);

            fileWatcher.EnableRaisingEvents = true;
        }

        static void Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                string fileText = File.ReadAllText(e.FullPath, Encoding.UTF8);
                Arquivo arquivo = new Arquivo();

                arquivo.ProcessaArquivo(fileText);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void OnStop()
        {
        }
    }
}
