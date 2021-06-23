//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.Activities;
using System.Activities.Hosting;
using System.Collections.Generic;

namespace Microsoft.Samples.WF.PurchaseProcess
{

    /// <summary>
    /// Sample implementation of the host. It uses custom persistence
    /// for loading and saving the workflow instances and caches them in 
    /// a private dictionary.
    /// </summary>
    public class PurchaseProcessHost : IPurchaseProcessHost
    {                        
        IDictionary<Guid, WorkflowApplication> instances;

        public PurchaseProcessHost()
        {
            instances = new Dictionary<Guid, WorkflowApplication>();
        }

        // load and resume a workflow instance. If the instance is in memory, 
        // will return the version from memory. If not, will load it from the 
        // persistent store
        public WorkflowApplication LoadInstance(Guid instanceId)
        {
            // if the instance is in memory, return it
            if (this.instances.ContainsKey(instanceId))
                return this.instances[instanceId];

            // load the instance
            XmlWorkflowInstanceStore instStore = new XmlWorkflowInstanceStore(instanceId);
            WorkflowApplication instance = new WorkflowApplication(new PurchaseProcessWorkflow())
                {
                    InstanceStore = instStore,
                    Completed = OnWorkflowCompleted,
                    Idle = OnIdle
                };

            // add a tracking participant
            instance.Extensions.Add(new SaveAllEventsToTestFileTrackingParticipant());

            // add the instance to the list of running instances in the host
            instance.Load(instanceId);
            this.instances.Add(instance.Id, instance);
            return instance;
        }

        // creates a workflow application, binds parameters, links extensions and run it
        public WorkflowApplication CreateAndRun(RequestForProposal rfp)
        {
            // input parameters for the WF program
            IDictionary<string, object> inputs = new Dictionary<string, object>();
            inputs.Add("Rfp", rfp);            

            // create and run the WF instance
            Activity wf = new PurchaseProcessWorkflow();
            WorkflowApplication instance = new WorkflowApplication(wf, inputs)
            {
                PersistableIdle = OnIdleAndPersistable,
                Completed = OnWorkflowCompleted,
                Idle = OnIdle
            };
            XmlWorkflowInstanceStore store = new XmlWorkflowInstanceStore(instance.Id);
            instance.InstanceStore = store;

            //Create the persistence Participant and add it to the workflow instance
            XmlPersistenceParticipant xmlPersistenceParticipant = new XmlPersistenceParticipant(instance.Id);
            instance.Extensions.Add(xmlPersistenceParticipant);

            // add a tracking participant
            instance.Extensions.Add(new SaveAllEventsToTestFileTrackingParticipant());

            // add instance to the host list of running instances
            this.instances.Add(instance.Id, instance);
            
            // continue executing this instance
            instance.Run();

            return instance;
        }      

        // executed when instance goes idle
        public void OnIdle(WorkflowApplicationIdleEventArgs e)
        {            
        }

        public PersistableIdleAction OnIdleAndPersistable(WorkflowApplicationIdleEventArgs e)
        {
            return PersistableIdleAction.Persist;
        }

        // executed when instance is persisted
        public void OnWorkflowCompleted(WorkflowApplicationCompletedEventArgs e)        
        {
        }

        // submit a proposal to a vendor. To submit the proposal, a bookmark is resumed
        public void SubmitVendorProposal(Guid instanceId, int vendorId, double value)
        {
            WorkflowApplication instance = this.LoadInstance(instanceId);
            string bookmarkName = "waitingFor_" + vendorId.ToString();
            instance.ResumeBookmark(bookmarkName, value);
        }

        // returns true if the instance is waiting for proposals (has pending vendor bookmarks)
        public bool IsInstanceWaitingForProposals(Guid instanceId)
        {
            WorkflowApplication instance = this.LoadInstance(instanceId);
            return instance.GetBookmarks().Count > 0;
        }

        // returns true if a vendor can submit a proposal to an instance by 
        // checking if there is a pending bookmark for that vendor
        public bool CanSubmitProposalToInstance(Guid instanceId, int vendorId)
        {
            WorkflowApplication instance = this.LoadInstance(instanceId);

            // if there are no bookmarks, the process has finalized
            if (instance.GetBookmarks().Count == 0)
            {
                return false;
            }
            else // if there are bookmarks, check if one of them correspond with the "logged" vendor
            {
                foreach (BookmarkInfo bookmarkInfo in instance.GetBookmarks())
                {
                    if (bookmarkInfo.BookmarkName.Equals("waitingFor_" + vendorId))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
