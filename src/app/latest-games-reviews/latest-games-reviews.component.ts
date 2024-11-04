import { NgFor } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-latest-games-reviews',
  standalone: true,
  imports: [NgFor],
  templateUrl: './latest-games-reviews.component.html',
  styleUrl: './latest-games-reviews.component.css'
})
export class LatestGamesReviewsComponent {
  latestGamesReviews = [
    { href: 'https://metarankings.ru/dragon-age-the-veilguard/', title: 'Dragon Age: The Veilguard' },
    { href: 'https://metarankings.ru/call-of-duty-black-ops-6/', title: 'Call of Duty: Black Ops 6' },
    { href: 'https://metarankings.ru/game-silent-hill-2-remake/', title: 'Silent Hill 2' },
    { href: 'https://metarankings.ru/the-legend-of-zelda-echoes-of-wisdom/', title: 'The Legend of Zelda: Echoes of Wisdom' },
    { href: 'https://metarankings.ru/astro-bot/', title: 'Astro Bot' },
    { href: 'https://metarankings.ru/star-wars-outlaws/', title: 'Star Wars Outlaws' },
    { href: 'https://metarankings.ru/black-myth-wukong/', title: 'Black Myth: WuKong' },
    { href: 'https://metarankings.ru/senuas-saga-hellblade-2/', title: 'Senua’s Saga: Hellblade 2' },
    { href: 'https://metarankings.ru/stellar-blade/', title: 'Stellar Blade' },
    { href: 'https://metarankings.ru/rise-of-the-ronin/', title: 'Rise of the Ronin' }
  ];
}
