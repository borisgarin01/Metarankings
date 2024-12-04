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
Name nvarchar(255) not null,
Href nvarchar(255) not null unique,
Score float not null,
ImageSrc nvarchar(255) not null);

CREATE TABLE CollectionsComponentItems
(Id bigint not null primary key identity(1,1),
ItemHref nvarchar(255) not null unique,
Title nvarchar(255) not null,
ImageSrc nvarchar(255) not null,
ImageAlt nvarchar(255) not null,
CategoryTitle nvarchar(255) not null,
CategoryHref nvarchar(255) not null);

CREATE TABLE NewsComponentsItems
(Id bigint not null primary key identity(1,1),
LinkHref nvarchar(255) not null unique,
LinkTitle nvarchar(255) not null,
ImageSrc nvarchar(255) not null,
ImageAlt nvarchar(255) not null);

CREATE TABLE Genres
(Id bigint not null primary key identity(1,1),
Name nvarchar(255) not null unique,
Href nvarchar(255) not null);

CREATE TABLE IndexComponentsItems
(Id bigint not null primary key identity(1,1),
ItemHref nvarchar(255) not null unique,
Title nvarchar(255) not null,
Score float not null,
Description nvarchar(max) null,
ReleaseDate DATE not null,
ImageSrc nvarchar(255) not null,
ImageAlt nvarchar(255) not null,
CHECK (Score >= 0 AND Score <= 10));

CREATE TABLE LastReviewsComponentsItems
(Id bigint not null primary key identity(1,1),
LinkHref nvarchar(255) not null unique,
LinkTitle nvarchar(255) not null);

CREATE TABLE Platforms
(Id bigint not null primary key identity(1,1),
Href nvarchar(255) not null unique,
Name nvarchar(255) not null unique);

CREATE TABLE SpecialsComponentsItems
(Id bigint not null primary key identity(1,1),
LinkHref nvarchar(255) not null unique,
LinkTitle nvarchar(255) not null,
ImageSrc nvarchar(255) null,
ImageAlt nvarchar(255) null);

CREATE TABLE Studios
(Id bigint not null primary key identity(1,1),
Name nvarchar(255) not null unique);
");
    }
}
