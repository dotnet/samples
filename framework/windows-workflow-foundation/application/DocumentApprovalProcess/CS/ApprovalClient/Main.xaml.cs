//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activities;
using System.ServiceModel.Discovery;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Samples.DocumentApprovalProcess.ApprovalManager;
using Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary;


namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalClient
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        User user;                              // Subscribed users
        bool subscribed = false;                // Tracks if a subscription to the approval service exists
        ServiceHost sh;                         // Host for receiving approval requests to approve/reject
        WorkflowServiceHost wsh;                // Host for workflow -- receives responses to requests made by this client
        DiscoveryClient subscriptionDC;         // Discovery client to find the subscription service
        DiscoveryClient approvalDC;             // Discovery client to find the approval process management service
        int discoveryCount;                     // Used in discovery logic -- counts how many services discovered so far
        String discoveryLock = "";              // Used in discovery logic -- lock on this when manipulating discoveryCount
        EndpointAddress subscriptionAddr;       // Stores discovered subscription service address
        EndpointAddress approvalAddr;           // Stores discovered approval process management service address
        Random random = new Random();           // For generating random port numbers
        TextWriter statusWriter;                // For outputing to the status textbox

        string addrListenForApprovalResponses;  // Address for WFSH listening for approval responses
        string addrListenForApprovalRequests;   // Address for SH listening for approval requests (approving/rejecting other clients requests)

        static System.ServiceModel.Channels.Binding globalBinding = new BasicHttpBinding();

        private int GenerateRandomPort()
        {
            // Randomly generate port number from dynamic/private range
            return random.Next(49152, 65536);
        }

        public Main()
        {
            InitializeComponent();

            statusWriter = new WindowTextWriter(statusConsole);
            WriterParticipant.Writer = statusWriter;

            addrListenForApprovalResponses = "http://localhost:" + Convert.ToString(GenerateRandomPort()) + "/ClientService";
            addrListenForApprovalRequests = "http://localhost:" + Convert.ToString(GenerateRandomPort()) + "/ApprovalService";

            // Set static variable in another class so WCF service can find the user interface instance
            ExternalToMainComm.Context = this;

            // Service Host for approving requests from other clients
            sh = new ServiceHost(typeof(Microsoft.Samples.DocumentApprovalProcess.ApprovalClient.ApprovalRequestsService), new Uri(addrListenForApprovalRequests));

            // Create the workflow and service to be hosted by the Workflow ServiceHost
            Activity element = new ClientRequestApprovalWorkflow();
            WorkflowService wshservice = new WorkflowService
            {
                Name = "ApprovalMonitor",
                ConfigurationName = "Microsoft.Samples.DocumentApprovalProcess.ApprovalClient.ApprovalMonitor",
                Body = element
            };

            wsh = new WorkflowServiceHost(wshservice, new Uri(addrListenForApprovalResponses));

            subscriptionDC = new DiscoveryClient(new UdpDiscoveryEndpoint());
            approvalDC = new DiscoveryClient(new UdpDiscoveryEndpoint());

            sh.Open();
            wsh.Open();

            statusWriter.WriteLine(addrListenForApprovalResponses);
            statusWriter.WriteLine(addrListenForApprovalRequests);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Let the approval manager know that this client is no longer participating
            //  in the approval system because the client app is closing
            if (subscribed)
            {
                SubscriptionServiceClient sclient = new SubscriptionServiceClient();

                sclient.Endpoint.Address = subscriptionAddr;

                statusWriter.WriteLine("Unsubcribing from service ...");
                try
                {
                    sclient.Unsubscribe(user);
                    statusWriter.WriteLine("Unsubscribed");
                }
                catch (Exception)
                {
                    statusWriter.WriteLine("Failed to unsubscribe from service ...");
                }
            }

            wsh.Close();
            sh.Close();

            base.OnClosing(e);
        }

        private void discover_Click(object sender, RoutedEventArgs e)
        {
            name.IsEnabled = false;
            userType.IsEnabled = false;
            connect.IsEnabled = false;
            statusWriter.WriteLine("Discovering Services...");

            discoveryCount = 0;

            // Setup function to call when discovery completed -- two different discovery processes needed to find the two different services
            subscriptionDC.FindCompleted += new EventHandler<FindCompletedEventArgs>(SubscriptionFindCompleted);
            approvalDC.FindCompleted += new EventHandler<FindCompletedEventArgs>(ApprovalFindCompleted);

            // Start the discovery process for both services being looked for
            FindCriteria sfc = new FindCriteria(typeof(ISubscriptionService));
            FindCriteria afc = new FindCriteria(typeof(IApprovalProcess));
            sfc.Duration = new TimeSpan(0, 0, 2);
            afc.Duration = new TimeSpan(0, 0, 2);
            subscriptionDC.FindAsync(sfc);
            approvalDC.FindAsync(afc);
        }

        void DiscoveredCommonOps()
        {
            // Can't lock on an int -- so we use another object
            lock (discoveryLock)
            {
                discoveryCount++;  // used to track how many discovery services have completed
                if (discoveryCount >= 2)
                {
                    // if both services discovered, setup client for other operations
                    statusWriter.WriteLine("Services Discovered");
                    name.IsEnabled = true;
                    userType.IsEnabled = true;
                    connect.IsEnabled = true;
                }
            }
        }

        void SubscriptionFindCompleted(object sender, FindCompletedEventArgs e)
        {
            // If we didn't find any endpoints -- the discovery failed
            if (e.Result.Endpoints.Count > 0)
            {
                subscriptionAddr = e.Result.Endpoints[0].Address;
                DiscoveredCommonOps();
            }
            else
            {
                statusWriter.WriteLine("Discovery Failed");
            }
        }

        void ApprovalFindCompleted(object sender, FindCompletedEventArgs e)
        {
            // If we didn't find any endpoints -- the discovery failed
            if (e.Result.Endpoints.Count > 0)
            {
                approvalAddr = e.Result.Endpoints[0].Address;
                DiscoveredCommonOps();
            }
            else
            {
                statusWriter.WriteLine("Discovery Failed");
            }
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            connect.IsEnabled = false;
            name.IsEnabled = false;
            userType.IsEnabled = false;

            SubscriptionServiceClient sclient = new SubscriptionServiceClient();
            // Set endpoint to previously discovered address
            sclient.Endpoint.Address = subscriptionAddr;

            statusWriter.WriteLine("Subscribing to " + sclient.Endpoint.Address + " as " + name.Text + " of type " + userType.Text);

            user = sclient.Subscribe(new User(name.Text, userType.Text, addrListenForApprovalRequests, addrListenForApprovalResponses));
            subscribed = true;

            disconnect.IsEnabled = true;
            approvalType.IsEnabled = true;
            docName.IsEnabled = true;
            document.IsEnabled = true;
            requestApproval.IsEnabled = true;

            statusWriter.WriteLine("Subscribed with user guid " + user.Id);
        }

        private void disconnect_Click(object sender, RoutedEventArgs e)
        {
            disconnect.IsEnabled = false;
            approvalType.IsEnabled = false;
            docName.IsEnabled = false;
            document.IsEnabled = false;
            requestApproval.IsEnabled = false;

            SubscriptionServiceClient sclient = new SubscriptionServiceClient();

            sclient.Endpoint.Address = subscriptionAddr;

            statusWriter.WriteLine("Unsubcribing from service ...");
            try
            {
                sclient.Unsubscribe(user);
            }
            catch (Exception)
            {
                statusWriter.WriteLine("Unsubscribe failed ... forcing unsubcription");
            }
            subscribed = false;

            statusWriter.WriteLine("Unsubscribed");

            connect.IsEnabled = true;
            name.IsEnabled = true;
            userType.IsEnabled = true;
        }

        // This allows the client side WCF services to inform the UI that there is a new item waiting for approval
        public void AddApprovalItem(ApprovalRequest request)
        {
            lock (approvalList)
            {
                // This function may have been called from another thread so we must
                //  schedule the work with the WPF dispatcher
                approvalList.Items.Dispatcher.Invoke((Action<ApprovalRequest>)delegate(ApprovalRequest r) { approvalList.Items.Add(r); } ,
                                                     request);
            }
        }

        // Function called if ApprovalRequest is canceled
        public void RemoveApprovalItem(ApprovalRequest request)
        {
            lock (approvalList)
            {
                // This function may have been called from another thread so we must
                //  schedule the work with the WPF dispatcher
                approvalList.Items.Dispatcher.Invoke((Action<ApprovalRequest>)delegate(ApprovalRequest r) 
                                                        {
                                                            foreach (ApprovalRequest waitingRequest in approvalList.Items)
                                                            {
                                                                if (waitingRequest.Id == r.Id) approvalList.Items.Remove(waitingRequest);
                                                            }
                                                            if (approvalList.SelectedItem == null) approve.IsEnabled = false;
                                                        },
                                                     request);
            }
        }

        private void approvalList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (approvalList.SelectedItem != null)
            {
                approve.IsEnabled = true;
                reject.IsEnabled = true;
                titleReview.Text = ((ApprovalRequest)approvalList.SelectedItem).DocumentTitle;
                docReview.Text = ((ApprovalRequest)approvalList.SelectedItem).Document;
            }
            else
            {
                approve.IsEnabled = false;
                reject.IsEnabled = false;
                titleReview.Text = "";
                docReview.Text = "";
            }
        }

        public TextBox GetStatusTextBox()
        {
            return this.statusConsole;
        }

        private void approve_Click(object sender, RoutedEventArgs e)
        {
            if (approvalList.Items.Count > 0)
            {
                ApprovalProcessClient client = new ApprovalProcessClient(globalBinding, approvalAddr);
                ApprovalRequest request = (ApprovalRequest)approvalList.SelectedItem;

                client.Open();
                client.ResponsedToApprovalRequest(new ApprovalResponse(request, true));
                client.Close();

                lock (approvalList)
                {
                    approvalList.Items.Remove(request);
                }
                titleReview.Text = "";
                docReview.Text = "";
            }
        }

        private void reject_Click(object sender, RoutedEventArgs e)
        {
            if (approvalList.Items.Count > 0)
            {
                ApprovalProcessClient client = new ApprovalProcessClient(globalBinding, approvalAddr);
                ApprovalRequest request = (ApprovalRequest)approvalList.SelectedItem;

                client.Open();
                client.ResponsedToApprovalRequest(new ApprovalResponse(request, false));
                client.Close();

                lock (approvalList)
                {
                    approvalList.Items.Remove(request);
                }
                titleReview.Text = "";
                docReview.Text = "";
                
            }
        }


        private void requestApproval_Click(object sender, RoutedEventArgs e)
        {
            ApprovalRequest request = new ApprovalRequest(docName.Text, document.Text, approvalType.Text, user);
            PendingList.Items.Add(request);

            ApprovalResultsClient c = new ApprovalResultsClient(globalBinding, new EndpointAddress(addrListenForApprovalResponses));
            c.Open();
            c.StartGetApproval(request, approvalAddr.Uri);
            c.Close();
        }

        public void ProcessResponse(ApprovalResponse response)
        {
            object requestToDel = null;
            foreach (object o in PendingList.Items)
            {
                if(((ApprovalRequest)o).Id.Equals(response.Id))
                {
                    requestToDel = o;
                    break;
                }
            }
            if (requestToDel != null)
            {
                lock (PendingList)
                {
                    // This function may have been called from another thread so we must
                    //  schedule the work with the WPF dispatcher
                    PendingList.Items.Dispatcher.Invoke((Action<ApprovalRequest>)delegate(ApprovalRequest r) 
                                                            { PendingList.Items.Remove(r); PendingList_SelectionChanged(this, null); },
                                                        requestToDel);
                }
                lock (ApprovedList)
                {
                    ApprovedList.Items.Dispatcher.Invoke((Action<ApprovalResponse>)delegate(ApprovalResponse r) { ApprovedList.Items.Add(r); }, 
                                                         response);
                }
            }
        }

        private void PendingList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PendingList.SelectedItem != null)
            {
                CancelRequest.IsEnabled = true;
            }
            else
            {
                CancelRequest.IsEnabled = false;
            }
        }

        private void CancelRequest_Click(object sender, RoutedEventArgs e)
        {
            ApprovalResultsClient client = new ApprovalResultsClient(globalBinding, new EndpointAddress(addrListenForApprovalResponses));
            ApprovalRequest request = (ApprovalRequest)PendingList.SelectedItem;

            client.Open();
            client.CancelApprovalRequest(request);
            client.Close();

            lock (PendingList)
            {
                PendingList.Items.Remove(request);
            }
            CancelRequest.IsEnabled = false;
        }
    }
}
