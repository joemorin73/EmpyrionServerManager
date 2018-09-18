using System;
using EmpyrionModApi;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmpyrionServerManager
{
    public partial class Form1 : Form
    {
        public IGameServerConnection _gameServerConnection;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program prog = new Program();

            _gameServerConnection = prog.Run();
            _gameServerConnection.Event_ChatMessage += OnEvent_ChatMessage;

            timer_5sec.Interval = 5000;
            timer_5sec.Tick += new EventHandler(timer_5sec_Tick);
            timer_5sec.Enabled = true;

            sayTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(CheckEnterKeyPress);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.AppendText("more\r\n");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void inGameChatBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void sayBtn_Click(object sender, EventArgs e)
        {
            inGameChatBox.Invoke(new Action(() => inGameChatBox.AppendText("Server" + ": " + sayTextBox.Text + "\r\n")));
            _gameServerConnection.SendChatMessageToAll(sayTextBox.Text);
            sayTextBox.Text = "";
        }

        private void timer_5sec_Tick( object Sender, EventArgs e )
        {
            onlinePlayersList.Items.Clear();
            var factions = _gameServerConnection.GetFactions();
            foreach( KeyValuePair<int,Player> playerEntry in _gameServerConnection.GetOnlinePlayers() )
            {
                Faction faction;
                factions.TryGetValue(playerEntry.Value.FactionIdOrEntityId, out faction);
                var playerLine = String.Format(
                    "{0,-15}|{1,-7}|{2,-15}|{3,-19}|{4,-9}",
                    playerEntry.Value.Name.PadRight(15).Substring(0, 15),
                    ((faction != null) ? faction.Initials : "").PadRight(7).Substring(0, 7),
                    playerEntry.Value.Playfield.PadRight(15).Substring(0, 15),
                    playerEntry.Value.Position.position.ToString().Replace("<","").Replace(">","").PadRight(19).Substring(0,19),
                    playerEntry.Value.EntityId.ToString()
                    );
                onlinePlayersList.Items.Add(playerLine);
            }
        }

        private void sayTextBox_TextChanged(object sender, EventArgs e)
        {
        }


        ///-----------------------------------
        private void OnEvent_ChatMessage(ChatType chatType, string msg, Player player)
        {
            //inGameChatBox.AppendText(player.Name + ": " + msg);
            inGameChatBox.Invoke(new Action(() => inGameChatBox.AppendText(player.Name + ": " + msg + "\r\n")));

        }

        private void CheckEnterKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                e.Handled = true;
                sayBtn_Click(sender, e);
            }
        }
    }
}

