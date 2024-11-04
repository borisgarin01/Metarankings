import { NgFor } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-soon-at-movies',
  standalone: true,
  imports: [NgFor],
  templateUrl: './soon-at-movies.component.html',
  styleUrl: './soon-at-movies.component.css'
})
export class SoonAtMoviesComponent {
  movies = [{
    name: 'Бернард. Миссия: Марс',
    href: 'https://metarankings.ru/bernard-missiya-mars-2024/',
    imageSrc: 'https://metarankings.ru/images/uploads/2024/09/bernard-missiya-mars-cover-art-50x70.jpg',
    genres: [{ href: 'https://metarankings.ru/meta/movies/cartoons/', name: 'Мультфильмы' }],
    originalName: 'Bei ken xiong: huo xing ren wu',
    releaseDate: new Date(2024, 11, 7)
  },
  {
    name: 'Тайна трех пустынь',
    href: 'https://metarankings.ru/bernard-missiya-mars-2024/',
    imageSrc: 'https://metarankings.ru/images/uploads/2024/05/tajna-trex-pustyn-cover-art-50x70.jpg',
    genres: [{ href: 'https://metarankings.ru/meta/movies/cartoons/', name: 'Мультфильмы' }],
    originalName: 'Тайна трех пустынь',
    releaseDate: new Date(2024, 11, 14)
  },
  {
    name: 'За слова отвечаю',
    href: 'https://metarankings.ru/za-slova-otvechayu-2024/',
    imageSrc: 'https://metarankings.ru/images/uploads/2024/11/za-slova-otvechayu-cover-art-50x70.jpg',
    genres: [{ href: 'https://metarankings.ru/meta/movies/comedy/', name: 'Комедии' }],
    originalName: 'За слова отвечаю',
    releaseDate: new Date(2024, 11, 14)
  },
  {
    name: 'Расхитители гробниц. Зеркала призраков',
    href: 'https://metarankings.ru/rasxititeli-grobnic-zerkala-prizrakov-2024/',
    imageSrc: 'https://metarankings.ru/images/uploads/2024/11/rasxititeli-grobnic-zerkala-prizrakov-cover-art-50x70.jpg',
    genres: [
      { href: 'https://metarankings.ru/meta/movies/boeviki/', name: 'Боевики' },
      {href:'https://metarankings.ru/meta/movies/fantasy-movies/', name:'Фэнтези'}
    ],
    originalName: 'Gui chui deng zhi nan hai gui xu',
    releaseDate: new Date(2024, 11, 14)
  },
  {
    name: 'Гладиатор 2',
    href: 'https://metarankings.ru/gladiator-2/',
    imageSrc: 'https://metarankings.ru/images/uploads/2024/11/gladiator-2-2cover-art-50x70.jpg',
    genres: [
      { href: 'https://metarankings.ru/meta/movies/boeviki/', name: 'Боевики' },
      {href:'https://metarankings.ru/meta/movies/dramy/', name:'Драмы'},
      {href:'https://metarankings.ru/meta/movies/istoricheskie/', name:'Исторические'}
    ],
    originalName: 'Gladiator 2',
    releaseDate: new Date(2024, 11, 20)
  }]
}
