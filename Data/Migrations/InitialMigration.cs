using FluentMigrator;

namespace Data.Migrations;

[Migration(1)]
public sealed class InitialMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"
DROP TABLE BestInPastMonths;
DROP TABLE CollectionsComponentItems;
DROP TABLE NewsComponentsItems;
DROP TABLE Genres;
DROP TABLE IndexComponentsItems;
DROP TABLE LastReviewsComponentsItems;
DROP TABLE Platforms;
DROP TABLE SpecialsComponentsItems;
DROP TABLE Studios;
");
    }

    public override void Up()
    {
        Execute.Sql(@"
CREATE TABLE BestInPastMonths
(Id bigint not null primary key identity(1,1),
Name varchar(255) not null,
Href varchar(255) not null unique,
Score float not null,
ImageSrc varchar(255) not null);

CREATE TABLE CollectionsComponentItems
(Id bigint not null primary key identity(1,1),
ItemHref varchar(255) not null unique,
Title varchar(255) not null,
ImageSrc varchar(255) not null,
ImageAlt varchar(255) not null,
CategoryTitle varchar(255) not null,
CategoryHref varchar(255) not null);

CREATE TABLE NewsComponentsItems
(Id bigint not null primary key identity(1,1),
LinkHref varchar(255) not null unique,
LinkTitle varchar(255) not null,
ImageSrc varchar(255) not null,
ImageAlt varchar(255) not null);

CREATE TABLE Genres
(Id bigint not null primary key identity(1,1),
Name varchar(255) not null unique,
Href varchar(255) not null);

CREATE TABLE IndexComponentsItems
(Id bigint not null primary key identity(1,1),
ItemHref varchar(255) not null unique,
Title varchar(255) not null,
Score float not null,
Description varchar(max) null,
ReleaseDate DATE not null,
ImageSrc varchar(255) not null,
ImageAlt varchar(255) not null,
CHECK (Score >= 0 AND Score <= 10));

CREATE TABLE LastReviewsComponentsItems
(Id bigint not null primary key identity(1,1),
LinkHref varchar(255) not null unique,
LinkTitle varchar(255) not null);

CREATE TABLE Platforms
(Id bigint not null primary key identity(1,1),
Href varchar(255) not null unique,
Name varchar(255) not null unique);

CREATE TABLE SpecialsComponentsItems
(Id bigint not null primary key identity(1,1),
LinkHref varchar(255) not null unique,
LinkTitle varchar(255) not null,
ImageSrc varchar(255) null,
ImageAlt varchar(255) null);

CREATE TABLE Studios
(Id bigint not null primary key identity(1,1),
Name varchar(255) not null unique);
");
    }
}
