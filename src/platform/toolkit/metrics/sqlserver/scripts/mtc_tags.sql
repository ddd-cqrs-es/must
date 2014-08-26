if exists (select name from sys.tables where name = 'mtc_tags')
begin
  if (select count(*) from mtc_tags) > 0
  begin
    raiserror('A tabela [mtc_tags] contem dados e não pode ser excluida.',16,1)
  end
  drop table mtc_tags
end
go

create table mtc_tags (
  tags_id bigint identity(1,1) not null,
  tags_hash int not null,
  tags_count int not null
)

alter table mtc_tags
add constraint PK_mtc_tags
primary key (
  tags_id
)

create index IX_mtc_tags_similar
on mtc_tags (
  tags_hash, tags_count
)