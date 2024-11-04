import { NgFor } from '@angular/common';
import { Component } from '@angular/core';
import { MoviesDropDownMainPageComponent } from "../movies-drop-down-main-page/movies-drop-down-main-page.component";

@Component({
  selector: 'app-new-movies',
  standalone: true,
  imports: [NgFor, MoviesDropDownMainPageComponent],
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
      description: 'Фильм «Вечная зима» рассказывает о Владимире и Елене, которые по-разному реагируют на трагические события, связанные с их сыном. Елена стремится разобраться в прошлом...'
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
      description: 'В фильме «Проклятый остров» отпуск молодых друзей превращается в настоящий непрекращающийся ужас, когда они случайно будят зловещее существо на необитаемом острове, которому необходима...'
    },
    {
      href: 'https://metarankings.ru/vozvrashhenie-popugaya-keshi-2024/',
      name: 'Возвращение попугая Кеши',
      imageSource: 'https://metarankings.ru/images/uploads/2024/10/vozvrashhenie-popugaya-keshi-cover-art-50x70.jpg',
      score: 0,
      scoreStyle: 'small-score mark-0',
      genres: [
        { href: 'https://metarankings.ru/meta/movies/comedy/', name: 'Комедии' },
        { href: 'https://metarankings.ru/meta/movies/semejnye/', name: 'Семейные' }
      ],
      reviews: { source: 'https://metarankings.ru/vozvrashhenie-popugaya-keshi-2024/#reviews', count: 0 },
      comments: { source: 'https://metarankings.ru/vozvrashhenie-popugaya-keshi-2024/#comments', count: 0 },
      releaseDate: new Date(2024, 10, 31),
      description: 'В жизни Митяя, простого учителя географии из Сочи, появляется необычный подарок: не просто говорящий попугай, а настоящий гений из древнего рода. Иннокентий, которого...'
    },
    {
      href: 'https://metarankings.ru/les-chudes-2024/',
      name: 'Лес чудес',
      imageSource: 'https://metarankings.ru/images/uploads/2024/10/les-chudes-2024-cover-art-50x70.jpg',
      score: 0,
      scoreStyle: 'small-score mark-0',
      genres: [
        { href: 'https://metarankings.ru/meta/movies/cartoons/', name: 'Мультфильмы' }],
      reviews: { source: 'https://metarankings.ru/les-chudes-2024/#reviews', count: 0 },
      comments: { source: 'https://metarankings.ru/les-chudes-2024/#comments', count: 1 },
      releaseDate: new Date(2024, 10, 31),
      description: 'Десятилетний Анжело мечтает стать исследователем и зоологом, и его мечта вот-вот сбудется. Во время поездки к бабушке, его родители, как всегда, немного рассеянны...'
    },
    {
      href: 'https://metarankings.ru/rasxititeli-grobnic-manuskript-drakona-2024/',
      name: 'Расхитители гробниц. Манускрипт дракона',
      imageSource: 'https://metarankings.ru/images/uploads/2024/10/rasxititeli-grobnic-manuskript-drakona-cover-art-50x70.jpg',
      score: 0,
      scoreStyle: 'small-score mark-0',
      genres: [
        { href: 'https://metarankings.ru/meta/movies/fantasy-movies/', name: 'Фэнтези' }],
      reviews: { source: 'https://metarankings.ru/rasxititeli-grobnic-manuskript-drakona-2024/#reviews', count: 0 },
      comments: { source: 'https://metarankings.ru/rasxititeli-grobnic-manuskript-drakona-2024/#comments', count: 0 },
      releaseDate: new Date(2024, 10, 31),
      description: 'Ху Байи, Ширли Ян и Ван Панг Зи, трио смелых искателей сокровищ, отправляются в опасное путешествие к таинственной гробнице в Шэньси. Детектив Ху...'
    },
    {
      href: 'https://metarankings.ru/diplodochek-i-volshebnye-miry-2024/',
      name: 'Диплодочек и волшебные миры',
      imageSource: 'https://metarankings.ru/images/uploads/2024/10/diplodochek-i-volshebnye-miry-cover-art-50x70.jpg',
      score: 0,
      scoreStyle: 'small-score mark-0',
      genres: [
        { href: 'https://metarankings.ru/meta/movies/cartoons/', name: 'Мультфильмы' }],
      reviews: { source: 'https://metarankings.ru/diplodochek-i-volshebnye-miry-2024/#reviews', count: 0 },
      comments: { source: 'https://metarankings.ru/diplodochek-i-volshebnye-miry-2024/#comments', count: 0 },
      releaseDate: new Date(2024, 10, 31),
      description: 'Юный диплодочек, обладающий знатным происхождением, отправляется в увлекательное путешествие, чтобы разыскать своих пропавших родителей. Их таинственное исчезновение заставляет его покинуть родные места и...'
    },
    {
      href: 'https://metarankings.ru/kadavr-2024/',
      name: 'Кадавр',
      imageSource: 'https://metarankings.ru/images/uploads/2024/10/kadavr-2024-cover-art-50x70.jpg',
      score: 0,
      scoreStyle: 'small-score mark-0',
      genres: [
        { href: 'https://metarankings.ru/meta/movies/horror/', name: 'Ужасы' }],
      reviews: { source: 'https://metarankings.ru/kadavr-2024/#reviews', count: 0 },
      comments: { source: 'https://metarankings.ru/kadavr-2024/#comments', count: 0 },
      releaseDate: new Date(2024, 10, 31),
      description: 'Джемаль, водитель катафалка, привык к тишине и одиночеству своей работы. Но однажды ему поручают необычный заказ: перевезти тело молодой девушки и спрятать его...'
    },
    {
      href: 'https://metarankings.ru/zaklyatie-reinkarnaciya-otca-2024/',
      name: 'Заклятие: Реинкарнация отца',
      imageSource: 'https://metarankings.ru/images/uploads/2024/10/zaklyatie-reinkarnaciya-otca-cover-art-50x70.jpg',
      score: 0,
      scoreStyle: 'small-score mark-0',
      genres: [
        { href: 'https://metarankings.ru/meta/movies/horror/', name: 'Ужасы' }],
      reviews: { source: 'https://metarankings.ru/zaklyatie-reinkarnaciya-otca-2024/#reviews', count: 0 },
      comments: { source: 'hhttps://metarankings.ru/zaklyatie-reinkarnaciya-otca-2024/#comments', count: 0 },
      releaseDate: new Date(2024, 10, 31),
      description: 'Айзек, оставшийся без отца после его внезапной смерти, живет с мачехой. Но их спокойная жизнь рушится, когда появляется жуткое существо, поразительно похожее на...'
    },
    {
      href: 'https://metarankings.ru/dvojnaya-igra-2024/',
      name: 'Двойная игра',
      imageSource: 'https://metarankings.ru/images/uploads/2024/10/dvojnaya-igra-2024-cover-art-50x70.jpg',
      score: 0,
      scoreStyle: 'small-score mark-0',
      genres: [
        { href: 'https://metarankings.ru/meta/movies/boeviki/', name: 'Боевики' },
        { href: 'https://metarankings.ru/meta/movies/thrillers/', name: 'Триллеры' }
      ],
      reviews: { source: 'https://metarankings.ru/dvojnaya-igra-2024/#reviews', count: 0 },
      comments: { source: 'hhttps://metarankings.ru/dvojnaya-igra-2024/#comments', count: 0 },
      releaseDate: new Date(2024, 10, 31),
      description: 'Хо Сау, агент под прикрытием, уже давно внедрился в преступную организацию наркобарона Яу, но его двойная жизнь становится все опаснее, угрожая его работе...'
    },
    {
      href: 'https://metarankings.ru/yurka-2024/',
      name: 'Юрка',
      imageSource: 'https://metarankings.ru/images/uploads/2024/10/yurka-2024-cover-art-50x70.jpg',
      score: 0,
      scoreStyle: 'small-score mark-0',
      genres: [
        { href: 'https://metarankings.ru/meta/movies/melodramas/', name: 'Мелодрамы' }
      ],
      reviews: { source: 'https://metarankings.ru/yurka-2024/#reviews', count: 0 },
      comments: { source: 'hhttps://metarankings.ru/yurka-2024/#comments', count: 0 },
      releaseDate: new Date(2024, 10, 31),
      description: 'Станислав, авторитетный криминальный лидер, встречает Юрку, сироту из детского дома. В этом мальчишке он видит семью, которой ему всегда не хватало, и решает...'
    }
  ];
}
