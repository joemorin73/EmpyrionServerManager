using EmpyrionModApi;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;



namespace EmpyrionServerManager
{
    public class ControlWriter : TextWriter
    {
        private TextBox textbox;
        public ControlWriter(TextBox textbox)
        {
            this.textbox = textbox;
        }

        public override void Write(char value)
        {
            textbox.Invoke(new MethodInvoker(delegate { textbox.AppendText(value.ToString()); }));
        }

        public override void Write(string value)
        {
            textbox.Invoke(new MethodInvoker(delegate { textbox.AppendText(value); }));
        }

        public override void WriteLine(string value)
        {
            textbox.Invoke(new MethodInvoker(delegate { textbox.AppendText(value + "\r\n"); }));
            //textbox.Text += value;
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }



    public class Program
    {
        private static System.Diagnostics.TraceSource _traceSource =
                new System.Diagnostics.TraceSource("EmpyrionServerManager");

        CompositionContainer _container;

        IGameServerConnection _gameServerConnection;

        #pragma warning disable 0649
        [ImportMany]
        IEnumerable<IGameMod> _gameMods;
        #pragma warning restore 0649

        public IGameServerConnection Run()
        {
            var configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "Settings.yaml";
            var config = Configuration.GetConfiguration<Configuration>(configFilePath);

            var modPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Extensions";

            using (var catalog = new AggregateCatalog(
                new AssemblyCatalog(typeof(Program).Assembly),
                new DirectoryCatalog(modPath, "*Mod.dll")))
            {
                // iterate over all directories in .\Plugins dir and add all Plugin* dirs to catalogs
                foreach (var path in System.IO.Directory.EnumerateDirectories(modPath, "*", System.IO.SearchOption.TopDirectoryOnly))
                {
                    catalog.Catalogs.Add(new DirectoryCatalog(path, "*Mod.dll"));
                }

                _container = new CompositionContainer(catalog);

                try
                {
                    this._container.ComposeParts(this);

                    //using (_gameServerConnection = new GameServerConnection(config))
                    //{
                    _gameServerConnection = new GameServerConnection(config);

                        foreach (var gameMod in _gameMods)
                        {
                            gameMod.Start(_gameServerConnection);
                        }

                        _gameServerConnection.Connect();

//                        // wait until the user presses Enter.
//                        string input = Console.ReadLine();
//
//                        foreach (var gameMod in _gameMods)
//                        {
//                            gameMod.Stop();
//                        }

//                    }
                }
                catch (CompositionException compositionException)
                {
                    _traceSource.TraceEvent(System.Diagnostics.TraceEventType.Error, 1, compositionException.ToString());
                }
            }

            return (_gameServerConnection);
        }

        //        static void Main(string[] args)
        [STAThread]
        static void Main()
        {
            //var prog = new Program();
            //prog.Run();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 MainForm = new Form1();

            Console.SetOut(new ControlWriter(MainForm.textBox1));


            Application.Run(MainForm);


            _traceSource.Flush();
            _traceSource.Close();
        }
    }
}
