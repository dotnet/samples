//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.Samples.LocalChannel
{
    class ExceptionMessages
    {
        public const string BaseAddressCannotHaveFragment = "A base address cannot contain a Uri fragment.";
        public const string BaseAddressCannotHaveQuery = "A base address cannot contain a Uri query string.";
        public const string BaseAddressCannotHaveUserInfo = "A base address cannot contain a Uri user info section.";
        public const string BaseAddressMustBeAbsolute = "Only an absolute Uri can be used as a base address.";
        public const string ChannelListenerNotFound = "There is no channel listener listening at address '{0}'. Ensure that the ServiceHost is opened before opening the client channel.";
        public const string CloseTimedOut = "Close timed out after {0}.  Increase the timeout value passed to the call to Close or increase the CloseTimeout value on the Binding. The time allotted to this operation may have been a portion of a longer timeout.";
        public const string InvalidUriScheme = "The provided URI scheme '{0}' is invalid; expected '{1}'.";
        public const string ListenerAlreadyRegistered = "A listener has already being registered for Uri '{0}'.  Only one listener can be registered for the same Uri.";
        public const string MessageEncoderNotAllowed = "A MessageEncodingBindingElement can not be included in the BindingParameters when used in combination with the LocalTransportBindingElement.  Consider either removing the MessageEncodingBindingElement(s) or using a different TransportBindingElement.";
        public const string NoListenerRegistered = "No listener has being registered for Uri '{0}'.";
        public const string ObjectDisposed = "The {0} object has been disposed.";
        public const string ReceivedMessageVersionMismatch = "The message version of the outgoing message ({0}) does not match the default message version of the local transport channel.  The local transport channel only supports MessageVersion.Soap12WSAddressing10.";
        public const string ReceiveMessageSizeMustBeMaxValue = "The 'MaxReceivedMessageSize' property for the LocalTransportBindingElement must be Int64.MaxValue.";
        public const string ReceiveShutdownReturnedFault = "The channel received an unexpected fault input message while closing. The fault reason given is: '{0}'";
        public const string ReceiveShutdownReturnedLargeFault = "The channel received an unexpected fault input message with Action = '{0}' while closing. You should only close your channel when you are not expecting any more input messages.";
        public const string ReceiveShutdownReturnedMessage = "The channel received an unexpected input message with Action '{0}' while closing. You should only close your channel when you are not expecting any more input messages.";
        public const string ReceiveTimedOut = "Receive on local address {0} timed out after {1}. The time allotted to this operation may have been a portion of a longer timeout.";
        public const string ReceiveTimedOutNoLocalAddress = "Receive timed out after {0}. The time allotted to this operation may have been a portion of a longer timeout.";
        public const string SentMessageVersionMismatch = "The envelope version of the incoming message ({0}) does not match the default message version of the local transport channel.  The local transport channel only supports MessageVersion.Soap12WSAddressing10.";
        public const string UnsupportedChannelType = "Channel type '{0}' is not supported.";
        public const string ValueMustBePositive = "The value of this argument must be positive.";
        public const string CommunicationObjectInInvalidState = "Communication object is in an invalid state.";
        public const string MessageClosed = "Message is closed.";
        public const string InvalidAsyncResult = "The asynchronous result object used to end this operation was not the object that was returned when the operation was initiated.";
        public const string AsyncResultAlreadyEnded = "End cannot be called twice on an AsyncResult.";
        public const string TimeoutInputQueueDequeue = "A Dequeue operation timed out after {0}. The time allotted to this operation may have been a portion of a longer timeout.";
        public const string ActionItemIsAlreadyScheduled = "The ActionItem was already scheduled for execution that hasn't been completed yet.";
        public const string AsyncTransactionException = "An exception was thrown from a TransactionScope used to flow a transaction into an asynchronous operation.";
    }
}
