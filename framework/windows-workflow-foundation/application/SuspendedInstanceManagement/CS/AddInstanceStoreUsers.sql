-- Copyright (c) Microsoft Corporation.  All rights reserved.
USE DefaultSampleStore;
GO
CREATE USER [IIS APPPOOL\DefaultAppPool]
GO
EXEC sp_addrolemember 'System.Activities.DurableInstancing.InstanceStoreUsers', 'IIS APPPOOL\DefaultAppPool'
GO
CREATE USER [NT AUTHORITY\Network Service]
GO
EXEC sp_addrolemember 'System.Activities.DurableInstancing.InstanceStoreUsers', 'NT AUTHORITY\Network Service'
GO