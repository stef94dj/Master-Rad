--RECONFIGURE WITH OVERRIDE
--GO
----sp_configure 'contained database authentication', 1

----RECONFIGURE WITH OVERRIDE
--GO
use master

CREATE DATABASE [Output_Tables]
CONTAINMENT = PARTIAL

use [Output_Tables]
GO

CREATE USER TestContainedUser1  WITH PASSWORD = 'readOaad1213nlyuser123!';
GO

GRANT SELECT ON [dbo].Table_1 TO [TestContainedUser1]
--GRANT INSERT ON [dbo].Table_1 TO [TestContainedUser1]
--GRANT UPDATE ON [dbo].Table_1 TO [TestContainedUser1]
--GRANT DELETE ON [dbo].Table_1 TO [TestContainedUser1]
GO



GRANT VIEW DEFINITION ON SCHEMA :: dbo TO [TestContainedUser1]
GO

SELECT * FROM [dbo].[Table_1]
select * from dbo.Table_1
select * from [dbo].Table_2