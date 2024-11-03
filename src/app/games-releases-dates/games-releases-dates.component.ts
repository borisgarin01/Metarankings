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
      releaseDate: new Date('August 19, 1975 23:15:30')
    },
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
      releaseDate: new Date('2024-11-7')
    },
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
      releaseDate: new Date('2024-11-7')
    },
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
      releaseDate: new Date('2024-11-7')
    },
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
      releaseDate: new Date('2024-11-7')
    }
  ]
}
