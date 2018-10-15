use DiaryDB

CREATE TABLE Users(
id int primary key identity (1,1) not null,
login varchar(50) not null,
password varchar(50) not null
)

CREATE TABLE Titles(
id int primary key identity(1,1) not null,
user_id int not null foreign key (user_id) references Users(id),
title varchar(100) not null,
text varchar(1000) null
)

CREATE TABLE Images(
id int primary key identity(1,1) not null,
title_id int not null foreign key references Titles(id),
image image not null
)
