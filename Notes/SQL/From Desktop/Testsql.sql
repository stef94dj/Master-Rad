use [TSK_f79f72cc-52e2-4cc4-ab2f-18da61411025]
select * from t1

use [TSK_f79f72cc-52e2-4cc4-ab2f-18da61411025]
delete from t1 where id = 4


use [TSK_f79f72cc-52e2-4cc4-ab2f-18da61411025]
insert into t1 values(1,1,1)
insert into t1 values(2,1,1)
insert into t1 values(3,1,1)
insert into t1 values(4,1,1)
insert into t1 values(5,1,1)
insert into t1 values(6,1,1)
insert into t1 values(7,1,1)

use master
select * from dbo.spt_fallback_db