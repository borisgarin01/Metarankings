import { Component } from '@angular/core';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-games-releases-dates',
  standalone: true,
  imports: [NgFor],
  templateUrl: './games-releases-dates.component.html',
  styleUrl: './games-releases-dates.component.css'
})
export class GamesReleasesDatesComponent {
  gamesReleasesDates = [
    {
      href: 'https://metarankings.ru/metro-awakening/',
      title: 'Metro Awakening',
      imageSrc: 'https://metarankings.ru/images/uploads/2024/10/metro-awakening-boxart-cover-50x70.jpg',
      platforms: [
        { name: 'PC', platformLink: 'https://metarankings.ru/meta/games/pc/' },
        { name: 'PS5', platformLink: 'https://metarankings.ru/meta/games/ps5/' }
      ],
      genres: [
        { name: 'Шутер', genreLink: 'https://metarankings.ru/genre/shuter/' }
      ],
      releaseDate: new Date(2024, 11, 7)
    },
    {
      href: 'https://metarankings.ru/lego-horizon-adventures/',
      title: 'LEGO Horizon Adventures',
      imageSrc: 'https://metarankings.ru/images/uploads/2024/11/lego-horizon-adventures-cover-art-50x70.jpg',
      platforms: [
        { name: 'PC', platformLink: 'https://metarankings.ru/meta/games/pc/' },
        { name: 'PS5', platformLink: 'https://metarankings.ru/meta/games/ps5/' },
        { name: 'Switch', platformLink: 'https://metarankings.ru/meta/games/switch/' }
      ],
      genres: [
        { name: 'Приключение', genreLink: 'https://metarankings.ru/genre/priklyuchenie/' },
        { name: 'Экшен', genreLink: 'https://metarankings.ru/genre/ekshen/' }
      ],
      releaseDate: new Date(2024, 11, 14)
    },
    {
      href: 'https://metarankings.ru/assassins-creed-shadows/',
      title: 'Assassin’s Creed Shadows',
      imageSrc: 'https://metarankings.ru/images/uploads/2024/11/assassins-creed-shadows-boxart-cover-50x70.jpg',
      platforms: [
        { name: 'PC', platformLink: 'https://metarankings.ru/meta/games/pc/' },
        { name: 'PS5', platformLink: 'https://metarankings.ru/meta/games/ps5/' },
        { name: 'Xbox Series X', platformLink: 'https://metarankings.ru/meta/games/xbox-series-x/' }
      ],
      genres: [
        { name: 'Экшен', genreLink: 'https://metarankings.ru/genre/ekshen/' }
      ],
      releaseDate: new Date('2024-11-15')
    },
    {
      href: 'https://metarankings.ru/indiana-jones-and-the-great-circle/',
      title: 'Indiana Jones and the Great Circle',
      imageSrc: 'https://metarankings.ru/images/uploads/2024/02/indiana-jones-and-the-great-circle-cover-art-50x70.jpg',
      platforms: [
        { name: 'PC', platformLink: 'https://metarankings.ru/meta/games/pc/' },
        { name: 'Xbox Series X', platformLink: 'https://metarankings.ru/meta/games/xbox-series-x/' }
      ],
      genres: [
        { name: 'Приключение', genreLink: 'https://metarankings.ru/genre/priklyuchenie/' },
        { name: 'Экшен', genreLink: 'https://metarankings.ru/genre/ekshen/' }
      ],
      releaseDate: new Date('2024-1-1')
    },
    {
      href: 'https://metarankings.ru/mpioner/',
      title: 'PIONER',
      imageSrc: 'https://metarankings.ru/images/uploads/2024/02/pioner-boxart-cover-50x70.jpg',
      platforms: [
        { name: 'PC', platformLink: 'https://metarankings.ru/meta/games/pc/' },
        { name: 'PS5', platformLink: 'https://metarankings.ru/meta/games/ps5/' },
        { name: 'Xbox Series X', platformLink: 'https://metarankings.ru/meta/games/xbox-series-x/' }
      ],
      genres: [
        { name: 'ММО', genreLink: 'https://metarankings.ru/genre/mmo/' },
        { name: 'Шутер', genreLink: 'https://metarankings.ru/genre/shuter/' }
      ],
      releaseDate: new Date('2024-1-1')
    }
  ]
}
