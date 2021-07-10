//----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------------------

using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.Samples.ReliableSecureProfile
{
    abstract class MakeConnectionMessageFault : MessageFault
    {
        FaultCode code;
        string exceptionMessage;
        bool hasDetail;
        bool isRemote;
        FaultReason reason;
        string subcode;
        UniqueId originalMessageId;

        protected MakeConnectionMessageFault(Message message, string subcode, string faultReason, string exceptionMessage)
        {
            this.code = new FaultCode("Receiver", "");
            this.subcode = subcode;
            this.reason = new FaultReason(faultReason);
            this.exceptionMessage = exceptionMessage;
            this.isRemote = false;
            this.originalMessageId = message.Headers.MessageId;
        }

        protected MakeConnectionMessageFault(Message message, FaultCode code, string subcode, FaultReason reason)
        {
            this.code = code;
            this.subcode = subcode;
            this.reason = reason;
            this.isRemote = true;
            this.originalMessageId = message.Headers.MessageId;
        }

        public override FaultCode Code
        {
            get
            {
                return this.code;
            }
        }

        public override bool HasDetail
        {
            get
            {
                return this.hasDetail;
            }
        }

        public bool IsRemote
        {
            get
            {
                return this.isRemote;
            }
        }

        public override FaultReason Reason
        {
            get
            {
                return this.reason;
            }
        }

        public string Subcode
        {
            get
            {
                return this.subcode;
            }
        }

        public virtual CommunicationException CreateException()
        {
            string message;

            if (this.IsRemote)
            {
                message = this.reason.GetMatchingTranslation(CultureInfo.CurrentCulture).Text;
            }
            else
            {
                message = this.exceptionMessage;
            }

            return new CommunicationException(message);
        }

        public static CommunicationException CreateException(MakeConnectionMessageFault fault)
        {
            return fault.CreateException();
        }

        public Message CreateMessage(MessageVersion messageVersion)
        {
            string action = MakeConnectionConstants.Fault.Action;

            if (messageVersion.Envelope == EnvelopeVersion.Soap11)
            {
                this.code = this.Get11Code(this.code, this.subcode);
            }
            else if (messageVersion.Envelope == EnvelopeVersion.Soap12)
            {
                if (this.code.SubCode == null)
                {
                    FaultCode subCode = new FaultCode(this.subcode, MakeConnectionConstants.Namespace);
                    this.code = new FaultCode(this.code.Name, this.code.Namespace, subCode);
                }

                this.hasDetail = this.Get12HasDetail();
            }

            Message message = Message.CreateMessage(messageVersion, this, action);
            message.Headers.RelatesTo = this.originalMessageId;
            return message;
        }

        protected abstract FaultCode Get11Code(FaultCode code, string subcode);
        protected abstract bool Get12HasDetail();

        protected string GetExceptionMessage()
        {
            return this.exceptionMessage;
        }

        internal void WriteDetail(XmlDictionaryWriter writer)
        {
            this.OnWriteDetailContents(writer);
        }

        static public MakeConnectionMessageFault CreateUnsupportedSelectionFault(Message message, string elementName)
        {
            return new UnsupportedSelectionFault(message, elementName);
        }

        static public MakeConnectionMessageFault CreateMissingSelectionFault(Message message)
        {
            return new MissingSelectionFault(message);
        }

        static public bool TryCreateFault(Message message, MessageFault fault, out MakeConnectionMessageFault wsmcFault)
        {
            // all WS-MakeConnection faults are receiver faults
            if (!fault.Code.IsReceiverFault)
            {
                wsmcFault = null;
                return false;
            }

            if ((fault.Code.SubCode == null)
                || (fault.Code.SubCode.Namespace != MakeConnectionConstants.Namespace))
            {
                wsmcFault = null;
                return false;
            }

            XmlDictionaryReader detailReader = null;
            string subcodeName = fault.Code.SubCode.Name;
            if (fault.HasDetail)
            {
                detailReader = fault.GetReaderAtDetailContents();
            }

            wsmcFault = CreateMakeConnectionFault(message, fault.Code, subcodeName, fault.Reason, detailReader);
            return (wsmcFault != null);
        }

        static MakeConnectionMessageFault CreateMakeConnectionFault(Message message, FaultCode code, string subcode, FaultReason reason, XmlDictionaryReader detailReader)
        {
            if (subcode == MakeConnectionConstants.Fault.UnsupportedSelectionFault)
            {
                return new UnsupportedSelectionFault(message, code, reason, detailReader);
            }
            else if (subcode == MakeConnectionConstants.Fault.MissingSelectionFault)
            {
                return new MissingSelectionFault(message, code, reason, detailReader);
            }

            return null;
        }

        sealed class UnsupportedSelectionFault : MakeConnectionMessageFault
        {
            string elementName;
            public UnsupportedSelectionFault(Message message, string elementName)
                : base(message, MakeConnectionConstants.Fault.UnsupportedSelectionFault, 
                    ExceptionMessages.WsmcUnsupportedSelection, null)
            {
                this.elementName = elementName;
            }

            public UnsupportedSelectionFault(Message message, FaultCode code, FaultReason reason, XmlDictionaryReader detailReader)
                : base(message, code, MakeConnectionConstants.Fault.UnsupportedSelectionFault, reason)
            {
                if (detailReader != null)
                {
                    try
                    {
                        detailReader.ReadStartElement(MakeConnectionConstants.Fault.UnsupportedSelectionFault, MakeConnectionConstants.Namespace);
                        this.elementName = detailReader.ReadContentAsString();
                        detailReader.ReadEndElement();
                    }
                    finally
                    {
                        detailReader.Close();
                    }
                }
            }

            protected override FaultCode Get11Code(FaultCode code, string subcode)
            {
                return new FaultCode(subcode, MakeConnectionConstants.Namespace);
            }

            protected override bool Get12HasDetail()
            {
                return true;
            }

            protected override void OnWriteDetailContents(XmlDictionaryWriter writer)
            {
                writer.WriteElementString(MakeConnectionConstants.Fault.UnsupportedSelectionFault, MakeConnectionConstants.Namespace, this.elementName);
            }
        }

        sealed class MissingSelectionFault : MakeConnectionMessageFault
        {
            public MissingSelectionFault(Message message)
                : base(message, MakeConnectionConstants.Fault.MissingSelectionFault, 
                    ExceptionMessages.WsmcMissingSelection, null)
            {
            }

            public MissingSelectionFault(Message message, FaultCode code, FaultReason reason, XmlDictionaryReader detailReader)
                : base(message, code, MakeConnectionConstants.Fault.MissingSelectionFault, reason)
            {
            }

            protected override FaultCode Get11Code(FaultCode code, string subcode)
            {
                return new FaultCode(subcode, MakeConnectionConstants.Namespace);
            }

            protected override bool Get12HasDetail()
            {
                return false;
            }

            protected override void OnWriteDetailContents(XmlDictionaryWriter writer)
            {
            }
        }
    }
}
