import { NgFor } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-games-news',
  standalone: true,
  imports: [NgFor],
  templateUrl: './games-news.component.html',
  styleUrl: './games-news.component.css'
})
export class GamesNewsComponent {
  games = [
    {
      link: 'https://metarankings.ru/sony-obyavila-sistemnye-trebovaniya-god-of-war-ragnarok-dlya-pc/',
      title: 'Sony объявила системные требования God of War Ragnarök для PC',
      imageLink: 'https://metarankings.ru/images/uploads/2024/08/god-of-war-ragnarok-445x250.jpg'
    },
    {
      link: 'https://metarankings.ru/activision-predstavila-kinematograficheskij-trejler-zombi-rezhima-call-of-duty-black-ops-6/',
      title: 'Activision представила кинематографический трейлер зомби-режима Call of Duty: Black Ops 6',
      imageLink: 'https://metarankings.ru/images/uploads/2024/08/kinematograficheskij-trejler-zombi-rezhima-v-call-of-duty-black-ops-6-445x250.jpg'
    },
    {
      link: 'https://metarankings.ru/nintendo-pokazala-metroid-prime-4-beyond-dlya-switch/',
      title: 'Nintendo показала Metroid Prime 4: Beyond для Switch',
      imageLink: 'https://metarankings.ru/images/uploads/2024/06/metroid-prime-4-beyond-445x250.jpg'
    },
    
  ];
}
