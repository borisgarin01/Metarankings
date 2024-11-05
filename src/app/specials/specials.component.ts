import { NgFor } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-specials',
  standalone: true,
  imports: [NgFor],
  templateUrl: './specials.component.html',
  styleUrl: './specials.component.css'
})
export class SpecialsComponent {
  specials=[
    {
      href:'https://metarankings.ru/vyjdet-li-god-of-war-ragnarok-na-pk/',
      title:'Выйдет ли God Of War Ragnarok на ПК?',
      imageSrc:'https://metarankings.ru/images/uploads/2022/08/god-of-war-ragnarok1-445x250.jpg'
    },
    {
      href:'https://metarankings.ru/stoit-li-zhdat-overwatch-2/',
      title:'Стоит ли ждать Overwatch 2?',
      imageSrc:'https://metarankings.ru/images/uploads/2022/08/overwatch-2-445x250.jpg'
    },
    {
      href:'https://metarankings.ru/pyat-prichin-posmotret-serial-dom-drakona/',
      title:'5 причин посмотреть сериал «Дом Дракона»',
      imageSrc:'https://metarankings.ru/images/uploads/2022/08/dom-drakona-445x250.jpg'
    },
    {
      href:'https://metarankings.ru/evolyuciya-znaka-cheloveka-pauka-za-vsyu-istoriyu-geroya-marvel/',
      title:'Эволюция знака Человека-Паука — Как менялся логотип за всю историю героя Marvel',
      imageSrc:'https://metarankings.ru/images/uploads/2021/10/chelovek-pauk-445x250.jpg'
    },
  ]
}
