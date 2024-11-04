import { NgFor } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-collections',
  standalone: true,
  imports: [NgFor],
  templateUrl: './collections.component.html',
  styleUrl: './collections.component.css'
})
export class CollectionsComponent {
  collection = [
    {
      href: 'https://metarankings.ru/samye-slozhnye-igry/',
      title: 'Самые сложные игры',
      imageSrc: 'https://metarankings.ru/images/uploads/2023/06/slozhnye-igry-445x250.jpg',
      categoryLink: 'https://metarankings.ru/meta/collections/films-games/',
      categoryTitle: 'Контент'
    },
    {
      href: 'https://metarankings.ru/best-films-pro-zhenshhin/',
      title: 'Лучшие фильмы про женщин',
      imageSrc: 'https://metarankings.ru/images/uploads/2023/06/films-pro-zhenshhin-445x250.jpg',
      categoryLink: 'https://metarankings.ru/meta/collections/films-games/',
      categoryTitle: 'Контент'
    },
    {
      href: 'https://metarankings.ru/filmy-pro-realnye-sobytiya/',
      title: 'Фильмы про реальные события',
      imageSrc: 'https://metarankings.ru/images/uploads/2023/06/pro-realnye-sobytiya-445x250.jpg',
      categoryLink: 'https://metarankings.ru/meta/collections/films-games/',
      categoryTitle: 'Контент'
    },
    {
      href: 'https://metarankings.ru/luchshie-filmy-pro-monstrov/',
      title: 'Лучшие фильмы про монстров',
      imageSrc: 'https://metarankings.ru/images/uploads/2023/06/filmy-pro-monstrov-445x250.jpg',
      categoryLink: 'https://metarankings.ru/meta/collections/films-games/',
      categoryTitle: 'Контент'
    },
    {
      href: 'https://metarankings.ru/samye-strashnye-igry/',
      title: 'Самые страшные игры',
      imageSrc: 'https://metarankings.ru/images/uploads/2023/06/strashnye-igry-445x250.jpg',
      categoryLink: 'https://metarankings.ru/meta/collections/films-games/',
      categoryTitle: 'Контент'
    },
    {
      href: 'https://metarankings.ru/best-games-open-world/',
      title: 'Лучшие игры с открытым миром',
      imageSrc: 'https://metarankings.ru/images/uploads/2023/06/best-igry-s-otkrytym-mirom-445x250.jpg',
      categoryLink: 'https://metarankings.ru/meta/collections/films-games/',
      categoryTitle: 'Контент'
    }
  ];
}
