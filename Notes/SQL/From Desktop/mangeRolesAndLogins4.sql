
--*********** SERVER ***********
USE master 
GO

CREATE LOGIN TestLogin4  WITH PASSWORD = 'readOaad1213nlyuser123!';
GO

GRANT CONNECT ANY DATABASE TO TestLogin4


--*********** DATABASE ***********
use [TSK_f79f72cc-52e2-4cc4-ab2f-18da61411025]
GO

CREATE USER [TestUser4] FOR LOGIN [TestLogin4]
GO

GRANT SELECT TO [TestUser4]
GO