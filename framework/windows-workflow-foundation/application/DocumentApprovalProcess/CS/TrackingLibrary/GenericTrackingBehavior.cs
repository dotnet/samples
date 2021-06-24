//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------ 

using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activities.Tracking.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalManager
{
    public class GenericTrackingBehavior : IServiceBehavior
    {
        IList<TrackingComponentElement> trackingComponentElements;

        public GenericTrackingBehavior()
        {
        }

        public IList<TrackingComponentElement> TrackingComponentElements
        {
            get
            {
                if (this.trackingComponentElements == null)
                {
                    this.trackingComponentElements = new List<TrackingComponentElement>();
                }
                return this.trackingComponentElements;
            }
        }

        public virtual void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public virtual void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            string workflowDisplayName = "";
            System.ServiceModel.Activities.WorkflowServiceHost workflowServiceHost = serviceHostBase as System.ServiceModel.Activities.WorkflowServiceHost;
            if (null != workflowServiceHost)
            {
                workflowDisplayName = ((System.ServiceModel.Activities.WorkflowServiceHost)serviceHostBase).Activity.DisplayName;
            }

            System.ServiceModel.Activities.WorkflowServiceHost host = serviceHostBase as System.ServiceModel.Activities.WorkflowServiceHost;
            if (this.TrackingComponentElements != null && host != null)
            {
                foreach (TrackingComponentElement trackingComponentElement in this.TrackingComponentElements)
                {
                    TrackingParticipant trackingComponent = this.CreateTrackingComponent(trackingComponentElement);
                    if (trackingComponent != null)
                    {
                        if (!string.IsNullOrEmpty(trackingComponentElement.ProfileName))
                        {
                            trackingComponent.TrackingProfile = this.GetProfile(trackingComponentElement.ProfileName, workflowDisplayName);
                        }

                        host.WorkflowExtensions.Add(trackingComponent);
                    }
                    else
                    {
                        throw new Exception(string.Format("Tracking component is not a known type: {0}", trackingComponentElement.Name));
                    }
                }
            }
        }

        public virtual void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        TrackingProfile GetProfile(string profileName, string profileTarget)
        {
            TrackingSection trackingSection =
                    (TrackingSection)ConfigurationManager.GetSection("system.serviceModel/tracking");

            TrackingProfile bestMatch = null;

            foreach (TrackingProfile profile in trackingSection.TrackingProfiles)
            {
                // Check the profile matches the requested name, and scope type
                if (string.Compare(profileName, profile.Name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    // If we find a global scope profile, use it as the default profile
                    if (string.Compare("*", profile.ActivityDefinitionId, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (bestMatch == null)
                        {
                            bestMatch = profile;
                        }
                    }
                    else if (string.Compare(profileTarget, profile.ActivityDefinitionId, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        //specific profile for scopetarget found. 
                        bestMatch = profile;
                        break;
                    }
                }
            }
            if (bestMatch == null)
            {
                //If the profile is not found in config, return an empty profile to suppress
                //events. If .config does not have profiles, return null.
                bestMatch = new TrackingProfile()
                {
                    ActivityDefinitionId = profileTarget,
                };

            }

            return bestMatch;
        }

        TrackingParticipant CreateTrackingComponent(TrackingComponentElement participantElement)
        {
            if (participantElement.ComponentRuntimeType == null)
            {
                participantElement.ComponentRuntimeType = System.Type.GetType(participantElement.Type);
                if (participantElement.ComponentRuntimeType == null)
                {
                    throw new InvalidOperationException("Tracking component type not found");
                }
            }

            if (CheckType(participantElement.ComponentRuntimeType, typeof(TrackingParticipant)))
            {
                if ((participantElement.TrackingComponentArguments != null) && (participantElement.TrackingComponentArguments.Count != 0))
                {
                    ConstructorInfo cInfo = participantElement.ComponentRuntimeType.GetConstructor(new Type[] { typeof(NameValueCollection) });
                    if (cInfo != null)
                    {
                        return cInfo.Invoke(new object[] { participantElement.TrackingComponentArguments }) as TrackingParticipant;
                    }
                }
                else
                {
                    ConstructorInfo cInfo = participantElement.ComponentRuntimeType.GetConstructor(new Type[] { });

                    if (cInfo != null)
                    {
                        return cInfo.Invoke(null) as TrackingParticipant;
                    }
                }
            }
            return null;
        }

        static bool CheckType(Type newType, Type isType)
        {
            while (newType != null)
            {
                if (newType == isType)
                {
                    return true;
                }
                newType = newType.BaseType;
            }
            return false;
        }
    }
}
