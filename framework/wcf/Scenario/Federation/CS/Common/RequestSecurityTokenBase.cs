//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;

using System.Runtime.InteropServices;

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.Federation
{
    /// <summary>
    /// abstract base class containing properties that are shared by RST and RSTR classes
    /// </summary>
    [ComVisible(false)]
    public abstract class RequestSecurityTokenBase : BodyWriter
    {
        // private members
        private string m_context;
        private string m_tokenType;
        private int m_keySize;
        private EndpointAddress m_appliesTo;
        
        // Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        protected RequestSecurityTokenBase() : this(String.Empty,String.Empty,0, null)
        {
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="context">The value of the wst:RequestSecurityToken/@Context attribute in the request message, if any</param>
        /// <param name="tokenType">The content of the wst:RequestSecurityToken/wst:TokenType element in the request message, if any</param>
        /// <param name="keySize">The content of the wst:RequestSecurityToken/wst:KeySize element in the request message, if any</param>
        /// <param name="appliesTo">An EndpointRefernece corresponding to the content of the wst:RequestSecurityToken/wsp:AppliesTo element in the request message, if any</param>
        protected RequestSecurityTokenBase(string context, string tokenType, int keySize, EndpointAddress appliesTo )
            : base(true)
        {
            this.m_context = context;
            this.m_tokenType = tokenType;
            this.m_keySize = keySize;
            this.m_appliesTo = appliesTo;
        }

        // public properties

        /// <summary>
        /// Context for the RST/RSTR exchange. 
        /// The value of the wst:RequestSecurityToken/@Context attribute from RequestSecurityToken messages
        /// The value of the wst:RequestSecurityTokenResponse/@Context attribute from RequestSecurityTokenResponse messages        
        /// </summary>
        public string Context
        {
            get { return m_context; }
            set { m_context = value; }
        }

        /// <summary>
        /// The type of token requested or returned.
        /// The value of the wst:RequestSecurityToken/wst:TokenType element from RequestSecurityToken messages
        /// The value of the wst:RequestSecurityTokenResponse/wst:TokenType element from RequestSecurityTokenResponse messages       
        /// </summary>
        public string TokenType 
        { 
            get { return m_tokenType; }
            set { m_tokenType = value; }
        }


        /// <summary>
        /// The size of the requested proof key
        /// The value of the wst:RequestSecurityToken/wst:KeySize element from RequestSecurityToken messages
        /// The value of the wst:RequestSecurityTokenResponse/wst:KeySize element from RequestSecurityTokenResponse messages       
        /// </summary>
        public int KeySize
        {
            get { return m_keySize; }
            set { m_keySize = value; }
        }

        /// <summary>
        /// The EndpointAddress a token is being requested or returned for 
        /// The content of the wst:RequestSecurityToken/wsp:AppliesTo element from RequestSecurityToken messages
        /// The content of the wst:RequestSecurityTokenResponse/wsp:AppliesTo element from RequestSecurityTokenResponse messages       
        /// </summary>public int KeySize
        public EndpointAddress AppliesTo
        {
            get { return m_appliesTo; }
            set { m_appliesTo = value; }
        }
    }
}

