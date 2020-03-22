--RECONFIGURE WITH OVERRIDE
--GO
----sp_configure 'contained database authentication', 1

----RECONFIGURE WITH OVERRIDE
--GO


CREATE DATABASE [Contained_Db]
CONTAINMENT = PARTIAL

use [Contained_Db]
GO

CREATE USER TestContainedUser1  WITH PASSWORD = 'readOaad1213nlyuser123!';
GO

GRANT SELECT TO [TestContainedUser1]
GO


