import { NgFor } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-latest-movies-reviews',
  standalone: true,
  imports: [NgFor],
  templateUrl: './latest-movies-reviews.component.html',
  styleUrl: './latest-movies-reviews.component.css'
})
export class LatestMoviesReviewsComponent {
  latestMoviesReviews = [
    { href: 'https://metarankings.ru/vozvrashhenie-popugaya-keshi-2024/', title: 'Возвращение попугая Кеши' },
    { href: 'https://metarankings.ru/venom-3-poslednij-tanec-2024/', title: 'Веном 3: Последний танец' },
    { href: 'https://metarankings.ru/dzhoker-bezumie-na-dvoix/', title: 'Джокер: Безумие на двоих' },
    { href: 'https://metarankings.ru/xellboj-proklyatie-gorbuna-2024/', title: 'Хеллбой: Проклятие Горбуна' },
    { href: 'https://metarankings.ru/ekzorcizm-2024/', title: 'Экзорцизм' },
    { href: 'https://metarankings.ru/bitldzhus-bitldzhus/', title: 'Битлджус Битлджус' },
    { href: 'https://metarankings.ru/largo-vinch-gnev-proshlogo-2024/', title: 'Ларго Винч: Гнев прошлого' },
    { href: 'https://metarankings.ru/voron-2024/', title: 'Ворон' },
    { href: 'https://metarankings.ru/chuzhoj-romul-2024/', title: 'Чужой: Ромул' },
    { href: 'https://metarankings.ru/pokazhi-mne-lunu-2024/', title: 'Покажи мне Луну' },

  ];
}
