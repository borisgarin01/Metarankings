import { Component } from '@angular/core';
import { NgFor } from '@angular/common';
import { GamesDropDownMainPageComponent } from "../games-drop-down-main-page/games-drop-down-main-page.component";

@Component({
  selector: 'app-new-games',
  standalone: true,
  imports: [NgFor, GamesDropDownMainPageComponent],
  templateUrl: './new-games.component.html',
  styleUrl: './new-games.component.css'
})
export class NewGamesComponent {
  newGames = [
    {
      href: 'https://metarankings.ru/horizon-zero-dawn-remastered/',
      name: 'Horizon Zero Dawn Remastered',
      imageSrc: 'https://metarankings.ru/images/uploads/2024/10/horizon-zero-dawn-remastered-boxart-cover-50x70.jpg',
      rating: 6.2,
      platforms: [
        { 'href': 'https://metarankings.ru/meta/games/pc/', name: 'PC' },
        { 'href': 'https://metarankings.ru/meta/games/ps5/', name: 'PS5' },
      ],
      reviewsPage: 'https://metarankings.ru/horizon-zero-dawn-remastered/#reviews',
      reviewsCount: 2,
      commentsPage: 'https://metarankings.ru/horizon-zero-dawn-remastered/#comments',
      commentsCount: 0,
      releaseDate: new Date(2024, 10, 31),
      description: 'Horizon Zero Dawn — это приключенческая ролевая игра, завоевавшая множество наград и признание критиков. В версии Remastered, знакомые игрокам дикие земли оживают с...'
    },
    {
      href: 'https://metarankings.ru/dragon-age-the-veilguard/',
      name: 'Dragon Age: The Veilguard',
      imageSrc: 'https://metarankings.ru/images/uploads/2024/12/dragon-age-the-veilguard-boxart-cover-50x70.jpg',
      rating: 6.7,
      platforms: [
        { 'href': 'https://metarankings.ru/meta/games/pc/', name: 'PC' },
        { 'href': 'https://metarankings.ru/meta/games/ps5/', name: 'PS5' },
        { 'href': 'https://metarankings.ru/meta/games/xbox-series-x/', name: 'Xbox Series X' },
      ],
      reviewsPage: 'https://metarankings.ru/dragon-age-the-veilguard/#reviews',
      reviewsCount: 6,
      commentsPage: 'https://metarankings.ru/dragon-age-the-veilguard/#comments',
      commentsCount: 1,
      releaseDate: new Date(2024, 10, 31),
      description: 'Ролевая экшен приключенческая игра Dragon Age: The Veilguard отправляет в мир Тедаса, яркую страну суровой дикой природы, коварных лабиринтов и сверкающих городов. Вас...'
    },
    {
      href: 'https://metarankings.ru/call-of-duty-black-ops-6/',
      name: 'Call of Duty: Black Ops 6',
      imageSrc: 'https://metarankings.ru/images/uploads/2024/09/call-of-duty-black-ops-6-boxart-cover-50x70.jpg',
      rating: 6.7,
      platforms: [
        { 'href': 'https://metarankings.ru/meta/games/pc/', name: 'PC' },
        { 'href': 'https://metarankings.ru/meta/games/ps4/', name: 'PS4' },
        { 'href': 'https://metarankings.ru/meta/games/ps5/', name: 'PS5' },
        { 'href': 'https://metarankings.ru/meta/games/xbox-one/', name: 'Xbox One' },
        { 'href': 'https://metarankings.ru/meta/games/xbox-series-x/', name: 'Xbox Series X' },
      ],
      reviewsPage: 'https://metarankings.ru/call-of-duty-black-ops-6/#reviews',
      reviewsCount: 5,
      commentsPage: 'https://metarankings.ru/call-of-duty-black-ops-6/#comments',
      commentsCount: 3,
      releaseDate: new Date(2024, 10, 25),
      description: 'Call of Duty: Black Ops 6 — новая часть культовой серии шутеров. В игре вас ждут динамичные и разнообразные сражения, развернутые на масштабных...'
    },
    {
      href: 'https://metarankings.ru/kong-survivor-instinct/',
      name: 'Kong: Survivor Instinct',
      imageSrc: 'https://metarankings.ru/images/uploads/2024/10/kong-survivor-instinct-boxart-cover-50x70.jpg',
      rating: 0,
      platforms: [
        { 'href': 'https://metarankings.ru/meta/games/pc/', name: 'PC' },
        { 'href': 'https://metarankings.ru/meta/games/ps4/', name: 'PS4' },
        { 'href': 'https://metarankings.ru/meta/games/ps5/', name: 'PS5' },
        { 'href': 'https://metarankings.ru/meta/games/xbox-series-x/', name: 'Xbox Series X' },
      ],
      reviewsPage: 'https://metarankings.ru/kong-survivor-instinct/#reviews',
      reviewsCount: 0,
      commentsPage: 'https://metarankings.ru/kong-survivor-instinct/#comments',
      commentsCount: 0,
      releaseDate: new Date(2024, 10, 22),
      description: 'Kong: Survivor Instinct – это 2.5D экшен-приключение, которое сочетает в себе реалистичный платформер, динамичные бои и исследование в стиле \'метроидвания\'. Вас ждет захватывающий...'
    },
    {
      href: 'https://metarankings.ru/unknown-9-awakening/',
      name: 'Unknown 9: Awakening',
      imageSrc: 'https://metarankings.ru/images/uploads/2024/10/unknown-9-awakening-cover-art-50x70.jpg',
      rating: 6.2,
      platforms: [
        { 'href': 'https://metarankings.ru/meta/games/pc/', name: 'PC' },
        { 'href': 'https://metarankings.ru/meta/games/ps4/', name: 'PS4' },
        { 'href': 'https://metarankings.ru/meta/games/ps5/', name: 'PS5' },
        { 'href': 'https://metarankings.ru/meta/games/xbox-one/', name: 'Xbox One' },
        { 'href': 'https://metarankings.ru/meta/games/xbox-series-x/', name: 'Xbox Series X' },
      ],
      reviewsPage: 'https://metarankings.ru/unknown-9-awakening/#reviews',
      reviewsCount: 1,
      commentsPage: 'https://metarankings.ru/unknown-9-awakening/#comments',
      commentsCount: 0,
      releaseDate: new Date(2024, 10, 18),
      description: 'В игре вас ждет история Харуны – квестор, обладающий уникальным даром: она может погружаться в Складку – загадочное измерение, тесно связанное с нашим...'
    },
    {
      href: 'https://metarankings.ru/a-quiet-place-the-road-ahead/',
      name: 'A Quiet Place: The Road Ahead',
      imageSrc: 'https://metarankings.ru/images/uploads/2024/10/a-quiet-place-the-road-ahead-boxart-cover-50x70.jpg',
      rating: 6,
      platforms: [
        { 'href': 'https://metarankings.ru/meta/games/pc/', name: 'PC' },
        { 'href': 'https://metarankings.ru/meta/games/ps5/', name: 'PS5' },
        { 'href': 'https://metarankings.ru/meta/games/xbox-series-x/', name: 'Xbox Series X' },
      ],
      reviewsPage: 'https://metarankings.ru/a-quiet-place-the-road-ahead/#reviews',
      reviewsCount: 1,
      commentsPage: 'https://metarankings.ru/a-quiet-place-the-road-ahead/#comments',
      commentsCount: 0,
      releaseDate: new Date(2024, 10, 17),
      description: 'A Quiet Place: The Road Ahead — это напряженная однопользовательская игра в жанре хоррор, основанная на популярной кинофраншизе «Тихое место». Она рассказывает уникальную...'
    },
    {
      href: 'https://metarankings.ru/neva/',
      name: 'Neva',
      imageSrc: 'https://metarankings.ru/images/uploads/2024/10/neva-cover-art-50x70.jpg',
      rating: 6,
      platforms: [
        { 'href': 'https://metarankings.ru/meta/games/pc/', name: 'PC' },
        { 'href': 'https://metarankings.ru/meta/games/ps5/', name: 'PS5' },
        { 'href': 'https://metarankings.ru/meta/games/xbox-series-x/', name: 'Xbox Series X' },
      ],
      reviewsPage: 'https://metarankings.ru/neva/#reviews',
      reviewsCount: 2,
      commentsPage: 'https://metarankings.ru/neva/#comments',
      commentsCount: 0,
      releaseDate: new Date(2024, 10, 15),
      description: 'Neva — это трогательное приключение, созданное теми же разработчиками, что подарили нам GRIS — игру, покорившую сердца критиков и игроков. В центре сюжета...'
    },
    {
      href: 'https://metarankings.ru/metaphor-refantazio/',
      name: 'Metaphor: ReFantazio',
      imageSrc: 'https://metarankings.ru/images/uploads/2023/06/metaphor-refantazio-boxart-cover-50x70.jpg',
      rating: 7.4,
      platforms: [
        { 'href': 'https://metarankings.ru/meta/games/pc/', name: 'PC' },
        { 'href': 'https://metarankings.ru/meta/games/xbox-series-x/', name: 'Xbox Series X' },
      ],
      reviewsPage: 'https://metarankings.ru/metaphor-refantazio/#reviews',
      reviewsCount: 2,
      commentsPage: 'https://metarankings.ru/metaphor-refantazio/#comments',
      commentsCount: 0,
      releaseDate: new Date(2024, 10, 11),
      description: 'В игре Metaphor: ReFantazio вам нужно определить свою судьбу и преодолеть страх, попадая в фантастический мир, который не похож ни на один другой....'
    },
    {
      href: 'https://metarankings.ru/starfield-shattered-space/',
      name: 'Starfield: Shattered Space',
      imageSrc: 'https://metarankings.ru/images/uploads/2024/10/starfield-shattered-spacecover-art-50x70.jpg',
      rating: 4.6,
      platforms: [
        { 'href': 'https://metarankings.ru/meta/games/pc/', name: 'PC' },
        { 'href': 'https://metarankings.ru/meta/games/xbox-series-x/', name: 'Xbox Series X' },
      ],
      reviewsPage: 'https://metarankings.ru/starfield-shattered-space/#reviews',
      reviewsCount: 1,
      commentsPage: 'https://metarankings.ru/starfield-shattered-space/#comments',
      commentsCount: 5,
      releaseDate: new Date(2024, 9, 30),
      description: 'Это масштабное сюжетное дополнение для Starfield, которое отправляет в город Дазра, расположенном на скрытой родной планете Дома Ва’Руун. Там бушует таинственная сила и...'
    },
    {
      href: 'https://metarankings.ru/game-silent-hill-2-remake/',
      name: 'Silent Hill 2',
      imageSrc: 'https://metarankings.ru/images/uploads/2023/03/silent-hill-2-remake-boxart-cover-50x70.jpg',
      rating: 4.6,
      platforms: [
        { 'href': 'https://metarankings.ru/meta/games/pc/', name: 'PC' },
        { 'href': 'https://metarankings.ru/meta/games/xbox-series-x/', name: 'Xbox Series X' },
      ],
      reviewsPage: 'https://metarankings.ru/game-silent-hill-2-remake/#reviews',
      reviewsCount: 9,
      commentsPage: 'https://metarankings.ru/game-silent-hill-2-remake/#comments',
      commentsCount: 3,
      releaseDate: new Date(2024, 10, 8),
      description: 'Культовая игра перезапускается, чтобы игроки могли снова пережить запоминающуюся историю оригинала, но уже с камерой за спиной главного героя. За разработку отвечает японский...'
    }
  ];
}
