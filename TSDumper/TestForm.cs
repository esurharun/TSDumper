using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DirectShow;
using DomainObjects;
using DVBServices;

namespace EPGCollectorGUI
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            #region Test Data
            TVStation new1 = new TVStation("TVOne");
            new1.AudioPID = 1;
            new1.DSMCCPID = 1332;
            new1.Frequency = 538000;
            new1.MappedServiceID = 29;
            new1.Name = "TVOne";
            new1.OriginalNetworkID = 100;
            new1.ServiceID = 1200;
            new1.ServiceType = 22;
            new1.TransportStreamID = 3;
            TVStation.StationCollection.Add(new1);

            new1 = new TVStation("TVTwo");
            new1.AudioPID = 2;
            new1.DSMCCPID = 2332;
            new1.Frequency = 638000;
            new1.MappedServiceID = 25;
            new1.Name = "TVTwo";
            new1.OriginalNetworkID = 200;
            new1.ServiceID = 1201;
            new1.ServiceType = 22;
            new1.TransportStreamID = 4;
            TVStation.StationCollection.Add(new1);

            new1 = new TVStation("TV3");
            new1.AudioPID = 3;
            new1.DSMCCPID = 4332;
            new1.Frequency = 738000;
            new1.MappedServiceID = 21;
            new1.Name = "TV3";
            new1.OriginalNetworkID = 300;
            new1.ServiceID = 1203;
            new1.ServiceType = 22;
            new1.TransportStreamID = 4;
            TVStation.StationCollection.Add(new1);
            #endregion

            BindingList<TVStation> stations = new BindingList<TVStation>();

            _dgCars.AutoGenerateColumns = false;

            foreach (TVStation item in TVStation.StationCollection)
            {
                stations.Add(item);
            }

            _dgCars.DataSource = stations;

            // network ID, transport ID, service ID, channel ID, name
            DataGridViewCheckBoxColumn selectChannel = new DataGridViewCheckBoxColumn();
            
            selectChannel.HeaderText = "Collect EPG Data";

            DataGridViewTextBoxColumn nidColumn = new DataGridViewTextBoxColumn();
            nidColumn.DataPropertyName = "OriginalNetworkID";
            nidColumn.HeaderText = "Network ID (NID)";
            nidColumn.ReadOnly = true;

            DataGridViewTextBoxColumn tidColumn = new DataGridViewTextBoxColumn();
            tidColumn.DataPropertyName = "TransportStreamID";
            tidColumn.HeaderText = "Transport ID (TID)";
            tidColumn.ReadOnly = true;

            DataGridViewTextBoxColumn sidColumn = new DataGridViewTextBoxColumn();
            sidColumn.DataPropertyName = "ServiceID";
            sidColumn.HeaderText = "Service ID (SID)";
            sidColumn.ReadOnly = true;

            DataGridViewTextBoxColumn channelIDColumn = new DataGridViewTextBoxColumn();
            channelIDColumn.DataPropertyName = "MappedServiceID";
            channelIDColumn.HeaderText = "Original Channel ID";

            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.DataPropertyName = "Name";
            nameColumn.HeaderText = "Original Name";

            _dgCars.Columns.Add(selectChannel);
            _dgCars.Columns.Add(nidColumn);
            _dgCars.Columns.Add(tidColumn);
            _dgCars.Columns.Add(sidColumn);
            _dgCars.Columns.Add(channelIDColumn);
            _dgCars.Columns.Add(nameColumn);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }


    }
}
