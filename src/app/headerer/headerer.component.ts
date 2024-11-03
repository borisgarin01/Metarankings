import { Component } from '@angular/core';
import { TitleComponent } from "../title/title.component";

@Component({
  selector: 'app-headerer',
  standalone: true,
  imports: [TitleComponent],
  templateUrl: './headerer.component.html',
  styleUrl: './headerer.component.css'
})
export class HeadererComponent {

}
