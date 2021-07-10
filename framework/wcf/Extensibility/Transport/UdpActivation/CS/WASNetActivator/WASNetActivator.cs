//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.ServiceModel.Samples.Activation;
using Microsoft.ServiceModel.Samples.Install;

namespace Microsoft.ServiceModel.Samples
{
    public partial class WASNetActivator : Form
    {
        public WASNetActivator()
        {
            InitializeComponent();
            chkListenerAdapter.Checked = UdpInstaller.IsListenerAdapterInstalled;
            chkProtocols.Checked = UdpInstaller.IsProtocolHandlerInstalled;
            this.cbbProtocol.SelectedIndex = 0;
            this.btnStart.Select();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            UdpListenerAdapter.Start();
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            UdpListenerAdapter.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            UdpInstallerOptions options;
            options.ListenerAdapterChecked = chkListenerAdapter.Checked;
            options.ProtocolHandlerChecked = chkProtocols.Checked;

            UdpInstaller.Install(options);
            MessageBox.Show(this, "Installation finished", "Setup");
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            UdpInstallerOptions options;
            options.ListenerAdapterChecked = chkListenerAdapter.Checked;
            options.ProtocolHandlerChecked = chkProtocols.Checked;

            UdpInstaller.Uninstall(options);
            MessageBox.Show(this, "Uninstall finished", "Setup");
        }
    }
}
