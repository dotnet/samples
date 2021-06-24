//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activities;
using System.ServiceModel.Activities.Tracking.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Microsoft.Samples.HiringService
{
    public class HistoryFileTrackingBehavior : IServiceBehavior
    {
        string profileName;        
        
        public HistoryFileTrackingBehavior(string profileName)
        {            
            this.profileName = profileName;
        }
        
        public virtual void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // get the tracking profile
            TrackingProfile trackingProfile = GetProfile(this.profileName);

            // configure the custom tracking with the new tracking profile
            HistoryFileTrackingParticipant participant = new HistoryFileTrackingParticipant() { TrackingProfile = trackingProfile };
            
            // add it to the extensions collection
            ((WorkflowServiceHost)serviceHostBase).WorkflowExtensions.Add(participant);
        }

        TrackingProfile GetProfile(string profileName)
        {
            TrackingProfile profile = new TrackingProfile();
            TrackingSection trackingSection = (TrackingSection)ConfigurationManager.GetSection("system.serviceModel/tracking");
            if (trackingSection == null) return null;

            var match = from p in new List<TrackingProfile>(trackingSection.TrackingProfiles)
                        where p.Name == profileName
                        select p;

            if (match.Count() == 0) throw new ConfigurationErrorsException(string.Format("Could not find a profile with name '{0}'", profileName));
            else return match.First();            
        }

        public virtual void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) { }

        public virtual void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }
    }
}