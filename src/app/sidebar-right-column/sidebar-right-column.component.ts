import { Component } from '@angular/core';
import { LatestMoviesReviewsComponent } from "../latest-movies-reviews/latest-movies-reviews.component";
import { LatestGamesReviewsComponent } from "../latest-games-reviews/latest-games-reviews.component";

@Component({
  selector: 'app-sidebar-right-column',
  standalone: true,
  imports: [LatestMoviesReviewsComponent, LatestGamesReviewsComponent],
  templateUrl: './sidebar-right-column.component.html',
  styleUrl: './sidebar-right-column.component.css'
})
export class SidebarRightColumnComponent {

}
