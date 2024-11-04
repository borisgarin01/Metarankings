import { Component } from '@angular/core';
import { CommonModule, NgFor } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { NewGamesComponent } from './new-games/new-games.component';
import { MoviesDropDownMainPageComponent } from "./movies-drop-down-main-page/movies-drop-down-main-page.component";
import { GamesDropDownMainPageComponent } from "./games-drop-down-main-page/games-drop-down-main-page.component";
import { TitleComponent } from "./title/title.component";
import { HeadererComponent } from "./headerer/headerer.component";
import { BestInThisMonthSlideshowComponent } from "./best-in-this-month-slideshow/best-in-this-month-slideshow.component";
import { NewMoviesComponent } from "./new-movies/new-movies.component";
import { MoviesNewsComponent } from "./movies-news/movies-news.component";
import { GamesNewsComponent } from "./games-news/games-news.component";
import { CollectionsComponent } from "./collections/collections.component";
import { SpecialsComponent } from "./specials/specials.component";
import { SoonAtMoviesComponent } from "./soon-at-movies/soon-at-movies.component";
import { GamesReleasesDatesComponent } from "./games-releases-dates/games-releases-dates.component";
import { FooterComponent } from "./footer/footer.component";
import { LatestGamesReviewsComponent } from "./latest-games-reviews/latest-games-reviews.component";
import { LatestMoviesReviewsComponent } from "./latest-movies-reviews/latest-movies-reviews.component";
import { SidebarRightColumnComponent } from "./sidebar-right-column/sidebar-right-column.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, NgFor, RouterOutlet, NewGamesComponent, MoviesDropDownMainPageComponent, GamesDropDownMainPageComponent, TitleComponent, HeadererComponent, BestInThisMonthSlideshowComponent, NewMoviesComponent, MoviesNewsComponent, GamesNewsComponent, CollectionsComponent, SpecialsComponent, SoonAtMoviesComponent, GamesReleasesDatesComponent, FooterComponent, LatestGamesReviewsComponent, LatestMoviesReviewsComponent, SidebarRightColumnComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Frontend';
  
  subtitle = 'Frontend';
}
