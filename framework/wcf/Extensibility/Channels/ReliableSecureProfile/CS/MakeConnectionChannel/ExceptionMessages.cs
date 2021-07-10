//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Samples.ReliableSecureProfile
{
    static class ExceptionMessages
    {
        public const string TimedOutWhileWaitingForPoll = "Timed out while waiting for outstanding polls to complete after {0}.  Increase the timeout value passed to the call to Close or increase the CloseTimeout value on the Binding. The time allotted to this operation may have been a portion of a longer timeout.";
        public const string WsmcMissingSelection = "The MakeConnection element did not contain any selection criteria.";
        public const string WsmcNotAnonUri = "The message could not be sent because the To header was not set to the Make Connection Anonymous Uri.  The value of the To header was ({0}).";
        public const string WsmcNoToHeader = "The message could not be sent because no To header was present.  If using Manual Addressing, be sure that the To header is being set on outgoing messages.";
        public const string WsmcRequiresAddressing = "Make Connection requires WS-Addressing.";
        public const string WsmcUnsupportedSelection = "The extension element used in the message selection is not supported by the MakeConnection receiver.";
        public const string ChannelTypeNotSupported = "The specified channel type {0} is not supported by this channel manager.";
        public const string IAnonymousUriPrefixMatcherPropertyNotFound = "The IAnonymousUriPrefixMatcher property was not found in the BindingContext. Make sure that the sample is running with .Net 4.0.";
        public const string AsyncResultCompletedTwice = "The IAsyncResult implementation '{0}' tried to complete a single operation multiple times. This could be caused by an incorrect application IAsyncResult implementation or other extensibility code, such as an IAsyncResult that returns incorrect CompletedSynchronously values or invokes the AsyncCallback multiple times.";
        public const string InvalidNullAsyncResult = "A null value was returned from an async 'Begin' method or passed to an AsyncCallback. Async 'Begin' implementations must return a non-null IAsyncResult and pass the same IAsyncResult object as the parameter to the AsyncCallback.";
        public const string InvalidAsyncResultImplementation = "An incorrect implementation of the IAsyncResult interface may be returning incorrect values from the CompletedSynchronously property or calling the AsyncCallback more than once. The type {0} could be the incorrect implementation.";
        public const string InvalidAsyncResultImplementationGeneric = "An incorrect implementation of the IAsyncResult interface may be returning incorrect values from the CompletedSynchronously property or calling the AsyncCallback more than once.";
        public const string InvalidAsyncResult = "An incorrect IAsyncResult was provided to an 'End' method. The IAsyncResult object passed to 'End' must be the one returned from the matching 'Begin' or passed to the callback provided to 'Begin'.";
        public const string AsyncResultAlreadyEnded = "End cannot be called twice on an AsyncResult.";
        public const string AsyncTransactionException = "An exception was thrown from a TransactionScope used to flow a transaction into an asynchronous operation.";
        public const string ActionItemIsAlreadyScheduled = "The ActionItem was already scheduled for execution that hasn't been completed yet";
        public const string ReceiveTimedOut = "Receive on local address {0} timed out after {1}. The time allotted to this operation may have been a portion of a longer timeout.";
        public const string ReceiveTimedOutNoLocalAddress = "Receive timed out after {0}. The time allotted to this operation may have been a portion of a longer timeout.";
        public const string TimeoutInputQueueDequeue = "A Dequeue operation timed out after {0}. The time allotted to this operation may have been a portion of a longer timeout.";
    }
}
