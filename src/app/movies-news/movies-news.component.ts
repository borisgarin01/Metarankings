import { NgFor } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-movies-news',
  standalone: true,
  imports: [NgFor],
  templateUrl: './movies-news.component.html',
  styleUrl: './movies-news.component.css'
})
export class MoviesNewsComponent {
  moviesNews=
  [
    {
      href: 'https://metarankings.ru/ridli-skott-predstavil-trejler-gladiator-2-s-pedro-paskal-i-denzelem-vashingtonom/',
      title: 'Ридли Скотт представил трейлер «Гладиатор 2» с  Педро Паскаль и Дензелем Вашингтоном',
      imageSrc:'https://metarankings.ru/images/uploads/2024/07/gladiator-2-445x250.jpg'
    },
    {
      href: 'https://metarankings.ru/tom-xardi-uxodit-ot-pogoni-v-trejlere-venom-3-poslednij-tanec/',
      title: 'Том Харди уходит от погони в трейлере «Веном 3: Последний танец»',
      imageSrc:'https://metarankings.ru/images/uploads/2024/06/venom-3-poslednij-tanec-445x250.jpg'
    },
    {
      href: 'https://metarankings.ru/dzhenna-ortega-uillem-defo-moniki-belluchchi-i-drugie-v-trejlere-bitldzhus-bitldzhus/',
      title: 'Дженна Ортега, Уиллем Дефо, Моники Беллуччи и другие в трейлере «Битлджус Битлджус»',
      imageSrc:'https://metarankings.ru/images/uploads/2024/05/bitldzhus-bitldzhus-445x250.jpg'
    },
  ];
}
