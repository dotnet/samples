//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Windows.Forms;
using Microsoft.Samples.WF.PurchaseProcess;

namespace Microsoft.Samples.WinFormsClient
{

    // this form shows all the Rfp (active and finished)
    public partial class ShowProposals : Form
    {
        // Purchase process workflow host. This instance of the host will be used in all other forms
        public PurchaseProcessHost PurchaseProcessHost;
        
        // selected rfp
        Guid selectedRfpId = Guid.Empty;
        bool selectedIsFinished = false;

        public ShowProposals()
        {
            InitializeComponent();
        }

        private void ShowProposals_Load(object sender, EventArgs e)
        {
            // create an instance of the host 
            this.PurchaseProcessHost = new PurchaseProcessHost();

            // refresh the lists of Requests for Proposals (active and finished)
            this.RefreshLists();

            // load the participants combo in the toolbar
            this.LoadParticipantsCombo();
        }

        // load the combo in the toolbar to select the current participant
        void LoadParticipantsCombo()
        {
            foreach (Vendor vendor in VendorRepository.RetrieveAll())
            {
                this.cboParticipant.Items.Add(vendor);
            }
            this.cboParticipant.Items.Add("Company X employee");
            this.cboParticipant.SelectedIndex = 0;
        }

        // refresh the list of Rfps
        void RefreshLists()
        {

            //-------------------------------------------
            // Show list of active Request for Proposals
            //-------------------------------------------
            // initialize the list
            this.InitList(this.lstActive);

            // create headers
            this.AddHeaderToList(this.lstActive, "ID", 220);
            this.AddHeaderToList(this.lstActive, "Title", 150);
            this.AddHeaderToList(this.lstActive, "Created", 150);
            this.AddHeaderToList(this.lstActive, "Invited Vendors", 300);            

            foreach (RequestForProposal rfp in RfpRepository.RetrieveActive())
            {
                ListViewItem item = new ListViewItem(rfp.ID.ToString());
                item.SubItems.Add(rfp.Title);
                item.SubItems.Add(rfp.CreationDate.ToString());
                item.SubItems.Add(rfp.GetInvitedVendorsStatus(true));
                this.lstActive.Items.Add(item);
            }

            //-------------------------------------------
            // Show list of finished Request for Proposals
            //-------------------------------------------
            // initialize the list
            this.InitList(this.lstFinished);

            // create headers
            this.AddHeaderToList(this.lstFinished, "Rfp ID", 220);
            this.AddHeaderToList(this.lstFinished, "Title", 150);
            this.AddHeaderToList(this.lstFinished, "Created", 150);            
            this.AddHeaderToList(this.lstFinished, "Finished", 150);
            this.AddHeaderToList(this.lstFinished, "Invited Vendors", 200);            
            this.AddHeaderToList(this.lstFinished, "Winner", 120);

            // show rfps in the list
            foreach (var rfp in RfpRepository.RetrieveFinished())
            {
                ListViewItem item = new ListViewItem(rfp.ID.ToString());
                item.SubItems.Add(rfp.Title);
                item.SubItems.Add(rfp.CreationDate.ToString());                
                item.SubItems.Add(rfp.CompletionDate.ToString());
                item.SubItems.Add(rfp.GetInvitedVendorsStatus());
                item.SubItems.Add(string.Format("{0} ({1} USD)", rfp.BestProposal.Vendor.Name, rfp.BestProposal.Value));
                this.lstFinished.Items.Add(item);
            }
        }

        // initialize and clear a list
        void InitList(ListView list)
        {
            list.Columns.Clear();
            list.Items.Clear();
            list.View = View.Details;
        }

        // add a header to a list
        void AddHeaderToList(ListView list, string headerCaption, int headerWidth)
        {
            ColumnHeader header = new ColumnHeader();
            header.Text = headerCaption;
            header.Width = headerWidth;   
            list.Columns.Add(header);
        }

        // show a Request for Proposals in a new window
        void ShowRfp()
        {
            if (this.selectedRfpId == Guid.Empty)
            {
                MessageBox.Show("Select an RFP from the lists in this screen");
            }
            else
            {
                if (this.cboParticipant.SelectedItem is Vendor)
                {
                    if (this.selectedIsFinished)
                    {
                        MessageBox.Show("A vendor can't see a finished Request for Proposals");
                    }
                    else
                    {
                        SubmitProposal view = new SubmitProposal();
                        view.VendorId = ((Vendor)this.cboParticipant.SelectedItem).Id;
                        view.RfpId = this.selectedRfpId;
                        view.PurchaseProcessHost = this.PurchaseProcessHost;
                        view.ShowDialog();
                    }
                }
                else
                {
                    ViewRfp viewProposal = new ViewRfp();
                    viewProposal.RfpId = this.selectedRfpId;
                    viewProposal.ShowDialog();
                }
            }
        }

        // create a new Request for Proposals (toolbar button)
        void tbbCreate_Click(object sender, EventArgs e)
        {
            NewRfp newRfpForm = new NewRfp();
            newRfpForm.PurchaseProcessHost = this.PurchaseProcessHost;
            newRfpForm.Show();
        }

        // refresh the lists (toolbar button)
        void tbbRefresh_Click(object sender, EventArgs e)
        {
            this.RefreshLists();
        }

        // show a Request for Proposals (toolbar button)
        void btnView_Click(object sender, EventArgs e)
        {
            this.ShowRfp();
        }

        // show a Request for Proposals when double clickin in the list
        void lstActive_DoubleClick(object sender, EventArgs e)
        {
            this.ShowRfp();
        }

        // show a Request for Proposals when double clickin in the list
        void lstFinished_DoubleClick(object sender, EventArgs e)
        {
            this.ShowRfp();
        }

        // set the id of the selected Request for Proposals
        void lstActive_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            this.lblStatus.Text = "Selected RFP: " + e.Item.Text;
            this.selectedRfpId = new Guid(e.Item.Text);
            this.selectedIsFinished = false;
        }

        // set the id of the selected Request for Proposals
        void lstFinished_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            this.lblStatus.Text = "Selected RFP: " + e.Item.Text;
            this.selectedRfpId = new Guid(e.Item.Text);
            this.selectedIsFinished = true;
        }
    }
}
