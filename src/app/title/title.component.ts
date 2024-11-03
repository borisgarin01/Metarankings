import { Component } from '@angular/core';
import { GamesDropDownMainPageComponent } from "../games-drop-down-main-page/games-drop-down-main-page.component";
import { MoviesDropDownMainPageComponent } from "../movies-drop-down-main-page/movies-drop-down-main-page.component";

@Component({
  selector: 'app-title',
  standalone: true,
  imports: [GamesDropDownMainPageComponent, MoviesDropDownMainPageComponent],
  templateUrl: './title.component.html',
  styleUrl: './title.component.css'
})
export class TitleComponent {

}
