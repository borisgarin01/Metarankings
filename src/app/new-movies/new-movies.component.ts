import { Component } from '@angular/core';

@Component({
  selector: 'app-new-movies',
  standalone: true,
  imports: [],
  templateUrl: './new-movies.component.html',
  styleUrl: './new-movies.component.css'
})
export class NewMoviesComponent {
  movies = [
    {
      href: 'https://metarankings.ru/vechnaya-zima-2024/',
      name: 'Вечная зима',
      imageSource: 'https://metarankings.ru/images/uploads/2024/10/vechnaya-zima-cover-art-50x70.jpg',
      score: 6.4,
      scoreStyle: 'small-score mark-6',
      genres: [
        { href: 'https://metarankings.ru/meta/movies/dramy/', name: 'Драмы' }
      ],
      reviews: { source: 'https://metarankings.ru/vechnaya-zima-2024/#reviews', count: 4 },
      comments: { source: 'https://metarankings.ru/vechnaya-zima-2024/#comments', count: 0 },
      releaseDate: new Date(2024, 10, 31),
      description:'Фильм «Вечная зима» рассказывает о Владимире и Елене, которые по-разному реагируют на трагические события, связанные с их сыном. Елена стремится разобраться в прошлом...'
    },
    {
      href: 'https://metarankings.ru/proklyatyj-ostrov-2024/',
      name: 'Проклятый остров',
      imageSource: 'https://metarankings.ru/images/uploads/2024/06/proklyatyj-ostrov-2024-cover-art-50x70.jpg',
      score: 0,
      scoreStyle: 'small-score mark-0',
      genres: [
        { href: 'https://metarankings.ru/meta/movies/horror/', name: 'Ужасы' }
      ],
      reviews: { source: 'https://metarankings.ru/proklyatyj-ostrov-2024/#reviews', count: 0 },
      comments: { source: 'https://metarankings.ru/proklyatyj-ostrov-2024/#comments', count: 0 },
      releaseDate: new Date(2024, 10, 31),
      description:'В фильме «Проклятый остров» отпуск молодых друзей превращается в настоящий непрекращающийся ужас, когда они случайно будят зловещее существо на необитаемом острове, которому необходима...'
    },
    {
      href: 'https://metarankings.ru/vozvrashhenie-popugaya-keshi-2024/',
      name: 'Возвращение попугая Кеши',
      imageSource: 'https://metarankings.ru/images/uploads/2024/10/vozvrashhenie-popugaya-keshi-cover-art-50x70.jpg',
      score: 0,
      scoreStyle: 'small-score mark-0',
      genres: [
        { href: 'https://metarankings.ru/meta/movies/comedy/', name: 'Комедии' },
        { href: 'https://metarankings.ru/meta/movies/semejnye/', name: 'Семейные'}
      ],
      reviews: { source: 'https://metarankings.ru/vozvrashhenie-popugaya-keshi-2024/#reviews', count: 0 },
      comments: { source: 'https://metarankings.ru/vozvrashhenie-popugaya-keshi-2024/#comments', count: 0 },
      releaseDate: new Date(2024, 10, 31),
      description:'В жизни Митяя, простого учителя географии из Сочи, появляется необычный подарок: не просто говорящий попугай, а настоящий гений из древнего рода. Иннокентий, которого...'
    },
  ];
}
