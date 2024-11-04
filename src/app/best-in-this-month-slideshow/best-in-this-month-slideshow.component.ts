import { NgFor } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-best-in-this-month-slideshow',
  standalone: true,
  imports: [NgFor],
  templateUrl: './best-in-this-month-slideshow.component.html',
  styleUrl: './best-in-this-month-slideshow.component.css'
})
export class BestInThisMonthSlideshowComponent {
  games = [
    {
      title: 'Анора',
      link: 'https://metarankings.ru/anora-2024/',
      score: 6.5,
      scoreStyle: 'small-score mark-7',
      imageLink: 'https://metarankings.ru/images/uploads/2024/10/anora-2024-cover-art-187x281.jpg'
    },
    {
      title: 'Ужасающий 3',
      link: 'https://metarankings.ru/uzhasayushhij-3/',
      score: 5.9,
      scoreStyle: 'small-score mark-6',
      imageLink: 'https://metarankings.ru/images/uploads/2024/06/uzhasayushhij-3-cover-art-197x281.jpg'
    },
    {
      title: 'Свидание с монстром',
      link: 'https://metarankings.ru/svidanie-s-monstrom-2024/',
      score: 6.1,
      scoreStyle: 'small-score mark-6',
      imageLink: 'https://metarankings.ru/images/uploads/2024/10/svidanie-s-monstrom-cover-art-196x281.jpg'
    },
    {
      title: 'Вечная зима',
      link: 'https://metarankings.ru/vechnaya-zima-2024/',
      score: 4.6,
      scoreStyle: 'small-score mark-5',
      imageLink: 'https://metarankings.ru/images/uploads/2024/10/vechnaya-zima-cover-art-196x281.jpg'
    },
    {
      title: 'Подземная бездна',
      link: 'https://metarankings.ru/podzemnaya-bezdna-2024/',
      score: 5.0,
      scoreStyle: 'small-score mark-5',
      imageLink: 'https://metarankings.ru/images/uploads/2024/10/podzemnaya-bezdna-cover-art-196x281.jpg'
    },
    {
      title: 'Крик. Сезон призраков',
      link: 'https://metarankings.ru/krik-sezon-prizrakov-2024/',
      score: 6.8,
      scoreStyle: 'small-score mark-7',
      imageLink: 'https://metarankings.ru/images/uploads/2024/10/krik-sezon-prizrakov-2024-cover-art-190x281.jpg'
    }
  ];
}
